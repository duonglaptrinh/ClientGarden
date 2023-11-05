using Game.Client;
using System;
using System.IO;
using TWT.Model;
using TWT.Utility;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CheckActiveUserComponent : MonoBehaviour
{
    public enum ECheckActiveType
    {
        None,
        Is_BACK_DATE,
        IS_CHECK_ONLINE,
        IS_CHECK_ONLINE_FAIL,
        IS_CHECK_OFFLINE,
        Is_CODE_TEST
    }
    Action<ECheckActiveType> onDone;

    bool isCheckTrainingOnlineSuccess = false;

    public const string keyTest = "CHEAT-CODEX-MIRAB-O2050";

    const string keyLastTime = "Key_Open_App_Time";
    static JsonDateTime lasttimeOpenAppData;
    public static JsonDateTime LastTimeOpenApp
    {
        get
        {
            string json;
            if (PlayerPrefs.HasKey(keyLastTime))
                json = PlayerPrefs.GetString(keyLastTime);
            else
                json = JsonUtility.ToJson(new JsonDateTime());
            lasttimeOpenAppData = JsonUtility.FromJson<JsonDateTime>(json);
            return lasttimeOpenAppData;
        }
        set
        {
            lasttimeOpenAppData = value;
            PlayerPrefs.SetString(keyLastTime, JsonUtility.ToJson(value));
        }
    }

    void CallbackOnDone(ECheckActiveType type)
    {
        if (type == ECheckActiveType.IS_CHECK_OFFLINE)
        {
            JsonDateTime now = DateTime.Now;
            TimeSpan temp = SubTimeFromOldToNow(LastTimeOpenApp);
            DebugExtension.Log("Totalseconds from LastTimeOpenApp to now = " + temp.TotalSeconds);
            if (temp.TotalSeconds < 0) // if Back date
            {
                LastTimeOpenApp = DateTime.Now;

                CheckActiveUser.ResetTrainingMode();// App is back date -- Reset all
                CheckActiveUser.ResetRealtimeMode();// App is back date -- Reset all
                DebugExtension.Log("TIME ON THIS APP IS BACK DATE -> RESET ALL DATA");

                //RESET FILE edit_password.json --> client can not get permission
#if ADMIN
            VrResourceStruct.CheckFilePassword(); 
#endif

                onDone?.Invoke(ECheckActiveType.Is_BACK_DATE);
                return;
            }
        }
        LastTimeOpenApp = DateTime.Now;
        onDone?.Invoke(type);
    }

    public void CheckTypeUnlimited()
    {
        TypeActiveMode type = (TypeActiveMode)PlayerPrefs.GetInt(PlayerPrefsConstant.TRAINING_TYPE_ACTIVE, 0);
        if (type == TypeActiveMode.Unlimited)
        {
            DebugExtension.Log("TYPE Training mode ACTIVE UNLIMITED");
            GameContext.ActivedTrainingMode = true;
            PlayerPrefs.SetInt(PlayerPrefsConstant.TRAINING_ACTIVED, 1); // Set license code active unlimited
        }
        type = (TypeActiveMode)PlayerPrefs.GetInt(PlayerPrefsConstant.REALTIME_TYPE_ACTIVE, 0);
        if (type == TypeActiveMode.Unlimited)
        {
            DebugExtension.Log("TYPE Realtime ACTIVE UNLIMITED");
            GameContext.ActivedRealtimeMode = true;
            PlayerPrefs.SetInt(PlayerPrefsConstant.REALTIME_ACTIVED, 1); // Set license code active unlimited
        }
    }

    public virtual void CheckActiveUserOnline(Action<ECheckActiveType> OnCheckActive)
    {
        onDone = OnCheckActive;
        CheckActiveTrainningOnline(() =>
        {
            CallbackOnDone(ECheckActiveType.IS_CHECK_OFFLINE);
            Destroy(this);
        });
    }
    void CheckActiveTrainningOnline(Action onOffline)
    {
        string training_code = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_ACTIVE, "");
        string training_date_expired = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_ACTIVE_EXPIRED_DATE, "");
        string training_device_id = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_DEVICE_ID, "");
        string training_active_date = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_ACTIVE_DATE, "");
        string training_version = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_VERSION_APP, "");

        if (GameContext.IsUseCodeTest)
        {
            if (training_code.Equals(keyTest))
            {
                CallbackOnDone(ECheckActiveType.Is_CODE_TEST);
                Destroy(this);
                return;
            }
            else
            {
                string realtime_code = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_ACTIVE, "");
                if (realtime_code.Equals(keyTest))
                {
                    CallbackOnDone(ECheckActiveType.Is_CODE_TEST);
                    Destroy(this);
                    return;
                }
            }
        }

        if (string.IsNullOrEmpty(training_code))
        {
            CheckActiveRealtimeOnline(onOffline);
            return;
        }
        //Check online Training Mode
        ApiController.SendCodeRequest(UrlConfig.CHECK_ACTIVE_API, training_code, CheckActiveUser.DeviceID, (int)GameMode.TrainingMode,
        codeData =>
        {
            isCheckTrainingOnlineSuccess = true;
            PlayerPrefs.SetString(PlayerPrefsConstant.TRAINING_ACTIVE_EXPIRED_DATE, GetExpiredTime(codeData));
            //DebugExtension.Log(PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_ACTIVE_EXPIRED_DATE));
            GameContext.ActivedTrainingMode = codeData.code_check;
            CheckActiveRealtimeOnline(onOffline);
        },
        () => // Check Error 
        {
            isCheckTrainingOnlineSuccess = false;
            GameContext.ActivedTrainingMode = false;
            CheckActiveRealtimeOnline(onOffline);
        }, () => // No found network
        {
            onOffline?.Invoke();
        }, () => // Check code fail - not same device id
        {
            PlayerPrefs.SetString(PlayerPrefsConstant.TRAINING_ACTIVE, "");
            //CallbackOnDone(ECheckActiveType.IS_CHECK_ONLINE_FAIL);
            //Destroy(this);
            isCheckTrainingOnlineSuccess = false;
            GameContext.ActivedTrainingMode = false;
            CheckActiveRealtimeOnline(onOffline);
        });
    }
    void CheckActiveRealtimeOnline(Action onOffline)
    {
        string realtime_code = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_ACTIVE, "");
        string realtime_date_expired = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_ACTIVE_EXPIRED_DATE, "");
        string realtime_device_id = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_DEVICE_ID, "");
        string realtime_active_date = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_ACTIVE_DATE, "");
        string realtime_version = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_VERSION_APP, "");
        if (string.IsNullOrEmpty(realtime_code))
        {
            if (isCheckTrainingOnlineSuccess)
                OnDoneCheckOnline();
            else
            {
                CallbackOnDone(ECheckActiveType.IS_CHECK_OFFLINE);
                Destroy(this);
            }
            return;
        }
        //Check online realtime Mode
        ApiController.SendCodeRequest(UrlConfig.CHECK_ACTIVE_API, realtime_code, CheckActiveUser.DeviceID, (int)GameMode.RealtimeMode,
            codeData =>
            {
                PlayerPrefs.SetString(PlayerPrefsConstant.REALTIME_ACTIVE_EXPIRED_DATE, GetExpiredTime(codeData));
                GameContext.ActivedRealtimeMode = codeData.code_check;
                OnDoneCheckOnline();
            },
            () => // Check Error 
            {
                GameContext.ActivedRealtimeMode = false;
                //OnDoneCheckOnline();
                onOffline?.Invoke();
            }, () => // No found network
            {
                onOffline?.Invoke();
            }, () => // Check code fail - not same device id
            {
                GameContext.ActivedRealtimeMode = false;
                PlayerPrefs.SetString(PlayerPrefsConstant.REALTIME_ACTIVE, "");
                CallbackOnDone(ECheckActiveType.IS_CHECK_ONLINE_FAIL);
                Destroy(this);
            });
    }

    string GetExpiredTime(CodeData data)
    {
        DateTime myDate;
        JsonDateTime expiredDate;
        if (string.IsNullOrEmpty(data.end_date))
        {
            DateTime dateTime = DateTime.Now.AddYears(100);
            expiredDate = dateTime;
        }
        else
        {
            if (DateTime.TryParse(data.end_date, out myDate))
                expiredDate = myDate;
            else
            {
                DateTime dateTime = DateTime.Now.AddYears(100);
                expiredDate = dateTime;
            }
        }
        string expired = JsonUtility.ToJson(expiredDate);
        return expired;
    }
    void OnDoneCheckOnline()
    {
        CallbackOnDone(ECheckActiveType.IS_CHECK_ONLINE);
        Destroy(this);
    }
    /// <summary>
    /// Distance time from oldDate to now
    /// </summary>
    /// <param name="old"></param>
    /// <returns></returns>
    public static TimeSpan SubTimeFromOldToNow(DateTime old)
    {
        //DateTime now = DateTime.Now;
        JsonDateTime date = DateTime.Now;
        DateTime now = date;

        //DebugExtension.Log("old = " + old.Date.ToString() + "    now = " + now.Date.ToString());
        TimeSpan temp = now - old;
        //TimeSpan temp = now.Date - old.Date; //only Day

        return temp;
    }
}
