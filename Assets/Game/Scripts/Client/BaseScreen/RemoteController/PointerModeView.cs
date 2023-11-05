using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TWT.Client
{
    public class PointerModeView : ViewComponentBase
    {
        [SerializeField] private Button selectModeBtn;
        [SerializeField] private Button moveFreeModeBtn;
        [SerializeField] private Button drawModeBtn;
        [SerializeField] private Button resetPositionBtn;

        public IObservable<Unit> OnClickSelectModeAsObservable => selectModeBtn.onClick.AsObservable();
        public IObservable<Unit> OnClickMoveFreeAsObservable => moveFreeModeBtn.onClick.AsObservable();
        public IObservable<Unit> OnClickDrawAsObservable => drawModeBtn.onClick.AsObservable();
        public IObservable<Unit> OnClickResetPositionAsObservable => resetPositionBtn.onClick.AsObservable();

        public void SwitchToMoveFreeMode()
        {
            selectModeBtn.interactable = true;
            moveFreeModeBtn.interactable = false;
            drawModeBtn.interactable = true;
        }

        public void SwitchToSelectMode()
        {
            selectModeBtn.interactable = false;
            moveFreeModeBtn.interactable = true;
            drawModeBtn.interactable = true;
        }

        public void SwitchToDrawMode()
        {
            selectModeBtn.interactable = true;
            moveFreeModeBtn.interactable = true;
            drawModeBtn.interactable = false;
        }
    }
}