using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TWT.Client
{
    public class DomeSizeRemoteView : ViewComponentBase
    {
        [SerializeField] private Button increaseSizeBtn;
        [SerializeField] private Button reduceSizeBtn;

        [SerializeField] private Text sizeText;

        private IObservable<float> OnClickIncreaseSizeBtnAsObservable() =>
            increaseSizeBtn.OnSelectUIAsObservable().Select(_ => -0.01f);

        private IObservable<float> OnClickReduceSizeBtnAsObservable() =>
            reduceSizeBtn.OnSelectUIAsObservable().Select(_ => 0.01f);

        public IObservable<float> OnAdjustDomeSideAsObservable()
        {
            return OnClickIncreaseSizeBtnAsObservable().Merge(OnClickReduceSizeBtnAsObservable());
        }

        public void ShowSize(float value)
        {
            value = (1 - value) + 1;
            sizeText.text = $"{value:0.00}";
        }
    }
}