using Game.Networking.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPingPong : MonoBehaviour
{
    [SerializeField] float _sendInterval = 5f;

    void Start()
    {
        StartCoroutine(DoSendPingPong());
    }

    IEnumerator DoSendPingPong()
    {
        //while (true)
        //{
        //    if (Msf.Connection != null && Msf.Connection.IsConnected)
        //        Msf.Connection.SendMessage((short)MyMes.PingPong);
        yield return new WaitForSeconds(_sendInterval);
        //}
    }
}
