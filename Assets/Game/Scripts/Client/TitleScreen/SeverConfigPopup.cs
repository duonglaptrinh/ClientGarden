using Game.Client;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TWT.Model;
using TWT.Utility;
using UnityEngine;
using UnityEngine.UI;

public class SeverConfigPopup : MonoBehaviour
{
    [SerializeField] InputField maxTimeoutAutoDisconnect;
    // Ping Training Setup
    [SerializeField] Toggle pingIsCheck;
    [SerializeField] Toggle pingLog;
    [SerializeField] Toggle pingPopup;
    [SerializeField] InputField pingtimeDis;
    [SerializeField] InputField pingMax;
    [SerializeField] InputField pingLoop;
    // Ping Realtime Setup
    [SerializeField] Toggle pingIsCheckRealtime;
    [SerializeField] Toggle pingLogRealtime;
    [SerializeField] Toggle pingPopupRealtime;
    [SerializeField] InputField pingtimeDisRealtime;
    [SerializeField] InputField pingMaxRealtime;
    [SerializeField] InputField pingLoopRealtime;
    //Image Compress
    [SerializeField] Toggle imageCompress;
    [SerializeField] Toggle imageLog;
    [SerializeField] InputField imageQuaility;

    VrConfigServer content;
    static string filePath
    {
        get
        {
            return GetPath(Application.persistentDataPath);
        }
    }
    static string filePathRoot
    {
        get
        {
            return GetPath(GameContext.VrResourceRootPathOnServer);
        }
    }

    static string GetPath(string systemPath)
    {
        string root = $"{systemPath}/{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}";
        if (!Directory.Exists(root))
        {
            DebugExtension.Log("Create Folder " + root);
            Directory.CreateDirectory(root);
        }
        string systemRoot = $"{root}/{VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT}";
        if (!Directory.Exists(systemRoot))
        {
            DebugExtension.Log("Create Folder " + systemRoot);
            Directory.CreateDirectory(systemRoot);
        }
        return $"{systemRoot}/{VrDomeAssetResourceNameDefine.CONFIG_SERVER_FILENAME}";
    }
    private void OnEnable()
    {
        content = ReadFileOnServer();
        DebugExtension.Log(JsonUtility.ToJson(content));
        maxTimeoutAutoDisconnect.text = content.max_timeout_auto_disconnect.ToString();

        pingIsCheck.isOn = content.PingTrainingMode.is_check_ping;
        pingLog.isOn = content.PingTrainingMode.is_log;
        pingPopup.isOn = content.PingTrainingMode.popup_warning;
        pingtimeDis.text = content.PingTrainingMode.time_distance_to_show_popup.ToString();
        pingMax.text = content.PingTrainingMode.max_stable_ping.ToString();
        pingLoop.text = content.PingTrainingMode.time_ping_loop.ToString();

        pingIsCheckRealtime.isOn = content.PingRealtimeMode.is_check_ping;
        pingLogRealtime.isOn = content.PingRealtimeMode.is_log;
        pingPopupRealtime.isOn = content.PingRealtimeMode.popup_warning;
        pingtimeDisRealtime.text = content.PingRealtimeMode.time_distance_to_show_popup.ToString();
        pingMaxRealtime.text = content.PingRealtimeMode.max_stable_ping.ToString();
        pingLoopRealtime.text = content.PingRealtimeMode.time_ping_loop.ToString();

        imageCompress.isOn = content.Image360Encode.is_allow_compress;
        imageLog.isOn = content.Image360Encode.is_log;
        imageQuaility.text = content.Image360Encode.quaility.ToString();
    }

    void CheckValue(InputField input, string value, int defaultValue)
    {
        int time;
        if (int.TryParse(value, out time))
        {
            if (time < 0)
            {
                time = defaultValue;
                input.text = time.ToString();
            }
        }
    }

