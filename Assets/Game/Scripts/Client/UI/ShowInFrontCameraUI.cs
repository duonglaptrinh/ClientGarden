using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInFrontCameraUI : MonoBehaviour
{
    public float distance = 1f;
    public float alignHeight = 0f;

    private void OnEnable()
    {
        transform.position = Camera.main.transform.position + Camera.main.transform.forward.normalized * distance;
        transform.rotation = Quaternion.Euler(new Vector3(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, 0));
        transform.position += transform.up * alignHeight;
    }
}
