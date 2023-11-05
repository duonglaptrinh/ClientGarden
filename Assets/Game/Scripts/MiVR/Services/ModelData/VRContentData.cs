using System;
using System.Collections.Generic;
using System.Linq;
using Game.Client;
using SyncRoom.Schemas;
using TWT.Networking;
using UnityEngine;
using UnityEngine.Networking.Types;
using Object = UnityEngine.Object;

namespace TWT.Model
{
    [Serializable]
    public class VRContentData
    {
        public string content_name;
        public VRDomeData[] vr_dome_list;

        public VRContentData()
        {

        }

        public VRContentData(string content_name)
        {
            this.content_name = content_name;

            VRDomeData vRDomeData = new VRDomeData(1, "", new ModelDataHouseNetwork(), "", new VRObjectList());
            vr_dome_list = new VRDomeData[1];
            vr_dome_list[0] = vRDomeData;
        }

        public VRContentData(VRContentDataTemplate oldData)
        {
            this.content_name = oldData.content_name;
            List<VRDomeData> vRPlanData = new List<VRDomeData>();
            foreach (VRPlanDataTemplate item in oldData.vr_dome_list)
            {
                vRPlanData.Add(new VRDomeData(item));
            }
            vr_dome_list = vRPlanData.ToArray();
        }

        public static VRContentData FromJson(string json)
        {
            return JsonUtility.FromJson<VRContentData>(json);
        }

        public static string ToJson(VRContentData contentData)
        {
            return JsonUtility.ToJson(contentData);
        }

        public string GetJson()
        {
            return ToJson(this);
        }

        public void AddVRModelData(int domeId, VRModelData newModelData)
        {
            VRDomeData dome = vr_dome_list.FirstOrDefault(x => x.dome_id == domeId);
            if (dome != null)
            {
                dome.AddVRModelData(newModelData);
            }
        }

