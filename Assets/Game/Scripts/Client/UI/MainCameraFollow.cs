using UnityEngine;

public class MainCameraFollow : MonoBehaviour
{
    private void LateUpdate()
    {
        if (Camera.main != null)
        {
            transform.position = Camera.main.transform.position;
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}
