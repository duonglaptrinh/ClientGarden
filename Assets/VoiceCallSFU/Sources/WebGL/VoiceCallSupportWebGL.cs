using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace Mirabo.VoiceCall.WebGL
{
    public class VoiceCallSupportWebGL : MonoBehaviour
    {
        private static VoiceCallSupportWebGL _instance;
        public static VoiceCallSupportWebGL Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("VoiceCallSupportWebGL").AddComponent<VoiceCallSupportWebGL>();
                    GameObject.DontDestroyOnLoad(_instance.gameObject);
                }

                return _instance;
            }
        }

#if UNITY_WEBGL

        [DllImport("__Internal")]
        private static extern void CreatePeerFromOfferJs(string id, string sdpOffer, string config);

        [DllImport("__Internal")]
        private static extern void CreatePeerJs(string id, string config);

        [DllImport("__Internal")]
        private static extern void DestroyPeerJs(string id);

        [DllImport("__Internal")]
        private static extern void OnOpenVoiceCallJs();

        [DllImport("__Internal")]
        private static extern void OnCloseVoiceCallJs();

        [DllImport("__Internal")]
        private static extern void FeedAnswerJs(string id, string sdpAnswer);

        [DllImport("__Internal")]
        private static extern void FeedIceCandidateJs(string id, string iceCandidate);
#endif
        public event System.Action<string, SignalingSdpMsg.SdpData> OnSendSdpOffer;

        [System.Serializable]
        public class SignalingSdpMsg
        {
            [System.Serializable]
            public class SdpData
            {
                public string type;
                public string sdp;
            }

            public string id;
            public SdpData data;
        }

        [System.Serializable]
        public class SignalingIceCandidateMsg
        {
            [System.Serializable]
            public class CandidateData
            {
                public string candidate;
                public string sdpMid;
                public int sdpMLineIndex;
            }

            public string id;
            public CandidateData data;
        }

        public void SendSdpOffer(string msg)
        {
            Debug.Log($"Receive in Unity : {msg}");
            var sdpMessage = JsonConvert.DeserializeObject<SignalingSdpMsg>(msg);
            OnSendSdpOffer?.Invoke(sdpMessage.id, sdpMessage.data);
        }

        public event System.Action<string, SignalingSdpMsg.SdpData> OnSendSdpAnswer;
        public void SendSdpAnswer(string msg)
        {
            Debug.Log($"Receive in Unity : {msg}");
            var sdpMessage = JsonConvert.DeserializeObject<SignalingSdpMsg>(msg);
            OnSendSdpAnswer?.Invoke(sdpMessage.id, sdpMessage.data);
        }

        public event System.Action<string, SignalingIceCandidateMsg.CandidateData> OnSendIceCandidate;
        public void SendIceCandidate(string msg)
        {
            Debug.Log($"Receive in Unity : {msg}");
            var candidateMessage = JsonConvert.DeserializeObject<SignalingIceCandidateMsg>(msg);
            OnSendIceCandidate?.Invoke(candidateMessage.id, candidateMessage.data);
        }

        public HashSet<string> _peerIds = new HashSet<string>();

        public void CreatePeer(string id, IWebGLPeerConfig config)
        {
#if UNITY_WEBGL
            CreatePeerJs(id, config.GetJsonConfig());
#endif
        }

        public void CreatePeerFromOffer(string id, string offer, IWebGLPeerConfig config)
        {
#if UNITY_WEBGL
            CreatePeerFromOfferJs(id, offer, config.GetJsonConfig());
#endif
        }

        public void DestroyPeer(string id)
        {
#if UNITY_WEBGL
            DestroyPeerJs(id);
#endif
        }

        public void OpenVoiceCall()
        {
#if UNITY_WEBGL
            OnOpenVoiceCallJs();
#endif
        }

        public void CloseVoiceCall()
        {
#if UNITY_WEBGL
            OnCloseVoiceCallJs();
#endif
        }

        public void FeedAnswer(string id, string answer)
        {
#if UNITY_WEBGL
            FeedAnswerJs(id, answer);
#endif
        }

        public void FeedIceCandidate(string id, string answer)
        {
#if UNITY_WEBGL
            FeedIceCandidateJs(id, answer);
#endif
        }

        public void Created(string id)
        {
            OnPeerReady?.Invoke(id);
        }

        public event System.Action<string> OnPeerReady;
    }

    public interface IWebGLPeerConfig
    {
        string GetJsonConfig();
    }
}