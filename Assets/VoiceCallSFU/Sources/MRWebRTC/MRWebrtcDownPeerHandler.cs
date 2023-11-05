using Microsoft.MixedReality.WebRTC;
using Microsoft.MixedReality.WebRTC.Unity;
using Mirabo.VoiceCall;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using PeerConnection = Microsoft.MixedReality.WebRTC.Unity.PeerConnection;

public class MRWebrtcDownPeerHandler : DownPeerHandler
{
    PeerConnection _peerConnection;

    bool _initialized;

    public MRWebrtcDownPeerHandler(string id, ISignaler signaler, PeerConnection peerConnection) : base(id, signaler)
    {
        _peerConnection = GameObject.Instantiate(peerConnection);

        _peerConnection.OnInitialized.AddListener(() =>
        {
            _initialized = true;
        });

        Connect(_cancellationTokenSource.Token).Forget();
    }

    CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    async UniTask Connect(CancellationToken cancellationToken = default)
    {
        var readyToHandleIces = false;
        var receiveCandidateCache = new List<IceData>();
        var sendCandidateCache = new List<IceData>();

        _signaler.OnSdpOffer += async sdp =>
        {
            while (!_initialized)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await UniTask.Yield();
            }
            var sdpMessage = new SdpMessage() { Type = SdpMessageType.Offer, Content = sdp.sdp };
            if (_peerConnection != null)
                await _peerConnection.HandleConnectionMessageAsync(sdpMessage);

            _peerConnection.Peer.CreateAnswer();

            readyToHandleIces = true;
        };

        _signaler.OnIceUpdate += iceData =>
        {
            if (readyToHandleIces)
            {
                _peerConnection.Peer.AddIceCandidate(new IceCandidate() { Content = iceData.candidate, SdpMid = iceData.sdpMid, SdpMlineIndex = iceData.sdpMLineIndex });
            }
            else
            {
                receiveCandidateCache.Add(iceData);
            }
        };

        while (!_initialized)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await UniTask.Yield();
        }

        _peerConnection.Peer.IceStateChanged += (value =>
        {
            Debug.Log($"Downstream {_peerId} IceStateChanged :{value}");
        });

        _peerConnection.Peer.LocalSdpReadytoSend += async x =>
        {
            await UniTask.SwitchToMainThread();
            _signaler.Answer(new SdpData() { type = "answer", sdp = x.Content });
        };

        _peerConnection.Peer.IceCandidateReadytoSend += x =>
        {
            var ice = new IceData()
            {
                candidate = x.Content,
                sdpMid = x.SdpMid,
                sdpMLineIndex = x.SdpMlineIndex
            };
            if (readyToHandleIces)
                _signaler.UpdateIce(ice);
            else
            {
                sendCandidateCache.Add(ice);
            }
        };

        while (!readyToHandleIces)
        {
            await UniTask.Yield();
        }

        foreach (var item in receiveCandidateCache)
        {
            _peerConnection.Peer.AddIceCandidate(new IceCandidate() { Content = item.candidate, SdpMid = item.sdpMid, SdpMlineIndex = item.sdpMLineIndex });
        }

        foreach (var item in sendCandidateCache)
        {
            _signaler.UpdateIce(item);
        }

    }

    public override void Dispose()
    {
        base.Dispose();
        _cancellationTokenSource?.Cancel();

        if (_peerConnection != null)
        {
            GameObject.Destroy(_peerConnection.gameObject);
        }
    }
}
