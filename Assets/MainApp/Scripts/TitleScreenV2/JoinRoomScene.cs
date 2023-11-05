using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoinRoomScene : MonoBehaviour
{
    [SerializeField] GameObject loading;
    // Start is called before the first frame update
    void OnEnable()
    {
        ConnectServer.OnStartLogin += ShowLoading;
        ConnectServer.OnStartLogin += HideLoading;
    }
    private void OnDisable()
    {
        ConnectServer.OnStartLogin -= ShowLoading;
        ConnectServer.OnStartLogin -= HideLoading;
    }
    private void ShowLoading() => loading.SetActive(true);
    private void HideLoading() => loading.SetActive(false);
}
