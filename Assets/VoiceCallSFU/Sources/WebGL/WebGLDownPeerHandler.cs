using Mirabo.VoiceCall;
using Mirabo.VoiceCall.WebGL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading;
using Cysharp.Threading.Tasks;

#if UNITY_WEBGL
public class WebGLDownPeerHandler : DownPeerHandler
{
    VoiceCallSupportWebGL _voiceCallSupportWebGL;

    IWebGLPeerConfig _peerConfig;
    public WebGLDownPeerHandler(string id, ISignaler signaler, VoiceCallSupportWebGL voiceCallSupportWebGL, IWebGLPeerConfig peerConfig) : base(id, signaler)
    {
        _voiceCallSupportWebGL = voiceCallSupportWebGL;
        _peerConfig = peerConfig;

        Connect(_cancellationTokenSource.Token).Forget();
    }

    bool webrtcReady = false;

    CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    async UniTask Connect(CancellationToken cancellationToken = default)
    {
        SdpData sdpOffer = null;
        _signaler.OnSdpOffer += sdp =>
        {
            sdpOffer = sdp;
        };

        while (sdpOffer == null)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await UniTask.Yield();
        }

        var sdpData = new VoiceCallSupportWebGL.SignalingSdpMsg.SdpData() { type = sdpOffer.type, sdp = sdpOffer.sdp };
        _voiceCallSupportWebGL.CreatePeerFromOffer(_peerId, JsonConvert.SerializeObject(sdpData), _peerConfig);

        _voiceCallSupportWebGL.OnPeerReady += readyId =>
        {
            if (_peerId == readyId)
            {
                webrtcReady = true;
            }
        };

        _signaler.OnIceUpdate += async iceData =>
        {
            while (!webrtcReady)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.Yield();
            }

            var convertedIce = new VoiceCallSupportWebGL.SignalingIceCandidateMsg.CandidateData() { candidate = iceData.candidate, sdpMid = iceData.sdpMid, sdpMLineIndex = iceData.sdpMLineIndex };
            _voiceCallSupportWebGL.FeedIceCandidate(_peerId, JsonConvert.SerializeObject(convertedIce));
        };

        _voiceCallSupportWebGL.OnSendSdpAnswer += HandleSendSdpAnswer;
        _voiceCallSupportWebGL.OnSendIceCandidate += HandleSendIceCandidate;

        while (!webrtcReady)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await UniTask.Yield();
        }
    }

    private void HandleSendIceCandidate(string id, VoiceCallSupportWebGL.SignalingIceCandidateMsg.CandidateData candidate)
    {
        if (_peerId == id)
        {
            _signaler.UpdateIce(new IceData() { candidate = candidate.candidate, sdpMid = candidate.sdpMid, sdpMLineIndex = candidate.sdpMLineIndex });
        }
    }

    private void HandleSendSdpAnswer(string id, VoiceCallSupportWebGL.SignalingSdpMsg.SdpData sdp)
    {
        if (_peerId == id)
        {
            _signaler.Answer(new SdpData() { type = sdp.type, sdp = sdp.sdp });
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        if (_voiceCallSupportWebGL != null)
        {
            _voiceCallSupportWebGL.OnSendSdpAnswer -= HandleSendSdpAnswer;
            _voiceCallSupportWebGL.OnSendIceCandidate -= HandleSendIceCandidate;
            _voiceCallSupportWebGL.DestroyPeer(_peerId);
        }

        _cancellationTokenSource?.Dispose();
    }
}

#endif