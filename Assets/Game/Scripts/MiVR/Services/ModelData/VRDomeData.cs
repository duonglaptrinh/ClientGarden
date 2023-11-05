using SyncRoom.Schemas;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

namespace TWT.Model
{
    [Serializable]
    public class VRTransformData
    {
        public Vector3 position;
        public Vector3 localScale;
        public Vector3 eulerAngel;
        public Quaternion rotation;
        public VRTransformData()
        {
            position = Vector3.zero;
            localScale = Vector3.zero;
            eulerAngel = Vector3.zero;
            rotation = Quaternion.identity;
        }
        public VRTransformData(VRTransformData oldData)
        {
            position = oldData.position;
            localScale = oldData.localScale;
            eulerAngel = oldData.eulerAngel;
            rotation = oldData.rotation;
        }
    }

    [Serializable]
    public class VRObjectList
    {
        public VRModelData[] vr_model_list;

        public VRObjectList()
        {
            vr_model_list = new List<VRModelData>().ToArray();
        }
        public VRObjectList(VRObjectList oldData)
        {
            List<VRModelData> list = new List<VRModelData>();
            foreach (VRModelData data in oldData.vr_model_list)
            {
                list.Add(VRModelData.CopyData(data));
            }
            this.vr_model_list = list.ToArray();
        }
    }

    [Serializable]
    public class VRDomeData
    {
        public int dome_id;
        public string d360_file_name;
        public ModelDataHouseNetwork modelData;
        public string d360_file_type;
        public bool d360_video_loop = true;
        public float dome_size;
        public float dome_rotate;
        public VRObjectList vr_object_list;
        public VRTransformData tranformCamera; // Not use
        public VRListStartPointData listStartPointData;
        public int id_url = -1;

        public VRDomeData(int dome_id, string d360_file_name, ModelDataHouseNetwork modelData, string d360_file_type, VRObjectList vr_object_list)
        {
            this.dome_id = dome_id;
            this.d360_file_name = d360_file_name;
            this.modelData = modelData;
            this.d360_file_type = d360_file_type;
            this.vr_object_list = vr_object_list;
            this.dome_size = 1;
            this.dome_rotate = 0;
            this.d360_video_loop = true;
            this.tranformCamera = new VRTransformData();
            this.listStartPointData = new VRListStartPointData();
            this.id_url = -1;
        }
        public VRDomeData()
        {
            this.dome_id = -1;
            this.d360_file_name = "";
            this.modelData = new ModelDataHouseNetwork();
            this.d360_file_type = "";
            this.vr_object_list = new VRObjectList();
            this.dome_size = 1;
            this.dome_rotate = 0;
            this.d360_video_loop = true;
            this.tranformCamera = new VRTransformData();
            this.listStartPointData = new VRListStartPointData();
            this.id_url = -1;
        }
        //convert old plan
        public VRDomeData(VRPlanDataTemplate planConvert)
        {
            this.dome_id = planConvert.dome_id;
            this.d360_file_name = planConvert.name;
            this.modelData = new ModelDataHouseNetwork(planConvert.modelData);
            this.vr_object_list = new VRObjectList(planConvert.vr_object_list);
            this.tranformCamera = new VRTransformData(planConvert.tranformCamera);
            this.id_url = -1;
        }
        //Copy old Plan
        public VRDomeData(VRDomeData planConvert)
        {
            this.dome_id = planConvert.dome_id;
            this.d360_file_name = planConvert.d360_file_name;
            this.modelData = new ModelDataHouseNetwork(planConvert.modelData);
            this.vr_object_list = new VRObjectList(planConvert.vr_object_list);
            this.tranformCamera = new VRTransformData(planConvert.tranformCamera);
            this.listStartPointData = new VRListStartPointData(planConvert.listStartPointData);
            this.id_url = planConvert.id_url;
        }
        #region function for VRListStartPointData
        public void AddDataStartPoint(VRStartPointData data)
        {
            this.listStartPointData.AddData(data);
        }
        public void DeleteDataStartPoint(int index, VRStartPointData data)
        {
            this.listStartPointData.DeleteData(index, data);
        }
        public void UpdateDataStartPoint(int index, VRStartPointData data)
        {
            this.listStartPointData.UpdateData(index, data);
        }
        public void SetCurrentIndexStartPoint(int newIndex)
        {
            this.listStartPointData.SetCurrentIndexStartPoint(newIndex);
        }
        #endregion
        public void ClearAllNewModel()
        {
            for (int i = 0; i < vr_object_list.vr_model_list.Length; i++)
            {
                vr_object_list.vr_model_list[i].IsJustCreatNew = false;
            }
        }
        public void SendAllDataModelOtherDome(int idDome)
        {
            for (int i = 0; i < vr_object_list.vr_model_list.Length; i++)
            {
                if (vr_object_list.vr_model_list[i].IsJustCreatNew)
                {
                    VRModelData.SendDataCreateNewModel(vr_object_list.vr_model_list[i], idDome);
                    DebugExtension.LogError("Save Model Other Dome ID = " + vr_object_list.vr_model_list[i].model_id);
                }
            }
        }
        public VRModelData GetModelById(int id)
        {
            foreach (var item in vr_object_list.vr_model_list)
            {
                if (item.model_id == id)
                    return item;
            }
            return null;
        }

