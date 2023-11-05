using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TWT.Utility
{
    public enum TypeActiveMode
    {
        None,
        Unlimited,
        Limited,
        Sub
    }
    public static class PlayerPrefsConstant
    {
        public const string IP_CONFIG = "IP_CONFIG";
        public const string SELECTED_AVT_NAME = "SELECTED_AVT_NAME";
        public const string PLAYER_NAME = "PLAYER_NAME";
        public const string PASSWORD = "PASSWORD";
        public const string TEACHER_PERMISSION = "TEACHER_PERMISSION";
        public const string TRAINING_ACTIVE= "TRAINING_ACTIVE";
        public const string TRAINING_ACTIVE_EXPIRED_DATE= "TRAINING_ACTIVE_EXPIRY_DATE";
        public const string TRAINING_TYPE_ACTIVE = "TRAINING_TYPE_ACTIVE";
        public const string TRAINING_DEVICE_ID = "TRAINING_DEVICE_ID";
        public const string TRAINING_ACTIVE_DATE = "TRAINING_ACTIVE_DATE";
        public const string TRAINING_VERSION_APP = "TRAINING_VERSION_APP";
        public const string TRAINING_ACTIVED = "TRAINING_ACTIVED";
        public const string REALTIME_ACTIVE = "REALTIME_ACTIVE";
        public const string REALTIME_ACTIVE_EXPIRED_DATE = "REALTIME_ACTIVE_EXPIRY_DATE";
        public const string REALTIME_TYPE_ACTIVE = "REALTIME_TYPE_ACTIVE";
        public const string REALTIME_DEVICE_ID = "REALTIME_DEVICE_ID";
        public const string REALTIME_ACTIVE_DATE = "REALTIME_ACTIVE_DATE";
        public const string REALTIME_VERSION_APP = "REALTIME_VERSION_APP";
        public const string REALTIME_ACTIVED = "REALTIME_ACTIVED";
        public const string SAVED_ID_TO_KEYCHAIN = "SAVED_ID_TO_KEYCHAIN";
        public const string DATA_FOLDER = "DATA_FOLDER";
        public const string SERVER_STATUS = "SERVER_STATUS";

        public static string GetVersionContent(string contentName) =>
            PlayerPrefs.GetString($"Version_{contentName}", "");

        public static void SetVersionContent(string contentName, string value)
        {
            PlayerPrefs.SetString($"Version_{contentName}", value);
            PlayerPrefs.Save();
        }

        public static void DeleteVersionContent(string contentName)
        {
            PlayerPrefs.DeleteKey($"Version_{contentName}");
            PlayerPrefs.Save();
        }

        public static bool IsTeacher;
        public static void SetTeacherPermission(bool isTeacher)
        {
            IsTeacher = isTeacher;
            PlayerPrefs.SetInt(TEACHER_PERMISSION, isTeacher ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}