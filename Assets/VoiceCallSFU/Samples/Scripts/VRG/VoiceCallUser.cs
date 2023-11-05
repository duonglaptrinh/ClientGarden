using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VoiceCallUser
{
    public string username;

    public VoiceCallUser(string username)
    {
        this.username = username;
    }
}