        public List<VRModelData> GetAllNewModel()
        {
            List<VRModelData> listModel = new List<VRModelData>();
            foreach (var item in vr_object_list.vr_model_list)
            {
                if (item.IsJustCreatNew)
                    listModel.Add(item);
            }
            return listModel;
        }
        public void SetAllNewModel(List<VRModelData> listModel)
        {
            List<VRModelData> newList = new List<VRModelData>();
            newList.AddRange(vr_object_list.vr_model_list);
            foreach (var item in listModel)
            {
                VRModelData data = VRModelData.CopyData(item);
                data.IsJustCreatNew = item.IsJustCreatNew;
                newList.Add(data);
            }
            vr_object_list.vr_model_list = newList.ToArray();
        }

        public void AddVRModelData(VRModelData newModelData)
        {
            List<VRModelData> listModel = new List<VRModelData>();
            foreach (var item in vr_object_list.vr_model_list)
            {
                listModel.Add(item);
            }
            listModel.Add(newModelData);
            vr_object_list.vr_model_list = listModel.ToArray();
        }
        public void DeleteVRModelData(int idModelDelete)
        {
            List<VRModelData> listModel = new List<VRModelData>();
            foreach (var item in vr_object_list.vr_model_list)
            {
                if (item.model_id != idModelDelete)
                    listModel.Add(item);
            }
            vr_object_list.vr_model_list = listModel.ToArray();
        }
        public void UpdateTransformVRModelData(int idModel, string translate, string rotation)
        {
            List<VRModelData> listModel = new List<VRModelData>();
            foreach (var item in vr_object_list.vr_model_list)
            {
                if (item.model_id == idModel)
                {
                    item.model_translate = translate;
                    item.model_rotation = rotation;
                }
                listModel.Add(item);
            }
            vr_object_list.vr_model_list = listModel.ToArray();
        }
        public void UpdateScaleVRModelData(int idModel, string scale)
        {
            List<VRModelData> listModel = new List<VRModelData>();
            foreach (var item in vr_object_list.vr_model_list)
            {
                if (item.model_id == idModel)
                {
                    item.model_scale = scale;
                }
                listModel.Add(item);
            }
            vr_object_list.vr_model_list = listModel.ToArray();
        }
        public void UpdateStartPointData(string translate, string rotation, string scale)
        {
            modelData.model_translate = translate;
            modelData.model_rotation = rotation;
            modelData.model_scale = scale;
        }

        public void UpdateSizeLand(TypeLandDirection type, float size)
        {
            modelData.UpdateSizeLand(type, size);
        }

        public void UpdateModel(VRModelData vrModel, bool isOutline, string color, bool isLock, string sessionId)
        {
            if (vrModel == null) return;
            vrModel.isOutline = isOutline;
            vrModel.color = color;
            vrModel.isLock = isLock;
            vrModel.sessionId = sessionId;
        }
    }


    [Serializable]
    public class VRPlanDataTemplate
    {
        public string name;
        public string englishName;
        //tags of the template
        public string tag;

        //price of the template
        public float price;
        public string priceUnit = "¥";

        public string description;

        //type of the house
        public string typeHouse;

        //list of exterior items
        public string listExteriorItems; // "Acb ab, def ghi, ahg bct" 

        //list of interior items
        public string listItemsInterior; // "Acb ab, def ghi, ahg bct" 

        //color of the template
        public string color = "(255,255,255,255)";

        //size of the template
        public string size;

        public int dome_id;
        public ModelDataHouseNetwork modelData;
        public VRObjectList vr_object_list;
        public VRTransformData tranformCamera;

