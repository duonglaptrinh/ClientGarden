using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareLaserBeamCtrl : MonoBehaviour
{
    private void Awake()
    {
        SettingConfig.is_sharing_beam = false;
    }

    // Update is called once per frame
    private void Update()
    {
#if !UNITY_WEBGL
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.All))
        {
            ShareBeam(true);
        }
        
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.All))
        {
            ShareBeam(false);
        }
#endif
    }

    private void ShareBeam(bool value)
    {
        //if (UnetPlayer.LocalPlayer)
        //{
        //    UnetPlayer.LocalPlayer.CmdShareMyBeam(value);
        //}

    }
}
