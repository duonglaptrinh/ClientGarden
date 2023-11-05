using jp.co.mirabo.Application.RoomManagement;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebGLAdapter : SingletonMonoBehaviour<WebGLAdapter>
{
    public static bool IsMobileDevice { get; private set; } = false;
    public static bool IsOverlayMenuVideoChat { get; private set; } = false;

#if !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadImage(string url);
#endif
    public static void DownloadImageByUrl(string url)
    {
#if !UNITY_EDITOR
        DownloadImage(url);
#endif
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    //call from javascript
    public void SetIDRoomOnCallFromWebGL(string idRoom)
    {
        RuntimeData.RoomID = idRoom;
        DebugExtension.Log("SET ROOM ID = " + idRoom);
    }
    //call from javascript
    public void SetOverlayMenuVideoChat(string strOverlay)
    {
        bool isOverlay = false;
        bool.TryParse(strOverlay, out isOverlay);

        IsOverlayMenuVideoChat = isOverlay;
    }
    //call from javascript
    public void SetIsMobile(string isMobile)
    {
        bool isMobileb = false;
        bool.TryParse(isMobile, out isMobileb);

        IsMobileDevice = isMobileb;
    }
}
