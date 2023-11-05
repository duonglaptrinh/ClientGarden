using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonDialog : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual void OnCloseClick()
    {
        gameObject.SetActive(false);
        if (VRObjectTransparentSettingController.instance !=null && 
            VRObjectTransparentSettingController.instance.vRObject != null)
        {
            VRObjectTransparentSettingController.instance.vRObject.ShowMenuUiEdit();
        }
        BaseScreenTopMenuV2.Instance.ResetCameraRotate();
    }
}
