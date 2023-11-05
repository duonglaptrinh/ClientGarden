using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TWT.Model;
using UnityEngine;

namespace jp.co.mirabo.Application.RoomManagement
{
    public class RoomManager : SingletonMonoBehaviour<RoomManager>
    {
        // public readonly GameRoom GameRoom = new GameRoom();
        public VrgRoomClient GameRoom { get; private set; }

        public static Action OnApplicationQuitEvent;
        public static Action OnGameRoomReady;
        public static Action<string> OnGameRoomError;
        public static Action<VRContentData> OnChangeRoomState;

        public static int mode;
        protected override void OnAwake()
        {

        }

        protected override void DoOnApplicationQuit()
        {
            base.DoOnApplicationQuit();

            OnApplicationQuitEvent?.Invoke();
        }

        (string roomName, Dictionary<string, object> options) _lastJoined;
        public void QuickConnect(string roomName, Dictionary<string, object> options)
        {
            _lastJoined = (roomName, options);
            GameRoom = new VrgRoomClient(RoomConfig.Room.mainRoomDomain);
            GameRoom.OnRoomChangeState += (data) => OnChangeRoomState?.Invoke(data);
            QuickConnectAsync(roomName, options).Forget();
        }

        public void ReJoinRoom()
        {
            ReJoinRoomAsync().Forget();
        }

        async UniTaskVoid ReJoinRoomAsync()
        {
            if (!string.IsNullOrEmpty(_lastJoined.roomName))
            {
                if (GameRoom != null)
                {
                    await GameRoom.LeaveRoomAsync();
                }

                SceneConfig.LoadScene(SceneConfig.Scene.BaseScreenV2);          
            }
        }

        public void Disconnect()
        {
            if (GameRoom != null)
                GameRoom.LeaveRoom();

            GameRoom = null;
        }

        protected override void DoOnDestroy()
        {
            Disconnect();
        }

        private static async UniTask QuickConnectAsync(string roomName, Dictionary<string, object> options)
        {
            try
            {
                await RoomManager.Instance.GameRoom.ConnectAsync(roomName, options);
                OnGameRoomReady?.Invoke();
            }
            catch (Exception ex)
            {
                OnGameRoomError?.Invoke(ex.Message);
            }
        }
    }
}