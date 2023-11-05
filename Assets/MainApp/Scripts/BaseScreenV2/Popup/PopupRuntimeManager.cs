using Shim.Utils;
using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.Client
{
    public class PopupRuntimeManager : MonoBehaviour
    {
        private static PopupRuntime popupRuntimePrefab;
        private static PopupRuntime popupConfirmRuntimePrefab;
        private static PopupRuntime popupWaitPrefab;
        private static PopupRuntime popupThreeButtonsRuntimePrefab;
        public static PopupRuntimeManager Instance { get; private set; }
        public static bool _IsPopup = false;
        private const string DEFAULT_TITLE = "";

        private static PopupRuntime PopupRuntimePrefab => popupRuntimePrefab
            ? popupRuntimePrefab
            : popupRuntimePrefab = Resources.Load<PopupRuntime>("UI/PopupV2/PopupRuntime");

        private static PopupRuntime PopupRuntimeOnlyConfirmPrefab => popupConfirmRuntimePrefab
            ? popupConfirmRuntimePrefab
            : popupConfirmRuntimePrefab = Resources.Load<PopupRuntime>("UI/PopupV2/PopupRuntimeOnlyConfirm");

        private static PopupRuntime PopupThreeButtonsRuntimePrefab => popupThreeButtonsRuntimePrefab
            ? popupThreeButtonsRuntimePrefab
            : popupThreeButtonsRuntimePrefab = Resources.Load<PopupRuntime>("UI/PopupV2/PopupThreeButtonsRuntime");

        private static PopupRuntime PopupWaitPrefab => popupWaitPrefab
            ? popupWaitPrefab
            : popupWaitPrefab = Resources.Load<PopupRuntime>("UI/PopupV2/PopupWait");

        private void Awake()
        {
            Instance = this;
        }

        public void ShowPopup(string title = "", Action onClickConfirm = null, Action onClickCancel = null)
        {
            CreatePopup(title, onClickConfirm, onClickCancel, PopupRuntimePrefab);
        }

        public void ShowPopupOnlyConfirm(string title = "", Action onClickConfirm = null)
        {
            CreatePopup(title, onClickConfirm, null, PopupRuntimeOnlyConfirmPrefab);
        }

        public void ShowPopupThreeButtons(string title = "", Action onClickConfirm = null, Action onClickCancel = null)
        {
            CreatePopup(title, onClickConfirm, onClickCancel, PopupThreeButtonsRuntimePrefab);
        }

        public GameObject ShowPopupWait(string title = "")
        {
            return CreatePopup(title, prefab: PopupWaitPrefab).gameObject;
        }

        private PopupRuntime CreatePopup(string title = "", Action onClickConfirm = null, Action onClickCancel = null,
            PopupRuntime prefab = null)
        {
            _IsPopup = true;
            var popup = Instantiate(prefab, transform, true);
            RectTransformExtensions.SetLeft(popup.transform, 0);
            RectTransformExtensions.SetRight(popup.transform, 0);
            RectTransformExtensions.SetTop(popup.transform, 0);
            RectTransformExtensions.SetBottom(popup.transform, 0);
            //popup.transform.localPosition = new Vector3(0, 0, 0.8f);
            //popup.transform.localRotation = Quaternion.identity;
            popup.transform.localScale = Vector3.one;
            var titlePopup = string.IsNullOrEmpty(title) ? DEFAULT_TITLE : title;
            return popup.Show(titlePopup, onClickConfirm, onClickCancel);
        }
    }
}