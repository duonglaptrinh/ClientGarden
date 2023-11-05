using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TWT.Client
{
    public class ContextRemoteView : ViewComponentBase
    {
        [SerializeField] private Button flickerAllVrObjectBtn;
        [SerializeField] private Button showAllVrObjectBtn;
        [SerializeField] private Button resetBtn;
        [SerializeField] private Button muteBtn;
        [SerializeField] private Button unmuteBtn;
        [SerializeField] private Button hideAllVrObjectBtn;
        [SerializeField] private Button offFlickerBtn;
        [SerializeField] private Button showAvatarBtn;
        [SerializeField] private Button hideAvatarBtn;
        [SerializeField] private Toggle enableMove;

        public bool IsUnMuteButtonActive => unmuteBtn.gameObject.activeInHierarchy;
        public bool IsHideAllVrObjectButtonActive => hideAllVrObjectBtn.gameObject.activeInHierarchy;
        public bool IsOffFlickerButtonActive => offFlickerBtn.gameObject.activeInHierarchy;
        public bool IsShowButtonActive => showAvatarBtn.gameObject.activeInHierarchy;
        public bool IsEnableMove => enableMove.isOn;

        public void SetEnableMove(bool status)
        {
            enableMove.isOn = status;
        }

        public void SetUnmuteButtonStatus(bool status)
        {
            unmuteBtn.gameObject.SetActive(status);
        }

        public void SetOffFlickerButtonStatus(bool status)
        {
            offFlickerBtn.gameObject.SetActive(status);
        }

        public void SetHideAllVrObjectButtonStatus(bool status)
        {
            hideAllVrObjectBtn.gameObject.SetActive(status);
        }

        public void SetHideButtonStatus(bool status)
        {
            showAvatarBtn.gameObject.SetActive(status);
        }

        public IObservable<bool> OnClickEnableMoveAsObservable() =>
            enableMove.OnValueChangedAsObservable();

        public IObservable<Unit> OnClickFlickerAllVrObjectBtnAsObservable() =>
            flickerAllVrObjectBtn.OnClickAsObservable();
        
        public IObservable<Unit> OnClickShowAllVrObjectBtnAsObservable() =>
            showAllVrObjectBtn.OnClickAsObservable();
        
        public IObservable<Unit> OnClickResetBtnAsObservable() =>
            resetBtn.OnClickAsObservable();
        
        public IObservable<Unit> OnClickMuteBtnAsObservable() =>
            muteBtn.OnClickAsObservable();

        public IObservable<Unit> OnClickUnMuteBtnAsObservable() =>
            unmuteBtn.OnClickAsObservable();

        public IObservable<Unit> OnClickHideAllVrObjectBtnAsObservable() =>
            hideAllVrObjectBtn.OnClickAsObservable();

        public IObservable<Unit> OnClickOffFlickerBtnAsObservable() =>
            offFlickerBtn.OnClickAsObservable();

        public IObservable<Unit> OnClickHideAvatarBtnAsObservable() =>
            hideAvatarBtn.OnClickAsObservable();

        public IObservable<Unit> OnClickShowAvatarBtnAsObservable() =>
            showAvatarBtn.OnClickAsObservable();
    }
}