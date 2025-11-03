// See https://aka.ms/new-console-template for more information

using System.Net;
using SIPSorcery.Net;
using WebRtcTestDemo.Server;
using WebSocketSharp.Server;

internal class Program
{
    public static async Task Main(string[] args)
    {
        var webSocketServer = new WebSocketServer(new IPAddress([127, 0, 0, 1]), 8094);
        List<string> streamNames = Enumerable.Range(0, 20).Select(x => x.ToString()).ToList();

        foreach (string streamName in streamNames)
        {
            string url = $"/input/{streamName}";
            Console.WriteLine($"Registering input stream URL: {url}");

            webSocketServer.AddWebSocketService<WebRTCWebSocketPeer>(
                url,
                peer => HandleInputConnection(peer, streamName)
            );
        }

        webSocketServer.Start();

        // Keep demo running
        while (true)
        {
            await Task.Delay(1000);
        }
    }

    private static void HandleInputConnection(WebRTCWebSocketPeer peer, string streamName)
    {
        peer.CreatePeerConnection = () => CreatePeerConnection(streamName);
    }

    private static Task<RTCPeerConnection> CreatePeerConnection(string streamName)
    {
        InputPeerConnection connection = new InputPeerConnection();
        return Task.FromResult(connection.Connection);
    }
}