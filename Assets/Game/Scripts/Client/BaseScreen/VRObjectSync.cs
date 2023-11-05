using Game.Client;
using System.Collections;
using System.Collections.Generic;
using TWT.Networking;
using Cysharp.Threading.Tasks;
using UnityEngine;

using System.Linq;
using TWT.Model;
using System;

public class VRObjectSync : MonoBehaviour
{
    public static Action<SyncCreateVrObjectMessage, VRModelData> OnCreateNewModel = null;
    public static Action<SyncDeleteVrObjectMessage> OnDeleteNewModel = null;
    public static Action<SyncStartPointMessage> OnStartPointSync = null;

    #region Server

    private async void Start()
    {
        await UniTask.WaitUntil(() => VrgSyncApi.IsReady);

        VrgSyncApi.RegisterHandler<SyncCreateVrObjectMessage>(
                (mes) =>
                {
                    DebugExtension.Log("SyncCreateVrObjectMessage: " + mes.model_id);
                    SyncCreateModel3D(mes);
                }, SyncCreateVrObjectMessage.EventKey);
        VrgSyncApi.RegisterHandler<SyncDeleteVrObjectMessage>(
               (mes) =>
               {
                   SyncDeleteVrObject(mes);
               }, SyncDeleteVrObjectMessage.EventKey);

        VrgSyncApi.RegisterHandler<SyncTranformVrObjectMessage>(
               (mes) =>
               {
                   SyncTranformVrObject(mes);
               }, SyncTranformVrObjectMessage.EventKey);
        VrgSyncApi.RegisterHandler<VrArrowNextDomeMessage>(
              (mes) =>
              {
                  VRStackDataSync.AddToStack(mes);
              }, VrArrowNextDomeMessage.EventKey);
        VrgSyncApi.RegisterHandler<SyncElementScaleVrObjectMessage>(
              (mes) =>
              {
                  SyncElementScaleVrObject(mes);
              }, SyncElementScaleVrObjectMessage.EventKey);
        VrgSyncApi.RegisterHandler<SyncMaterialModelMessage>(
              (mes) =>
              {
                  SyncMaterialModel(mes);
              }, SyncMaterialModelMessage.EventKey);
        VrgSyncApi.RegisterHandler<SyncMaterialHouseMessage>(
              (mes) =>
              {
                  SyncMaterialHouse(mes);
              }, SyncMaterialHouseMessage.EventKey);
        VrgSyncApi.RegisterHandler<DeleteDomePlantMessage>(
            (mes) =>
            {
                SyncDeleteDomeById(mes);
            }, DeleteDomePlantMessage.EventKey);
        VrgSyncApi.RegisterHandler<SaveNewPlanJsonMessage>(
            (mes) =>
            {
                SaveNewPlan(mes);
            }, SaveNewPlanJsonMessage.EventKey);
        VrgSyncApi.RegisterHandler<ApplyPlanTemplateMessage>(
           (mes) =>
           {
               ApplyNewPlanTemplate(mes);
           }, ApplyPlanTemplateMessage.EventKey);
        VrgSyncApi.RegisterHandler<SyncLandHouseMessage>(
           (mes) =>
           {
               SyncSizeLand(mes);
           }, SyncLandHouseMessage.EventKey);
        VrgSyncApi.RegisterHandler<SyncStartPointMessage>(
          (mes) =>
          {
              SyncStartPoint(mes);
          }, SyncStartPointMessage.EventKey);
        VrgSyncApi.RegisterHandler<SyncEditModel>(
            (mes) =>
            {
                SyncModelEdit(mes);
            }, SyncEditModel.EventKey);
    }
    #endregion

    #region Client
    VRObjectManagerV2 VRObjectManager => VRObjectManagerV2.Instance;

    public void SyncCreateModel3D(SyncCreateVrObjectMessage msg)
    {
        if (msg.idDome != VrDomeControllerV2.Instance.vrDomeData.dome_id) return;
        VRModelData newModel = new VRModelData
        (
            model_id: msg.model_id,
            model_url: msg.model_url,
            model_translate: msg.model_translate,
            model_rotation: msg.model_rotation,
            model_scale: msg.model_scale,
            model_default_animation: msg.model_default_animation,
            vr_model_transparent_type: msg.vr_model_transparent_type,
            isOutline: msg.isOutline,
            color : msg.color,
            isLock : msg.isLock,
            sessionId : msg.sessionId
        );
        newModel.nameTexture = msg.nameTexture;
        GameContext.ContentDataCurrent.AddVRModelData(msg.idDome, newModel);
        VRObjectManager.AddNewVrModel3D(newModel);
        OnCreateNewModel?.Invoke(msg, newModel);
    }

