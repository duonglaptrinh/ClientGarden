using UnityEngine;

public class CameraFocalLens : MonoBehaviour
{
    public float lenDistance = 0f;

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.forward * lenDistance;
    }
}
