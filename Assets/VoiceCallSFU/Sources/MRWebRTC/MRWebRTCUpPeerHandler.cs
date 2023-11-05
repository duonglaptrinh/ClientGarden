using Microsoft.MixedReality.WebRTC;
using Microsoft.MixedReality.WebRTC.Unity;
using Mirabo.VoiceCall;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using PeerConnection = Microsoft.MixedReality.WebRTC.Unity.PeerConnection;
using System;

public class MRWebRTCUpPeerHandler : UpPeerHandler
{
    PeerConnection _peerConnection;

    bool _initialized;

    public MRWebRTCUpPeerHandler(string id, ISignaler signaler, PeerConnection peerConnection) : base(signaler)
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
        while (!_initialized)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await UniTask.Yield();
        }

        var audioLine = _peerConnection.AddMediaLine(MediaKind.Audio);
        audioLine.Source = GameObject.FindObjectOfType<MicrophoneSource>();
        audioLine.Receiver = _peerConnection.GetComponent<AudioReceiver>();

        _peerConnection.Peer.IceStateChanged += (value =>
        {
            Debug.Log($"Upstream IceStateChanged :{value}");
        });

        var readyToHandleIces = false;
        var receiveCandidateCache = new List<IceData>();
        var sendCandidateCache = new List<IceData>();

        _peerConnection.Peer.LocalSdpReadytoSend += async x =>
        {
            await UniTask.SwitchToMainThread();
            _signaler.Publish(new SdpData() { type = "offer", sdp = x.Content });
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

        _signaler.OnSdpAnswer += async sdp =>
        {
            var sdpMessage = new SdpMessage() { Type = SdpMessageType.Answer, Content = sdp.sdp };
            try
            {
                await _peerConnection.HandleConnectionMessageAsync(sdpMessage);
            }
            catch (Exception ex)
            {
                // Debug.Log($"OnSdpAnswer Exception :{ex.Message}");
            }

            readyToHandleIces = true;
        };

        _signaler.OnIceUpdate += iceData =>
        {
            try
            {
                if (readyToHandleIces)
                {
                    _peerConnection.Peer.AddIceCandidate(new IceCandidate() { Content = iceData.candidate, SdpMid = iceData.sdpMid, SdpMlineIndex = iceData.sdpMLineIndex });
                }
                else
                {
                    receiveCandidateCache.Add(iceData);
                }
            }
            catch (Exception ex)
            {
                // Debug.Log($"OnIceUpdate Exception :{ex.Message}");
            }
        };

        _peerConnection.StartConnection();

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
