namespace Mirabo.VoiceCall
{
    public interface IPeerHandlerFactory
    {
        UpPeerHandler CreateUpPeerHandler(string peerId, string username, string device, ISignaler signaler);
        DownPeerHandler CreateDownPeerHandler(string peerId, string username, string device, ISignaler signaler);
    }
}
