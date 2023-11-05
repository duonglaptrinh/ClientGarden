using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DomeModelSizeSetting : VRObjectSizeSetting
{
    Transform currentTransform;
    float sizeMin = 0f;
    float sizeMax = 3f;
    private void Awake()
    {
        sizeMaxText.text = sizeMax.ToString();
        sizeMinText.text = sizeMin.ToString();
    }
    protected override void Start()
    {
        resetBtn.onClick.AddListener(ResetSliderAll);
        slider.onValueChanged.AddListener(ScaleVrObject);
    }

    public void SetScale(Transform trans, float scale)
    {
        slider.minValue = sizeMin;
        slider.maxValue = sizeMax;
        currentTransform = trans;
        slider.value = scale;
        ScaleVrObject(scale);
        //DebugExtension.LogError("Scale " + scale + "   " + slider.value);
    }

    protected override void ScaleVrObject(float value)
    {
        if (currentTransform)
            currentTransform.localScale = new Vector3(value, value, value);
        sizeText.text = value.ToString("0.0");
        //if (slider.value < 0.1f) slider.value = 0.1f;
    }
}
