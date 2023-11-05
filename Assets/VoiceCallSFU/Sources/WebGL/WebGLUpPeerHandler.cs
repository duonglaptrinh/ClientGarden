using Mirabo.VoiceCall;
using Mirabo.VoiceCall.WebGL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading;
using Cysharp.Threading.Tasks;

#if UNITY_WEBGL
public class WebGLUpPeerHandler : UpPeerHandler
{
    string _id;
    VoiceCallSupportWebGL _voiceCallSupportWebGL;
    IWebGLPeerConfig _peerConfig;

    public WebGLUpPeerHandler(string id, ISignaler signaler, VoiceCallSupportWebGL voiceCallSupportWebGL, IWebGLPeerConfig peerConfig) : base(signaler)
    {
        _id = id;
        _voiceCallSupportWebGL = voiceCallSupportWebGL;
        _peerConfig = peerConfig;

        Connect(_cancellationTokenSource.Token).Forget();
    }

    CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    async UniTask Connect(CancellationToken cancellationToken = default)
    {
        _signaler.OnSdpAnswer += sdp =>
        {
            var sdpData = new VoiceCallSupportWebGL.SignalingSdpMsg.SdpData() { type = sdp.type, sdp = sdp.sdp };
            _voiceCallSupportWebGL.FeedAnswer(_id, JsonConvert.SerializeObject(sdpData));
        };

        _signaler.OnIceUpdate += iceData =>
        {
            var convertedIce = new VoiceCallSupportWebGL.SignalingIceCandidateMsg.CandidateData() { candidate = iceData.candidate, sdpMid = iceData.sdpMid, sdpMLineIndex = iceData.sdpMLineIndex };
            _voiceCallSupportWebGL.FeedIceCandidate(_id, JsonConvert.SerializeObject(convertedIce));
        };

        _voiceCallSupportWebGL.CreatePeer(_id, _peerConfig);

        _voiceCallSupportWebGL.OnSendSdpOffer += HandleSendSdpOffer;
        _voiceCallSupportWebGL.OnSendIceCandidate += HandleSendIceCandidate;

        await UniTask.CompletedTask;
    }

    private void HandleSendIceCandidate(string id, VoiceCallSupportWebGL.SignalingIceCandidateMsg.CandidateData candidate)
    {
        if (_id == id)
        {
            _signaler.UpdateIce(new IceData() { candidate = candidate.candidate, sdpMid = candidate.sdpMid, sdpMLineIndex = candidate.sdpMLineIndex });
        }
    }

    private void HandleSendSdpOffer(string id, VoiceCallSupportWebGL.SignalingSdpMsg.SdpData sdp)
    {
        if (_id == id)
        {
            _signaler.Publish(new SdpData() { type = sdp.type, sdp = sdp.sdp });
        }
    }

    public override void Dispose()
    {
        base.Dispose();

        if (_voiceCallSupportWebGL != null)
        {
            _voiceCallSupportWebGL.OnSendSdpOffer -= HandleSendSdpOffer;
            _voiceCallSupportWebGL.OnSendIceCandidate -= HandleSendIceCandidate;
            _voiceCallSupportWebGL.DestroyPeer(_id);
        }

        _cancellationTokenSource?.Dispose();
    }
}

#endif