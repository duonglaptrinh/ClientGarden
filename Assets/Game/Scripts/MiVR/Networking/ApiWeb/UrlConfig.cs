using UnityEngine;

public class UrlConfig
{
    public static string BASE_URL
    {
        get
        {
            return "https://mirabo-lic-sv.net"  ;
        }
        set
        {
           // PlayerPrefs.SetString("BASE_URL", value);
        }
    }
    //public const string BASE_URL = "http://27.72.97.102:3000";
    //public const string BASE_URL = "http://27.72.97.102:7778";
    public const string CHECK_ACTIVE_API = "/api/v1/check_active";
    public const string SEND_ACTIVE_API = "/api/v1/active_code";
}

enum UserType
{
    PC = 0,
    Other = 1
}

enum ProductType
{
    XR360 = 0,
    XR = 1
}