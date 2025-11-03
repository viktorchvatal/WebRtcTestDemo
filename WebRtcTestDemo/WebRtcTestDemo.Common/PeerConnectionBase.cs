using System.Net;
using SIPSorcery.Net;

namespace WebRtcTestDemo.Common;

public abstract class PeerConnectionBase
{
    public const int DefaultFormatId = 98; // Can be anything between 96 - 127
    private readonly RTCPeerConnection connection;

    private volatile bool connected = false;

    public event Action<PeerConnectionBase>? Disconnected;

    public RTCPeerConnection Connection => connection;
    public bool Connected => connected;

    public PeerConnectionBase()
    {
        RTCConfiguration rtcConfig = new RTCConfiguration();
        connection = new RTCPeerConnection(rtcConfig);
        AttachEvents();
    }

    private void AttachEvents()
    {
        connection.onconnectionstatechange += this.OnConnectionStateChange;
        connection.OnReceiveReport += this.OnReceiveReport;
        connection.OnSendReport += this.OnSendReport;
        connection.GetRtpChannel().OnStunMessageReceived += this.OnStunMessageReceived;
        connection.oniceconnectionstatechange += this.OnIceConnectionStateChange;
    }

    private void DetachEvents()
    {
        connection.oniceconnectionstatechange -= this.OnIceConnectionStateChange;
        connection.GetRtpChannel().OnStunMessageReceived -= this.OnStunMessageReceived;
        connection.OnSendReport -= this.OnSendReport;
        connection.OnReceiveReport -= this.OnReceiveReport;
        connection.onconnectionstatechange -= this.OnConnectionStateChange;
    }

    private void OnIceConnectionStateChange(RTCIceConnectionState iceState)
    {
        Console.WriteLine("ICE connection state change to {0}.", iceState);
    }

    private void OnStunMessageReceived(STUNMessage message, IPEndPoint endPoint, bool isRelay)
    {
        Console.WriteLine("STUN {0} received from {1}.", message.Header.MessageType, endPoint);
    }

    private void OnSendReport(SDPMediaTypesEnum media, RTCPCompoundPacket packet)
    {
        Console.WriteLine("RTCP Send for {0}: {1}", media, packet.GetDebugSummary());
    }

    private void OnReceiveReport(IPEndPoint endPoint, SDPMediaTypesEnum media, RTCPCompoundPacket packet)
    {
        Console.WriteLine(
            "RTCP Receive for {0} from {1}: {2}", media, endPoint, packet.GetDebugSummary()
        );
    }

    private void SetConnectedState(bool connectedState)
    {
        lock (this)
        {
            connected = connectedState;

            if (!connected)
            {
                DetachEvents();
                OnDisconnected();
            }
        }

        if (!connected)
        {
            Disconnected?.Invoke(this);
        }
    }

    protected virtual Task OnDisconnected()
    {
        return Task.CompletedTask;
    }

    private void OnConnectionStateChange(RTCPeerConnectionState change)
    {
        Console.WriteLine("Peer connection state change to {0}.", change);

        if (change == RTCPeerConnectionState.connected)
        {
            SetConnectedState(true);
        }
        else if (change == RTCPeerConnectionState.failed)
        {
            connection.Close("ice disconnection");
            SetConnectedState(false);
        }
        else if (change == RTCPeerConnectionState.disconnected || change == RTCPeerConnectionState.closed)
        {
            SetConnectedState(false);
        }
    }
}