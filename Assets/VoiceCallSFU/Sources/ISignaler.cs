using System;

namespace Mirabo.VoiceCall
{
    public interface ISignaler : IDisposable
    {
        void UpdateIce(IceData ice);
        void Publish(SdpData sdp);
        void Answer(SdpData sdp);

        // event Action<OnNewPublisherDto> OnNewPublisher;
        event Action<SdpData> OnSdpAnswer;
        event Action<IceData> OnIceUpdate;
        event Action<SdpData> OnSdpOffer;
    }

    public class IceData
    {
        public string sdpMid;
        public int sdpMLineIndex;
        public string candidate;
    }

    public class SdpData
    {
        public string type;
        public string sdp;
    }

}


