using System.Collections.Generic;
using jp.co.mirabo.Application.RoomManagement;
using Player_Management;
using TWT.Networking;
using UnityEngine;

namespace TWT.Model
{
    [System.Serializable]
    public class VRModelData
    {
        public int model_id;
        public string model_url;
        public string model_translate;
        public string model_rotation;
        public string model_scale;
        public string model_default_animation;
        public int vr_model_transparent_type;
        public List<VRMaterialData> listMaterial = new List<VRMaterialData>();
        public int nameTexture;
        public bool isOutline;
        public string color;
        public bool isLock;
        public string sessionId;

        /// <summary>
        /// Temp varible -> using to save when change Dome.
        /// </summary>
        public bool IsJustCreatNew;//{ get; set; }
        public string RealNameModel
        {
            get
            {
                if (string.IsNullOrEmpty(model_url))
                    return "";
                string[] arr = model_url.Split('_');
                if (arr.Length >= 2) return arr[1];
                else return arr[0];
            }
        }

        public VRModelData(int model_id, string model_url, string model_translate, string model_rotation, string model_scale, string model_default_animation, int vr_model_transparent_type, bool isOutline, string color, bool isLock, string sessionId)
        {
            this.model_id = model_id;
            this.model_url = model_url;
            this.model_translate = model_translate;
            this.model_rotation = model_rotation;
            this.model_scale = model_scale;
            this.model_default_animation = model_default_animation;
            this.vr_model_transparent_type = vr_model_transparent_type;
            listMaterial = new List<VRMaterialData>();
            this.nameTexture = 0;
            this.isOutline = isOutline;
            this.color = color;
            this.isLock = isLock;
            this.sessionId = sessionId;
        }

        public VRModelData()
        {
            this.model_id = 0;
            this.model_url = "";
            this.model_translate = "0, -1.5, 0";
            this.model_rotation = "0, 0, 0";
            this.model_scale = "1, 1, 1";
            this.model_default_animation = null;
            this.vr_model_transparent_type = 255;
            listMaterial = new List<VRMaterialData>();
            this.nameTexture = 0;
            this.isOutline = false;
            this.color = null;
            this.isLock = false;
            this.sessionId = "";
        }

        public static VRModelData CopyData(VRModelData oldData)
        {
            VRModelData data = new VRModelData();
            data.model_id = oldData.model_id;
            data.model_url = oldData.model_url;
            data.model_translate = oldData.model_translate;
            data.model_rotation = oldData.model_rotation;
            data.model_scale = oldData.model_scale;
            data.model_default_animation = oldData.model_default_animation;
            data.vr_model_transparent_type = oldData.vr_model_transparent_type;
            data.nameTexture = oldData.nameTexture;
            data.isOutline = oldData.isOutline;
            data.color = oldData.color;
            data.isLock = oldData.isLock;
            data.sessionId = oldData.sessionId;
            data.listMaterial = new List<VRMaterialData>();
            foreach (var item in oldData.listMaterial)
            {
                data.listMaterial.Add(
                    new VRMaterialData(item.nameObjRenderer, item.nameTexture, item.indexTextureLocal)
                    );
            }
            return data;
        }

        public void ChangeNameTexture(int nameTexture)
        {
            //DebugExtension.LogError("ChangeNameTexture = " + nameTexture);
            this.nameTexture = nameTexture;
        }
        //New version not Use - from 8-8-2022
        public void AddMaterial(VRMaterialData data)
        {
            for (int i = 0; i < listMaterial.Count; i++)
            {
                VRMaterialData item = listMaterial[i];
                if (item.nameObjRenderer.Equals(data.nameObjRenderer))
                {
                    item.indexTextureLocal = data.indexTextureLocal;
                    item.colorRender = data.colorRender;
                    listMaterial[i] = item;
                    return;
                }
            }

            listMaterial.Add(data);
        }

        public static void SendDataCreateNewModel(VRModelData data, int domeId)
        {
            SyncCreateVrObjectMessage obj = new SyncCreateVrObjectMessage()
            {
                idDome = domeId,
                model_default_animation = data.model_default_animation,
                model_id = data.model_id,
                model_rotation = data.model_rotation,
                model_scale = data.model_scale,
                model_translate = data.model_translate,
                model_url = data.model_url,
                vr_model_transparent_type = data.vr_model_transparent_type,
                nameTexture = data.nameTexture,
                index_color = data.nameTexture.ToString(),
                isOutline = true,
                color = null,
                isLock = true,
                sessionId =  RoomManager.Instance.GameRoom.SessionId
            };
            VrgSyncApi.Send(obj, SyncCreateVrObjectMessage.EventKey);
        }
    }
    [System.Serializable]
    public class VRMaterialData
    {
        public string nameObjRenderer; // Name = render_name + ";" + material_name -> example: "Head;mat_head"
        public int nameTexture;
        public int indexTextureLocal;
        public Color32 colorRender = Color.white;
        public VRMaterialData(string nameObj, int nameTexture, int index)
        {
            this.nameObjRenderer = nameObj;
            this.nameTexture = nameTexture;
            this.indexTextureLocal = index;
            //this.colorRender = color;
        }
        public VRMaterialData()
        {
            colorRender = Color.white;
        }
        public static string GetNameMaterialToSave(Renderer ren, Material mat)
        {
            return ren.name + ";" + mat.name;
        }
    }

    [System.Serializable]
    public class DicDataMaterial
    {
        public Renderer renderer;
        public Material material;
        public Color colorOrigin;
        public float metallic;
        public float smoothness;
        public DicDataMaterial(Renderer ren, Material mat, Color colorOrigin)
        {
            renderer = ren;
            material = mat;
            this.colorOrigin = colorOrigin;
            metallic = material.GetFloat("_Metallic");
            smoothness = material.GetFloat("_Glossiness");
        }
        public void ChangeMetalic(bool isDayMode)
        {
#if UNITY_EDITOR
            float m = 0.9f;
#else
            float m = 0.99f;
#endif
            float newMetallicValue = isDayMode ? metallic : m;
            material.SetFloat("_Metallic", newMetallicValue);
            float newSmoothness = isDayMode ? smoothness : 0.1f;
            material.SetFloat("_Glossiness", newSmoothness);
        }
    }
}