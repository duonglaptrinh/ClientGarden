using System;
using System.Collections.Generic;
using System.Linq;
using Colyseus.Schema;
using Common.VGS;
using Game.Client;
//using Data;
using jp.co.mirabo.Application.RoomManagement;

using NaughtyAttributes;
using SyncRoom.Schemas;
using TWT.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;
using static jp.co.mirabo.Application.RoomManagement.RoomConfig;

namespace Player_Management
{
    public class HouseManagerSync : SingletonMonoBehaviour<HouseManagerSync>
    {
        VRDomeLoadHouse loadHouse => VrDomeControllerV2.Instance.DomeLoadhouse;

        string idEntity = "Sync_House";

        private Entity _entity;
        private void OnEnable()
        {
            BaseScreenCtrlV2.OnChangeDomeDone += OnChangeDomeDone;
        }
        private void OnDisable()
        {
            BaseScreenCtrlV2.OnChangeDomeDone -= OnChangeDomeDone;
        }
        private void Start()
        {
            RoomManager.Instance.GameRoom.OnAddEnityObject += AddEntityPlayer;
            RoomManager.Instance.GameRoom.OnPublisherRemoveEnity += RemoveEntityPlayer;
            //SetEntity();
        }

        void OnChangeDomeDone()
        {
            if (RoomManager.Instance.GameRoom.GetUsers().Count <= 1)
            {
                //DebugExtension.LogError("CREAT ENTITY HOUSE MANAGER SYNC");
                //CreateEntity();
            }
        }

        void AddEntityPlayer(Entity entity)
        {
            //DebugExtension.Log("Add Entity = " + entity.id);
            //var sessionID = entity.id.Replace("Player_", "");
            //if (idEntity.Equals(entity.id))
            //{
            //    SetEntity(entity);
            //}
        }

        private void RemoveEntityPlayer(Entity entity) { }

        protected override void DoOnDestroy()
        {
            if (!RoomManager.Instance || RoomManager.Instance.GameRoom == null)
            {
                return;
            }

            RoomManager.Instance.GameRoom.OnAddEnityObject -= AddEntityPlayer;
            RoomManager.Instance.GameRoom.OnPublisherRemoveEnity -= RemoveEntityPlayer;
        }

        private void CreateEntity()
        {
            _entity = new Entity
            {
                id = idEntity,
                type = SyncEntity.SYNC_HOUSE.ToString()
            };

            SyncMaterialHouseMessage data = CreatDataToSend(true, loadHouse.CurrentData.indexHouse, 0, 0, true);

            _entity.AddAttributeValue(EntityAttribute.INDEX.ToString(), SyncDataType.JSON_OBJECT.ToString(),
                JsonUtility.ToJson(data));

            data = CreatDataToSend(false, loadHouse.CurrentData.indexHouse, 0, 0, true);
            _entity.AddAttributeValue(EntityAttribute.HOUSE_MATERIALS_DETAIL.ToString(), SyncDataType.JSON_OBJECT.ToString(),
                JsonUtility.ToJson(data));

            RoomManager.Instance.GameRoom.Send(SyncMessage.CREATE_ENTITY.ToString(), _entity.OptimizeDataToSend());
        }

        public void UpdateEntity(bool isUpdateHouse, int indexHouse, int indexMaterialSet, int indexMaterialDetail)
        {
            //DebugExtension.Log("UPDATE ENTITY HOUSE MANAGER SYNC --- " + (isUpdateHouse ? " HOUSE INDEX" : " MATERIAL"));
            SyncMaterialHouseMessage data = CreatDataToSend(isUpdateHouse, indexHouse, indexMaterialSet, indexMaterialDetail, false);
            //if (isUpdateHouse)
            //    _entity.UpdateAttribute(EntityAttribute.INDEX.ToString(), JsonUtility.ToJson(data));
            //else
            //    _entity.UpdateAttribute(EntityAttribute.HOUSE_MATERIALS_DETAIL.ToString(), JsonUtility.ToJson(data));
            //RoomManager.Instance.GameRoom.Send(SyncMessage.UPDATE_ENTITY.ToString(), _entity.OptimizeDataToSend());
            VrgSyncApi.Send(data, SyncMaterialHouseMessage.EventKey);
        }
        public void SetEntity(Entity entity)
        {

            //if (_entity != null)
            //{
            //    return;
            //}

            _entity = entity;
            if (entity == null)
                return;

            //DebugExtension.Log("SET ENTITY HOUSE MANAGER SYNC *** ");
            _entity.attributes[EntityAttribute.INDEX.ToString()].OnChange += (List<DataChange> changes) =>
            {
                string str = entity.attributes[EntityAttribute.INDEX.ToString()].dataValue;
                SyncMaterialHouseMessage data = JsonUtility.FromJson<SyncMaterialHouseMessage>(str);
                if (data.isCreateEnity) return;
                VRObjectManagerV2.Instance.SetMaterialHouse(data);
            };

            _entity.attributes[EntityAttribute.HOUSE_MATERIALS_DETAIL.ToString()].OnChange += (List<DataChange> changes) =>
            {
                var str = entity.attributes[EntityAttribute.HOUSE_MATERIALS_DETAIL.ToString()].dataValue;
                SyncMaterialHouseMessage data = JsonUtility.FromJson<SyncMaterialHouseMessage>(str);
                if (data.isCreateEnity) return;
                VRObjectManagerV2.Instance.SetMaterialHouse(data);
            };
        }

        SyncMaterialHouseMessage CreatDataToSend(bool isUpdateHouse, int indexHouse, int indexMaterialSet, int indexMaterialDetail, bool isCreateEntity = false)
        {
            return new SyncMaterialHouseMessage()
            {
                isCreateEnity = isCreateEntity,
                isUpdateHouse = isUpdateHouse,
                idDome = VrDomeControllerV2.Instance.vrDomeData.dome_id,
                indexHouse = indexHouse,
                indexMaterialSet = indexMaterialSet,
                indexMaterialDetail = indexMaterialDetail
            };
        }
    }
}