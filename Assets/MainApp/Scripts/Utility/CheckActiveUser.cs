using Game.Client;
using System;
using System.IO;
using TWT.Client.TitleScene;
using TWT.Model;
using TWT.Utility;
using Cysharp.Threading.Tasks;
using UnityEngine;
#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class CheckActiveUser
{
    public static string EncryptTrainingDataPath => Path.Combine(Application.dataPath, "takasho.sys");
    public static string EncryptRealtimeDataPath => Path.Combine(Application.dataPath, "iRealtime.sys");
    public static string DeviceID;

    public static void CheckModeActive()
    {
        string training_code = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_ACTIVE, "");
        string training_date_expired = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_ACTIVE_EXPIRED_DATE, "");
        string training_device_id = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_DEVICE_ID, "");
        string training_active_date = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_ACTIVE_DATE, "");
        string training_version = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_VERSION_APP, "");

        GameContext.ActivedTrainingMode = Validate(training_version, training_device_id, training_code, training_active_date, training_date_expired, GameMode.TrainingMode);
       // TitleScreenCtrl.instance.ActiveBtnActiveTrainingMode();

        string realtime_code = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_ACTIVE, "");
        string realtime_date_expired = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_ACTIVE_EXPIRED_DATE, "");
        string realtime_device_id = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_DEVICE_ID, "");
        string realtime_active_date = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_ACTIVE_DATE, "");
        string realtime_version = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_VERSION_APP, "");

        GameContext.ActivedRealtimeMode = Validate(realtime_version, realtime_device_id, realtime_code, realtime_active_date, realtime_date_expired, GameMode.RealtimeMode);
       // TitleScreenCtrl.instance.ActiveBtnActiveRealtimeMode();
    }

    private static bool Validate(string version, string device_id, string code, string active_date, string date_expired, GameMode mode)
    {
        if (!CheckDateExpiry(date_expired, mode)) return false;

        return GameContext.APP_VERSION.Equals(version) && !string.IsNullOrEmpty(device_id)
            && device_id.Equals(DeviceID) && !string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(active_date);
    }

    public static bool CheckDateExpiry(string date_expired, GameMode mode)
    {
        if (string.IsNullOrEmpty(date_expired)) return false;
        try
        {
            DebugExtension.Log("Mode ==== " + mode);
            JsonDateTime expiredDate = JsonUtility.FromJson<JsonDateTime>(date_expired);
            TimeSpan time = SubTimeFromOldToNow(expiredDate, isDebug: true, mode);
            if (time.TotalSeconds < 0) // not expiry
            {
                return true;
            }
            else // Expired and need reset all code active
            {
                if (mode == GameMode.TrainingMode)
                {
                    ResetTrainingMode(); //license code expried
                    DebugExtension.Log("CODE TRAINNING MODE IS EXPRIED");
                }
                else
                {
                    ResetRealtimeMode(); //license code expried
                    DebugExtension.Log("CODE REALTIME MODE IS EXPRIED");
                }
                //RESET FILE edit_password.json --> client can not get permission
#if ADMIN
            VrResourceStruct.CheckFilePassword();
#endif
                return false;
            }
        }
        catch (Exception e)
        {
            DebugExtension.LogError("Expiry date not a Json string " + date_expired + "\n" + e.Message);
            return false;
        }
    }
    public static void CheckAdminActiveMode()
    {
        string training_code = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_ACTIVE, "");
        string training_date_expired = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_ACTIVE_EXPIRED_DATE, "");
        string training_device_id = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_DEVICE_ID, "");
        string training_active_date = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_ACTIVE_DATE, "");
        string training_version = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_VERSION_APP, "");

        GameContext.ActivedTrainingMode = CheckActiveModeInfo(training_code, training_device_id, training_active_date, training_date_expired, GameMode.TrainingMode);
        PlayerPrefs.SetInt(PlayerPrefsConstant.TRAINING_ACTIVED, GameContext.ActivedTrainingMode ? 1 : 0); // check license code active when start app offline

        string realtime_code = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_ACTIVE, "");
        string realtime_date_expired = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_ACTIVE_EXPIRED_DATE, "");
        string realtime_device_id = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_DEVICE_ID, "");
        string realtime_active_date = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_ACTIVE_DATE, "");
        string realtime_version = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_VERSION_APP, "");

        GameContext.ActivedRealtimeMode = CheckActiveModeInfo(realtime_code, realtime_device_id, realtime_active_date, realtime_date_expired, GameMode.RealtimeMode);
        PlayerPrefs.SetInt(PlayerPrefsConstant.REALTIME_ACTIVED, GameContext.ActivedRealtimeMode ? 1 : 0); // check license code active when start app offline
        SaveAdminActivedMode().Forget();
    }

    private static async UniTask SaveAdminActivedMode()
    {
        // validate data
        if (GameContext.ActivedTrainingMode && GameContext.VrResourceRootPathOnServer != null)
            await VrResourceStruct.ValidateAsync(GameContext.VrResourceRootPathOnServer);
    }

    private static bool CheckActiveModeInfo(string code, string device_id, string active_date, string date_expired, GameMode mode = GameMode.TrainingMode)
    {
        string path = EncryptTrainingDataPath;
        if (GameMode.RealtimeMode == mode)
            path = EncryptRealtimeDataPath;

        bool check_file = File.Exists(path);
        if (!check_file)
        {
            return false;
        }
        try
        {
            BinaryReader binReader = new BinaryReader(File.Open(path, FileMode.Open));
            string content_decrypt = Game.Client.Utility.Decrypt(binReader.ReadString());

            string[] content = content_decrypt.Split('+');

            bool check_code = content[0].Equals(code);
            bool check_device_id = content[1].Equals(SystemInfo.deviceUniqueIdentifier);
            bool check_app_version = content[2].Equals(GameContext.APP_VERSION);
            bool check_package_name = content[3].Equals(Application.identifier);
            bool check_active_date = !string.IsNullOrEmpty(content[4]) && content[4].Equals(active_date);
            //bool check_date_expired = !string.IsNullOrEmpty(content[5]) && content[5].Equals(date_expired.ToString());
            bool check_mode = !string.IsNullOrEmpty(content[6]) && content[6].Equals(mode.ToString());

            if (!CheckDateExpiry(date_expired, mode)) return false;

            return check_code && check_device_id && check_app_version && check_package_name && check_active_date && check_mode;
        }
        catch (Exception e)
        {
            return false;
        }

    }
    /// <summary>
    /// Distance time from oldDate to now
    /// </summary>
    /// <param name="old"></param>
    /// <returns></returns>
    public static TimeSpan SubTimeFromOldToNow(DateTime old, bool isDebug = false, GameMode mode = GameMode.TrainingMode)
    {
        //DateTime now = DateTime.Now;
        JsonDateTime date = DateTime.Now;
        DateTime now = date;
        TimeSpan temp = now - old;
        if (isDebug)
        {
            DebugExtension.Log($"Mode = {mode}    now = { now.ToString()}  expiry = {old.ToString()} ");
            DebugExtension.Log($"Seconds = {temp.TotalSeconds}");
        }
        //TimeSpan temp = now.Date - old.Date; //only Day

        return temp;
    }

    public static void ResetTrainingMode()
    {
        PlayerPrefs.SetString(PlayerPrefsConstant.TRAINING_ACTIVE, "");
        PlayerPrefs.SetString(PlayerPrefsConstant.TRAINING_ACTIVE_EXPIRED_DATE, "");
        PlayerPrefs.SetInt(PlayerPrefsConstant.TRAINING_ACTIVED, 0); // license code expried
        GameContext.ActivedTrainingMode = false;
        PlayerPrefs.Save();
    }

    public static void ResetRealtimeMode()
    {
        PlayerPrefs.SetString(PlayerPrefsConstant.REALTIME_ACTIVE, "");
        PlayerPrefs.SetString(PlayerPrefsConstant.REALTIME_ACTIVE_EXPIRED_DATE, "");
        PlayerPrefs.SetInt(PlayerPrefsConstant.REALTIME_ACTIVED, 0); // license code expried
        GameContext.ActivedRealtimeMode = false;
        PlayerPrefs.Save();
    }
}
