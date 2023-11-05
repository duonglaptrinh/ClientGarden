using Game.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TWT.Networking;
using UnityEngine;
using UnityEngine.UI;

public class VRObjectSizeSetting : MonoBehaviour
{
    protected float SIZE_MAX => GameContext.SIZE_MAX;
    protected float SIZE_MIN => GameContext.SIZE_MIN;

    [SerializeField] protected private Slider slider;
    [SerializeField] protected private Slider sliderX;
    [SerializeField] protected private Slider sliderY;
    [SerializeField] protected private Slider sliderZ;

    [SerializeField] protected private Button resetBtn;
    [SerializeField] protected private Button resetBtnX;
    [SerializeField] protected private Button resetBtnY;
    [SerializeField] protected private Button resetBtnZ;

    [SerializeField] protected private Text sizeText;
    [SerializeField] protected private Text sizeMaxText;
    [SerializeField] protected private Text sizeMinText;
    [SerializeField] protected private Text sizeXMaxText;
    [SerializeField] protected private Text sizeXMinText;
    [SerializeField] protected private Text sizeYMaxText;
    [SerializeField] protected private Text sizeYMinText;
    [SerializeField] protected private Text sizeZMaxText;
    [SerializeField] protected private Text sizeZMinText;
    [SerializeField] protected private Text txtSizeX;
    [SerializeField] protected private Text txtSizeY;
    [SerializeField] protected private Text txtSizeZ;

    [SerializeField] protected Button btnDecreaseX;
    [SerializeField] protected Button btnIncreaseX;
    [SerializeField] protected Button btnDecreaseY;
    [SerializeField] protected Button btnIncreaseY;
    [SerializeField] protected Button btnDecreaseZ;
    [SerializeField] protected Button btnIncreaseZ;

    protected VRObjectV2 vRObject;
    public Button btnBack;


    // Start is called before the first frame update
    protected virtual void Start()
    {
        btnIncreaseX.onClick.AddListener(delegate { ChangeValue(sliderX, txtSizeX, 1); });
        btnIncreaseY.onClick.AddListener(delegate { ChangeValue(sliderY, txtSizeY, 1); });
        btnIncreaseZ.onClick.AddListener(delegate { ChangeValue(sliderZ, txtSizeZ, 1); });
        btnDecreaseX.onClick.AddListener(delegate { ChangeValue(sliderX, txtSizeX, -1); });
        btnDecreaseY.onClick.AddListener(delegate { ChangeValue(sliderY, txtSizeY, -1); });
        btnDecreaseZ.onClick.AddListener(delegate { ChangeValue(sliderZ, txtSizeZ, -1); });
        resetBtn.onClick.AddListener(ResetSliderAll);
        resetBtnX.onClick.AddListener(delegate { ResetSlider(sliderX, 1); });
        resetBtnY.onClick.AddListener(delegate { ResetSlider(sliderY, 2); });
        resetBtnZ.onClick.AddListener(delegate { ResetSlider(sliderZ, 3); });
        //slider.onValueChanged.AddListener(ScaleVrObject);
        /*ScaleVrObject*/ //1:x, 2:y, 3:z
    }

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    public virtual void ResetSliderAll()
    {
        sliderX.value = 1;
        sliderY.value = 1;
        sliderZ.value = 1;
        slider.value = 1;
        //SendScaleVrObject(slider.value);
        SendElementScaleVrObject(sliderX.value, 1);
        SendElementScaleVrObject(sliderY.value, 2);
        SendElementScaleVrObject(sliderZ.value, 3);
    }
    public virtual void ResetSlider(Slider slider, int index)
    {
        slider.value = 1;
        SendElementScaleVrObject(slider.value, index);
    }

    public virtual void SetScale(VRObjectV2 vRObject, float scale)
    {
        this.vRObject = vRObject;
        slider.value = scale;
        sliderX = null;

        ScaleVrObject(scale);
        DebugExtension.Log("evaent ------------- " + slider.onValueChanged.GetPersistentEventCount());
        slider.onValueChanged.RemoveAllListeners();

        slider.onValueChanged.AddListener(ScaleVrObject);

        sizeMaxText.text = SIZE_MAX.ToString();
        sizeMinText.text = SIZE_MIN.ToString();
        slider.minValue = SIZE_MIN;
        slider.maxValue = SIZE_MAX;
    }

