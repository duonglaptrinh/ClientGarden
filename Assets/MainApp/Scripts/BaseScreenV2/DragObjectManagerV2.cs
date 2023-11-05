using Game.Client;
using SyncRoom.Schemas;
using System;
using System.Collections.Generic;
using Player_Management;
using TWT.Client.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;
using jp.co.mirabo.Application.RoomManagement;

public class DragObjectManagerV2 : ClickModel3D, IControllerInteractable
{
    public LayerMask targetLayer, newLayer, layerModel;

    Camera cam;

    bool isDrop = true;
    float defaultObjectDistance = 5f;
    bool isBeginTracking = true;
    float yAxisFirstTrack = 0;
    float moveObjectSpeed = 0.01f;
    public Transform currentDetectObj;
    public Transform placeObject;
    private bool wasLastClickOnObject = false;
    public static bool IsAllowDrag = false; // User for model 3D 

    VRModelV2 vRObject = null;
    bool isCanDrag = false;
    void Start()
    {
        cam = Camera.main;
        targetLayer |= (1 << 9);
        targetLayer |= (1 << 5);
        newLayer = targetLayer & ~(1 << 9);
        //layerModel |= (1 << 9);
    }

    void NewUpdate()
    {
        if (Input.GetMouseButtonDown(0) && currentDetectObj == null)
        {
            if (IsPointerOverCanvas())
                isCanDrag = false;
            else
                isCanDrag = true;
        }
        if (Input.GetMouseButton(0))
        {
            //DebugExtension.Log("------- EndOfClick = " + endOfClick);
            //DebugExtension.Log("GetMouseButton");
            if (IsPointerOverCanvas() || !isCanDrag) return;
            Ray ray = this.GetRay();
            if (draggableObject == null)
            {
                //DebugExtension.Log("GetMouseButtonádfs");
                // if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layerModel))
                // {
                //     VRModelV2 newModel = hit.transform.GetComponentInParent<VRModelV2>();
                //     Debug.Log("isCheckClickModel");
                //     //if (newModel && !newModel.GetData().isLock)
                //     if (newModel && newModel.isCheckClickModel)
                //     {
                //         Debug.Log("-----------------isCheckClickModel");
                //         vRObject = newModel;
                //         currentDetectObj = hit.transform;
                //         SetDraggableObject(currentDetectObj.gameObject);
                //         //DebugExtension.Log("Set Object Dragger = " + currentDetectObj.gameObject.name);
                //     }
                // }
                RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerModel);

                foreach (RaycastHit hit in hits)
                {
                    VRModelV2 newModel = hit.transform.GetComponentInParent<VRModelV2>();
                    if (newModel && newModel.isCheckClickModel)
                    {
                        vRObject = newModel;
                        currentDetectObj = hit.transform;
                        SetDraggableObject(currentDetectObj.gameObject);
                        break;
                    }
                }
            }
            else
            {
                if (vRObject)
                {
                    targetLayer = vRObject.LayerMaskObject;
                }
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, targetLayer))
                {
                    if (hit.transform.gameObject.layer != 11)
                    {
                        placeObject = hit.transform;
                        draggableObject.transform.position = VrDomeControllerV2.Instance.DomeLoadhouse.CheckGridSnap(hit.point, draggableObject.transform.position);
                        Debug.DrawRay(hit.point, hit.normal, Color.green);
                        //if (draggableObject.name.Contains("Model"))
                        //{
                        //}
                        //else
                        //{
                        //    Vector3 p = hit.point;
                        //    p.y += 0.02f;
                        //    draggableObject.transform.position = p;
                        //}
                        if (isDrop)
                            draggableObject = null;
                    }
                }
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            ////DebugExtension.Log("GetMouseButtonUp");
            //DropObject();
            //placeObject = null;
            //currentDetectObj = null;
            //isCanDrag = false;
            DropModel();
        }
    }

    protected override void DropModel()
    {
        //DebugExtension.LogError("GetMouseButtonUp");
        DropObject();
        placeObject = null;
        currentDetectObj = null;
        tempObjectToDrag = null;
        isCanDrag = false;
    }

    void Update()
    {
        NewUpdateV2();
        //DebugExtension.LogError("------- EndOfClick = " + endOfClick);
        ////if (!VRObjectManagerV2.Instance.IsAllowShowUIEdit) return;
        //if (IsAllowDrag && !isClicking)
        //{
        //    NewUpdate();
        //}
        //CheckClick(layerModel);
        //if (Input.GetMouseButtonUp(0))
        //{
        //    DebugExtension.LogError("------- Set EndOfClick UP = " + endOfClick);
        //    endOfClick = false;
        //}
    }

    Transform tempObjectToDrag;
    void NewUpdateV2()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverCanvas()) return;
            clickTimer = 0f;
            //DebugExtension.LogError("IsAllowDrag = " + IsAllowDrag + "   currentDetectObj = " + currentDetectObj);
            ////if (IsAllowDrag && currentDetectObj == null)
            //if (currentDetectObj == null)
            //{
            //    if (IsPointerOverCanvas())
            //        isCanDrag = false;
            //    else
            //        isCanDrag = true;
            //}
        }

        if (Input.GetMouseButton(0))
        {
            clickTimer += Time.deltaTime;

            if (IsPointerOverCanvas()) return;
            Ray ray = this.GetRay();

            //if (IsPointerOverCanvas() || !isCanDrag) return;
            if (draggableObject == null)
            {
                RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerModel);

                DebugExtension.Log("IsAllowDrag = " + IsAllowDrag + "   draggableObject = " + draggableObject + "   currentDetectObj = " + currentDetectObj);
                foreach (RaycastHit hit in hits)
                {
                    VRModelV2 newModel = hit.transform.GetComponentInParent<VRModelV2>();
                    if (newModel && newModel.isCheckClickModel)
                    {
                        //vRObject = newModel;
                        //currentDetectObj = hit.transform;
                        //SetDraggableObject(currentDetectObj.gameObject);
                        tempObjectToDrag = hit.transform;
                        DebugExtension.Log("currentDetectObj.gameObject = " + tempObjectToDrag.name);
                        break;
                    }
                }
            }
            if (clickTimer <= maxClickDuration) return;
            if (!IsAllowDrag) return;

            if (draggableObject == null)
            {
                if (tempObjectToDrag)
                {
                    VRModelV2 newModel = tempObjectToDrag.GetComponentInParent<VRModelV2>();
                    if (newModel.GetData().sessionId == RoomManager.Instance.GameRoom.SessionId)
                    {
                        if (newModel && newModel.isCheckClickModel)
                        {
                            vRObject = newModel;
                            currentDetectObj = tempObjectToDrag;
                            SetDraggableObject(currentDetectObj.gameObject);
                        }
                    }
                }
                //RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity, layerModel);

                //DebugExtension.Log("IsAllowDrag = " + IsAllowDrag + "   draggableObject = " + draggableObject + "   currentDetectObj = " + currentDetectObj);
                //foreach (RaycastHit hit in hits)
                //{
                //    VRModelV2 newModel = hit.transform.GetComponentInParent<VRModelV2>();
                //    if (newModel && newModel.isCheckClickModel)
                //    {
                //        vRObject = newModel;
                //        currentDetectObj = hit.transform;
                //        SetDraggableObject(currentDetectObj.gameObject);
                //        DebugExtension.Log("currentDetectObj.gameObject = " + currentDetectObj.name);
                //        break;
                //    }
                //}
            }
            else
            {
                if (vRObject)
                {
                    targetLayer = vRObject.LayerMaskObject;
                }
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, targetLayer))
                {
                    if (hit.transform.gameObject.layer != 11)
                    {
                        placeObject = hit.transform;
                        draggableObject.transform.position = VrDomeControllerV2.Instance.DomeLoadhouse.CheckGridSnap(hit.point, draggableObject.transform.position);
                        Debug.DrawRay(hit.point, hit.normal, Color.green);
                        if (isDrop)
                            draggableObject = null;
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (IsPointerOverCanvas()) return;
            if (clickTimer <= maxClickDuration)
            {
                isLock = false;
                DebugExtension.Log("isClick = true");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layerModel))
                {
                    isLock = true;
                    var modelV2 = hit.collider.GetComponentInParent<VRModelV2>();
                    DebugExtension.Log("------- hit.collider " + hit.collider.name);
                    if (modelV2 != null)
                    {
                        if (modelV2.GetData().isLock)
                        {
                            return;
                        }
                        ShowUIEditModel(modelV2, true);
                        DropModel();
                    }
                    else
                    {
                        if (draggableObject == null && !IsPointerOverCanvas())
                        {
                            DisableObjectLock();
                        }
                    }
                }
                else
                {
                    int allLayers = Physics.AllLayers;
                    int layersToCheck = allLayers & ~layerModel;
                    if (Physics.Raycast(ray, out var hit2, Mathf.Infinity, layersToCheck))
                    {
                        //if (draggableObject == null && !IsPointerOverCanvas())
                        DebugExtension.Log("mouse up can not raycast to vrModel draggableObject = " + draggableObject);
                        if (draggableObject == null && !IsPointerOverCanvas())
                        {
                            DisableObjectLock();
                        }
                    }
                }

                if (isLock == false)
                {
                    DebugExtension.Log("Click DropModel");
                    DropModel();
                }
            }
            else
            {
                DebugExtension.Log("DropModel");
                //isActiveMouseCanDrag = true;
                DropModel();
            }
        }

    }
    void TrackKeyboardController()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            defaultObjectDistance += moveObjectSpeed;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            defaultObjectDistance -= moveObjectSpeed;
        }
    }

