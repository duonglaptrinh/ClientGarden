using Game.Client.Extension;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class AutoSwitchControllerForMode : MonoBehaviour
{
    [SerializeField] private GameObject playerStatusPanel;
    [SerializeField] private GameObject video360RemotePanel;
    [SerializeField] private GameObject configBottomPanel;
    [SerializeField] private GameObject listCameraPanel;
    [SerializeField] private Button buttonExtension;
    [SerializeField] private GameObject buttonSelectMode;
    [SerializeField] private GameObject buttonShowVrObject;
    [SerializeField] private GameObject buttonFlickerVrObject;

    //[SerializeField] private CurvedUI.CurvedUISettings[] curvedUiSettings;

    private void Start()
    {
        buttonExtension.onClick.AddListener(() =>
        {
            return;
            switch (GameContext.GameMode)
            {
                case GameMode.None:
                    break;
                case GameMode.Editable:
                    break;
                case GameMode.TrainingMode:
                    video360RemotePanel.SetActive(true);
                    configBottomPanel.SetActive(true);
                    break;
                case GameMode.RealtimeMode:
                    configBottomPanel.SetActive(true);
                    listCameraPanel.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        });
        if(GameContext.GameMode == GameMode.RealtimeMode)
        {
            DisableOtherButtons();
        }
        EnableCurvedUi();
    }

    private void EnableCurvedUi()
    {
        //Observable.Timer(TimeSpan.FromSeconds(1))
        //    .Subscribe(_ => curvedUiSettings.ForEach(c => c.enabled = true));
    }

    private void DisableOtherButtons()
    {
        buttonFlickerVrObject.SetActive(false);
        buttonShowVrObject.SetActive(false);
    }
}