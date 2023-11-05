using Mirabo.VoiceCall;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Threading;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

using Cysharp.Threading.Tasks;

public class VoiceCallController : MonoBehaviour
{
    IPeerHandlerFactory _peerHandlerFactory;

    UpPeerHandler _upPeerHandler;
    Dictionary<string, DownPeerHandler> _downPeerHandlers = new Dictionary<string, DownPeerHandler>();
    Dictionary<string, VoiceCallStatus> _status = new Dictionary<string, VoiceCallStatus>();

    VrgVoiceCallRoomClient _roomClient;

    private void Awake()
    {
        _peerHandlerFactory = GetComponent<VrgPeerHandlerFactory>();
    }

    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
    {
        DebugExtension.Log($"{permissionName} PermissionDeniedAndDontAskAgain");
    }

    internal void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        DebugExtension.Log($"{permissionName} PermissionCallbacks_PermissionGranted");
    }

    internal void PermissionCallbacks_PermissionDenied(string permissionName)
    {
        DebugExtension.Log($"{permissionName} PermissionCallbacks_PermissionDenied");
    }

    private async void Start()
    {
        //if (Mirror.NetworkManager.singleton != null)
        //    await UniTask.WaitUntil(() => Mirror.NetworkClient.isConnected && Mirror.NetworkClient.localPlayer != null, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());

#if UNITY_ANDROID && !UNITY_EDITOR

            if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                // The user authorized use of the microphone.
            }
            else
            {
                bool useCallbacks = false;
                if (!useCallbacks)
                {
                    // We do not have permission to use the microphone.
                    // Ask for permission or proceed without the functionality enabled.
                    Permission.RequestUserPermission(Permission.Microphone);
                }
                else
                {
                    var callbacks = new PermissionCallbacks();
                    callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
                    callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
                    callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
                    Permission.RequestUserPermission(Permission.Microphone, callbacks);
                }
            }
            
            await UniTask.WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.Microphone));
#endif

        username = PlayerPrefs.GetString(TWT.Utility.PlayerPrefsConstant.PLAYER_NAME);
        _roomClient = new VrgVoiceCallRoomClient();
        await _roomClient.Connect();

#if UNITY_WEBGL && !UNITY_EDITOR
        Mirabo.VoiceCall.WebGL.VoiceCallSupportWebGL.Instance.OpenVoiceCall();
#endif
        SubcribeEvents();
        CreatePeerHandlers();
    }

    void CreatePeerHandlers()
    {
        publisherId = _roomClient.PeerId;
        _roomClient.GetCamMicStates();

        CreateUpPeerHandler();
    }

    private void SubcribeEvents()
    {
        _roomClient.OnNewPublisher += HandleNewPublisher;
        _roomClient.OnPublisherClosed += HandlePublisherClosed;
        _roomClient.OnStateCamMic += HandleStateCamMic;
        _roomClient.OnResetReady += HandleServerNotified;
    }

    [ContextMenu("ResetVoiceCall")]
    public void ResetVoiceCall()
    {
        if (_resetting)
            return;
        ReloadVoiceCallAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    void HandleServerNotified()
    {
        _hasServerResetted = true;
    }

    bool _hasServerResetted = false;

    bool _resetting = false;
    async UniTask ReloadVoiceCallAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _hasServerResetted = false;
            _resetting = true;
            Cleanup();
            SubcribeEvents();
            _roomClient.ResetVoiceCall();

            await UniTask.WaitUntil(() => _hasServerResetted, PlayerLoopTiming.Update, cancellationToken);

#if UNITY_WEBGL && !UNITY_EDITOR
        Mirabo.VoiceCall.WebGL.VoiceCallSupportWebGL.Instance.OpenVoiceCall();
#endif
            CreatePeerHandlers();
        }
        finally
        {
            _resetting = false;
        }
    }

    void HandleStateCamMic(string id, bool cam, bool mic)
    {
        var status = new VoiceCallStatus { IsMicOn = mic, IsVideoOn = cam };
        SetVoiceCallStatus(id, status);
        _status[id] = status;
    }

    public void UpdateLocalStateCamMic(bool cam, bool mic)
    {
        _roomClient?.UpdateCamMicStatus(publisherId, username, cam, mic);
    }

    public void UpdateRemoteStateCamMic(string id, bool cam, bool mic)
    {
        _roomClient?.UpdateCamMicStatus(id, username, cam, mic);
    }

    [System.Serializable]
    public class VoiceCallStatus
    {
        public bool IsMicOn;
        public bool IsVideoOn;
    }

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SetVoiceCallStatusJs(string id, string status);
#endif

    public void SetVoiceCallStatus(string id, VoiceCallStatus status)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SetVoiceCallStatusJs(id, JsonConvert.SerializeObject(status));  
