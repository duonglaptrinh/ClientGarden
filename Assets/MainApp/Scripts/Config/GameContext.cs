#if UNITY_ANDROID || UNITY_IOS
#define TWT_CLIENT
#endif

using Game.Client;
using Game.Log;
using System;
using TWT.Model;
using TWT.Networking;
using TWT.Networking.Config;
using TWT.Utility;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public enum GameMode
{
    None = 3,
    Editable = 2,
    TrainingMode = 0,
    RealtimeMode = 1
}
public enum EServerMode
{
    /// <summary>
    /// Change NetworkManager.prefab ->   clientUseWss: 0 and sslEnabled: 0
    /// Assets\Resources\Network\NetworkManager.prefab
    /// </summary>
    Local,  //localhost
    /// <summary>
    /// Change NetworkManager.prefab ->   clientUseWss: 1 and sslEnabled: 1
    /// Assets\Resources\Network\NetworkManager.prefab
    /// </summary>
    Develop, //sync.gld-lab.link
    Product  //demo.gld-lab.link
}


public static class GameContext
{
    public static float Land_Unit_count { get; set; } = 1;
    public static float Land_Count_Max { get; set; } = 10;
    public static float Land_Count_Min { get; set; } = 0;
    public static float Land_Setting_FrontOf
    {
        get
        {
            return VrDomeControllerV2.Instance.DomeLoadhouse.CurrentData.Land_Setting_FrontOf;
        }
    }
    public static float Land_Setting_Behide
    {
        get
        {
            return VrDomeControllerV2.Instance.DomeLoadhouse.CurrentData.Land_Setting_Behide;
        }
    }
    public static float Land_Setting_Left
    {
        get
        {
            return VrDomeControllerV2.Instance.DomeLoadhouse.CurrentData.Land_Setting_Left;
        }
    }
    public static float Land_Setting_Right
    {
        get
        {
            return VrDomeControllerV2.Instance.DomeLoadhouse.CurrentData.Land_Setting_Right;
        }
    }

    public const float SIZE_MAX = 2;
    public const float SIZE_MIN = 0;

    private static IResourceLoader resourceLoaderCache;
    private static IResourceLoader resourceLoaderCache_Demo;

    public const string ContentName = "Mirabo-Demo-v2";
    public static GameMode GameMode { get; set; }

    public static bool IsClient
    {
        get
        {
            return true;
        }
    }

    public static bool IsXrClient => IsClient && XRSettings.enabled;
    public static bool IsMobileClient => IsClient && !XRSettings.enabled;


    public static bool IsEditable
    {
        get => GameMode == GameMode.Editable;
        set => GameMode = value ? GameMode.Editable : GameMode.None;
    }

    public static VRContentData ContentDataCurrent { get; set; }
    public static VRContentDataTemplate ContentDataCurrentTemplate { get; set; } = null;
    public static int CurrentIdDome { get; set; }

    public static ContentInfo ContentInfoCurrent { get; set; }

    /// <summary>
    /// Build Local  -> change ServerMode to EServerMode.Local
    /// Change PlayerSetting to Decompression Fallback = true
    /// Change all group Adressable to LocalBuildPath and LocalLoadPath + change setting Addressable to Local
    /// </summary>

    public static EServerMode ServerMode = EServerMode.Develop;

    public static string LinkServerSyncRoom
    {
        get
        {
            switch (ServerMode)
            {
                case EServerMode.Local:
                    return @"ws://localhost:2567";
                case EServerMode.Develop:
                    return @"wss://signal.dev.gld-lab.link";
                //return @"wss://dev-proxy.gld-lab.link:2567";
                case EServerMode.Product:
                    return @"wss://signal.prod.gld-lab.link";
            };
            return "ws://";
        }
    }
    public static string LinkServerAPI
    {
        get
        {
            switch (ServerMode)
            {
                case EServerMode.Local:
                    return @"http://localhost:3000";
                case EServerMode.Develop:
                    return @"https://api.dev.gld-lab.link";
                    //return @"https://dev-proxy.gld-lab.link:6789";
                case EServerMode.Product:
                    return @"https://api.prod.gld-lab.link";
            };
            return "https://";
        }
    }

