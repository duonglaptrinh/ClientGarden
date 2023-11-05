using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;
using TWT.Utility;

public static class DeviceIDManager {

    public delegate void DeviceIDHandler(string deviceID);

    public static event DeviceIDHandler deviceIDHandler;

    public const string UnsupportMacAddress = "02:00:00:00:00:00";

	[DllImport("__Internal")]
	static extern string _Get_Device_id();

    [DllImport("__Internal")]
    static extern int _Update(string account, string password);

    // Use this for initialization
    public static void GetDeviceID () {
		string password = string.Empty;

#if UNITY_IPHONE && !UNITY_EDITOR
		password = _Get_Device_id();

        deviceIDHandler(password);

#else
        password = SystemInfo.deviceUniqueIdentifier;
        deviceIDHandler(password);
#endif

	}

    public static void SaveId(string id)
    {
#if UNITY_IPHONE && !UNITY_EDITOR
		var result = _Update("XR360", id);
        Debug.LogError("SaveOldId: " + result);
#endif
    }

    private static string calcMd5( string srcStr ) {

		System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

		byte[] srcBytes = System.Text.Encoding.UTF8.GetBytes(srcStr);
		byte[] destBytes = md5.ComputeHash(srcBytes);

		System.Text.StringBuilder destStrBuilder;
		destStrBuilder = new System.Text.StringBuilder();
		foreach (byte curByte in destBytes) {
			destStrBuilder.Append(curByte.ToString("x2"));
		}

		return destStrBuilder.ToString();
	}


    public static void SaveOldID()
    {
#if UNITY_IOS
        var saved_keychain = PlayerPrefs.GetInt(PlayerPrefsConstant.SAVED_ID_TO_KEYCHAIN, 0);
        Debug.LogError("saved_keychain: " + saved_keychain);
        if (saved_keychain == 0)
        {
            var old_training_id = PlayerPrefs.GetString(PlayerPrefsConstant.TRAINING_DEVICE_ID, "");
            var old_realtime_id = PlayerPrefs.GetString(PlayerPrefsConstant.REALTIME_DEVICE_ID, "");
            Debug.LogError("old_training_id: " + old_training_id);
            Debug.LogError("old_realtime_id: " + old_realtime_id);
            if (string.IsNullOrEmpty(old_training_id))
            {
                if (!string.IsNullOrEmpty(old_realtime_id))
                {
                    DeviceIDManager.SaveId(old_realtime_id);
                    PlayerPrefs.SetInt(PlayerPrefsConstant.SAVED_ID_TO_KEYCHAIN, 1);
                }
            }
            else
            {
                DeviceIDManager.SaveId(old_training_id);
                PlayerPrefs.SetInt(PlayerPrefsConstant.SAVED_ID_TO_KEYCHAIN, 1);
            }
        }
#endif

    }

}
