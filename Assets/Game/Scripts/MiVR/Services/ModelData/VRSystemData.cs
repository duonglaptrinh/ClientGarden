using System;

namespace TWT.Model
{
    [Serializable]
    public class VRSystemData
    {

    }

    [Serializable]
    public class VREditPermissionData
    {
        public string password;
        public string version;
        public string date_training;
        public string date_realtime;
        public PasswordForPlayMode passwordForPlayMode;
        public VREditPermissionData(string password = "12345", string date_training = "", string date_realtime = "")
        {
            this.password = password;
            this.version = GameContext.APP_VERSION;
            this.date_training = date_training;
            this.date_realtime = date_realtime;
            this.passwordForPlayMode = new PasswordForPlayMode();
        }
    }

    [Serializable]
    public class PasswordForPlayMode
    {
        public bool isActive = false;
        public string password = "1234";
    }
}
