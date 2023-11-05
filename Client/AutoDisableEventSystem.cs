using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisableEventSystem : MonoBehaviour
{
    private void Awake()
    {
#if !UNITY_EDITOR && !UNITY_STANDALONE
        gameObject.SetActive(false);
#endif
    }
}