    public virtual void SetScale(VRObjectV2 vRObject, float scaleX, float scaleY, float scaleZ)
    {
        sliderX.onValueChanged.RemoveAllListeners();
        sliderY.onValueChanged.RemoveAllListeners();
        sliderZ.onValueChanged.RemoveAllListeners();
        slider.onValueChanged.RemoveAllListeners();
        DebugExtension.Log("evaent ------------- " + sliderX.onValueChanged.GetPersistentEventCount());
        this.vRObject = vRObject;
        sliderX.value = scaleX;
        sliderY.value = scaleY;
        sliderZ.value = scaleZ;

        ScaleXVrObject(scaleX);
        ScaleYVrObject(scaleY);
        ScaleZVrObject(scaleZ);

        sliderX.onValueChanged.AddListener(ScaleXVrObject);
        sliderY.onValueChanged.AddListener(ScaleYVrObject);
        sliderZ.onValueChanged.AddListener(ScaleZVrObject);


        sizeXMaxText.text = SIZE_MAX.ToString();
        sizeXMinText.text = SIZE_MIN.ToString();
        sizeYMaxText.text = SIZE_MAX.ToString();
        sizeYMinText.text = SIZE_MIN.ToString();
        sizeZMaxText.text = SIZE_MAX.ToString();
        sizeZMinText.text = SIZE_MIN.ToString();


        sliderX.minValue = SIZE_MIN;
        sliderX.maxValue = SIZE_MAX;
        sliderY.minValue = SIZE_MIN;
        sliderY.maxValue = SIZE_MAX;
        sliderZ.minValue = SIZE_MIN;
        sliderZ.maxValue = SIZE_MAX;
    }

    protected virtual void ScaleVrObject(float value)
    {
        vRObject.SetObjectScale(value);
        sizeText.text = value.ToString("0.0");
        //SendScaleVrObject(value);
    }
    protected void ScaleXVrObject(float value)
    {
        vRObject.SetObjectScale(value, 1);
        //sizeText.text = "X : " + value.ToString("0.0");
        txtSizeX.text = value.ToString("0.0");
        //SendElementScaleVrObject(value, 1);
    }

    protected void ScaleYVrObject(float value)
    {
        vRObject.SetObjectScale(value, 2);
        //sizeText.text = "Y : " + value.ToString("0.0");
        txtSizeY.text = value.ToString("0.0");
        //SendElementScaleVrObject(value, 2);
    }
    protected void ScaleZVrObject(float value)
    {
        vRObject.SetObjectScale(value, 3);
        //sizeText.text = "Z : " + value.ToString("0.0");
        txtSizeZ.text = value.ToString("0.0");
        //SendElementScaleVrObject(value, 3);
    }
    protected virtual void ScaleVrObject(float value, int index)
    {

        switch (index)
        {
            case 1:
                sizeText.text = "X : ";
                break;
            case 2:
                sizeText.text = "Y : ";
                break;
            case 3:
                sizeText.text = "Z : ";
                break;
            default:
                break;
        }
        vRObject.SetObjectScale(value, index);
        sizeText.text += value.ToString("0.0");
        SendElementScaleVrObject(value, index);
    }

    public void SendScaleVrObject(float value)
    {
        if (vRObject.Type == ObjectType.Model3D) return;
        VrgSyncApi.Send(new SyncScaleVrObjectMessage()
        {
            idDome = VrDomeControllerV2.Instance.vrDomeData.dome_id,
            type = vRObject.Type,
            id = vRObject.Id,
            scale = value
        });
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
            localScale = VRObjectManagerV2.ConvertVector3ToString(vRObject.transform.localScale)
        }, SyncElementScaleVrObjectMessage.EventKey);
    }
    public void ChangeValue(Slider slider, Text txtSize, int plusOrLess)
    {
        slider.value += 0.1f * plusOrLess;
        txtSize.text = slider.value.ToString("0.0");
    }
    public virtual void OnPointerUpAll()
    {
        //DebugExtension.LogError("OnPointer Up All 1111");
        SendScaleVrObject(slider.value);
    }
    public virtual void OnPointerUpZ()
    {
        //DebugExtension.LogError("OnPointer Up Z 1111");
        SendElementScaleVrObject(sliderZ.value, 3);
    }
    public virtual void OnPointerUpY()
    {
        //DebugExtension.LogError("OnPointer Up Y 1111");
        SendElementScaleVrObject(sliderY.value, 2);
    }
    public virtual void OnPointerUpX()
    {
        //DebugExtension.LogError("OnPointer Up X 1111");
        SendElementScaleVrObject(sliderX.value, 1);
    }
}