    public static bool IsSettingNetworkManagerSSL
    {
        get
        {
            switch (ServerMode)
            {
                case EServerMode.Local:
                    return false;
                case EServerMode.Develop:
                case EServerMode.Product:
                    return true;
            };
            return false;
        }
    }
    public static string LinkVoiceWebRTC
    {
        get
        {
            switch (ServerMode)
            {
                case EServerMode.Local:
                    return "ws://";
                case EServerMode.Develop:
                    //    return "wss://webrtc.gld-lab.link:2567/";
                    //case EServerMode.Product:
                    return "wss://webrtc.vr-garden.dev.mirabo.tech:2567/"; // rtc.gld-lab.link:2567
            };
            return "ws://";
        }
    }
    public static string IpAddress
    {
        get
        {
            switch (ServerMode)
            {
                case EServerMode.Local:
                    //return "192.168.1.8"; My IP local
                    return PlayerPrefs.GetString(PlayerPrefsConstant.IP_CONFIG, "localhost"); //local
                case EServerMode.Develop:
                    return "sync.gld-lab.link";//Server dev takasho
                case EServerMode.Product:
                    return "demo.gld-lab.link"; //Server product
            };
            return "localhost";
        }
    }
    public static string Uri
    {
        get
        {
            switch (ServerMode)
            {
                case EServerMode.Local:
                    return $"http://{IpAddress}:{API_PORT}"; //local
                case EServerMode.Develop:
                case EServerMode.Product:
                    return $"https://{IpAddress}:{API_PORT}"; // develop || product 
            };
            return $"http://{IpAddress}:{API_PORT}"; //local
        }
    }
    public static string IpFixed
    {
        get
        {
            switch (ServerMode)
            {
                case EServerMode.Local:
                    //return "192.168.1.8"; //my Ip local
                    return PlayerPrefs.GetString(PlayerPrefsConstant.IP_CONFIG, "192.168.0.1");
                case EServerMode.Develop:
                    return "52.69.137.116"; //Ip server dev
                case EServerMode.Product:
                    return "54.168.124.56"; //Ip server product
            };
            return "192.168.0.1";
        }
    }
    public static string WS
    {
        get
        {
            switch (ServerMode)
            {
                case EServerMode.Local:
                    return $"ws"; //local
                case EServerMode.Develop:
                case EServerMode.Product:
                    return $"wss"; // develop || product 
            };
            return $"ws"; //local
        }
    }
    public static bool IsDayMode { get; set; } = true;
    public static bool IsGridMode { get; set; } = false;
    public static bool IsSnapGrid { get; set; } = false;
    public static string UploadImageUrl => $"{Uri}{HttpApiConfig.ABSOLUTE_PATH_UPLOAD_IMAGE}";

    public const int MASTER_SERVER_PORT = 8000;//5000;
    public const int API_PORT = 7778;
    public const int UNET_PORT = 8999;//7777
    public const int TCP_PORT = 7776;

    public const string APP_VERSION = "1.3.0";
    public const float DOWNLOAD_CONTENT_TIMEOUT = 10f;

    public static bool IsRealtimeMode => GameMode == GameMode.RealtimeMode;
    public static bool IsTrainingMode => GameMode == GameMode.TrainingMode;
    public static bool ActivedRealtimeMode;
    public static bool ActivedTrainingMode;
    public static bool IsUseCodeTest = true;

    public static bool IsTeacherLeaveRoom;

    public static bool IsTeacher => PlayerPrefsConstant.IsTeacher;
    public static string VrResourceRootPathOnServer { get; set; }

    public static bool IsOffline { get; set; } = false;
    public static bool IsDemo { get; set; } = false;
    public static VrConfigServer ConfigServerData { get; set; } = new VrConfigServer();
    public static async UniTask<string> GetVersionContentAsync(string contentName)
    {
        var absolutePath = AbsolutePathVrContentDataVersion(contentName);
        var url = $"{GameContext.Uri}{absolutePath}";

        DebugExtension.Log(url);
        var responseJson = await ObservableUnityWebRequest.GetAsObservable(url);
        DebugExtension.Log(responseJson);
        var vrContentVersion = JsonUtility.FromJson<VrContentVersion>(responseJson);
        return vrContentVersion.version;
    }

