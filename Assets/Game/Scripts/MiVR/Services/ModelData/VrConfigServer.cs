using System;

namespace TWT.Model
{
    [Serializable]
    public class VrConfigServer
    {
        public PingConfig PingTrainingMode;
        public PingConfig PingRealtimeMode;
        public Image360EncodeConfig Image360Encode;
        public int max_timeout_auto_disconnect = 30; //seconds
        //Create Default
        public VrConfigServer()
        {
            PingTrainingMode = new PingConfig();
            PingRealtimeMode = new PingConfig();
            Image360Encode = new Image360EncodeConfig();
        }
    }
    [Serializable]
    public class PingConfig
    {
        public bool is_check_ping = false;
        public bool is_log = true;
        public bool popup_warning = false;
        public int max_stable_ping = 150;
        public int time_distance_to_show_popup = 5; //minute
        public int time_ping_loop = 1; //second
    }
    [Serializable]
    public class Image360EncodeConfig
    {
        public bool is_allow_compress = false;
        public bool is_log = true;
        public int quaility = 75;
    }

}

