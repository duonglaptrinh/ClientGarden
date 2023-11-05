using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileControllerDetector : MonoBehaviour
{
    [SerializeField] GameObject uiJoyStick;
    [SerializeField] GameObject uiInputSystem;
    // Start is called before the first frame update
    void Start()
    {
        uiJoyStick.SetActive(WebGLAdapter.IsMobileDevice);
        uiInputSystem.SetActive(WebGLAdapter.IsMobileDevice);
    }
}
