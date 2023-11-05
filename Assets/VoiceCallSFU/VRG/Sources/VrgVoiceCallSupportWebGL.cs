using Game.Client;
using jp.co.mirabo.Application.RoomManagement;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class VrgVoiceCallSupportWebGL : MonoBehaviour
{
    public static VrgVoiceCallSupportWebGL Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

#if UNITY_WEBGL
    [DllImport("__Internal")]
    private static extern void SetBorderColorJs(string id, string color);
#endif

    Dictionary<string, string> _colorMap = new Dictionary<string, string>();

    public void UpdateBorderColor(string peerId, string colorStr)
    {
        DebugExtension.Log($"Update color of player {peerId} to {colorStr}");
        _colorMap[peerId] = colorStr;
        SetBorderColor(peerId, colorStr);
    }

    public static void SetBorderColor(string peerId, string colorStr)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SetBorderColorJs(peerId, colorStr);
#endif
    }

    [ContextMenu("ShowConfirmReload")]
    public void ShowConfirmReload()
    {
        PopupRuntimeManager.Instance.ShowPopup("カメラを再読み込みしますか？",
         onClickConfirm: () =>
         {
             //var voiceCallLoader = GameObject.FindObjectOfType<VoiceCallLoader>();
             //if (voiceCallLoader != null)
             //{
             //    voiceCallLoader.ReloadVoiceCallModule();
             //}
             /*
             var roomManager = RoomManager.Instance;
             if (roomManager != null)
             {
                 roomManager.ReJoinRoom();
             }
             */

             var voiceCallController = GameObject.FindObjectOfType<VoiceCallController>();
             if (voiceCallController != null)
             {
                 voiceCallController.ResetVoiceCall();
             }
         }, onClickCancel: () =>
         {

         });
    }

    public void LocalVoiceCallStatusUpdated(string status)
    {

        var voiceCallStatus = JsonConvert.DeserializeObject<VoiceCallStatus>(status);

        var voiceCallController = GameObject.FindObjectOfType<VoiceCallController>();
        if (voiceCallController != null)
        {
            voiceCallController.UpdateLocalStateCamMic(voiceCallStatus.IsVideoOn, voiceCallStatus.IsMicOn);
        }
    }

    public void UpdateRemoteVoiceCallStatus(string status)
    {
        var voiceCallStatus = JsonConvert.DeserializeObject<VoiceCallStatus>(status);

        var voiceCallController = GameObject.FindObjectOfType<VoiceCallController>();
        if (voiceCallController != null)
        {
            voiceCallController.UpdateRemoteStateCamMic(voiceCallStatus.Id, voiceCallStatus.IsVideoOn, voiceCallStatus.IsMicOn);
        }
    }

    [System.Serializable]
    public class VoiceCallStatus
    {
        public string Id;
        public bool IsMicOn;
        public bool IsVideoOn;
    }


}
