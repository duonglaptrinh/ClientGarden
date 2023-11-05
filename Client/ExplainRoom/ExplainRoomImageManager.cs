using UnityEngine;
using UnityEngine.Networking;

namespace Game.ExplainRoom
{
    public class ExplainRoomImageManager : ResourceLoadManagerBase<ErImageBlockUi>
    {
        [SerializeField] private ErImageBlockUi erImageBlockUiPrefab;

        protected override void Awake()
        {
            base.Awake();
            NetworkClient.allClients[0].RegisterHandler(MyMessageType.ER_PLAYER_CHOOSE_IMG, msg =>
            {
                var chooseImageMessage = msg.ReadMessage<ErPlayerChooseImageMessage>();
                if (SourceLoadedMap.TryGetValue(chooseImageMessage.blockImgId, out var imageBlockUi))
                {
                    imageBlockUi.SyncChooseImage(chooseImageMessage.imgIndex);
                }
            });
        }

        public override ErImageBlockUi Load(string nameDefine, string path)
        {
            var texture2d = ResourceLoadObject.Load<Texture2D>(path);

            if (SourceLoadedMap.TryGetValue(nameDefine, out var imageBlockUi))
            {
                imageBlockUi.Add(texture2d);
                return imageBlockUi;
            }

            var erImageBlockUi = Instantiate(erImageBlockUiPrefab, transform, true);
            erImageBlockUi.Add(texture2d);
            erImageBlockUi.name = nameDefine;
            erImageBlockUi.Initialize();
            SourceLoadedMap.Add(nameDefine, erImageBlockUi);

            return erImageBlockUi;
        }

        public ErImageBlockUi Load(string nameDefine, string[] paths)
        {
            ErImageBlockUi erImageBlockUi;

            if (SourceLoadedMap.TryGetValue(nameDefine, out var imageBlockUi))
            {
                erImageBlockUi = imageBlockUi;
                erImageBlockUi.Refresh();
            } 
            else
            {
                erImageBlockUi = Instantiate(erImageBlockUiPrefab, transform, true);
                erImageBlockUi.name = nameDefine;
                SourceLoadedMap.Add(nameDefine, erImageBlockUi);
            }

            bool isDefaultImage = true;
            foreach (var path in paths)
            {
                var texture2d = ResourceLoadObject.Load<Texture2D>(path);
                if (texture2d != null)
                {
                    erImageBlockUi.Add(texture2d);

                    if (isDefaultImage)
                    {
                        erImageBlockUi.SetDefaultImage(texture2d);
                        isDefaultImage = false;
                    }
                }
            }

            //erImageBlockUi.Initialize();

            return erImageBlockUi;
        }
    }
}