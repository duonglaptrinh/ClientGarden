using jp.co.mirabo.Application.RoomManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using TWT.Networking;
using UnityEngine;

public static class VrgSyncApi
{
    //public static void Send(SyncCreateVrObjectMessage message)
    //public static void Send(SyncTranformVrObjectMessage message)
    //public static void Send(SyncElementScaleVrObjectMessage message)
    //public static void Send(SyncDeleteVrObjectMessage message)

    //public static void Send(SyncMaterialHouseMessage message)
    //public static void Send(SyncMaterialModelMessage message)
    //public static void Send(VrArrowNextDomeMessage message)

    public static bool IsReady => SyncRoom != null && SyncRoom.Connected;

    public static void RegisterHandler<T>(Action<T> callback, string eventKey = null)
    {
        if (TemplatePlanTestGroup.IsActiveSettingPlanTemplateOnEditor) { return; }
        if (string.IsNullOrEmpty(eventKey))
            return;
        SyncRoom.Register(eventKey, callback);
    }

    static VrgRoomClient SyncRoom => RoomManager.Instance.GameRoom;
    // TODO
    public static void Send<T>(T message, string eventKey = null)
    {
        if (TemplatePlanTestGroup.IsActiveSettingPlanTemplateOnEditor)
        {
            AutoCallMessageInTestMode(message, eventKey);
            return;
        }
        if (string.IsNullOrEmpty(eventKey))
            return;
        SyncRoom.Send(eventKey, message);
    }

    static void AutoCallMessageInTestMode<T>(T message, string eventKey = null)
    {
        VRObjectSync vrSync = GameObject.FindObjectOfType<VRObjectSync>();
        switch (eventKey)
        {
            case SyncCreateVrObjectMessage.EventKey:
                SyncCreateVrObjectMessage createMessage = (SyncCreateVrObjectMessage)(object)message;
                vrSync.SyncCreateModel3D(createMessage);
                break;
            case SyncDeleteVrObjectMessage.EventKey:
                SyncDeleteVrObjectMessage convertedMessage = (SyncDeleteVrObjectMessage)(object)message;
                vrSync.SyncDeleteVrObject(convertedMessage);
                break;
            case SyncTranformVrObjectMessage.EventKey:
                SyncTranformVrObjectMessage transMessage = (SyncTranformVrObjectMessage)(object)message;
                vrSync.SyncTranformVrObject(transMessage);
                break;
            case SyncElementScaleVrObjectMessage.EventKey:
                SyncElementScaleVrObjectMessage scaleMessage = (SyncElementScaleVrObjectMessage)(object)message;
                vrSync.SyncElementScaleVrObject(scaleMessage);
                break;
            case SyncMaterialModelMessage.EventKey:
                SyncMaterialModelMessage modelMaterialMessage = (SyncMaterialModelMessage)(object)message;
                vrSync.SyncMaterialModel(modelMaterialMessage);
                break;
            case SyncMaterialHouseMessage.EventKey:
                SyncMaterialHouseMessage houselMessage = (SyncMaterialHouseMessage)(object)message;
                vrSync.SyncMaterialHouse(houselMessage);
                break;
        }
    }
}