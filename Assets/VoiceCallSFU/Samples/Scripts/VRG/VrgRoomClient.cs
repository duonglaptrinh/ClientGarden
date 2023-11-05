using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Colyseus;
using System.Threading.Tasks;
using Colyseus.Schema;
using jp.co.mirabo.Application.RoomManagement;
using TWT.Model;
using LucidSightTools;
using Newtonsoft.Json;
using static UnityEngine.UIElements.UxmlAttributeDescription;
using SyncRoom.Schemas;
using static VrgSyncApi;
using System.Linq;

public class VrgRoomClient
{
    protected ColyseusClient _client;
    protected ColyseusRoom<VRGRoomState> _room;

    public bool Connected { get; private set; }
    public bool Connecting { get; private set; }

    string _roomName;
    Dictionary<string, object> _options;

    public event Action<VoiceCallUser> OnVoiceCallUsedAdded;
    public event Action<VoiceCallUser> OnVoiceCallUserRemoved;
    public event Action OnLeftRoom;
    public Action<Entity> OnPublisherRemoveEnity;
    public Action<Entity> OnPublisherAddEnity;
    public Action<Entity> OnAddEnityObject;
    public Action<Entity> OnPublisherUpdateEnity;

    public string SessionId { get; private set; }

    public VrgRoomClient(string endPoint)
    {
        _client = new ColyseusClient(endPoint);
    }

    public async Task ConnectAsync(string roomName, Dictionary<string, object> options = null, CancellationToken ct = default)
    {
        if (Connecting || Connected)
            return;

        try
        {
            _roomName = roomName;
            _options = options;
            Connecting = true;
            _room = await _client.JoinOrCreate<VRGRoomState>(_roomName, _options);

            SessionId = _room.SessionId;
            DebugExtension.Log($"Room session : {_room.SessionId}");
            // DebugExtension.LogError(_room.State.sync.vr_dome_list.items.Count);

            // _room.OnMessage<VoiceCallUser>("CLIENT_JOINED", OnVoiceCallUserJoin);
            // _room.OnMessage<VoiceCallUser>("CLIENT_LEAVE", OnVoiceCallUserLeft);
            // _room.OnMessage<Player>("UPDATE_NUM_CLIENTS", OnPlayerUpdate);

            //_room.OnMessage<SyncEvent>("EVENT_SYNC", @event =>
            // {
            //     DebugExtension.LogError($"Receive event: {@event.Command}");
            // });

            _room.OnLeave += OnLeave;

            _room.OnStateChange += OnChangeRoomState;
            _room.State.visitors.OnAdd += OnAddPlayer;
            _room.State.visitors.OnRemove += OnRemovePlayer;
            _room.State.entities.OnAdd += OnAddEntityHandler;
            _room.State.entities.OnRemove += OnRemoveEntityHandler;
            _room.State.entities.OnChange += OnUpdateEntityHandler;
            Connected = true;
        }
        catch (Exception ex)
        {
            DebugExtension.LogError(ex);
            throw ex;
        }
        finally
        {
            Connecting = false;
        }
    }

    public Action<VRContentData> OnRoomChangeState;
    public Action<Visitor, bool> OnPublisherAddPlayer;
    public Action<string> OnPublisherRemovePlayer;
    void OnChangeRoomState(VRGRoomState state, bool isFirstState)
    {
        if (isFirstState)
        {
            // DebugExtension.LogError("OnChangeRoomState: " + isFirstState + "  " + JsonConvert.SerializeObject(state));
            // VRContentData data = JsonUtility.FromJson<VRContentData>(state.gameData);
            DebugExtension.Log(JsonConvert.SerializeObject(state.sync));
            VRContentData data = state.sync.ToVRContentData();

            var currentDomeId = Mathf.RoundToInt(_room.State.sync.currentDomeId);
            var domeIdExists = _room.State.sync.vr_dome_list.items.FirstOrDefault(x => x.Value.dome_id == currentDomeId);

            // DebugExtension.LogError(_room.State.sync.vr_dome_list.items.Count);
            if (domeIdExists.Value == null && _room.State.sync.vr_dome_list.items.Count > 0)
            {
                currentDomeId = Mathf.RoundToInt(_room.State.sync.vr_dome_list.items[0].dome_id);
            }

            GameContext.CurrentIdDome = currentDomeId;

            // DebugExtension.LogError(JsonConvert.SerializeObject(data));
            OnRoomChangeState?.Invoke(data);
        }
    }

    public Dictionary<string, Visitor> GetUsers()
    {
        return Visitors;
    }

    public Dictionary<string, Visitor> Visitors { get; private set; } = new Dictionary<string, Visitor>();
    void OnAddPlayer(string key, Visitor player)
    {
        var userData = JsonConvert.DeserializeObject<UserVisitorData.UserData>(player.userData);
        DebugExtension.Log($"OnAddPlayer : {userData.themeColor}");
        OnPublisherAddPlayer?.Invoke(player, CheckIsLocalPlayer(player.sessionId));
        VrgVoiceCallSupportWebGL.Instance.UpdateBorderColor(player.sessionId, userData.themeColor);
        Visitors.Add(key, player);
    }

    void OnRemovePlayer(string key, Visitor player)
    {
        DebugExtension.Log($"OnRemovePlayer : {key}");
        Visitors.Remove(key);

        LSLog.Log($"OnRemovePlayer: {player.name}\n{JsonConvert.SerializeObject(player)}");

        OnPublisherRemovePlayer?.Invoke(player.sessionId);
    }

    public bool CheckIsLocalPlayer(string sessionId)
    {
        return sessionId.Equals(_room.SessionId);
    }

    void OnAddEntityHandler(string key, Entity entity)
    {
        if (Visitors.ContainsKey(entity.id))
        {
            Visitors[entity.id].entity = entity;
            OnPublisherAddEnity?.Invoke(entity);
        }
        else OnAddEnityObject?.Invoke(entity);
    }

    void OnRemoveEntityHandler(string key, Entity entity)
    {
        OnPublisherRemoveEnity?.Invoke(entity);
    }

    void OnUpdateEntityHandler(string key, Entity entity)
    {

    }

    void OnLeave(int code)
    {
        Connected = false;
        OnLeftRoom?.Invoke();
    }

    public void Send(string eventKey, object message)
    {
        if (Connected)
            _room.Send(eventKey, message).ConfigureAwait(false);
    }

    public void Register<T>(string eventKey, Action<T> handler)
    {
        _room.OnMessage<T>(eventKey, handler);
    }

    public void LeaveRoom()
    {
        LeaveRoomAsync();
    }

    public async Task LeaveRoomAsync()
    {
        if (_room != null)
        {
            await _room.Leave();
        }
    }
}
