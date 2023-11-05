using System;
using System.Collections.Generic;
using TWT.Networking;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class VRObjectPositionSetting : VRObjectRotationSetting
{
    VRModelV2 vrModel;
    float indexX = 0;
    float indexY = 0;
    float indexZ = 0;
    float MinValue = -5;
    float MaxValue = 5;
    protected override int intGetValue => 1;
    protected override void OnRotationValueChanged(int sign, RotationAxis rotationAxis)
    {
        if (rotationUpdateDisposable == null && currentEditedVRObjectTransform != null)
        {
            rotationUpdateDisposable = Observable.EveryUpdate().Subscribe(_ =>
            {
                switch (rotationAxis)
                {
                    case RotationAxis.X:
                        {
                            if (MinValue < indexX && indexX < MaxValue)
                            {
                                rotationValue.x += sign * changeRate * Time.deltaTime;
                                indexX += sign;
                                DebugExtension.Log("indexX =  " + indexX);
                            }
                        }
                        //if (Mathf.Abs(rotationValue.x) >= 360)
                        //    rotationValue.x = 0;

                        textRotationValueX.text = string.Format("{0:0.00}", rotationValue.x);
                        break;
                    case RotationAxis.Y:
                        {
                            if (MinValue < indexY && indexY < MaxValue)
                            {
                                rotationValue.y += sign * changeRate * Time.deltaTime;
                                indexY += sign;
                            }
                        }
                        //if (Mathf.Abs(rotationValue.y) >= 360)
                        //    rotationValue.y = 0;

                        textRotationValueY.text = string.Format("{0:0.00}", rotationValue.y);
                        break;
                    case RotationAxis.Z:
                        rotationValue.z += sign * changeRate * Time.deltaTime;
                        //if (Mathf.Abs(rotationValue.z) >= 360)
                        //    rotationValue.z = 0;

                        textRotationValueZ.text = string.Format("{0:0.00}", rotationValue.z);
                        break;
                }
                vrModel.SetOnAnotherObject(false);
                currentEditedVRObjectTransform.localPosition = rotationValue;
                SendSyncTransform();
            });
        }
    }

    public override void SetVRObjectToEdit(VRModelV2 vrObject)
    {
        RemoveLisenerSlider();
        this.vrObject = vrObject;
        VRModelV2 vr = vrObject;
        vrModel = vrObject;
        if (vr && vr.GetData() != null)
        {
            resetValue = vr.GetData().model_translate;
        }
        SetVRObjectToEdit(vrObject.transform);
        SetLisenerSlider();
    }
    public override void SetVRObjectToEdit(Transform transform)
    {
        currentEditedVRObjectTransform = transform;
        if (currentEditedVRObjectTransform != null)
        {
            rotationValue = currentEditedVRObjectTransform.localPosition;
            textRotationValueX.text = string.Format("{0:0.00}", rotationValue.x);
            textRotationValueY.text = string.Format("{0:0.00}", rotationValue.y);
            textRotationValueZ.text = string.Format("{0:0.00}", rotationValue.z);
        }
    }

    public override void ResetValue()
    {
        currentEditedVRObjectTransform.localPosition = VRObjectManagerV2.ConverStringToVector3(resetValue);
        //SendSyncTransform();

        vrModel.SetOnAnotherObject(false);
        rotationValue.x = VRObjectManagerV2.ConverStringToVector3(resetValue).x;
        currentEditedVRObjectTransform.localPosition = rotationValue;
        //SendSyncTransform();
        textRotationValueX.text = string.Format("{0:0.00}", rotationValue.x);
        sliderX.value = 0;
        indexX = 0;

        vrModel.SetOnAnotherObject(false);
        rotationValue.y = VRObjectManagerV2.ConverStringToVector3(resetValue).y;
        currentEditedVRObjectTransform.localPosition = rotationValue;
        //SendSyncTransform();
        textRotationValueY.text = string.Format("{0:0.00}", rotationValue.y);
        sliderY.value = 0;
        indexY = 0;

        vrModel.SetOnAnotherObject(false);
        rotationValue.z = VRObjectManagerV2.ConverStringToVector3(resetValue).z;
        currentEditedVRObjectTransform.localPosition = rotationValue;
        SendSyncTransform();
        textRotationValueZ.text = string.Format("{0:0.00}", rotationValue.z);
        sliderZ.value = 0;
        indexZ = 0;
    }

    public override void ResetValueX()
    {
        vrModel.SetOnAnotherObject(false);
        rotationValue.x = VRObjectManagerV2.ConverStringToVector3(resetValue).x;
        currentEditedVRObjectTransform.localPosition = rotationValue;
        SendSyncTransform();
        textRotationValueX.text = string.Format("{0:0.00}", rotationValue.x);
        sliderX.value = 0;
        indexX = 0;
    }
    public override void ResetValueY()
    {
        vrModel.SetOnAnotherObject(false);
        rotationValue.y = VRObjectManagerV2.ConverStringToVector3(resetValue).y;
        currentEditedVRObjectTransform.localPosition = rotationValue;
        SendSyncTransform();
        textRotationValueY.text = string.Format("{0:0.00}", rotationValue.y);
        sliderY.value = 0;
        indexY = 0;
    }
    public override void ResetValueZ()
    {
        vrModel.SetOnAnotherObject(false);
        rotationValue.z = VRObjectManagerV2.ConverStringToVector3(resetValue).z;
        currentEditedVRObjectTransform.localPosition = rotationValue;
        SendSyncTransform();
        textRotationValueZ.text = string.Format("{0:0.00}", rotationValue.z);
        sliderZ.value = 0;
        indexZ = 0;
    }
    public override void AddValueX(float value)
    {
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            indexX += value;
            if (indexX < MinValue)
            {
                indexX = MinValue;
                return;
            }
            if (indexX > MaxValue)
            {
                indexX = MaxValue;
                return;
            }
            vrModel.SetOnAnotherObject(false);
            rotationValue.x += value;
            sliderX.value += value;
            currentEditedVRObjectTransform.localPosition = rotationValue;
            SendSyncTransform();
            textRotationValueX.text = string.Format("{0:0.00}", rotationValue.x);
        }
        //rotationValue.x += value;
        //currentEditedVRObjectTransform.localPosition = rotationValue;
        //SendSyncTransform();
        //textRotationValueX.text = string.Format("{0:0.00}", rotationValue.x);
    }
    public override void AddValueY(float value)
    {
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            indexY += value;
            if (indexY < MinValue)
            {
                indexY = MinValue;
                return;
            }
            if (indexY > MaxValue)
            {
                indexY = MaxValue;
                return;
            }
            vrModel.SetOnAnotherObject(false);
            rotationValue.y += value;
            sliderY.value += value;
            currentEditedVRObjectTransform.localPosition = rotationValue;
            SendSyncTransform();
            textRotationValueY.text = string.Format("{0:0.00}", rotationValue.y);
            vrModel.IsEditedTransform = true;
        }
        //rotationValue.y += value;
        //currentEditedVRObjectTransform.localPosition = rotationValue;
        //SendSyncTransform();
        //textRotationValueY.text = string.Format("{0:0.00}", rotationValue.y);
        //vrModel.IsEditedTransform = true;
    }
    public override void AddValueZ(float value)
    {
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            indexZ += value;
            if (indexZ < MinValue)
            {
                indexZ = MinValue;
                return;
            }
            if (indexZ > MaxValue)
            {
                indexZ = MaxValue;
                return;
            }
            vrModel.SetOnAnotherObject(false);
            rotationValue.z += value;
            sliderZ.value += value;
            currentEditedVRObjectTransform.localPosition = rotationValue;
            SendSyncTransform();
            textRotationValueZ.text = string.Format("{0:0.00}", rotationValue.z);
        }
        //rotationValue.z += value;
        //currentEditedVRObjectTransform.localPosition = rotationValue;
        //SendSyncTransform();
        //textRotationValueZ.text = string.Format("{0:0.00}", rotationValue.z);
    }
    public override void ChangeValueSlider()
    {
        rotationValue.x = VRObjectManagerV2.ConverStringToVector3(resetValue).x + sliderX.value;
        textRotationValueX.text = rotationValue.x.ToString("0.0");
        rotationValue.y = VRObjectManagerV2.ConverStringToVector3(resetValue).y + sliderY.value;
        textRotationValueY.text = rotationValue.y.ToString("0.0");
        rotationValue.z = VRObjectManagerV2.ConverStringToVector3(resetValue).z + sliderZ.value;
        textRotationValueZ.text = rotationValue.z.ToString("0.0");
        currentEditedVRObjectTransform.localPosition = rotationValue;
    }
    private void OnEnable()
    {
        resetValue = VRObjectManagerV2.ConvertVector3ToString(currentEditedVRObjectTransform.localPosition);
        sliderX.value = 0;
        sliderY.value = 0;
        sliderZ.value = 0;
    }
    protected override void SetMinMaxSlider()
    {
    }
}