        public VRPlanDataTemplate(VRDomeData planConvert)
        {
            this.dome_id = planConvert.dome_id;
            this.name = planConvert.d360_file_name;
            this.modelData = new ModelDataHouseNetwork(planConvert.modelData);
            this.vr_object_list = new VRObjectList(planConvert.vr_object_list);
            this.tranformCamera = new VRTransformData(planConvert.tranformCamera);
        }
        public VRPlanDataTemplate()
        {
            this.dome_id = -1;
            this.name = "";
            this.modelData = new ModelDataHouseNetwork();
            this.vr_object_list = new VRObjectList();
            this.tranformCamera = new VRTransformData();
        }
        public VRPlanDataTemplate(VRPlanDataTemplate planConvert)
        {
            this.dome_id = planConvert.dome_id;
            this.name = planConvert.name;
            this.englishName = planConvert.englishName;
            this.tag = planConvert.tag;
            this.price = planConvert.price;
            this.priceUnit = planConvert.priceUnit;
            this.description = planConvert.description;
            this.typeHouse = planConvert.typeHouse;
            this.listExteriorItems = planConvert.listExteriorItems;
            this.listItemsInterior = planConvert.listItemsInterior;
            this.color = planConvert.color;
            this.size = planConvert.size;
            this.modelData = new ModelDataHouseNetwork(planConvert.modelData);
            this.vr_object_list = new VRObjectList(planConvert.vr_object_list);
            this.tranformCamera = new VRTransformData(planConvert.tranformCamera);
        }
        // Use for create template Plan
        public void UpdateDataFromOldPlan(VRDomeData planConvert)
        {
            this.modelData = new ModelDataHouseNetwork(planConvert.modelData);
            this.vr_object_list = new VRObjectList(planConvert.vr_object_list);
            this.tranformCamera = new VRTransformData(planConvert.tranformCamera);
            this.modelData.model_rotation = VRObjectManagerV2.ConvertVector3ToString(Camera.main.transform.parent.eulerAngles);
            this.modelData.model_scale = VRObjectManagerV2.ConvertVector3ToString(Camera.main.transform.parent.localScale);
            this.modelData.model_translate = VRObjectManagerV2.ConvertVector3ToString(Camera.main.transform.parent.position);
        }

        public void ClearAllNewModel()
        {
            for (int i = 0; i < vr_object_list.vr_model_list.Length; i++)
            {
                vr_object_list.vr_model_list[i].IsJustCreatNew = false;
            }
        }
        public void SendAllDataModelOtherDome(int idDome)
        {
            for (int i = 0; i < vr_object_list.vr_model_list.Length; i++)
            {
                if (vr_object_list.vr_model_list[i].IsJustCreatNew)
                {
                    VRModelData.SendDataCreateNewModel(vr_object_list.vr_model_list[i], idDome);
                    DebugExtension.LogError("Save Model Other Dome ID = " + vr_object_list.vr_model_list[i].model_id);
                }
            }
        }
        public VRModelData GetModelById(int id)
        {
            foreach (var item in vr_object_list.vr_model_list)
            {
                if (item.model_id == id)
                    return item;
            }
            return null;
        }

        public List<VRModelData> GetAllNewModel()
        {
            List<VRModelData> listModel = new List<VRModelData>();
            foreach (var item in vr_object_list.vr_model_list)
            {
                if (item.IsJustCreatNew)
                    listModel.Add(item);
            }
            return listModel;
        }
        public void SetAllNewModel(List<VRModelData> listModel)
        {
            List<VRModelData> newList = new List<VRModelData>();
            newList.AddRange(vr_object_list.vr_model_list);
            foreach (var item in listModel)
            {
                VRModelData data = VRModelData.CopyData(item);
                data.IsJustCreatNew = item.IsJustCreatNew;
                newList.Add(data);
            }
            vr_object_list.vr_model_list = newList.ToArray();
        }

        public void AddVRModelData(VRModelData newModelData)
        {
            List<VRModelData> listModel = new List<VRModelData>();
            foreach (var item in vr_object_list.vr_model_list)
            {
                listModel.Add(item);
            }
            listModel.Add(newModelData);
            vr_object_list.vr_model_list = listModel.ToArray();
        }
        public void DeleteVRModelData(int idModelDelete)
        {
            List<VRModelData> listModel = new List<VRModelData>();
            foreach (var item in vr_object_list.vr_model_list)
            {
                if (item.model_id != idModelDelete)
                    listModel.Add(item);
            }
            vr_object_list.vr_model_list = listModel.ToArray();
        }
        public void UpdateTransformVRModelData(int idModel, string translate, string rotation)
        {
            List<VRModelData> listModel = new List<VRModelData>();
            foreach (var item in vr_object_list.vr_model_list)
            {
                if (item.model_id == idModel)
                {
                    item.model_translate = translate;
                    item.model_rotation = rotation;
                }
                listModel.Add(item);
            }
            vr_object_list.vr_model_list = listModel.ToArray();
        }
        public void UpdateScaleVRModelData(int idModel, string scale)
        {
            List<VRModelData> listModel = new List<VRModelData>();
            foreach (var item in vr_object_list.vr_model_list)
            {
                if (item.model_id == idModel)
                {
                    item.model_scale = scale;
                }
                listModel.Add(item);
            }
            vr_object_list.vr_model_list = listModel.ToArray();
        }
        public void UpdateStartPointData(string translate, string rotation, string scale)
        {
            modelData.model_translate = translate;
            modelData.model_rotation = rotation;
            modelData.model_scale = scale;
        }
    }
}
