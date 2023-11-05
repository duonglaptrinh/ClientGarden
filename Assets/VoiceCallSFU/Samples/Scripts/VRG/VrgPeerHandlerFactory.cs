using Microsoft.MixedReality.WebRTC.Unity;
using Mirabo.VoiceCall;
using Mirabo.VoiceCall.WebGL;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VrgPeerHandlerFactory : MonoBehaviour, IPeerHandlerFactory
{
    [SerializeField] PeerConnection _upPeerConnection;
    [SerializeField] PeerConnection _downPeerConnection;

#if UNITY_WEBGL
    VoiceCallSupportWebGL _voiceCallSupportWebGL;
#endif
    void Awake()
    {
#if UNITY_WEBGL
        _voiceCallSupportWebGL = VoiceCallSupportWebGL.Instance;
#endif
    }

    public DownPeerHandler CreateDownPeerHandler(string peerId, string username, string device, ISignaler signaler)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return new WebGLDownPeerHandler(peerId, signaler, _voiceCallSupportWebGL, GetPeerConfig(username,device));  
#endif
        return new MRWebrtcDownPeerHandler(peerId, signaler, _downPeerConnection);
    }

    public UpPeerHandler CreateUpPeerHandler(string peerId, string username, string device, ISignaler signaler)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return new WebGLUpPeerHandler(peerId, signaler, _voiceCallSupportWebGL, GetPeerConfig(username,device));  
#endif
        return new MRWebRTCUpPeerHandler(peerId, signaler, _upPeerConnection);
    }

    PeerConfig GetPeerConfig(string username, string device)
    {
        var iceServerList = new List<IceServerConfig>()
        {
             new IceServerConfig(){
                 urls = "stun:stun.l.google.com:19302"
             }
        };

        if (ConnectServer.Instance.TurnServerCredential != null)
        {
            foreach (var item in ConnectServer.Instance.TurnServerCredential.URIs)
            {
                iceServerList.Add(new IceServerConfig()
                {
                    urls = item,
                    username = ConnectServer.Instance.TurnServerCredential.Username,
                    credential = ConnectServer.Instance.TurnServerCredential.Password
                });
            }
        }

        // UnityEngine.DebugExtension.LogError(JsonConvert.SerializeObject(iceServerList));

        var config = new PeerConfig()
        {
            username = username,
            device = device,
            iceServers = iceServerList.ToArray()
        };

        return config;
    }


    [Serializable]
    public class PeerConfig : IWebGLPeerConfig
    {
        public string username;
        public string device;
        public IceServerConfig[] iceServers;

        public string GetJsonConfig()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    [Serializable]
    public class IceServerConfig
    {
        public string urls;
        public string username;
        public string credential;
    }
}
