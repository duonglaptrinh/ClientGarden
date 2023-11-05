using System.Collections;
using UnityEngine;


namespace Game
{
    public interface INetworkPlayer
    {
        bool IsLocalPlayer { get; }
        bool IsReady { get; set; }
        int RoleIndex { get; }
        Color ColorUnique { get; }
        GameObject GameObject { get; }
    }
}