    void CheckImageQuaility(string value)
    {
        //if (string.IsNullOrEmpty(value))
        //{
        //    imageQuaility.text = data.Image360Encode.quaility.ToString();
        //    return;
        //}
        int time;
        if (int.TryParse(value, out time))
        {
            if (time < 1)
            {
                time = 1;
                imageQuaility.text = time.ToString();
            }
            if (time > 100)
            {
                time = 100;
                imageQuaility.text = time.ToString();
            }
        }
    }
    private void Start()
    {
        maxTimeoutAutoDisconnect.onValueChanged.AddListener(value =>
        {
            CheckValue(maxTimeoutAutoDisconnect, value, 15);
        });

        pingtimeDis.onValueChanged.AddListener(value =>
        {
            CheckValue(pingtimeDis, value, 5);
        });
        pingMax.onValueChanged.AddListener(value =>
        {
            CheckValue(pingMax, value, 150);
        });
        pingLoop.onValueChanged.AddListener(value =>
        {
            CheckValue(pingLoop, value, 1);
        });

        pingtimeDisRealtime.onValueChanged.AddListener(value =>
        {
            CheckValue(pingtimeDisRealtime, value, 5);
        });
        pingMaxRealtime.onValueChanged.AddListener(value =>
        {
            CheckValue(pingMaxRealtime, value, 150);
        });
        pingLoopRealtime.onValueChanged.AddListener(value =>
        {
            CheckValue(pingLoopRealtime, value, 1);
        });

        imageQuaility.onValueChanged.AddListener(CheckImageQuaility);
    }

    public static void CreateFileConfigOnServer()
    {
        //create file on persistentDatapath --> client app will download on it
        CreateFileOnServer(filePath);
        //create file on Root folder --> change setting and restart pc admin to update.
        CreateFileOnServer(filePathRoot);
    }
    static void CreateFileOnServer(string path)
    {
        string file = path;
        if (!File.Exists(file))
        {
            VrConfigServer data;
            data = new VrConfigServer();
            File.WriteAllText(file, JsonUtility.ToJson(data, true));
            DebugExtension.Log($"Auto generate config server file");
        }
    }
    public static VrConfigServer ReadFileOnServer()
    {
        return JsonUtility.FromJson<VrConfigServer>(File.ReadAllText(filePath));
    }

    public void SaveFileConfigServer()
    {
        if (content == null) return;
        int number;
        if (int.TryParse(maxTimeoutAutoDisconnect.text, out number))
        {
            content.max_timeout_auto_disconnect = number;
        }

        content.PingTrainingMode.is_check_ping = pingIsCheck.isOn;
        content.PingTrainingMode.is_log = pingLog.isOn;
        content.PingTrainingMode.popup_warning = pingPopup.isOn;
        if (int.TryParse(pingtimeDis.text, out number))
        {
            content.PingTrainingMode.time_distance_to_show_popup = number;
        }
        if (int.TryParse(pingMax.text, out number))
        {
            content.PingTrainingMode.max_stable_ping = number;
        }
        if (int.TryParse(pingLoop.text, out number))
        {
            content.PingTrainingMode.time_ping_loop = number;
        }

        content.PingRealtimeMode.is_check_ping = pingIsCheckRealtime.isOn;
        content.PingRealtimeMode.is_log = pingLogRealtime.isOn;
        content.PingRealtimeMode.popup_warning = pingPopupRealtime.isOn;
        if (int.TryParse(pingtimeDisRealtime.text, out number))
        {
            content.PingRealtimeMode.time_distance_to_show_popup = number;
        }
        if (int.TryParse(pingMaxRealtime.text, out number))
        {
            content.PingRealtimeMode.max_stable_ping = number;
        }
        if (int.TryParse(pingLoopRealtime.text, out number))
        {
            content.PingRealtimeMode.time_ping_loop = number;
        }

        content.Image360Encode.is_allow_compress = imageCompress.isOn;
        content.Image360Encode.is_log = imageLog.isOn;
        if (int.TryParse(imageQuaility.text, out number))
        {
            content.Image360Encode.quaility = number;
        }

        string json = JsonUtility.ToJson(content);

        //Save persistentdatapath
        File.WriteAllText(filePath, json);

        //Save root data
        var savedFolder = PlayerPrefs.GetString(PlayerPrefsConstant.DATA_FOLDER, "");
        if (!string.IsNullOrEmpty(savedFolder))
        {
            File.WriteAllText(filePathRoot, json);
        }
        DebugExtension.Log(json);
    }

}
