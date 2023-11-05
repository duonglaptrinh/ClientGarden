using System;
using System.Linq;
using Game;
using Game.Client.Extension;
using UnityEngine;

namespace TWT.Networking.RealtimeMode
{
    public class PlayerStatusManager : MonoBehaviour
    {
        [SerializeField] private PlayerStatus[] playerStatuses;
        [SerializeField] private PlayerStatus[] playerStatusesInStudentControl;

        private void OnValidate()
        {
            if (playerStatuses.IsNullOrEmpty())
                playerStatuses = FindObjectsOfType<PlayerStatus>();
        }

        private void OnEnable()
        {
            //PlayerManager.Players.ForEach(OnNewPlayerAdded);
        }

        private void Awake()
        {
            PlayerManager.OnNewPlayerAdded += OnNewPlayerAdded;
            PlayerManager.OnRemovePlayer += OnRemovePlayer;
            if (!GameContext.IsTeacher)
                playerStatuses = playerStatusesInStudentControl;
        }

        private void OnNewPlayerAdded(INetworkPlayer player)
        {
            var seat = playerStatuses.FirstOrDefault(x => !x.InUsing);

            if (seat != null)
            {
                seat.EnterRole(player);
            }
        }

        private void OnRemovePlayer(INetworkPlayer player)
        {
            var seat = playerStatuses.FirstOrDefault(x => x.RoleIndex == player.RoleIndex);

            if (seat != null)
            {
                seat.LeaveRole();
            }
        }

        private void OnDisable()
        {
            //playerStatuses.ForEach(x => x.LeaveRole());
        }

        private void OnDestroy()
        {
            PlayerManager.OnNewPlayerAdded -= OnNewPlayerAdded;
            PlayerManager.OnRemovePlayer -= OnRemovePlayer;
        }
    }
}