    public void SyncDeleteVrObject(SyncDeleteVrObjectMessage msg)
    {
        GameContext.ContentDataCurrent.DeleteVRModelData(msg.idDome, msg.id);
        VRObjectManager.DeleteVrModel(msg.id);
        OnDeleteNewModel?.Invoke(msg);
    }

    public void SyncTranformVrObject(SyncTranformVrObjectMessage msg)
    {
        GameContext.ContentDataCurrent.UpdateTransformVRModelData(msg.idDome, msg.id, msg.translate, msg.rotation);
        VRObjectManager.SetTranformModel(msg.id, msg.translate, msg.rotation);
    }

    public void SyncDomeId(VrArrowNextDomeMessage msg)
    {
        DebugExtension.Log("Recived Data: message DomeId = " + msg.DomeId);
        GameContext.CurrentIdDome = msg.DomeId;
        BaseScreenCtrlV2.Instance.RequestChangeDome(GameContext.CurrentIdDome);
        BaseScreenDataManager.SyncDomeById(msg.DomeId);
    }
    public void SyncDeleteDomeById(DeleteDomePlantMessage msg)
    {
        BaseScreenDataManager.DeleteDome(msg.idDome);
    }

    public void SyncElementScaleVrObject(SyncElementScaleVrObjectMessage msg)
    {
        GameContext.ContentDataCurrent.UpdateScaleVRModelData(msg.idDome, msg.id, msg.localScale);
        VRObjectManager.SetElementScaleModel(msg.id, msg.localScale, msg.index);
    }

    public void SyncMaterialModel(SyncMaterialModelMessage msg)
    {
        VRObjectManager.SetMaterialModel(msg);
    }
    public void SyncMaterialHouse(SyncMaterialHouseMessage msg)
    {
        VRObjectManager.SetMaterialHouse(msg);
    }
    public void SaveNewPlan(SaveNewPlanJsonMessage msg)
    {
        DebugExtension.Log("SaveNewPlanJsonMessage = " + msg.json);
        GameContext.ContentDataCurrent.AddNewPlan(msg.json);
    }
    public void ApplyNewPlanTemplate(ApplyPlanTemplateMessage msg)
    {
        GameContext.ContentDataCurrent.UpdateOverridePlan(msg.idDome, msg.json);
    }
    public void SyncSizeLand(SyncLandHouseMessage msg)
    {
        DebugExtension.Log("Recived Data: message = " + msg.json);
        GameContext.ContentDataCurrent.UpdateSizeLand(msg.idDome, msg.json);
        BaseScreenUiControllerV2.Instance.NewMainMenu.UpdateTextLand();
        VrDomeControllerV2.Instance.DomeLoadhouse.UpdateLand();
    }
    public void SyncStartPoint(SyncStartPointMessage msg)
    {
        DebugExtension.Log("Recived Data SyncStartPoint: message = " + msg.json);
        GameContext.ContentDataCurrent.UpdateStartPoint(msg.idDome, msg.type, msg.json, msg.indexInList);
        if ((EStartPoint)msg.type == EStartPoint.View)
        {
            VrDomeControllerV2.Instance.ChangeViewPlayer();
        }
        OnStartPointSync?.Invoke(msg);
    }
    public void SyncModelEdit(SyncEditModel msg)
    {
        DebugExtension.Log("Recived Data SyncEditModel: message = " + msg.json);
        GameContext.ContentDataCurrent.UpdateModel(msg.idDome, msg.type, msg.model_id, msg.json);
        if ((EditModel)msg.type == EditModel.EditModel)
        {
            VRObjectManager.UpdateModel(msg);
        }
    }

    #endregion


    public class MediaType
    {
        public int type;
        public int id;

        public MediaType(int type, int id)
        {
            this.type = type;
            this.id = id;
        }
    }

}
