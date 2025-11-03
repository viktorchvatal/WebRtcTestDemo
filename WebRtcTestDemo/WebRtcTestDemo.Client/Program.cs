// See https://aka.ms/new-console-template for more information

using Microsoft.VisualBasic.CompilerServices;
using SIPSorcery.Net;
using WebRtcTestDemo.Client;

internal class Program
{
    public static async Task Main(string[] args)
    {
        // Load test video buffer (encoded blue frame as H264)
        byte[] buffer = Helpers.LoadTestBuffer();

        List<string> streamNames = Enumerable.Range(0, 20).Select(x => x.ToString()).ToList();

        foreach (string streamName in streamNames)
        {
            StartClient(streamName, buffer);
        }

        // Keep demo running
        while (true)
        {
            await Task.Delay(1000);
        }
    }

    private static void StartClient(string stream, byte[] buffer)
    {
        Task.Run(() => RunClient($"ws://127.0.0.1:8094/input/{stream}", buffer));
    }

    private static async Task RunClient(string url, byte[] buffer)
    {
        WebRTCWebSocketClient client = new WebRTCWebSocketClient(
            url,
            () => CreatePeerConnection(url, buffer)
        );
        await client.Start(CancellationToken.None);
    }

    private static Task<RTCPeerConnection> CreatePeerConnection(string url, byte[] buffer)
    {
        Console.WriteLine("New peer connection for {0}", url);

        OutputPeerConnection connection = new OutputPeerConnection();

        Task.Run(() => StreamWorker(connection, url, buffer, CancellationToken.None));
        return Task.FromResult(connection.Connection);
    }

    private static async Task StreamWorker(
        OutputPeerConnection peerConnection, string url, byte[] buffer, CancellationToken cancellationToken
    ) {
        while (!peerConnection.Connected)
        {
            Console.WriteLine("Waiting for peer connection for {0}", url);
            await Task.Delay(100, cancellationToken);
        }

        try
        {
            Console.WriteLine("Stream worker for {0} started", url);

            while (!cancellationToken.IsCancellationRequested && peerConnection.Connected)
            {
                peerConnection.SendVideoToWebRtc(buffer, 90000 / 40);
                await Task.Delay(40, cancellationToken);
            }
        } catch (TaskCanceledException) { }

        Console.WriteLine("Stream worker for {0} was stopped", url);
    }
}