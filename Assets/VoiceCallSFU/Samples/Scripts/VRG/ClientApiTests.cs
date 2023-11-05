using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientApiTests : MonoBehaviour
{
    string endpoint = "wss://webrtc.vr-garden.dev.mirabo.tech:2567/";
    private void Start()
    {
        Dictionary<string, object> options = new Dictionary<string, object>
        {
            ["username"] = "LamNT",
            ["lang"] = "en"
        };
        var clientApi = new VrgRoomClient(endpoint);
        clientApi.ConnectAsync("vrg", options).ConfigureAwait(true);
    }
}
