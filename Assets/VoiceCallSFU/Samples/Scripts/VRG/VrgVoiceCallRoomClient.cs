using jp.co.mirabo.Application.RoomManagement;
using Mirabo.VoiceCall;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class VrgVoiceCallRoomClient : ISignalingClientApi
{
    // string endpoint = GameContext.LinkVoiceWebRTC;//"wss://webrtc.gld-lab.link:2567/";

    VrgRoomClient _roomClient;

    public event Action OnResetReady;
    public event Action<string, string, string> OnNewPublisher;
    public event Action<string, bool, bool> OnStateCamMic;
    public event Action<string> OnPublisherClosed;
    public event Action<SdpData> OnSdpAnswer;
    public event Action<string, SdpData> OnSdpOffer;
    public event Action<string, IceData> OnIceUpdate;

    public string PeerId { get; private set; }

    public async Task Connect(CancellationToken cancellationToken = default)
    {
        _roomClient = RoomManager.Instance.GameRoom;

        while (!_roomClient.Connected)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Yield();
        }

        PeerId = _roomClient.SessionId;

        _roomClient.Register<OnNewPublisherDto>("WEBRTC_NEW_PUBLISHER", HandleNewPublisher);
        _roomClient.Register<OnPublisherClosedDto>("WEBRTC_PUBLISHER_CLOSED", HandlePublisherClosed);
        _roomClient.Register<OnSdpAnswerDto>("WEBRTC_SDP_ANSWER", HandleSdpAnswer);
        _roomClient.Register<OnIceUpdateDto>("WEBRTC_ICE_UPDATE", HandleIceUpdate);
        _roomClient.Register<OnSdpOfferDto>("WEBRTC_SDP_OFFER", HandleSdpOffer);
        _roomClient.Register<OnStateCamMicDto>("UPDATE_STATE_CAM_MIC", HandleStateCamMic);
        _roomClient.Register<ReceiveStateCamMicDto>("GET_STATE_CAM_MIC", HandleReceiveStateCamMic);
        _roomClient.Register<object>("WEBRTC_RESET_CALL", _ =>
        {
            DebugExtension.Log("WEBRTC_RESET_CALL");
            OnResetReady?.Invoke();
        });

        GetCamMicStates();
    }

    public void ResetVoiceCall()
    {
        _roomClient?.Send("WEBRTC_RESET_CALL", new { });
    }

    void HandleStateCamMic(OnStateCamMicDto dto)
    {
        if (dto != null && dto.stateCamMic != null)
        {
            var item = dto.stateCamMic;
            DebugExtension.Log($"HandleReceiveStateCamMic : {item.publisherId}, {item.camera},{item.micro}");
            OnStateCamMic?.Invoke(item.publisherId, item.camera, item.micro);
        }
    }

    void HandleReceiveStateCamMic(ReceiveStateCamMicDto dto)
    {
        DebugExtension.Log($"HandleReceiveStateCamMic");

        if (dto != null && dto.users != null)
        {
            foreach (var item in dto.users)
            {
                OnStateCamMic?.Invoke(item.publisherId, item.camera, item.micro);
            }
        }
    }

    public void GetCamMicStates()
    {
        DebugExtension.Log($"Get cam mic states");

        _roomClient?.Send("GET_STATE_CAM_MIC", new { });
    }

    public void UpdateCamMicStatus(string publisherId, string username, bool camState, bool micState)
    {
        DebugExtension.Log($"Update state cam mic : {publisherId},{camState},{micState}");
        _roomClient?.Send("UPDATE_STATE_CAM_MIC", new StateCamMic() { publisherId = publisherId, username = username, camera = camState, micro = micState });
    }

    void HandleNewPublisher(OnNewPublisherDto dto)
    {
        DebugExtension.Log($"Detect new publisher {dto.publisherId}, {dto.username}");
        _roomClient.Send("WEBRTC_SUBSCRIBE", new SubcribeDto() { publisherId = dto.publisherId });

        OnNewPublisher?.Invoke(dto.publisherId, dto.username, dto.uiClientUnity.typeClient);
    }

    void HandlePublisherClosed(OnPublisherClosedDto dto)
    {
        DebugExtension.Log($"Detect publisher closed {dto.publisherId}");
        OnPublisherClosed?.Invoke(dto.publisherId);
    }

    void HandleSdpAnswer(OnSdpAnswerDto dto)
    {
        OnSdpAnswer?.Invoke(dto.sdp);
    }

    void HandleIceUpdate(OnIceUpdateDto dto)
    {
        OnIceUpdate?.Invoke(dto.publisherId, dto.ice);
    }

    void HandleSdpOffer(OnSdpOfferDto dto)
    {
        OnSdpOffer?.Invoke(dto.publisherId, dto.sdp);
    }

    public void SendAnswer(string otherPeerId, SdpData sdp)
    {
        _roomClient.Send("WEBRTC_SDP_ANSWER", new AnswerSdpDto() { publisherId = otherPeerId, sdp = sdp });
    }

    public void SendIceUpdate(string otherPeerId, IceData ice)
    {
        _roomClient.Send("WEBRTC_ICE_UPDATE", new UpdateIceDto() { publisherId = otherPeerId, ice = ice });
    }

    public void SendOffer(SdpData sdp)
    {
        DebugExtension.Log("WEBRTC_PUBLISH");
        _roomClient.Send("WEBRTC_PUBLISH", new PublishDto() { sdp = sdp });
    }
}
