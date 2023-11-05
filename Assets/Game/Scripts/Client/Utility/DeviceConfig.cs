using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class DeviceConfig : MonoBehaviour
{
    public static DeviceConfig instance;
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this);
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        SetDeviceNeverSleep();
        RequestMicrophonePermission();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDeviceNeverSleep()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public void RequestMicrophonePermission()
    {
#if UNITY_IOS
            Application.RequestUserAuthorization(UserAuthorization.Microphone);
#elif UNITY_ANDROID
            Permission.RequestUserPermission(Permission.Microphone);
#endif
    }

    public void SetFPS(int fps)
    {
        Application.targetFrameRate = fps;
    }
}
