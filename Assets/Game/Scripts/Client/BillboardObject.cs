using UnityEngine;

public class BillboardObject : MonoBehaviour
{
#if !UNITY_WEBGL
    OVRCameraRig ovrCam;

    void Start()
    {
        ovrCam = FindObjectOfType<OVRCameraRig>();
    }
#endif

    void LateUpdate()
    {
#if UNITY_EDITOR || UNITY_WEBGL
        transform.forward = -(Camera.main.transform.position - transform.position).normalized;
#elif UNITY_ANDROID || UNITY_IOS
        if (ovrCam != null) 
        {
            transform.forward = -(ovrCam.transform.position - transform.position).normalized;
        }
#endif
    }
}