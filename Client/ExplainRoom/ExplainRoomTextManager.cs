using UnityEngine;
using UnityEngine.Networking;

namespace Game.ExplainRoom
{
    public class ExplainRoomTextManager : ResourceLoadManagerBase<ErTextUi>
    {
        [SerializeField]
        private ErTextUi erTextUiPrefab;

        protected override void Awake()
        {
            base.Awake();

            NetworkClient.allClients[0].RegisterHandler(MyMessageType.ER_SCROLL_TEXT, msg =>
            {
                var message = msg.ReadMessage<ErScrollText>();

                if (MyLobbyPlayer.lobbyId == message.clientId)
                    return;

                if (SourceLoadedMap.TryGetValue(message.idText, out var textUiCtrl))
                {
                    textUiCtrl.DoScrollValueChange(message.scrollValue);
                }
            });
        }

        public override ErTextUi Load(string nameDefine, string content)
        {
            if (SourceLoadedMap.TryGetValue(nameDefine, out var erText))
            {
                erText.SetContent(content);
                return erText;
            }
            
            var erTextUi = Instantiate(erTextUiPrefab, transform, true);
            erTextUi.gameObject.name = nameDefine;
            erTextUi.SetContent(content);
            SourceLoadedMap.Add(nameDefine, erTextUi);

            return erTextUi;
        }
    }
}