#if !UNITY_WEBGL
    void TrackOculusGoController()
    {
        // returns true if right-handed controller connected
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RHand))
        {
            if (OVRInput.Get(OVRInput.Touch.PrimaryTouchpad))
            {
                if (isBeginTracking)
                {
                    yAxisFirstTrack = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y;
                    isBeginTracking = false;
                }

                if (OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad).y > yAxisFirstTrack)
                {
                    defaultObjectDistance += moveObjectSpeed * 10;
                }
                else
                {
                    defaultObjectDistance -= moveObjectSpeed * 10;
                }
            }
            else
            {
                isBeginTracking = true;
            }
        }
    }


    void TrackOculusQuestController()
    {
        // returns true if right-handed controller connected
        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTouch))
        {
            if (OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick).y > 0)
            {
                defaultObjectDistance += moveObjectSpeed * 10;
            }
            else
            {
                defaultObjectDistance -= moveObjectSpeed * 10;
            }
        }
    }
#endif
    LayerMask oldLayerMask;
    public void SetDraggableObject(GameObject draggableObject)
    {
        if (currentDetectObj != null)
        {
            this.draggableObject = currentDetectObj.GetComponentInParent<VRObjectV2>().gameObject;
            //layer = newLayer;
            VRModelV2 model = this.draggableObject.gameObject.GetComponent<VRModelV2>();
            oldLayerMask = currentDetectObj.gameObject.layer;
            if (model)
            {
                model.SaveOldPositionY();
                //model.IsEditedTransform = true;
                foreach (Collider md in model.gameObject.GetComponentsInChildren<Collider>())
                {
                    if (md is MeshCollider a)
                        a.convex = true;
                    md.isTrigger = true;
                }
                model.ChangeLayer(20);
            }
            isDrop = false;
            this.draggableObject.GetComponent<VrObjectEditSelectHelperV2>().EditableVrObject_OnPointerDown();
        }
        else
        {
            this.draggableObject = null;
            draggableObject.GetComponent<VrObjectEditSelectHelperV2>().AnotherDrop();
            currentDetectObj = null;
        }
        // ignore vrobject layer
        //currentTargetLayer = 1 << 9;
        //currentTargetLayer = ~currentTargetLayer;
    }

    public void DropObject()
    {
        //if (draggableObject == null && !IsPointerOverCanvas())
        //{
        //    DisableObjectLock();
        //}
        if (draggableObject)
        {
            isDrop = true;
            //layer = targetLayer;

            VRModelV2 model = this.draggableObject.gameObject.GetComponent<VRModelV2>();
            if (model)
            {
                model.ChangeLayer(oldLayerMask);
                //model.IsEditedTransform = true;
                foreach (Collider md in model.gameObject.GetComponentsInChildren<Collider>())
                {
                    md.isTrigger = false;
                    if (md is MeshCollider a)
                    {
                        a.convex = false;
                    }
                }
                model.colliderComp.isTrigger = true;
            }
            draggableObject.GetComponent<VrObjectEditSelectHelperV2>().EditableVrObject_OnPointerUp();
            draggableObject = null;
            currentDetectObj = null;
        }
    }

    private bool CheckButtonEdit()
    {
        var ray = this.GetRay();
        if (draggableObject != null) return false;

        if (Physics.Raycast(ray, out var hit))
        {
            var isClickOnObject = false;

            foreach (Transform child in hit.transform)
            {
                if (child.gameObject.layer == LayerMask.NameToLayer("VRObject"))
                {
                    isClickOnObject = true;
                    break;
                }
            }

            if (isClickOnObject && !wasLastClickOnObject)
            {
                wasLastClickOnObject = true;
                return true;
            }

            wasLastClickOnObject = false;
        }

        return false;
    }



    public Transform Transform => transform;
}
