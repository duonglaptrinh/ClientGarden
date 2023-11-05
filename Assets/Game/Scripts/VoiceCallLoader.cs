using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceCallLoader : MonoBehaviour
{
    [SerializeField] GameObject _voiceCallPrefab;
    GameObject _instantiated;

    private void Start()
    {
        ReloadVoiceCallModule();
    }

    [ContextMenu("ReloadVoiceCallModule")]
    public void ReloadVoiceCallModule()
    {
        if (_instantiated != null)
        {
            GameObject.Destroy(_instantiated);
        }

        _instantiated = GameObject.Instantiate(_voiceCallPrefab);
    }
}
