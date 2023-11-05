using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Game.ExplainRoom
{
    public class ExplainRoomVideoManager : ResourceLoadManagerBase<ErVideoUiCtrl>
    {
        [SerializeField] private ErVideoUiCtrl erVideoUiCtrlPrefab;

        protected override void Awake()
        {
            base.Awake();

            RegisterSyncVideoUi(MyMessageType.ER_PLAY_VIDEO, (msg, ctrl) => ctrl.PlayVideo());
            RegisterSyncVideoUi(MyMessageType.ER_PAUSE_VIDEO, (msg, ctrl) => ctrl.PauseVideo());
            RegisterSyncVideoUi(MyMessageType.ER_BACK_SECOND_VIDEO);
            RegisterSyncVideoUi(MyMessageType.ER_NEXT_SECOND_VIDEO);
            RegisterSyncVideoUi(MyMessageType.ER_RESTART_VIDEO, (msg, ctrl) => ctrl.Restart());

            NetworkClient.allClients[0].RegisterHandler(MyMessageType.ER_CHANGE_VOLUME_VIDEO, msg =>
            {
                var message = msg.ReadMessage<ErSyncVolumeVideo>();
                if (SourceLoadedMap.TryGetValue(message.Id, out var videoUiCtrl))
                {
                    videoUiCtrl.Volume = message.volume;
                }
            });
        }

        private void RegisterSyncVideoUi(short msgDefine, Action<ErSyncTimeVideo, ErVideoUiCtrl> callback = null)
        {
            NetworkClient.allClients[0].RegisterHandler(msgDefine, msg =>
            {
                var message = msg.ReadMessage<ErSyncTimeVideo>();
                if (SourceLoadedMap.TryGetValue(message.idVideo, out var videoUiCtrl))
                {
                    videoUiCtrl.SyncTimePlayVideo(message.time);
                    callback?.Invoke(message, videoUiCtrl);
                }
            });
        }

        public override ErVideoUiCtrl Load(string nameDefine, string path)
        {
            ErVideoUiCtrl erVideoUiCtrl;

            if (SourceLoadedMap.TryGetValue(nameDefine, out var videoUi))
            {
                videoUi.gameObject.SetActive(true);
                erVideoUiCtrl = videoUi;
            }
            else
            {
                erVideoUiCtrl = Instantiate(erVideoUiCtrlPrefab, transform, true);
                erVideoUiCtrl.gameObject.name = nameDefine;
                erVideoUiCtrl.CreateRenderTexture();
                SourceLoadedMap.Add(nameDefine, erVideoUiCtrl);
            }
            
            erVideoUiCtrl.SetVideoUrl(path);

            return erVideoUiCtrl;
        }
    }
}