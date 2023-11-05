using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Game.ExplainRoom
{
    public class ErTextUi : MonoBehaviour
    {
        [SerializeField]
        private ScrollRect scrollRect;

        [SerializeField]
        private Text contentText;

        [SerializeField]
        private Scrollbar scroll;

        private int clientId => MyLobbyPlayer.lobbyId;
        private string Id => gameObject.name;

        private bool isUpdateScroll = false;
        private float startScrollValue;
        private float newScrollValue;
        private float scrollTime;

        void Start()
        {
            scroll.onValueChanged.AddListener(OnScrollValueChange);
        }

        void Update()
        {
            if (!isUpdateScroll)
                return;

            scrollTime += Time.deltaTime / 0.1f;    // 0.5 is the time need to get the scroll reach to target
            scroll.value = Mathf.Lerp(startScrollValue, newScrollValue, scrollTime);

            if (scrollTime >= 1)
                isUpdateScroll = false;
        }

        private void OnEnable()
        {
            StartCoroutine(WaitToUpdateCanvas());
        }

        public void SetContent(string content)
        {
            contentText.text = content;
            scrollRect.verticalNormalizedPosition = 1;
        }

        IEnumerator WaitToUpdateCanvas()
        {
            yield return new WaitForEndOfFrame();
            contentText.transform.parent.gameObject.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentText.transform.parent.GetComponent<RectTransform>());
            contentText.transform.parent.gameObject.SetActive(true);
        }

        void OnScrollValueChange(float value)
        {
            if (isUpdateScroll)
                return;

            NetworkClient.allClients[0].Send(MyMessageType.ER_SCROLL_TEXT, new ErScrollText(clientId, Id, scroll.value));
        }

        public void DoScrollValueChange(float scrollValue)
        {
            isUpdateScroll = true;
            startScrollValue = scroll.value;
            newScrollValue = scrollValue;
            scrollTime = 0;
        }
    }
}