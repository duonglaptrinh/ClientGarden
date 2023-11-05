using System.Text;
using UnityEngine;

public static class DebugExtension
{
    private static StringBuilder sb = new StringBuilder();
    const string RoomIDToShowTest = "20";
    static bool IsCanShow
    {
        get
        {
#if UNITY_EDITOR
            return true;
#endif
            return RuntimeData.RoomID.Equals(RoomIDToShowTest);
        }
    }
    public static void Log(object message, Color color = default)
    {
        if (!IsCanShow) return;
        Debug.Log(sb.Clear()
                    .Append("<color=#")
                    .Append(ColorUtility.ToHtmlStringRGB(color))
                    .Append(">")
                    .Append(message)
                    .Append("</color>"));
    }
    public static void Log(object message)
    {
        if (!IsCanShow) return;
        Debug.Log(sb.Clear().Append(message));
    }
    public static void LogError(object message)
    {
        if (!IsCanShow) return;
        Debug.LogError(sb.Clear().Append(message));
    }
    public static void LogWarning(object message)
    {
        if (!IsCanShow) return;
        Debug.LogWarning(sb.Clear().Append(message));
    }
}