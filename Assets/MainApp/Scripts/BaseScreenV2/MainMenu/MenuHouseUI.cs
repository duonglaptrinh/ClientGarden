using Cysharp.Threading.Tasks.Triggers;
using Game.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TWT.Networking;
using UnityEngine;
using UnityEngine.UI;

public class MenuHouseUI : MonoBehaviour
{
    public Action OnClickBtnListHouse = null;
    public Action OnClickBtnMaterial = null;
    public Action OnCloseMenu = null;

    [SerializeField] Button btnListHouse;
    [SerializeField] Button btnHouseMaterial;
    [SerializeField] Button btnSettingLand;

    [Header("GroupObject")]
    [SerializeField] GameObject groupButton;
    [SerializeField] GameObject groupGrid;

    [Header("Grid Menu")]
    [SerializeField] Button btnSetting;
    [SerializeField] Toggle toggleButtonGrid;
    [SerializeField] Toggle toggleButtonSnap;
    [SerializeField] UpDownButtonComponent txtFrontOf, txtBehide, txtLeft, txtright;

    // Start is called before the first frame update
    void Start()
    {
        btnListHouse.onClick.AddListener(() => { OnClickBtnListHouse?.Invoke(); });
        btnHouseMaterial.onClick.AddListener(() => { OnClickBtnMaterial?.Invoke(); });
        btnSettingLand.onClick.AddListener(ShowSettingLand);

        toggleButtonGrid.onValueChanged.AddListener(OnOffGridMode);
        toggleButtonSnap.onValueChanged.AddListener(OnOffSnapGridMode);

        txtFrontOf.onUp = OnUpLand;
        txtFrontOf.onDown = OnDownLand;

        txtBehide.onUp = OnUpLand;
        txtBehide.onDown = OnDownLand;

        txtLeft.onUp = OnUpLand;
        txtLeft.onDown = OnDownLand;

        txtright.onUp = OnUpLand;
        txtright.onDown = OnDownLand;
    }

    float CheckMinMax(bool isMax, float value)
    {
        if (isMax)
        {
            if (value > GameContext.Land_Count_Max) value = GameContext.Land_Count_Max;
        }
        else
        {
            if (value < GameContext.Land_Count_Min) value = GameContext.Land_Count_Min;
        }
        return value;
    }
    void OnUpLand(TypeLandDirection type)
    {
        float value = GetValue(type);
        value += GameContext.Land_Unit_count;
        value = CheckMinMax(isMax: true, value);
        UpdateTextLand(type, value);
        SendDataSync(type, value);
    }
    void OnDownLand(TypeLandDirection type)
    {
        float value = GetValue(type);
        value -= GameContext.Land_Unit_count;
        value = CheckMinMax(isMax: false, value);
        UpdateTextLand(type, value);
        SendDataSync(type, value);
    }
    float GetValue(TypeLandDirection type)
    {
        switch (type)
        {
            case TypeLandDirection.frontOf:
                return GameContext.Land_Setting_FrontOf;
            case TypeLandDirection.behide:
                return GameContext.Land_Setting_Behide;
            case TypeLandDirection.left:
                return GameContext.Land_Setting_Left;
            case TypeLandDirection.right:
                return GameContext.Land_Setting_Right;
        }
        return 0;
    }
    void SendDataSync(TypeLandDirection type, float value)
    {
        DebugExtension.LogError("Send Data: type = " + type.ToString() + "     value = " + value);

        VrgSyncApi.Send(new SyncLandHouseMessage()
        {
            idDome = VrDomeControllerV2.Instance.vrDomeData.dome_id,
            json = JsonUtility.ToJson(new JsonLandHouse(type, value))
        }, SyncLandHouseMessage.EventKey);
    }
    void UpdateTextLand(TypeLandDirection type, float value)
    {
        switch (type)
        {
            case TypeLandDirection.frontOf:
                txtFrontOf.text = value.ToString() + "m";
                break;
            case TypeLandDirection.behide:
                txtBehide.text = value.ToString() + "m";
                break;
            case TypeLandDirection.left:
                txtLeft.text = value.ToString() + "m";
                break;
            case TypeLandDirection.right:
                txtright.text = value.ToString() + "m";
                break;
        }
    }
    public void UpdateTextLand()
    {
        UpdateTextLand(TypeLandDirection.frontOf, GameContext.Land_Setting_FrontOf);
        UpdateTextLand(TypeLandDirection.behide, GameContext.Land_Setting_Behide);
        UpdateTextLand(TypeLandDirection.left, GameContext.Land_Setting_Left);
        UpdateTextLand(TypeLandDirection.right, GameContext.Land_Setting_Right);
    }
    private void OnEnable()
    {
        ShowGroupButton();
        UpdateTextLand();

        toggleButtonSnap.isOn = GameContext.IsSnapGrid;
        toggleButtonGrid.isOn = GameContext.IsGridMode;
    }
    public void OnOffSnapGridMode(bool isActive)
    {
        if (isActive == GameContext.IsSnapGrid) return;
        GameContext.IsSnapGrid = isActive;
    }
    public void OnOffGridMode(bool isActive)
    {
        if (isActive == GameContext.IsGridMode) return;
        GameContext.IsGridMode = isActive;
        VrDomeControllerV2.Instance.DomeLoadhouse.OnOffGridMode();
    }
    void ShowSettingLand()
    {
        groupButton.gameObject.SetActive(false);
        groupGrid.gameObject.SetActive(true);
    }
    public void ShowGroupButton()
    {
        groupButton.gameObject.SetActive(true);
        groupGrid.gameObject.SetActive(false);
    }
    public void CLoseAllMenu()
    {
        gameObject.SetActive(false);
        OnCloseMenu?.Invoke();
    }
}