        public void DeleteVRModelData(int domeId, int idModelDelete)
        {
            VRDomeData dome = vr_dome_list.FirstOrDefault(x => x.dome_id == domeId);
            if (dome != null)
            {
                dome.DeleteVRModelData(idModelDelete);
            }
        }
        public void UpdateTransformVRModelData(int domeId, int idModel, string translate, string rotation)
        {
            VRDomeData dome = vr_dome_list.FirstOrDefault(x => x.dome_id == domeId);
            if (dome != null)
            {
                dome.UpdateTransformVRModelData(idModel, translate, rotation);
            }
        }
        public void UpdateScaleVRModelData(int domeId, int idModel, string scale)
        {
            VRDomeData dome = vr_dome_list.FirstOrDefault(x => x.dome_id == domeId);
            if (dome != null)
            {
                dome.UpdateScaleVRModelData(idModel, scale);
            }
        }
        public void UpdateStartPointData(int domeId, string translate, string rotation, string scale)
        {
            VRDomeData dome = vr_dome_list.FirstOrDefault(x => x.dome_id == domeId);
            if (dome != null)
            {
                dome.UpdateStartPointData(translate, rotation, scale);
            }
        }
        public void AddNewPlan(string jsonNewPlan)
        {
            VRDomeData newDome = JsonUtility.FromJson<VRDomeData>(jsonNewPlan);
            AddNewPlan(newDome);
        }
        public void AddNewPlan(VRDomeData newDome)
        {
            List<VRDomeData> list = new List<VRDomeData>();
            list.AddRange(vr_dome_list);
            list.Add(newDome);
            vr_dome_list = list.ToArray();
        }
        public void UpdateOverridePlan(int dome_id, string jsonNewPlan)
        {
            VRDomeData newDome = JsonUtility.FromJson<VRDomeData>(jsonNewPlan);
            List<VRDomeData> list = new List<VRDomeData>();
            list.AddRange(vr_dome_list);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].dome_id == dome_id)
                    list[i] = newDome;
            }
            vr_dome_list = list.ToArray();
        }
        public void UpdateSizeLand(int dome_id, string jsonSizeLand)
        {
            VRDomeData dome = vr_dome_list.FirstOrDefault(x => x.dome_id == dome_id);
            if (dome != null)
            {
                JsonLandHouse data = JsonLandHouse.Convert(jsonSizeLand);
                dome.UpdateSizeLand(data.type, data.size);
            }
        }
        public void UpdateStartPoint(int dome_id, int type, string json, int indexInList)
        {
            VRDomeData dome = vr_dome_list.FirstOrDefault(x => x.dome_id == dome_id);
            if (dome != null)
            {
                EStartPoint Etype = (EStartPoint)type;
                VRStartPointData data = JsonUtility.FromJson<VRStartPointData>(json);
                switch (Etype)
                {
                    case EStartPoint.Add:
                        dome.AddDataStartPoint(data);
                        break;
                    case EStartPoint.Update:
                        dome.UpdateDataStartPoint(indexInList, data);
                        break;
                    case EStartPoint.Delete:
                        dome.DeleteDataStartPoint(indexInList, data);
                        break;
                    case EStartPoint.View:
                        dome.SetCurrentIndexStartPoint(indexInList);
                        break;
                }
            }
            //DebugExtension.LogError("UpdateStartPoint = " + type.ToString());
        }
        public void UpdateModel(int dome_id, int type, int model_id, string jsonEditModel)
        {
            VRDomeData dome = vr_dome_list.FirstOrDefault(x => x.dome_id == dome_id);
            if (dome != null)
            {
                var vRModel = dome.vr_object_list.vr_model_list.FirstOrDefault(item => item.model_id == model_id);
                if (vRModel != null)
                {
                    EditModel etype = (EditModel)type;
                    if (etype == 0)
                    {
                        ClickModel3D.JsonEditModel data = ClickModel3D.JsonEditModel.Convert(jsonEditModel);
                        dome.UpdateModel(vRModel, data.isOutline, data.color, data.isLock, data.sessionId);
                    }
                }
            }
        }
    }
    [Serializable]
    public class VRContentDataTemplate
    {
        public string content_name;
        public VRPlanDataTemplate[] vr_dome_list;

        public VRContentDataTemplate()
        {

        }

        public VRContentDataTemplate(string content_name)
        {
            this.content_name = content_name;

            VRPlanDataTemplate vRDomeData = new VRPlanDataTemplate();
            vr_dome_list = new VRPlanDataTemplate[1];
            vr_dome_list[0] = vRDomeData;
        }
        public VRContentDataTemplate(VRContentData oldData)
        {
            this.content_name = oldData.content_name;
            List<VRPlanDataTemplate> vRPlanData = new List<VRPlanDataTemplate>();
            foreach (VRDomeData item in oldData.vr_dome_list)
            {
                vRPlanData.Add(new VRPlanDataTemplate(item));
            }
            vr_dome_list = vRPlanData.ToArray();
        }

        public VRContentDataTemplate FromJson(string json)
        {
            return JsonUtility.FromJson<VRContentDataTemplate>(json);
        }

        public string ToJson(VRContentDataTemplate contentData)
        {
            return JsonUtility.ToJson(contentData);
        }
        public void AddNewPlan(string jsonNewPlan)
        {
            VRPlanDataTemplate newDome = JsonUtility.FromJson<VRPlanDataTemplate>(jsonNewPlan);
            AddNewPlan(newDome);
        }
        public void AddNewPlan(VRPlanDataTemplate newDome)
        {
            List<VRPlanDataTemplate> list = new List<VRPlanDataTemplate>();
            list.AddRange(vr_dome_list);
            list.Add(newDome);
            vr_dome_list = list.ToArray();
        }
    }

    [Serializable]
    public class ContentInfo
    {
        public string contentName;
        public string iconUrlAbsolutePath;
        public VrContentTitle contentTitle;
    }


    [Serializable]
    public class VrContentVersion
    {
        public static VrContentVersion CreateFirstVersion()
        {
            return new VrContentVersion("v1.0");
        }

        public static VrContentVersion CreateInstance(string version)
        {
            return new VrContentVersion(version);
        }

        public string version;

        private VrContentVersion(string version)
        {
            this.version = version;
        }
    }

    [Serializable]
    public class VrContentTitle
    {
        public static VrContentTitle CreateInstance(string title, string description, string icon)
        {
            return new VrContentTitle(title, description, icon);
        }

        public string title;
        public string description;
        public string icon;

        private VrContentTitle(string title, string description, string icon)
        {
            this.title = title;
            this.description = description;
            this.icon = icon;
        }
    }

    public class VrVideo360PreviewInfo
    {
        public Texture2D ImagePreview { get; }
        public double Length { get; }

        public VrVideo360PreviewInfo(Texture2D imagePreview, double length)
        {
            ImagePreview = imagePreview;
            Length = length;
        }

        // ~VrVideo360PreviewInfo()
        // {
        //     Object.Destroy(ImagePreview);
        // }
    }
}
