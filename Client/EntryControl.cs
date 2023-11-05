using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EntryControl : MonoBehaviour {

    public void StartClient()
    {
        if(NetworkManager.singleton!=null)
        {
            NetworkManager.singleton.StartClient();
        }
    }
}
