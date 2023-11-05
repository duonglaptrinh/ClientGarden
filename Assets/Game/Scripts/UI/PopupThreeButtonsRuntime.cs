using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Client
{
    public class PopupThreeButtonsRuntime : PopupRuntime
    {
        [SerializeField] private Button exitBtn;

        private event Action OnClickConfirm;
        private event Action OnClickCancel;
        private event Action OnClickExit;

        public override void Awake()
        {
            base.Awake();
            exitBtn.onClick.AddListener(() =>
            {
                OnClickExit?.Invoke();
                Destroy(gameObject);
            });
        }

        public override PopupRuntime Show(string title, Action confirm = null, Action cancel = null)
        {
            base.Show(title,confirm,cancel);
            OnClickExit = () => { SceneConfig.LoadScene(SceneConfig.Scene.TitleScreen); };
            return this;
        }
    }
}