using System;
using System.Collections.Generic;
using System.Linq;
//using Data;
using jp.co.mirabo.Application.RoomManagement;
using NaughtyAttributes;
using SyncRoom.Schemas;
using UnityEngine;

namespace Player_Management
{
    public class PlayerManager : SingletonMonoBehavior<PlayerManager>
    {
        [InfoBox(
            "In the newest version, we don't need the multi play feature anymore, but for some reason, we should not remove this, so just disable this feature")]

        [SerializeField] private GameObject visitorPrefab;
        [SerializeField] private List<Sprite> playerPrefab;
        private static List<Sprite> listPlayerPrefab = new List<Sprite>();

        private readonly Dictionary<string, VrgVisitorController> _playerControllers =
            new Dictionary<string, VrgVisitorController>();

        public bool isPrivate;

        public static Sprite GetPlayerAvatar(int avatarIdx)
        {
            try
            {
                return listPlayerPrefab[avatarIdx];
            }
            catch (Exception e)
            {
                DebugExtension.LogWarning($"Can not get avatar of index: {avatarIdx}");
            }
            return null;
        }

        private void Start()
        {
            var textures = Resources.LoadAll("Textures", typeof(Sprite)).Cast<Sprite>().ToArray();
            foreach (var t in textures)
            {
                playerPrefab.Add(t);
            }

            listPlayerPrefab = playerPrefab;
            //isPrivate = AppRuntimeData.instance.RoomType == AppSemantics.RoomType.PRIVATE;
            if (isPrivate)
            {
                return;
            }
            DebugExtension.Log("-----------------------------------Player Manager Start");
            RoomManager.Instance.GameRoom.OnPublisherAddPlayer += AddPlayer;
            RoomManager.Instance.GameRoom.OnPublisherRemovePlayer += RemovePlayer;

            RoomManager.Instance.GameRoom.OnPublisherAddEnity += AddEntityPlayer;
            RoomManager.Instance.GameRoom.OnPublisherRemoveEnity += RemoveEntityPlayer;
        }
        public void CreateAllJoinedPlayer()
        {
            foreach (var player in RoomManager.Instance.GameRoom.GetUsers().Values)
            {
                AddPlayer(player, RoomManager.Instance.GameRoom.CheckIsLocalPlayer(player.sessionId));
            }
        }

        private void AddPlayer(Visitor player, bool isLocalPlayer)
        {
            if (isPrivate) return;
            VrgVisitorController playerController;
            var playerObject = Instantiate(visitorPrefab);
            playerController = playerObject.GetComponent<VrgVisitorController>();

            playerController.SetData(player, isLocalPlayer);
            playerController.SetEntity(player.entity);
            _playerControllers.Add(player.sessionId, playerController);
        }

        private void RemovePlayer(string id)
        {
            DebugExtension.Log(id + "    " + _playerControllers.ContainsKey(id));
            if (!_playerControllers.ContainsKey(id))
            {
                return;
            }
            _playerControllers[id].DestroyPlayer();
            _playerControllers.Remove(id);
        }

        void AddEntityPlayer(Entity entity)
        {
            DebugExtension.Log(entity.id);
            var sessionID = entity.id;
            if (_playerControllers.ContainsKey(sessionID))
            {
                _playerControllers[sessionID].SetEntity(entity);
            }
        }

        private void RemoveEntityPlayer(Entity entity) { }

        protected void OnDestroy()
        {
            if (!RoomManager.Instance || RoomManager.Instance.GameRoom == null || isPrivate)
            {
                return;
            }
            DebugExtension.Log("-----------------------------------Player Manager Out");
            RoomManager.Instance.GameRoom.OnPublisherAddPlayer -= AddPlayer;
            RoomManager.Instance.GameRoom.OnPublisherRemovePlayer -= RemovePlayer;

            RoomManager.Instance.GameRoom.OnPublisherAddEnity -= AddEntityPlayer;
            RoomManager.Instance.GameRoom.OnPublisherRemoveEnity -= RemoveEntityPlayer;
        }

        public string GetSessionId()
        {
            foreach (var player in _playerControllers)
            {
                var vrgVisitorController = player.Value;
                if (vrgVisitorController.isLocalPlayer)
                {
                    return player.Key;
                }
            }

            return null;
        }
    }
}