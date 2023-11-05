using System;
using System.Collections;
using System.Collections.Generic;
using Game.Client;
using jp.co.mirabo.Application.RoomManagement;
using Player_Management;
using TWT.Networking;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickModel3D : MonoBehaviour
{
    float lastClickTime = 0;
    float catchTimeWindow = 0.25f;
    protected float clickTimer = 0f;
    protected float maxClickDuration = 0.2f;
    public static bool isClicking = false;
    protected bool isLock;
    public GameObject draggableObject;

    #region DoubleClick
    public void CheckDoubleClick(LayerMask layerModel)
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if ((Time.time - lastClickTime) < catchTimeWindow)
        {
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, layerModel))
                {
                    var modelV2 = hit.collider.GetComponentInParent<VRModelV2>();
                    if (modelV2 != null)
                    {
                        ShowUIEditModel(modelV2, true);
                    }
                }
                else
                {
                    ShowUIEditModel(null, false);
                }
            }
        }
        lastClickTime = Time.time;
    }
    #endregion

    #region Click

    // ReSharper disable Unity.PerformanceAnalysis
    protected void CheckClick(LayerMask layerModel)
    {
        if (DragObjectManagerV2.IsPointerOverCanvas()) return;
        if (Input.GetMouseButtonDown(0))
        {
            clickTimer = 0f;
            isClicking = true;
        }

        if (!isClicking) return;
        clickTimer += Time.deltaTime;
        if (Input.GetMouseButtonUp(0) && clickTimer <= maxClickDuration)
        {
            isLock = false;
            if (Camera.main != null)
            {
                DebugExtension.Log("isLock" + isLock);
                DebugExtension.Log("layerModel" + layerModel.value);
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
                            //if (modelV2.GetData().sessionId == PlayerManager.Instance.GetSessionId())
                            //{
                            //    //DisableEditButton(modelV2, isSendSync: true);
                            //}
                            return;
                        }
                        //DisableObjectLock();
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
                        if (draggableObject == null && !IsPointerOverCanvas())
                        {
                            DisableObjectLock();
                        }
                    }
                }
            }

            if (isLock == false)
            {
                DropModel();
            }
        }
        else if (clickTimer > maxClickDuration)
        {
            isClicking = false;
        }
    }
    #endregion

    protected virtual void DropModel()
    {

    }

    protected void DisableObjectLock()
    {
        DragObjectManagerV2.IsAllowDrag = false;
        VRObjectManagerV2 manager = VRObjectManagerV2.Instance;
        foreach (var item in manager.vrModels)
        {
            if (item.GetData().isLock && item.GetData().sessionId == RoomManager.Instance.GameRoom.SessionId)
            {
                BaseScreenCtrlV2.Instance.MenuTabControllerV2.vrModelSettingTab.OnCloseClick();
                item.isCheckClickModel = false;
                item.UiButtons.gameObject.SetActive(item.isCheckClickModel);
                item.RemoveOutline();
                SendSyncEditModel(EditModel.EditModel, item, false,
                    MyUtils.GetColorString(item.GetComponent<Renderer>()), false, "");
                break;
            }
        }
    }
    private void DisableEditButton(VRModelV2 model, bool isSendSync = false)
    {
        model.UiButtons.SetActive(false);
        model.isCheckClickModel = false;
        BaseScreenCtrlV2.Instance.MenuTabControllerV2.vrModelSettingTab.OnCloseClick();
        model.RemoveOutline();
        if (isSendSync)
        {
            SendSyncEditModel(EditModel.EditModel, model, false,
                MyUtils.GetColorString(model.GetComponent<Renderer>()), false, "");
        }
    }
    private void DisableEditButton()
    {
        foreach (var model in VRObjectManagerV2.Instance.vrModels)
        {
            if (!model.GetData().isLock)
            {
                model.UiButtons.SetActive(false);
                model.isCheckClickModel = false;
                BaseScreenCtrlV2.Instance.MenuTabControllerV2.vrModelSettingTab.OnCloseClick();
                model.RemoveOutline();
                SendSyncEditModel(EditModel.EditModel, model, false,
                    MyUtils.GetColorString(model.GetComponent<Renderer>()), false, "");
            }
            else
            {
                if (model.GetData().sessionId != RoomManager.Instance.GameRoom.SessionId) continue;
                model.UiButtons.SetActive(false);
                model.isCheckClickModel = false;
                BaseScreenCtrlV2.Instance.MenuTabControllerV2.vrModelSettingTab.OnCloseClick();
                model.RemoveOutline();
                SendSyncEditModel(EditModel.EditModel, model, false,
                    MyUtils.GetColorString(model.GetComponent<Renderer>()), false, "");
            }
        }
    }

    protected void ShowUIEditModel(VRModelV2 modelV2, bool isCheckClick)
    {
        VRObjectManagerV2 manager = VRObjectManagerV2.Instance;
        if (!manager.IsAllowShowUIEdit)
        {
            if (isCheckClick)
            {
                //if (modelV2.GetData().sessionId == PlayerManager.Instance.GetSessionId())
                //{
                // Disable item locked before
                foreach (var item in manager.vrModels)
                {
                    if (item.GetData().isLock && item.GetData().sessionId == RoomManager.Instance.GameRoom.SessionId)
                    {
                        DisableEditButton(item, isSendSync: true);
                        break;
                    }
                }
                //}
                // Active Editmode + Lock
                modelV2.isCheckClickModel = true;
                modelV2.SetOutline();
                SendSyncEditModel(EditModel.EditModel, modelV2, true, MyUtils.GetColorString(modelV2.GetComponent<Renderer>()), true, RoomManager.Instance.GameRoom.SessionId);
                MenuTabControllerV2.Instance.ShowVrModelSettingDialog(modelV2, modelV2.DataAsset);
                MenuTabControllerV2.Instance.ShowVRObjectSettingTab(modelV2);
                DragObjectManagerV2.IsAllowDrag = true;

                //foreach (var item in manager.vrModels)
                //{
                //    if (item == modelV2)
                //    {
                //        item.isCheckClickModel = true;
                //        item.SetOutline();
                //        SendSyncEditModel(EditModel.EditModel, item, true, MyUtils.GetColorString(item.GetComponent<Renderer>()), true, PlayerManager.Instance.GetSessionId());
                //    }
                //    else
                //    {
                //        if (!item.GetData().isLock)
                //        {
                //            item.RemoveOutline();
                //        }
                //        else
                //        {
                //            if (item.GetData().sessionId != PlayerManager.Instance.GetSessionId()) continue;
                //            item.RemoveOutline();
                //        }
                //        // SendSyncEditModel(EditModel.EditModel, item, false,
                //        //     item.GetColorString(item.GetComponent<Renderer>()), false, "");
                //    }
                //}
                //MenuTabControllerV2.Instance.ShowVrModelSettingDialog(modelV2, modelV2.DataAsset);
                //MenuTabControllerV2.Instance.ShowVRObjectSettingTab(modelV2);
            }

            //else
            //{
            //    foreach (var item in manager.vrModels)
            //    {
            //        if (!item.GetData().isLock)
            //        {
            //            item.isCheckClickModel = false;
            //            item.UiButtons.gameObject.SetActive(item.isCheckClickModel);
            //            item.RemoveOutline();
            //            SendSyncEditModel(EditModel.EditModel, item, false,
            //                MyUtils.GetColorString(item.GetComponent<Renderer>()), false, "");
            //        }
            //        else
            //        {
            //            if (item.GetData().sessionId != PlayerManager.Instance.GetSessionId()) continue;
            //            item.isCheckClickModel = false;
            //            item.UiButtons.gameObject.SetActive(item.isCheckClickModel);
            //            item.RemoveOutline();
            //            SendSyncEditModel(EditModel.EditModel, item, false,
            //                MyUtils.GetColorString(item.GetComponent<Renderer>()), false, "");
            //        }
            //    }
            //    BaseScreenCtrlV2.Instance.MenuTabControllerV2.vrModelSettingTab.OnCloseClick();
            //}
        }
    }
    public static void SendSyncEditModel(EditModel type, VRModelV2 vRObject, bool isOutline, string colorString, bool isLock, string sessionId)
    {
        VrgSyncApi.Send(new SyncEditModel()
        {
            idDome = VrDomeControllerV2.Instance.vrDomeData.dome_id,
            type = (int)type,
            model_id = vRObject.GetData().model_id,
            json = JsonUtility.ToJson(new JsonEditModel(isOutline, colorString, isLock, sessionId))
        }, SyncEditModel.EventKey);
    }
    [Serializable]
    public class JsonEditModel
    {
        public bool isOutline;
        public string color;
        public bool isLock;
        public string sessionId;
        public JsonEditModel(bool isOutline, string color, bool isLock, string sessionId)
        {
            this.isOutline = isOutline;
            this.color = color;
            this.isLock = isLock;
            this.sessionId = sessionId;
        }
        public static JsonEditModel Convert(string jsonModel)
        {
            return JsonUtility.FromJson<JsonEditModel>(jsonModel);
        }
    }

    public static bool IsPointerOverCanvas(Action<List<RaycastResult>> action = null)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        action?.Invoke(raysastResults);
        return raysastResults.Count > 0;
    }

    public static void SetCheckClickModel()
    {
        isClicking = false;
        DragObjectManagerV2.IsAllowDrag = true;
    }
}
