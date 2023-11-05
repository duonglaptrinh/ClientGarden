using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TWT.Client
{
    public class DomeRotateView : ViewComponentBase
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Button resetBtn;

        [SerializeField] private Text angleText;

        public IObservable<float> OnClickResetBtnAsObservable() =>
            resetBtn.OnSelectUIAsObservable().Select(_ => 0f);

        public IObservable<float> OnAdjustDomeAngleAsObservable()
        {
            return OnSlideAsObserable();
        }

        public IObservable<float> OnSlideAsObserable() =>
            slider.OnValueChangedAsObservable();

        public void ShowAngle(float value)
        {
            value = (int)value;
            angleText.text = value > 0 ? ("+" + value) : value.ToString();
        }

        public void ResetSlider()
        {
            slider.value = 0;
        }

        public void SetDomeRotation(float value)
        {
            slider.value = value;
        }
    }
}