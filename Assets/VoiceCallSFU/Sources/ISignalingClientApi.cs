using System.Threading.Tasks;
using System;

namespace Mirabo.VoiceCall
{
    public interface ISignalingClientApi
    {
        event Action<SdpData> OnSdpAnswer;
        event Action<string, SdpData> OnSdpOffer;
        event Action<string, IceData> OnIceUpdate;

        void SendOffer(SdpData sdp);
        void SendAnswer(string otherPeerId, SdpData sdp);
        void SendIceUpdate(string otherPeerId, IceData ice);

        public string PeerId { get; }
    }
}
