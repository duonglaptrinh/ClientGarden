using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraFollowForMobile : MonoBehaviour
{
    public Vector3 distance;
    private void LateUpdate()
    {
        if (Camera.main != null)
        {
            transform.position = Camera.main.transform.position + distance;
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
