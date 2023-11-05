using System;
using Game.Client.Extension;
using UnityEngine;

namespace TWT.Client
{
    public abstract class UiControllerBase<TView> : MonoBehaviour where TView : ViewComponentBase
    {
        [SerializeField] private TView view;

        protected TView View => view ? view : view = GetComponent<TView>();

        protected virtual void Awake()
        {
        }

        private void Start()
        {
            View.Initialize();
            Initialize();
        }

        protected abstract void Initialize();

        private void OnValidate()
        {
            if (view == null)
                view = gameObject.GetOrAddComponent<TView>();
        }
    }
}