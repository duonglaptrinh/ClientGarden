using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using LucidSightTools;
using NativeWebSocket;
using Newtonsoft.Json;
using SyncRoom.Schemas;
using TWT.Model;
using UnityEngine;
using Attribute = SyncRoom.Schemas.Attribute;

namespace jp.co.mirabo.Application.RoomManagement
{
    public partial class GameRoom : RoomBase<VRGRoomState>
    {
        public GameRoom(string endpoint) : base(endpoint)
        {
        }

        CancellationTokenSource joinRoomCancellation;
        public RoomData currentRoom = null;
        bool isAutoChangeScene = true;

        public Action<string> OnPublisherRemovePlayer;
        public Action<Visitor, bool> OnPublisherAddPlayer;
        public Action<VRContentData> OnRoomChangeState;

        public Action<Entity> OnPublisherRemoveEnity;
        public Action<Entity> OnPublisherAddEnity;
        public Action<Entity> OnAddEnityObject;
        public Action<Entity> OnPublisherUpdateEnity;

        Dictionary<string, Visitor> users = new Dictionary<string, Visitor>();

        public GameRoom() : base(string.Empty)
        {
        }

        public Dictionary<string, Visitor> GetUsers()
        {
            return users;
        }

        protected override void OnRegisterRoomHandlers()
        {
            RegisterRoomEvent();
        }

        void RegisterRoomEvent()
        {
            _room.OnLeave += OnLeaveRoom;
            _room.OnError += OnError;

            _room.OnJoin += OnJoin;

            _room.OnStateChange += OnChangeRoomState;

            _room.State.visitors.OnAdd += OnAddPlayer;
            _room.State.visitors.OnRemove += OnRemovePlayer;

            _room.State.entities.OnAdd += OnAddEntityHandler;
            _room.State.entities.OnRemove += OnRemoveEntityHandler;
            _room.State.entities.OnChange += OnUpdateEntityHandler;

        }

        void OnJoin()
        {
            DebugExtension.LogError("OnJoin: .........");

        }

        void OnChangeRoomState(VRGRoomState state, bool isFirstState)
        {
            if (isFirstState)
            {
                DebugExtension.LogError("OnChangeRoomState: " + isFirstState + "  " + JsonConvert.SerializeObject(state));
                //VRContentData data = JsonUtility.FromJson<VRContentData>(state.gameData);
                OnRoomChangeState?.Invoke(null);
                //OnRoomChangeState?.Invoke(data);
            }
        }

        void OnAddPlayer(string key, Visitor player)
        {
            LSLog.Log($"OnAddPlayer: {player.name}\n{JsonConvert.SerializeObject(player)}");

            OnPublisherAddPlayer?.Invoke(player, CheckIsLocalPlayer(player.sessionId));
            users.Add($"Player_{player.sessionId}", player);
        }

        void OnRemovePlayer(string key, Visitor player)
        {
            LSLog.Log($"OnRemovePlayer: {player.name}\n{JsonConvert.SerializeObject(player)}");

            OnPublisherRemovePlayer?.Invoke(player.sessionId);
            users.Remove($"Player_{player.sessionId}");
        }

        void OnAddEntityHandler(string key, Entity entity)
        {
            LSLog.Log($"OnAddEntityHandler: " + entity.id);
            if (users.ContainsKey(entity.id))
            {
                // users[entity.id].entity = entity;
                OnPublisherAddEnity?.Invoke(entity);
            }
            else OnAddEnityObject?.Invoke(entity);
        }

        void OnRemoveEntityHandler(string key, Entity entity)
        {
            LSLog.Log($"OnRemoveEntityHandler: {entity.id}\n{JsonConvert.SerializeObject(entity)}");
            OnPublisherRemoveEnity?.Invoke(entity);
        }

        void OnUpdateEntityHandler(string key, Entity entity)
        {
            LSLog.Log($"OnUpdateEntityHandler: {entity.id}\n{JsonConvert.SerializeObject(entity)}");
        }

        void OnUpdateAttributeHandler(string key, Attribute entity)
        {
            LSLog.Log($"OnUpdateAttributeHandler: ");
        }

        void OnLeaveRoom(int code)
        {
            WebSocketCloseCode closeCode = WebSocketHelpers.ParseCloseCodeEnum(code);
            LSLog.Log(string.Format("ROOM : ON LEAVE =- Reason: {0} ({1})", closeCode, code));

            users.Clear();

            if (UnityEngine.Application.isPlaying)
            {
                joinRoomCancellation?.Dispose();
            }
        }

        void OnError(int code, string message)
        {
            WebSocketCloseCode closeCode = WebSocketHelpers.ParseCloseCodeEnum(code);
            string err = string.Format("ROOM : ON ERROR =- Reason: {0} ({1})", closeCode, code);
            LSLog.Log(err);
        }

        public async UniTask JoinRoomAndSendDataAsync(RoomData room, Action onSuccess = null,
            Action<string> onError = null, bool isAutoChangeScene = true)
        {
            if (room == null)
            {
                string err = $"Room is null";
                DebugExtension.LogError(err);
                onError?.Invoke(err);
                // Popup.CreateYesOnly(err, null);
                return;
            }

            LSLog.Log($"Changing Room: {room.name}, id: {room.roomId}");

            this.isAutoChangeScene = isAutoChangeScene;
            currentRoom = room;

            Dictionary<string, object> options = new Dictionary<string, object>
            {
                ["username"] = "",
                ["lang"] = "en"
            };

            if (room.roomId != currentRoomId)
            {
                await JoinRoomAsync(room, onSuccess, onError, options)
                    .Timeout(TimeSpan.FromSeconds(defaultTimeoutInSec));
            }
            else onSuccess?.Invoke();
        }

        public bool CheckIsLocalPlayer(string sessionId)
        {
            DebugExtension.Log("CheckIsLocalPlayer: " + _room.SessionId);
            return sessionId.Equals(_room.SessionId);
        }

        public string GetSessionId()
        {
            DebugExtension.Log(_room.SessionId);
            return _room.SessionId;
        }
    }
}