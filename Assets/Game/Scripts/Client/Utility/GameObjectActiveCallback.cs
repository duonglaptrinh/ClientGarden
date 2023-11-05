using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameObjectActiveCallback : MonoBehaviour
{
    public UnityEvent onActive;
    public UnityEvent onInactive;

    private void OnEnable()
    {
        onActive.Invoke();
    }

    private void OnDisable()
    {
        onInactive.Invoke();
    }
}
