using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Mirabo.VoiceCall;

public class VrgDownPeerSignaler : ISignaler, IDisposable
{
    public string _otherPeerId;
    ISignalingClientApi _roomClient;

    public VrgDownPeerSignaler(string otherPeerId, ISignalingClientApi roomClient)
    {
        _otherPeerId = otherPeerId;
        _roomClient = roomClient;

        _roomClient.OnSdpOffer += HandleOnSdpOffer;
        _roomClient.OnIceUpdate += HandleOnIceUpdate;
    }

    public event Action<SdpData> OnSdpAnswer;
    public event Action<IceData> OnIceUpdate;
    public event Action<SdpData> OnSdpOffer;


    void HandleOnIceUpdate(string id, IceData value)
    {
        if (id == _otherPeerId)
            OnIceUpdate?.Invoke(value);
    }

    void HandleOnSdpOffer(string id, SdpData value)
    {
        if (id == _otherPeerId)
            OnSdpOffer?.Invoke(value);
    }

    public void Answer(SdpData sdp)
    {
        _roomClient.SendAnswer(_otherPeerId, sdp);
    }

    public void Publish(SdpData sdp)
    {
        // DO NOTHING
    }

    public void UpdateIce(IceData ice)
    {
        _roomClient.SendIceUpdate(_otherPeerId, ice);
    }

    ~VrgDownPeerSignaler()
    {
        Dispose(false);
        GC.SuppressFinalize(this);
    }

    public void Dispose()
    {
        Dispose(true);
    }

    bool _disposed;

    public void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        _disposed = true;

        if (disposing)
        {
            if (_roomClient != null)
            {
                _roomClient.OnSdpOffer -= HandleOnSdpOffer;
                _roomClient.OnIceUpdate -= HandleOnIceUpdate;
            }
        }
    }
}
