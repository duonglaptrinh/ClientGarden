using System;
using System.Collections;
using System.Collections.Generic;
using Common.VGS;

using Game.Client;
using Game.Client.Extension;

using TWT.Networking;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;


public class VrObjectEditSelectHelperV2 : MonoBehaviour
{
    private Color startcolor;

    private GameObject vrObjectEditUi;
    private DragObjectManagerV2 dragObjectManager;
    private VRObjectV2 vrObject;
    private bool isDragable;
    private IEditableVrObject editableVrObject;

    public static event Action<Vector3> OnSelectVrObject;

    private void Start()
    {
        if (GameContext.IsEditable)
        {

            editableVrObject = GetComponent<IEditableVrObject>();
            if (editableVrObject != null)
            {
                //editableVrObject.OnPointerDown += EditableVrObject_OnPointerDown;
                //editableVrObject.OnDrag += EditableVrObject_OnDrag;
                //editableVrObject.OnPointerUp += EditableVrObject_OnPointerUp;
            }

            if (gameObject.GetComponent<Collider>())
                gameObject.GetComponent<Collider>().enabled = false;
            dragObjectManager = FindObjectOfType<DragObjectManagerV2>();
            vrObject = GetComponent<VRObjectV2>();

            vrObjectEditUi = vrObject.UiButtons;
           // Drop();
        }
    }

    private void Drop()
    {
        BaseScreenTopMenuV2.Instance.ResetCameraRotate();
        //dragObjectManager.DropObject();
        vrObject.ShowUiEditsExceptSelf();
        vrObject.ShowMenuUiEdit();
        if (PlayerManagerSwitch.isEdit)
        {
            VrObjectEditSelectHelperV2 selectHelper =  MenuTabControllerV2.Instance.GetObjectSelected();
            //selectHelper.vrObjectEditUi.gameObject.SetActive(false);
            selectHelper.vrObject.ShowUiEditsExceptSelf();
        }
        DebugExtension.Log(vrObject.Type);
        DebugExtension.Log(vrObject.Id);
        //Sync
        VrgSyncApi.Send(new SyncTranformVrObjectMessage()
        {
            idDome = VrDomeControllerV2.Instance.vrDomeData.dome_id,
            type = vrObject.Type,
            id = vrObject.Id,
            translate = VRObjectManagerV2.ConvertVector3ToString(transform.localPosition),
            rotation = VRObjectManagerV2.ConvertVector3ToString(transform.localEulerAngles)
        }, SyncTranformVrObjectMessage.EventKey);
    }
    private void OnDestroy()
    {
        if (editableVrObject != null)
        {
            //editableVrObject.OnPointerDown -= EditableVrObject_OnPointerDown;
            //editableVrObject.OnDrag -= EditableVrObject_OnDrag;
            //editableVrObject.OnPointerUp -= EditableVrObject_OnPointerUp;
        }
    }

    public void EditableVrObject_OnPointerDown()
    {
        BaseScreenTopMenuV2.Instance.SaveCurrentStatusCameraRotate();
        BaseScreenTopMenuV2.Instance.SetCameraRotate(false, true);
        isDragable = true;
        //if (vrObjectEditUi.gameObject.activeSelf)
        //{
        //dragObjectManager.SetDraggableObject(gameObject);
        vrObjectEditUi.gameObject.SetActive(false);
        //DebugExtension.LogError("EditableVrObject_OnPointerDown = " + VRObjectManagerV2.Instance.IsAllowShowUIEdit);
        vrObject.ShowUiEditsExceptSelf();
        PlayerManagerSwitch.isDrag = true;
        //}
        //else
        //{
        //    dragObjectManager.DropObject();
        //    vrObject.ShowUiEditsExceptSelf();
        //    vrObject.ShowMenuUiEdit();
        //}
        if(vrObject is VRModelV2 item)
        {
            item.SaveOldPositionY();
        }
    }

    private void EditableVrObject_OnDrag()
    {
        if (isDragable)
            dragObjectManager.SetDraggableObject(gameObject);
    }

    public void EditableVrObject_OnPointerUp()
    {
        if (vrObject is VRModelV2 item && dragObjectManager.placeObject == null && !item.GetOnAnotherObject())
        {
            item.ResetPositionY();
        }
        if(vrObject is VRModelV2 items)
        {
            items.SetOnAnotherObject(dragObjectManager.placeObject);
        }
        Drop();
        isDragable = false;
        PlayerManagerSwitch.isDrag = false;
    }

    public void AnotherDrop()
    {
        EditableVrObject_OnPointerUp();
    }
}