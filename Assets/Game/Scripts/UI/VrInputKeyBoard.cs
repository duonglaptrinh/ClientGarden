using System;
using UnityEngine;

namespace Game.Client
{
    public class VrInputKeyBoard : MonoBehaviour
    {
        [SerializeField] private Vector3 localPosOnActive = new Vector3(0, 100, 0);
        [SerializeField] private GameObject keys, upperCaseKeys, lowCaseKeys;
        [SerializeField] private GameObject normalKeyboard, numberKeyboard;

        public event Action<string> OnClickKey;

        public event Action OnClickBackspace;

        public event Action OnClickEnter;
        public GameObject left { get; set; }
        public GameObject right { get; set; }
        public void OpenKeyBoard(Transform parent = null, bool isNormalKeyboard = true)
        {
            transform.parent = parent;
            keys.SetActive(true);
            var parentLocalScale = parent.localScale;
            var localScale = new Vector3(1 / parentLocalScale.x, 1 / parentLocalScale.y, 1 / parentLocalScale.z);
            transform.localScale = localScale;
            transform.localRotation = Quaternion.identity;
            CheckOnFullScreen();
            normalKeyboard.SetActive(isNormalKeyboard);
            numberKeyboard.SetActive(!isNormalKeyboard);
            if (!left && !right)
            {
                Vector2 size = GetComponent<RectTransform>().sizeDelta;
                float width = size.x / 2;
                float height = size.y / 2;

                left = new GameObject("left");
                left.transform.SetParent(transform);
                left.transform.localPosition = new Vector3(-width, 0, 0);

                right = new GameObject("right");
                right.transform.SetParent(transform);
                right.transform.localPosition = new Vector3(width, 0, 0);
            }
        }

        public void HideKeyBoard()
        {
            keys.SetActive(false);
            OnClickKey = null;
            OnClickEnter = null;
            OnClickBackspace = null;
        }

        public void ClickKey(string character)
        {
            OnClickKey?.Invoke(character);
        }

        public void CapsLockPress()
        {
            upperCaseKeys.SetActive(!upperCaseKeys.activeInHierarchy);
            lowCaseKeys.SetActive(!lowCaseKeys.activeInHierarchy);
        }

        public void Backspace()
        {
            OnClickBackspace?.Invoke();
            // if (inputCurrent.text.Length > 0)
            // {
            //     inputCurrent.text = inputCurrent.text.Substring(0, inputCurrent.text.Length - 1);
            // }
        }

        public void Enter()
        {
            OnClickEnter?.Invoke();
            Destroy(gameObject);
            // VRTK_Logger.Info("You've typed [" + inputCurrent.text + "]");
            // inputCurrent.text = "";
        }

        private void OnDestroy()
        {
            OnClickKey = null;
            OnClickEnter = null;
            OnClickBackspace = null;
        }

        private void CheckOnFullScreen()
        {
            var rect_transform = GetComponent<RectTransform>();
            rect_transform.localPosition = localPosOnActive;
        }
    }
}