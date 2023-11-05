using Game.Client;
using System;
using System.Collections.Generic;
using TWT.Model;
using TWT.Networking;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class VRObjectRotationSetting : MonoBehaviour
{
    [Header("UI Comps")]
    [Header("X")]
    [SerializeField] protected Button buttonDecreaseValueX;
    [SerializeField] protected Button buttonDecreaseX;
    [SerializeField] protected Button buttonIncreaseX;
    [SerializeField] protected Button buttonIncreaseValueX;
    [SerializeField] protected Button buttonResetValueX;
    [SerializeField] protected Text textRotationValueX;
    [Header("Y")]
    [SerializeField] protected Button buttonDecreaseValueY;
    [SerializeField] protected Button buttonDecreaseY;
    [SerializeField] protected Button buttonIncreaseY;
    [SerializeField] protected Button buttonIncreaseValueY;
    [SerializeField] protected Button buttonResetValueY;
    [SerializeField] protected Text textRotationValueY;
    [Header("Z")]
    [SerializeField] protected Button buttonDecreaseValueZ;
    [SerializeField] protected Button buttonDecreaseZ;
    [SerializeField] protected Button buttonIncreaseZ;
    [SerializeField] protected Button buttonIncreaseValueZ;
    [SerializeField] protected Button buttonResetValueZ;
    [SerializeField] protected Text textRotationValueZ;

    [Header("All")]
    [SerializeField] protected Button buttonResetValue;
    public Button btnBack;
    float maxValue = 180;
    float minValue = -180;

    [Header("Slide")]
    [SerializeField] protected private Slider sliderX;
    [SerializeField] protected private Slider sliderY;
    [SerializeField] protected private Slider sliderZ;
    [Header("Customizable")]
    [SerializeField] protected float changeRate = 1f;
    [SerializeField] protected float addValue = 90f;

    [Header("Other Ref")]
    protected Transform currentEditedVRObjectTransform;
    protected List<IDisposable> listDisposables;
    protected IDisposable rotationUpdateDisposable;
    protected Vector3 rotationValue;
    protected string resetValue;
    protected VRObjectV2 vrObject;


    private void Update()
    {
        //ChangeValueSlider();
        //DebugExtension.LogError(rotationValue);
    }
    protected void AddEvent()
    {
        //X Axis
        //buttonDecreaseValueX.GetComponentInChildren<Text>().text = "-" + addValue;
        //buttonIncreaseValueX.GetComponentInChildren<Text>().text = "+" + addValue;
        buttonDecreaseValueX.onClick.AddListener(() => AddValueX(-addValue));
        buttonIncreaseValueX.onClick.AddListener(() => AddValueX(addValue));
        buttonResetValueX.onClick.AddListener(ResetValueX);

        //Y Axis
        //buttonDecreaseValueY.GetComponentInChildren<Text>().text = "-" + addValue;
        //buttonIncreaseValueY.GetComponentInChildren<Text>().text = "+" + addValue;
        buttonDecreaseValueY.onClick.AddListener(() => AddValueY(-addValue));
        buttonIncreaseValueY.onClick.AddListener(() => AddValueY(addValue));
        buttonResetValueY.onClick.AddListener(ResetValueY);

        //Z Axis
        //buttonDecreaseValueZ.GetComponentInChildren<Text>().text = "-" + addValue;
        //buttonIncreaseValueZ.GetComponentInChildren<Text>().text = "+" + addValue;
        buttonDecreaseValueZ.onClick.AddListener(() => AddValueZ(-addValue));
        buttonIncreaseValueZ.onClick.AddListener(() => AddValueZ(addValue));
        buttonResetValueZ.onClick.AddListener(ResetValueZ);

        //Reset All
        buttonResetValue.onClick.AddListener(ResetValue);
    }

    protected void SetLisenerSlider()
    {
        sliderX.onValueChanged.AddListener(OnValueChange);
        sliderY.onValueChanged.AddListener(OnValueChange);
        sliderZ.onValueChanged.AddListener(OnValueChange);
    }
    protected void RemoveLisenerSlider()
    {
        sliderX.onValueChanged.RemoveAllListeners();
        sliderY.onValueChanged.RemoveAllListeners();
        sliderZ.onValueChanged.RemoveAllListeners();
    }

    protected virtual void SetMinMaxSlider()
    {
        sliderX.minValue = minValue;
        sliderX.maxValue = maxValue;

        sliderY.minValue = minValue;
        sliderY.maxValue = maxValue;

        sliderZ.minValue = minValue;
        sliderZ.maxValue = maxValue;
    }
    void OnValueChange(float value)
    {
        //DebugExtension.LogError("OnChangeValue");
        ChangeValueSlider();
    }
    string formatText(float num)
    {
        return string.Format("{0:0.00}", num);
    }
    float formatSliderValue(float num)
    {
        return num >= 180 ? num - 360 : num;
    }
    void SetSlideX()
    {
        sliderX.value = formatSliderValue(rotationValue.x);
        textRotationValueX.text = formatText(sliderX.value);
    }
    void SetSlideY()
    {
        sliderY.value = formatSliderValue(rotationValue.y);
        textRotationValueY.text = formatText(sliderY.value);
    }
    void SetSlideZ()
    {
        sliderZ.value = formatSliderValue(rotationValue.z);
        textRotationValueZ.text = formatText(sliderZ.value);
    }
    float CheckMaxMin(float value)
    {
        if (value >= maxValue)
            value = maxValue;
        else if (value <= minValue)
            value = minValue;
        return value;
    }

    protected virtual int intGetValue => 45;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        addValue = intGetValue;
        AddEvent();
        // Setup button event
        listDisposables = new List<IDisposable>(12);
        // Button -X Press
        listDisposables.Add(buttonDecreaseX.targetGraphic.OnPointerDownAsObservable().Subscribe(_ => { OnRotationValueChanged(-1, RotationAxis.X); }));
        // Button -X Release
        listDisposables.Add(buttonDecreaseX.targetGraphic.OnPointerUpAsObservable().Subscribe(_ => { OnRotationValueStopChanging(); }));
        // Button +X Press
        listDisposables.Add(buttonIncreaseX.targetGraphic.OnPointerDownAsObservable().Subscribe(_ => { OnRotationValueChanged(1, RotationAxis.X); }));
        // Button +X Release
        listDisposables.Add(buttonIncreaseX.targetGraphic.OnPointerUpAsObservable().Subscribe(_ => { OnRotationValueStopChanging(); }));

        // Button -Y Press
        listDisposables.Add(buttonDecreaseY.targetGraphic.OnPointerDownAsObservable().Subscribe(_ => { OnRotationValueChanged(-1, RotationAxis.Y); }));
        // Button -Y Release
        listDisposables.Add(buttonDecreaseY.targetGraphic.OnPointerUpAsObservable().Subscribe(_ => { OnRotationValueStopChanging(); }));
        // Button +Y Press
        listDisposables.Add(buttonIncreaseY.targetGraphic.OnPointerDownAsObservable().Subscribe(_ => { OnRotationValueChanged(1, RotationAxis.Y); }));
        // Button +Y Release
        listDisposables.Add(buttonIncreaseY.targetGraphic.OnPointerUpAsObservable().Subscribe(_ => { OnRotationValueStopChanging(); }));

        // Button -Z Press
        listDisposables.Add(buttonDecreaseZ.targetGraphic.OnPointerDownAsObservable().Subscribe(_ => { OnRotationValueChanged(-1, RotationAxis.Z); }));
        // Button -Z Release
        listDisposables.Add(buttonDecreaseZ.targetGraphic.OnPointerUpAsObservable().Subscribe(_ => { OnRotationValueStopChanging(); }));
        // Button +Z Press
        listDisposables.Add(buttonIncreaseZ.targetGraphic.OnPointerDownAsObservable().Subscribe(_ => { OnRotationValueChanged(1, RotationAxis.Z); }));
        // Button +Z Release
        listDisposables.Add(buttonIncreaseZ.targetGraphic.OnPointerUpAsObservable().Subscribe(_ => { OnRotationValueStopChanging(); }));
    }

    protected virtual void OnRotationValueChanged(int sign, RotationAxis rotationAxis)
    {
        if (rotationUpdateDisposable == null && currentEditedVRObjectTransform != null)
        {
            rotationUpdateDisposable = Observable.EveryUpdate().Subscribe(_ =>
            {
                //DebugExtension.LogError("Evevry time update");
                switch (rotationAxis)
                {
                    case RotationAxis.X:
                        rotationValue.x += sign * changeRate * Time.deltaTime;
                        rotationValue.x = CheckMaxMin(rotationValue.x);
                        textRotationValueX.text = formatText(sliderX.value);
                        break;
                    case RotationAxis.Y:
                        rotationValue.y += sign * changeRate * Time.deltaTime;
                        rotationValue.y = CheckMaxMin(rotationValue.y);
                        textRotationValueY.text = formatText(sliderY.value);
                        break;
                    case RotationAxis.Z:
                        rotationValue.z += sign * changeRate * Time.deltaTime;
                        rotationValue.z = CheckMaxMin(rotationValue.z);
                        textRotationValueZ.text = formatText(sliderZ.value);
                        break;
                }
                currentEditedVRObjectTransform.localEulerAngles = rotationValue;
                SendSyncTransform();
            });
        }
    }

    protected virtual void OnRotationValueStopChanging()
    {
        rotationUpdateDisposable.Dispose();
        rotationUpdateDisposable = null;
    }

    protected virtual void OnDestroy()
    {
        //DebugExtension.LogError("OnDestroy");
        for (int i = 0; i < listDisposables.Count; i++)
            listDisposables[i].Dispose();
        listDisposables.Clear();
        listDisposables = null;

        if (rotationUpdateDisposable != null)
        {
            rotationUpdateDisposable.Dispose();
            rotationUpdateDisposable = null;
        }

        currentEditedVRObjectTransform = null;
    }


    public virtual void SetVRObjectToEdit(VRObjectV2 vrObject)
    {
        this.vrObject = vrObject;

        SetVRObjectToEdit(vrObject.transform);
    }
    public virtual void SetVRObjectToEdit(VRModelV2 vrObject)
    {
        SetMinMaxSlider();
        RemoveLisenerSlider();
        //DebugExtension.LogError("Setvr Edit VRModelV2");
        this.vrObject = vrObject;
        VRModelV2 vr = (VRModelV2)vrObject;
        if (vr && vr.GetData() != null)
        {
            resetValue = vr.GetData().model_rotation;
        }
        SetVRObjectToEdit(vrObject.transform);
        SetLisenerSlider();
    }
    public virtual void SetVRObjectToEdit(Transform transform, string valueReset)
    {
        resetValue = valueReset;
        SetVRObjectToEdit(transform);
    }
    public virtual void SetVRObjectToEdit(Transform trans)
    {
        //DebugExtension.LogError("Set vr Edit Transform");
        currentEditedVRObjectTransform = trans;
        //currentEditedVRObjectTransform.localEulerAngles = VRObjectManagerV2.ConverStringToVector3(resetValue);
        if (currentEditedVRObjectTransform != null)
        {
            rotationValue = currentEditedVRObjectTransform.localEulerAngles;
            SetSlideX();
            SetSlideY();
            SetSlideZ();
        }
    }

    public virtual void ResetValue()
    {
        RemoveLisenerSlider();
        //DebugExtension.LogError("Reset All");
        rotationValue = Vector3.zero;
        SetSlideX();
        SetSlideY();
        SetSlideZ();
        currentEditedVRObjectTransform.localEulerAngles = rotationValue;
        SendSyncTransform();
        SetLisenerSlider();
    }

    public virtual void ResetValueX()
    {
        //DebugExtension.LogError("ResetX");
        rotationValue.x = 0;
        SetSlideX();
        currentEditedVRObjectTransform.localEulerAngles = rotationValue;
        SendSyncTransform();
    }
    public virtual void ResetValueY()
    {
        //DebugExtension.LogError("ResetY");
        rotationValue.y = 0;
        SetSlideY();
        currentEditedVRObjectTransform.localEulerAngles = rotationValue;
        SendSyncTransform();
    }
    public virtual void ResetValueZ()
    {
        //DebugExtension.LogError("ResetZ");
        rotationValue.z = 0;
        SetSlideZ();
        currentEditedVRObjectTransform.localEulerAngles = rotationValue;
        SendSyncTransform();
    }
    public virtual void AddValueX(float value)
    {
        //DebugExtension.LogError("AddValueX");
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            rotationValue.x += value;
            rotationValue.x = CheckMaxMin(rotationValue.x);
            currentEditedVRObjectTransform.localEulerAngles = rotationValue;
            SendSyncTransform();
            SetSlideX();
        }
    }
    public virtual void AddValueY(float value)
    {
        //DebugExtension.LogError("AddValueY");
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            rotationValue.y += value;
            rotationValue.y = CheckMaxMin(rotationValue.y);
            currentEditedVRObjectTransform.localEulerAngles = rotationValue;
            SendSyncTransform();
            SetSlideY();
        }
    }
    public virtual void AddValueZ(float value)
    {
        //DebugExtension.LogError("AddValueZ");
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            rotationValue.z += value;
            rotationValue.z = CheckMaxMin(rotationValue.z);
            currentEditedVRObjectTransform.localEulerAngles = rotationValue;
            SendSyncTransform();
            SetSlideZ();
        }
    }

    protected enum RotationAxis : byte
    {
        X, Y, Z
    }

    protected void SendSyncTransform()
    {
        if (vrObject == null) return;
        VrgSyncApi.Send(new SyncTranformVrObjectMessage()
        {
            idDome = VrDomeControllerV2.Instance.vrDomeData.dome_id,
            type = vrObject.Type,
            id = vrObject.Id,
            rotation = VRObjectManagerV2.ConvertVector3ToString(currentEditedVRObjectTransform.localEulerAngles),
            translate = VRObjectManagerV2.ConvertVector3ToString(currentEditedVRObjectTransform.localPosition),
        }, SyncTranformVrObjectMessage.EventKey);

        //DebugExtension.LogError(VRObjectManagerV2.ConvertVector3ToString(currentEditedVRObjectTransform.localEulerAngles));
    }
    public virtual void ChangeValueSlider()
    {
        //DebugExtension.LogError("Change Value");
        rotationValue.x = sliderX.value;
        textRotationValueX.text = formatText(sliderX.value);
        rotationValue.y = sliderY.value;
        textRotationValueY.text = formatText(sliderY.value);
        rotationValue.z = sliderZ.value;
        textRotationValueZ.text = formatText(sliderZ.value);
        currentEditedVRObjectTransform.localEulerAngles = rotationValue;
    }
    /// <summary>
    /// Use when drag slider and stop, mouse up
    /// </summary>
    public virtual void OnPointerUp()
    {
        //DebugExtension.LogError("OnPointer Up 1111");
        SendSyncTransform();
    }
}