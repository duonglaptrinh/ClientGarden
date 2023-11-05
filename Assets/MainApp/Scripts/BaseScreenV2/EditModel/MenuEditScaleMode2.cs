using Game.Client;
using SyncRoom.Schemas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TWT.Networking;
using UnityEngine;
using UnityEngine.UI;

public enum ETypeSizeHWD
{
    Height = 2, //Y
    Width = 1,  // X
    Depth = 3  //Z
}
public class MenuEditScaleMode2 : MonoBehaviour
{
    public Action OnBackButtonClick = null;
    [SerializeField] protected private Button resetBtn;
    [SerializeField] protected private EditValueComponent editYHeight;
    [SerializeField] protected private EditValueComponent editXWidth;
    [SerializeField] protected private EditValueComponent editZDepth;
    [SerializeField] Button btnBackToMenuEdit;

    [Header("Toogle Unit")]
    [SerializeField] Toggle toggleMM;
    [SerializeField] Toggle toggleCM;
    [SerializeField] Toggle toggleM;

    VRObjectV2 vRObject;
    ProductSizeController sizeController;

    public const string Unit_MM = "mm";
    public const string Unit_CM = "cm";
    public const string Unit_M = "m";
    string key_unit = "Key_Size_Unit_Product";
    string unit
    {
        get => PlayerPrefs.GetString(key_unit, Unit_MM);
        set => PlayerPrefs.SetString(key_unit, value);
    }

    // Start is called before the first frame update
    void Start()
    {
        btnBackToMenuEdit.onClick.AddListener(() =>
        {
            OnBackButtonClick?.Invoke();
        });
        editYHeight.OnSendData = OnSendData;
        editXWidth.OnSendData = OnSendData;
        editZDepth.OnSendData = OnSendData;
        resetBtn.onClick.AddListener(ResetAll);

        toggleMM.onValueChanged.AddListener(isOn => { if (isOn) OnChangeUnit(Unit_MM); });
        toggleCM.onValueChanged.AddListener(isOn => { if (isOn) OnChangeUnit(Unit_CM); });
        toggleM.onValueChanged.AddListener(isOn => { if (isOn) OnChangeUnit(Unit_M); });
    }
    private void OnEnable()
    {
        if (unit == Unit_CM) toggleCM.isOn = true;
        else if (unit == Unit_MM) toggleMM.isOn = true;
        else if (unit == Unit_M) toggleM.isOn = true;
    }
    void OnChangeUnit(string type)
    {
        switch (type)
        {
            case Unit_MM:
                unit = Unit_MM;
                break;
            case Unit_CM:
                unit = Unit_CM;
                break;
            case Unit_M:
                unit = Unit_M;
                break;
        }
        editYHeight.UpdateUnit(unit);
        editXWidth.UpdateUnit(unit);
        editZDepth.UpdateUnit(unit);
    }
    public void SetupData(VRObjectV2 vRObject, float scaleX, float scaleY, float scaleZ, ProductSizeController sizeController)
    {
        this.vRObject = vRObject;
        this.sizeController = sizeController;
        editYHeight.Setup(ETypeSizeHWD.Height, sizeController.HeightList, scaleY, unit);
        editXWidth.Setup(ETypeSizeHWD.Width, sizeController.WidthList, scaleX, unit);
        editZDepth.Setup(ETypeSizeHWD.Depth, sizeController.DepthList, scaleZ, unit);
    }

    void ResetAll()
    {
        editYHeight.ResetData();
        editXWidth.ResetData();
        editZDepth.ResetData();
    }
    void OnSendData(ETypeSizeHWD type, float value)
    {
        SendElementScaleVrObject(value, (int)type);
    }
    public void SendElementScaleVrObject(float value, int index)
    {
        VrgSyncApi.Send(new SyncElementScaleVrObjectMessage()
        {
            idDome = VrDomeControllerV2.Instance.vrDomeData.dome_id,
            type = vRObject.Type,
            id = vRObject.Id,
            scale = value,
            index = index,
            localScale = VRObjectManagerV2.ConvertVector3ToString(
                new Vector3(editXWidth.CurrentValueData, editYHeight.CurrentValueData, editZDepth.CurrentValueData)
                )
        }, SyncElementScaleVrObjectMessage.EventKey);
    }
}
