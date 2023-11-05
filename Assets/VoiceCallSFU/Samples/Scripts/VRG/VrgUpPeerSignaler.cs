using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Mirabo.VoiceCall;

public class VrgUpPeerSignaler : ISignaler, IDisposable
{

    public string _peerId;
    ISignalingClientApi _roomClient;

    public VrgUpPeerSignaler(string peerId, ISignalingClientApi roomClient)
    {
        _peerId = peerId;
        _roomClient = roomClient;

        _roomClient.OnSdpAnswer += HandleOnSdpAnswer;
        _roomClient.OnIceUpdate += HandleOnIceUpdate;
    }

    public event Action<SdpData> OnSdpAnswer;
    public event Action<IceData> OnIceUpdate;
    public event Action<SdpData> OnSdpOffer;

    void HandleOnSdpAnswer(SdpData value)
    {
        OnSdpAnswer?.Invoke(value);
    }

    void HandleOnIceUpdate(string id, IceData value)
    {
        if (id == _peerId)
            OnIceUpdate?.Invoke(value);
    }


    public void Answer(SdpData sdp)
    {
        // DO NOTHING
    }

    public void Publish(SdpData sdp)
    {
        _roomClient.SendOffer(sdp);
        // DebugExtension.Log($"Publish:{JsonConvert.SerializeObject(sdp)}");
    }

    public void UpdateIce(IceData ice)
    {
        _roomClient.SendIceUpdate(_peerId, ice);
    }

    ~VrgUpPeerSignaler()
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
                _roomClient.OnSdpAnswer -= HandleOnSdpAnswer;
                _roomClient.OnIceUpdate -= HandleOnIceUpdate;
            }
        }
    }
}
