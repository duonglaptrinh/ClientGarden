using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;


namespace TWT.Networking.RealtimeMode
{
    public class PlayerStatus : MonoBehaviour
    {
        [SerializeField] private Image onlineImage;
        [SerializeField] private Image downloadSuccessIcon;
        [SerializeField] private Image selfIcon;

        private INetworkPlayer playerCurrent;

        public bool InUsing => playerCurrent != null;

        public int RoleIndex => InUsing ? playerCurrent.RoleIndex : -1;

        public void EnterRole(INetworkPlayer networkPlayer)
        {
            playerCurrent = networkPlayer;
        }

        private void Start()
        {
            if (GameContext.GameMode == GameMode.TrainingMode)
                downloadSuccessIcon.gameObject.SetActive(false);
        }

        private void Update()
        {
            if (playerCurrent != null)
            {
                onlineImage.color = playerCurrent.ColorUnique;
                onlineImage.gameObject.SetActive(true);
                if(GameContext.GameMode == GameMode.RealtimeMode)
                    downloadSuccessIcon.gameObject.SetActive(playerCurrent.IsReady);
                selfIcon.gameObject.SetActive(playerCurrent.IsLocalPlayer);
            }
        }

        public void LeaveRole()
        {
            playerCurrent = null;
            onlineImage.gameObject.SetActive(false);
            selfIcon.gameObject.SetActive(false);
            downloadSuccessIcon.gameObject.SetActive(false);
        }
    }
}