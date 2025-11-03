using System.Net;
using SIPSorcery.Net;
using SIPSorceryMedia.Abstractions;
using WebRtcTestDemo.Common;

namespace WebRtcTestDemo.Server;

public class InputPeerConnection : PeerConnectionBase
{
    public InputPeerConnection()
    {
        MediaStreamTrack videoTrack = new MediaStreamTrack(
            new VideoFormat(
                VideoCodecsEnum.H264,
                DefaultFormatId,
                parameters: "profile-level-id=42e01f;level-asymmetry-allowed=1;packetization-mode=1"
            ),
            MediaStreamStatusEnum.RecvOnly
        );
        Connection.addTrack(videoTrack);

        Connection.OnVideoFrameReceived += OnVideoFrameReceived;
    }

    private void OnVideoFrameReceived(IPEndPoint endPoint, uint timestamp, byte[] data, VideoFormat format)
    {
        Console.WriteLine(
            "Video frame received: {0} {1} {2} {3}.",
            format.FormatName, endPoint, timestamp, data.Length
        );
    }
}