#endif
    }

    string publisherId;
    string username;

    [ContextMenu("ReloadUpPeerhandler")]
    public void CreateUpPeerHandler()
    {
        _upPeerHandler?.Dispose();
        var singaler = new VrgUpPeerSignaler(publisherId, _roomClient);
        var device = "Desktop";
#if UNITY_WEBGL
        device = "WebGL";
# elif UNITY_ANDROID
        device = "Quest";
#endif
        _upPeerHandler = _peerHandlerFactory.CreateUpPeerHandler(publisherId, username, device, singaler);
    }

    string GetRandomId()
    {
        return Guid.NewGuid().ToString();
    }

    void HandleNewPublisher(string publisherId, string username, string device)
    {
        if (_downPeerHandlers.ContainsKey(publisherId))
        {
            _downPeerHandlers[publisherId].Dispose();
            _downPeerHandlers.Remove(publisherId);
        }

        var signaler = new VrgDownPeerSignaler(publisherId, _roomClient);
        _downPeerHandlers.Add(publisherId, _peerHandlerFactory.CreateDownPeerHandler(publisherId, username, device, signaler));
        if (_status.ContainsKey(publisherId) && _status[publisherId] != null)
        {
            SetVoiceCallStatus(publisherId, _status[publisherId]);
        }
    }

    void HandlePublisherClosed(string publisherId)
    {
        if (_downPeerHandlers.ContainsKey(publisherId))
        {
            _downPeerHandlers[publisherId].Dispose();
            _downPeerHandlers.Remove(publisherId);
        }

        _status.Remove(publisherId);
    }

    private void OnDestroy()
    {
        Cleanup();
    }

    void Cleanup()
    {
        if (_roomClient != null)
        {
            _roomClient.OnNewPublisher -= HandleNewPublisher;
            _roomClient.OnPublisherClosed -= HandlePublisherClosed;
            _roomClient.OnStateCamMic -= HandleStateCamMic;
            _roomClient.OnResetReady -= HandleServerNotified;
        }

        // _roomClient?.Disconnect();
        _upPeerHandler?.Dispose();

        foreach (var item in _downPeerHandlers)
        {
            item.Value?.Dispose();
        }

        _downPeerHandlers.Clear();

#if UNITY_WEBGL && !UNITY_EDITOR
        Mirabo.VoiceCall.WebGL.VoiceCallSupportWebGL.Instance.CloseVoiceCall();
#endif
    }
}

[Serializable]
public class UpdateIceDto
{
    public string publisherId;
    public IceData ice;
}

[Serializable]
public class PublishDto
{
    public SdpData sdp;
}

[Serializable]
public class AnswerSdpDto
{
    public SdpData sdp;
    public string publisherId;
}

[Serializable]
public class OnNewPublisherDto
{
    public string publisherId;
    public string username;
    public UIClientUnity uiClientUnity;
}

[Serializable]
public class OnStateCamMicDto
{
    public StateCamMic stateCamMic;
}

[Serializable]
public class StateCamMic
{
    public string username;
    public string publisherId;
    public bool camera;
    public bool micro;
}

[Serializable]
public class ReceiveStateCamMicDto
{
    public List<StateCamMic> users;
}

[Serializable]
public class UIClientUnity
{
    public string mirrorId;
    public string avatarBorderColor;
    public string typeClient;
}

[Serializable]
public class OnPublisherClosedDto
{
    public string publisherId;
}

[Serializable]
public class SubcribeDto
{
    public string publisherId;
}

[Serializable]
public class OnSdpAnswerDto
{
    public SdpData sdp;
}

[Serializable]
public class OnIceUpdateDto
{
    public string publisherId;
    public IceData ice;
}

public class OnSdpOfferDto
{
    public string publisherId;
    public SdpData sdp;
}