using TWT.Client.Interaction;
using UnityEngine;

public class AutoHideUILayout : MonoBehaviour, IControllerInteractable
{
    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        if ((Input.GetMouseButton(0) 
        #if !UNITY_WEBGL
            || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) ||
             OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) == 1
        #endif
             ) && gameObject.activeSelf)
        {
            Ray ray = this.GetRay();

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity))
            {
                if (hit.transform.gameObject == this.gameObject)
                {
                    return;
                }

                foreach (Transform child in this.gameObject.transform)
                {
                    if (hit.transform.gameObject == child.gameObject)
                        return;
                }
            }

            gameObject.SetActive(false);
        }
    }
    
    public Transform Transform => transform;
}