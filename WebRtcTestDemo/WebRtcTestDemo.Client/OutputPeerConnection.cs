using SIPSorcery.Net;
using SIPSorceryMedia.Abstractions;
using WebRtcTestDemo.Common;

namespace WebRtcTestDemo.Client;

public class OutputPeerConnection: PeerConnectionBase
{
    private readonly CancellationTokenSource cancellation = new CancellationTokenSource();

    public OutputPeerConnection()
    {
        Connection.addTrack(CreateVideoTrack());
    }

    protected override Task OnDisconnected()
    {
        cancellation.Cancel();
        return Task.CompletedTask;
    }


    public void SendVideoToWebRtc(byte[] buffer, uint duration)
    {
        try
        {
            if (Connected)
            {
                Connection.SendVideo(90000/40, buffer);
            }
        }
        catch (Exception exception)
        {
            Console.WriteLine("Failed to push video buffer " + exception.Message);
        }
    }


    private MediaStreamTrack CreateVideoTrack()
    {
        return new MediaStreamTrack(
            new VideoFormat(
                VideoCodecsEnum.H264,
                DefaultFormatId,
                parameters: "profile-level-id=42e01f;level-asymmetry-allowed=1;packetization-mode=1"
            ),
            MediaStreamStatusEnum.SendOnly
        );
    }
}