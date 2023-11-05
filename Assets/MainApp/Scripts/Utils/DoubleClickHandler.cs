using UnityEngine;

public class DoubleClickHandler : MonoBehaviour
{
    public LayerMask layerMask;

    private GameObject lastClickedObject;


    float lastClickTime = 0;
    float catchTimeWindow = 0.25f; // Customize the catch time window here

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if ((Time.time - lastClickTime) < catchTimeWindow)
            {
                // Double click detected
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {
                    VRModelV2 a = hit.collider.GetComponentInParent<VRModelV2>();
                    if (a != null)
                    {
                        lastClickedObject = a.gameObject;
                        // Do something with A here
                        DebugExtension.Log(a.DataAsset.NameOnApp);
                    }
                }
                //if (Input.GetMouseButtonUp(0))
                //{
                //    lastClickTime = 0;
                //}
            }
            lastClickTime = Time.time;
        }
    }

}