    public static async UniTask<VREditPermissionData> GetPasswordForEditContentAsync()
    {
        var absolutePath =
            $"/{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT}/{VrDomeAssetResourceNameDefine.PASSWORD_FILENAME}";
        var url = $"{GameContext.Uri}{absolutePath}";

        try
        {
            DebugExtension.Log(url);
            var responseJson = await ObservableUnityWebRequest.GetAsObservable(url).Timeout(TimeSpan.FromSeconds(30));
            DebugExtension.Log(responseJson);
            var VREditPermissionData = JsonUtility.FromJson<VREditPermissionData>(responseJson);
            return VREditPermissionData;
        }
        catch (Exception e)
        {
            //UnityLogCustom.DebugErrorLog("GetPasswordForEditContentAsync catch");
            DebugExtension.Log(e.Message);
            if ("HTTP/1.1 404 Not Found".Equals(e.Message))
                PopupRuntimeManager.Instance.ShowPopupOnlyConfirm(StringManager.FILE_NOT_FOUND_IN_SERVER);
            else if ("Cannot connect to destination host".Equals(e.Message))
                PopupRuntimeManager.Instance.ShowPopupOnlyConfirm(StringManager.CAN_NOT_CONNECT_TO_SERVER_IP);
            else if ("Unknonw Error".Equals(e.Message))
                PopupRuntimeManager.Instance.ShowPopupOnlyConfirm(StringManager.CAN_NOT_CONNECT_TO_SERVER_IP);
            else
                PopupRuntimeManager.Instance.ShowPopupOnlyConfirm(StringManager.CAN_NOT_CONNECT_TO_SERVER_IP);
            return null;
        }
    }

    public static async UniTask GetConfigServer()
    {
        ConfigServerData = await GetConfigServerAsync();
    }
    public static async UniTask<VrConfigServer> GetConfigServerAsync()
    {
        var absolutePath = $"/{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT}/{VrDomeAssetResourceNameDefine.CONFIG_SERVER_FILENAME}";
        var url = $"{GameContext.Uri}{absolutePath}";

        try
        {
            DebugExtension.Log(url);
            var responseJson = await ObservableUnityWebRequest.GetAsObservable(url).Timeout(TimeSpan.FromSeconds(30));
            DebugExtension.Log(responseJson);
            var vrConfig = JsonUtility.FromJson<VrConfigServer>(responseJson);
            return vrConfig;
        }
        catch (Exception e)
        {
            return new VrConfigServer();
        }
    }

    public static string AbsolutePathVrContentDataVersion(string contentName)
    {
        var absolutePath = contentName != VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT
            ? $"/{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT}/{contentName}/{VrContentFileNameConstant.VERSION_JSON}"
            : $"/{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT}/{VrContentFileNameConstant.VERSION_JSON}";

        return absolutePath;
    }

    public static string AbsolutePathVrDataJson(string contentName)
    {
        return
            $"/{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT}/{contentName}/{VrDomeAssetResourceNameDefine.VR_JSON}/{VrContentFileNameConstant.VR_DATA_JSON}";
    }
    public static string AbsolutePathVrDataJsonTemp(string contentName)
    {
        return
            $"/{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT}/{contentName}/{VrDomeAssetResourceNameDefine.VR_JSON}/{VrContentFileNameConstant.VR_DATA_JSON_TEMP}";
    }

    public static string AbsolutePathVrContentDataIcon(string contentName, string iconName)
    {
        return
            $"/{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT}/{contentName}/{iconName}";
    }

    public static string AbsolutePathVrContentDataTitle(string contentName)
    {
        return
            $"/{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT}/{contentName}/{VrContentFileNameConstant.CONTENT_DATA_TITLE_JSON}";
    }

    public static string AbsolutePathVrModel(string contentName, string modelFile)
    {
        return
            $"/{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT}/{contentName}/{VrDomeAssetResourceNameDefine.VR_MODEL}/{modelFile}";
    }

    public static string AbsolutePathVrPdf(string contentName, string pdfFile)
    {
        return
            $"/{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT}/{contentName}/{VrDomeAssetResourceNameDefine.VR_PDF}/{pdfFile}";
    }

    public static IResourceLoader ResourceLoader
    {
        get
        {
            // #if UNITY_ANDROID
            if (IsDemo)
                return resourceLoaderCache_Demo ?? (resourceLoaderCache_Demo = new LocalResourceLoader(VrDomeAssetResourceNameDefine.DEMO_ROOT_PATH));
            else
                return resourceLoaderCache ?? (resourceLoaderCache = new LocalResourceLoader(Application.persistentDataPath));
            // #else
            //             return resourceLoaderCache ??
            //                    (resourceLoaderCache = new LocalResourceLoader(GameContext.VrResourceRootPathOnServer));
            // #endif
        }
    }
}