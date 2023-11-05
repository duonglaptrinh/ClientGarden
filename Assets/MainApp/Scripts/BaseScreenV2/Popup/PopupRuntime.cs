using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Client
{
    public class PopupRuntime : MonoBehaviour
    {
        [SerializeField] private Text titleText;
        [SerializeField] private Button confirmBtn;
        [SerializeField] private Button cancelBtn;

        private event Action OnClickConfirm;
        private event Action OnClickCancel;
        public virtual void Awake()
        {
            confirmBtn.onClick.AddListener(() =>
            {
                OnClickConfirm?.Invoke();
                Destroy(gameObject);
            });
            cancelBtn.onClick.AddListener(() =>
            {
                OnClickCancel?.Invoke();
                Destroy(gameObject);
            });
        }
        public virtual PopupRuntime Show(string title, Action confirm = null, Action cancel = null)
        {
            titleText.text = title;
            OnClickConfirm = confirm;
            OnClickCancel = cancel;

            return this;
        }
        private void OnDestroy()
        {
            PopupRuntimeManager._IsPopup = false;
        }
    }
}