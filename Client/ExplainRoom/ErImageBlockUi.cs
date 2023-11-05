using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

namespace Game.ExplainRoom
{
    public class ErImageBlockUi : MonoBehaviour
    {
        [SerializeField] private ErImageUiElement erImageUiElementPrefab;

        [SerializeField] private Transform container;

        [SerializeField] private RawImage bigImage;

        [SerializeField] private Texture2D defaultBigImage;

        [SerializeField] private Rect bigImageRect;

        private string Id => gameObject.name;

        private List<ErImageUiElement> ErImageUiElements { get; } = new List<ErImageUiElement>();

        public void Add(Texture2D element)
        {
            var uiElement = Instantiate(erImageUiElementPrefab, container, true);
            uiElement.transform.localPosition = Vector3.zero;
            uiElement.transform.localRotation = Quaternion.Euler(Vector3.zero);
            uiElement.transform.localScale = erImageUiElementPrefab.transform.localScale;
            ErImageUiElements.Add(uiElement);
            var idUi = ErImageUiElements.Count - 1;
            uiElement.GetComponent<Button>().onClick.AddListener(() =>
                NetworkClient.allClients[0].Send(MyMessageType.ER_PLAYER_CHOOSE_IMG,
                    new ErPlayerChooseImageMessage(Id, idUi)));
            uiElement.SetImage(element);
        }

        public void Initialize()
        {
            var erImageUiElement = GetComponentInChildren<ErImageUiElement>();
            if (erImageUiElement)
            {
                var image = erImageUiElement.GetTexture;
                if (image)
                    SetBigImage(image);
                else
                    SetBigImage(defaultBigImage);
            }
        }

        public void SetDefaultImage(Texture2D texture)
        {
            SetBigImage(texture);
        }

        public void SyncChooseImage(int imgIndex)
        {
            SetBigImage(ErImageUiElements[imgIndex].GetTexture);
        }

        public void Refresh()
        {
            SetBigImage(defaultBigImage);

            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }
        }

        void SetBigImage(Texture texture)
        {
            Utility.SetImageWithRect(texture, bigImage, bigImageRect);
        }
    }
}