using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VRObjectTransparentSettingController : MonoBehaviour
{
    public static VRObjectTransparentSettingController instance;

    public VRObjectV2 vRObject;

    private void Awake()
    {
        
    }

    private void Start()
    {
        
    }

    private void OnDisable()
    {
        //ReleaseVRObject();
    }

    public void OpenWithVRobject(VRObjectV2 vRObject) { 
        this.vRObject = vRObject;
    }
    void ReleaseVRObject()
    {
        if (this.vRObject != null)
        {
            vRObject.ShowMenuUiEdit();
        }
    }

    public void OnTransparent0Click()
    {
        vRObject.SetObjectTransparent(0);
    }

    public void OnTransparent60Click()
    {
        vRObject.SetObjectTransparent(155);
    }

    public void OnTransparent100Click()
    {
        vRObject.SetObjectTransparent(255);
    }
}
