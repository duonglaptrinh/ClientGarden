using TWT.Model;
using TWT.Networking;
using UnityEngine;

public class VRStackDataSync
{
    static VRObjectSync vrSync = null;
    static VRObjectSync VRSync
    {
        get
        {
            if (vrSync == null)
                vrSync = GameObject.FindObjectOfType<VRObjectSync>();
            return vrSync;
        }
    }
    public static VrArrowNextDomeMessage vrArrowNextDomeMessage { get; set; } = null;
    public static void CheckNextDomeStack(int domeId)
    {
        if (vrArrowNextDomeMessage == null)
            return;
        if (domeId != vrArrowNextDomeMessage.DomeId)
        {
            //DebugExtension.LogError("CheckNextDomeStack Load = " + domeId);
            VRSync.SyncDomeId(vrArrowNextDomeMessage);
        }
    }
    public static void AddToStack(VrArrowNextDomeMessage mes)
    {
        if (BaseScreenCtrlV2.IsLoadingRoom)
        {
            vrArrowNextDomeMessage = mes;
        }
        else
        {
            vrArrowNextDomeMessage = mes;
            VRSync.SyncDomeId(mes);
        }
    }
}
