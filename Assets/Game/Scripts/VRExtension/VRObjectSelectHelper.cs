using Game.Client;
using System;
using TWT.Client.Interaction;
using UnityEngine;

public class VRObjectSelectHelper : MonoBehaviour, IControllerInteractable
{
    public static event Action<Vector3> OnTouchToScreen;
    [SerializeField] private LayerMask layerMaskDetect;

    public Transform Transform => transform;

    bool isOverUI;
    private void Update()
    {
        if (BaseScreenTopMenuV2.Instance.isCameraRotating) return;
        if (GameContext.IsEditable) return;
        //if (!VrDomeControllerV2.Instance.select_mode) return;
        float length = 10000;
        //get direction of the controller
        var myRay = this.GetRay();

        RaycastHit hit;

#if !UNITY_WEBGL
        if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger))
#else
        if(false)   //will be changed in webGL
#endif
        {
            int myLayerMask = -1;

            if (Physics.Raycast(myRay, out hit, length, myLayerMask))
            {
                var vrobject = hit.collider.GetComponent<VRObjectV2>();
                if (vrobject != null)
                {
                    vrobject.OnSelectedObject();
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            isOverUI = this.IsMouseOrTouchOverUI();
            if (isOverUI)
                return;
            if (Physics.Raycast(myRay, out hit, length, layerMaskDetect))
            {
                OnTouchToScreen?.Invoke(hit.point);
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (isOverUI)
                return;
            if (Physics.Raycast(myRay, out hit, length, layerMaskDetect))
            {
                OnTouchToScreen?.Invoke(hit.point);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            OnTouchToScreen?.Invoke(Vector3.one * 10000);
        }

    }
}
