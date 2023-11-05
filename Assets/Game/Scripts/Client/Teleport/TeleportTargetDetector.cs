using System;
using System.Collections;
using System.Collections.Generic;
using ClientOnly.Oculus;
using Game.Client;
using UnityEngine;
// using Oculus;
using UniRx;
using Object = UnityEngine.Object;
using TWT.Client.Interaction;
using UnityEngine.EventSystems;

public class TeleportTargetDetector : MonoBehaviour, IControllerInteractable
{
    public const float HEAD_HIGH = 1.7f;
    [SerializeField] GameObject teleportMarker;

#if !UNITY_WEBGL
    OVRCameraRig oculusCameraRig;
#endif
    [SerializeField] private LayerMask layerMaskTargetDetect = -1; //TeleportPoint

    [SerializeField] private bool IsDebuging = true;
    private LayerMask LayerMaskTargetDetect => layerMaskTargetDetect;

    public Transform Transform => transform;

    private Camera cam;

    public static event Action<Vector3> OnTeleport;

    private float touch_time = 0;
    private void OnDisable()
    {
        teleportMarker.gameObject.SetActive(false);
    }
    private void Start()
    {
        cam = Camera.main;
#if !UNITY_WEBGL
        if (oculusCameraRig == null)
        {
            //find the oculus rig - via manager or by findObjectOfType, if unavailable
            if (OVRManager.instance != null)
            {
                oculusCameraRig = OVRManager.instance.GetComponent<OVRCameraRig>();
            }

            if (oculusCameraRig == null)
            {
                oculusCameraRig = Object.FindObjectOfType<OVRCameraRig>();
            }
        }

        SwitchHandTo(CurvedUIInputModule.Hand.Right);
#endif
    }

    bool isOverUI;
    private void Update()
    {
        if (BaseScreenTopMenuV2.Instance.isCameraRotating
            || GameContext.IsEditable)
        {
            isOverUI = true;
            return;
        }

        Ray myRay = this.GetRay();
        float length = 10000;

        RaycastHit hit;
        if (Physics.Raycast(myRay, out hit, length, LayerMaskTargetDetect))
        {
            var teleportView = hit.collider.GetComponent<ITeleportView>();

            if (teleportView != null && teleportView.CanTeleport)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isOverUI = this.IsMouseOrTouchOverUI();
                    DebugExtension.Log("GetMouseButtonDown: " + isOverUI);
                    if (!isOverUI)
                    {
                        OnTeleport?.Invoke(hit.point);
                        teleportView.OnLookAt();
                        teleportMarker.transform.position = hit.point;
                        teleportMarker.transform.up = Vector3.up;
                        teleportMarker.SetActive(true);
                    }
                }

                if (Input.GetMouseButton(0) && !isOverUI)
                {
                    OnTeleport?.Invoke(hit.point);
                    teleportView.OnLookAt();
                    teleportMarker.transform.position = hit.point;
                    teleportMarker.transform.up = Vector3.up;
                    teleportMarker.SetActive(true);
                }

                if (!isOverUI && Input.GetMouseButtonUp(0))
                {
                    bool isFloor = teleportView.TeleportAreaType == TeleportAreaType.Area;
                    Vector3 teleportPos = isFloor ? hit.point : hit.collider.transform.parent.position;
                    teleportPos.y += HEAD_HIGH;
                    teleportView.TeleportTo(teleportPos);
                    OnTeleport?.Invoke(Vector3.one * 10000);
                    teleportMarker.SetActive(false);
                }
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    OnTeleport?.Invoke(Vector3.one * 10000);
                    teleportMarker.SetActive(false);
                }
            }
        }
    }

//    void SwitchHandTo(CurvedUIInputModule.Hand newHand)
//    {
//#if UNITY_ANDROID && CURVEDUI_TOUCH
//        CurvedUIInputModule.Instance.UsedHand = newHand;
//        //transform.SetParent(CurvedUIInputModule.Instance.OculusTouchUsedControllerTransform);
//        transform.localPosition = Vector3.zero;
//        transform.localRotation = Quaternion.identity;
//        transform.localScale = Vector3.one;
//#endif
//    }

    public bool IsDoubleTap()
    {
        bool result = false;
        float MaxTimeWait = 0.3f;
        if (Input.GetTouch(0).phase == TouchPhase.Began)
        {
            if (touch_time == 0)
            {
                touch_time = Time.time;
            }
            else
            {
                if (Time.time - touch_time < MaxTimeWait)
                {
                    touch_time = 0;
                    return true;
                }
                touch_time = 0;
            }
        }

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
            return true;
#endif
        return result;
    }
}