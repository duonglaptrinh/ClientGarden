using System;
using Game.Client;
using TWT.Networking;
using UnityEngine;

using System.IO;

public class DisconnectRoomComponent : MonoBehaviour
{
    //public bool isShowDisconnect = false;
    //public static event Action<NetworkConnection> OnOnServerDisconnect;

    //public float DeltaTime { get; set; }
    //double timeStart;

    //public override void OnServerDisconnect(NetworkConnectionToClient conn)
    //{
    //    base.OnServerDisconnect(conn);
    //    OnOnServerDisconnect?.Invoke(conn);
    //    DebugExtension.LogError("OnServerDisconnect -> check player");
    //    if (numPlayers <= 0)
    //    {
    //        DebugExtension.LogError("Server auto Save data");
    //        string contentName = "Mirabo-Demo-v2";
    //        var absolutePath = $"{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT}/{contentName}/{VrDomeAssetResourceNameDefine.VR_JSON}";
    //        var jsonPath = Path.Combine(GameContext.VrResourceRootPathOnServer, absolutePath, VrContentFileNameConstant.VR_DATA_JSON);
    //        DebugExtension.LogError("jsonPath = " + jsonPath);

    //        System.IO.File.WriteAllText(jsonPath, JsonUtility.ToJson(GameContext.ContentDataCurrent));
    //    }
    //}

    //[Obsolete]
    //public override void OnClientDisconnect(NetworkConnection conn)
    //{
    //    base.OnClientNotReady(conn);
    //    ClientDisconnect();
    //}

    //private void OnApplicationPause(bool pause)
    //{
    //    //if (!pause) return;
    //    //if (!GameContext.IsOffline)
    //    //{
    //    //    StopClient();
    //    //    if (!isShowDisconnect)
    //    //    {
    //    //        isShowDisconnect = true;
    //    //        ClientDisconnect();
    //    //    }
    //    //}

    //    if (pause)
    //    {
    //        timeStart = DateTime.Now.Subtract(DateTime.MinValue).TotalMilliseconds;
    //        // DebugExtension.Log("Time start: " + timeStart);
    //    }
    //    else
    //    {
    //        double timeEnd = DateTime.Now.Subtract(DateTime.MinValue).TotalMilliseconds;
    //        //DebugExtension.Log("Time end: " + timeEnd);
    //        DeltaTime = (int)(timeEnd - timeStart);
    //        //DebugExtension.Log("Delta Time: " + DeltaTime);
    //        DeltaTime /= 1000f;
    //        //DebugExtension.Log("Delta Time: " + DeltaTime);
    //        if (DeltaTime > GameContext.ConfigServerData.max_timeout_auto_disconnect)
    //        {
    //            if (!GameContext.IsOffline)
    //            {
    //                StopClient();
    //                if (!isShowDisconnect)
    //                {
    //                    isShowDisconnect = true;
    //                    ClientDisconnect();
    //                }
    //            }
    //        }
    //        else
    //        {
    //            //If you are student --> call On Join Room to sync time data video
    //            if (GameContext.IsTrainingMode && !GameContext.IsPlayerHost)
    //                VrgSyncApi.Send(new SyncAllVrMediaMessage());

    //        }
    //    }
    //}

    private void ClientDisconnect()
    {
        GameContext.ContentDataCurrent = null;
        if (GameContext.IsTeacher)
        {
            PopupRuntimeManager.Instance.ShowPopupOnlyConfirm("端末の接続が切れました。タイトル画面に戻ります。", () => SceneConfig.LoadScene(SceneConfig.Scene.TitleScreen));
        }
        else
        {
#if ADMIN
            SceneConfig.LoadScene(SceneConfig.Scene.AdminScreen);
            //PopupRuntimeManager.Instance.ShowPopupOnlyConfirm("講師用端末の接続が切れました。タイトル画面に戻ります。", () => SceneConfig.LoadScene(SceneConfig.Scene.AdminScreen));
#else
            PopupRuntimeManager.Instance.ShowPopupOnlyConfirm("講師用端末の接続が切れました。タイトル画面に戻ります。", () => SceneConfig.LoadScene(SceneConfig.Scene.TitleScreen));
#endif
        }
        //var lstPlayers = FindObjectsOfType<UnetPlayer>();
        //if (lstPlayers != null && lstPlayers.Length > 1)
        //{
        //    foreach (var player in lstPlayers)
        //    {
        //        if (player != UnetPlayer.LocalPlayer)
        //        {
        //            player.gameObject.SetActive(false);
        //        }
        //    }
        //}
    }

    //private void OnApplicationPause(bool pause)
    //{
    //    if(!pause)
    //    {
    //        DebugExtension.Log(client.isConnected);
    //        client.GetConnectionStats();
    //    }
    //}
}