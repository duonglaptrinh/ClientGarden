using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TWT.Model;
using Cysharp.Threading.Tasks;
using static TWT.Model.VRDomeData;
using Game.Client;
using TWT.Networking;
using System.Runtime.Serialization;
using jp.co.mirabo.Application.RoomManagement;
using Player_Management;
using SyncRoom.Schemas;

public class VRObjectManagerV2 : MonoBehaviour
{
    public static VRObjectManagerV2 Instance => BaseScreenCtrlV2.Instance.VRObjectManagerV2;
    public static Transform ParentEditMenu { get; set; }

    public GameObject vrModelPrefab;

    private readonly List<GameObject> vrObjects = new List<GameObject>();

    public IEnumerable<VRObjectV2> VrObjects => vrObjects.Select(x => x.GetComponent<VRObjectV2>()).ToArray();


    public List<VRModelV2> vrModels = new List<VRModelV2>();
    public List<GameObject> ListEditButton = new List<GameObject>();
    public bool IsAllowShowUIEdit { get; set; } = false;
    public float distanceEditButtonToPlayer = 0;
    public float distanceMax = 2;
    public float distanceMin = 1;
    public float sizeMax = 1;
    public float sizeMin = 0.3f;

    private void Awake()
    {
        GameObject obj = new GameObject("ParentEditMenu");
        ParentEditMenu = obj.transform;
        ParentEditMenu.transform.parent = transform;
    }
    public void Update()
    {
        if (ListEditButton.Count == vrModels.Count)
            ChangeSizeEditButton();
        else
            AddListEditButton();
    }
    void OnDestroy()
    {
    }

    public bool loadVRSuccess = false;
    public void LoadVrObjectList(VRObjectList objects)
    {
        ListEditButton.Clear();
        //ClearObjects();
        //DebugExtension.LogError("Load All Object DomeID = " + GameContext.CurrentIdDome);
        foreach (VRModelData vRModelData in objects.vr_model_list)
        {
            AddNewVrModel3D(vRModelData);
        }
        loadVRSuccess = true;
    }

    public void ClearObjects()
    {
        //DebugExtension.LogError("Clear All Object Dome = " + GameContext.CurrentIdDome);
        foreach (GameObject gameObject in vrObjects)
            Destroy(gameObject);

        vrObjects.Clear();
        vrModels.Clear();

        // Resources.UnloadUnusedAssets();
        VideoPlayerManager.Destroy();
    }

    public static Vector3 ConverStringToVector3(string vector)
    {
        if (string.IsNullOrEmpty(vector))
        {
            return Vector3.one;
        }

        string[] split = vector.Split(',');
        Vector3 result = new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
        return result;
    }

    public static string ConvertVector3ToString(Vector3 vector)
    {
        string result = vector.x + ", " + vector.y + ", " + vector.z;
        return result;
    }


    public GameObject AddNewVrModel3D(string model_Name, Vector3 startPos)
    {
        DebugExtension.Log(model_Name);
        //VRModelV2 newVrModel = Instantiate(vrModelPrefab, this.transform).GetComponent<VRModelV2>();
        //newVrModel.SetButtonEdit();
        int vrVideoID = -1;

        List<VRModelV2> list = vrModels.Where(x => x.IsJustCreatNew).ToList();

        if (TemplatePlanTestGroup.IsActiveSettingPlanTemplateOnEditor)
        {
            int max = vrModels.Count > 0 ? vrModels.Max(x => x.Id) : 0;
            vrVideoID = max + 1;
        }
        else
        {
            if (list.Count > 0)
            {
                int min = list.Min(x => x.Id);
                vrVideoID = min - 1;
            }
            else
                vrVideoID = -1;
        }

        //Vector3 rotate = Camera.main.transform.eulerAngles;
        //float y = rotate.y + 180;
        float y = 0;

        VRModelData newModel = new VRModelData
        (
            model_id: vrVideoID,
            model_url: model_Name,
            model_translate: ConvertVector3ToString(startPos),
            model_rotation: "0, " + y + ", 0",
            model_scale: "1, 1, 1",
            model_default_animation: null,
            vr_model_transparent_type: 255,
            isOutline: true,
            color : null,
            isLock: true,
            sessionId: RoomManager.Instance.GameRoom.SessionId
        );
        newModel.IsJustCreatNew = true;

        //newVrModel.transform.localPosition = ConverStringToVector3(newModel.model_translate);
        //newVrModel.transform.localEulerAngles = ConverStringToVector3(newModel.model_rotation);
        //newVrModel.transform.localScale = ConverStringToVector3(newModel.model_scale);
        //newVrModel.SetJustCreateNew(true);
        //newVrModel.SetData(newModel, true).Forget();
        //newVrModel.Initialize(this);
        //newVrModel.ShowMenuUiEdit();

        //vrModels.Add(newVrModel);
        //vrObjects.Add(newVrModel.gameObject);

        //return newVrModel.gameObject;
        VRModelData.SendDataCreateNewModel(newModel, GameContext.CurrentIdDome);
        return null;
    }


    public void AddNewVrModel3D(VRModelData vRModelData)
    {
        var vrModel = vrModels.Find(item => item.GetData().model_id == vRModelData.model_id);
        if (vrModel != null)
            return;
        VRModelV2 newModel = Instantiate(vrModelPrefab, this.transform).GetComponent<VRModelV2>();
        newModel.SetButtonEdit();
        newModel.transform.localPosition = ConverStringToVector3(vRModelData.model_translate);
        newModel.transform.localEulerAngles = ConverStringToVector3(vRModelData.model_rotation);
        newModel.transform.localScale = Vector3.one;
        newModel.IsJustCreatNew = vRModelData.IsJustCreatNew;
        newModel.SetData(vRModelData).Forget();
        newModel.Initialize(this);
        newModel.ShowMenuUiEdit();

        vrModels.Add(newModel);
        vrObjects.Add(newModel.gameObject);
    }


    public void DeleteVrModel(VRModelV2 vRModel)
    {
        //if (vRModel.IsJustCreatNew)
        //{
        //    DeleteVrModel(vRModel.Id);
        //    return;
        //}
        VrgSyncApi.Send(new SyncDeleteVrObjectMessage()
        {
            idDome = VrDomeControllerV2.Instance.vrDomeData.dome_id,
            type = ObjectType.Model3D,
            id = vRModel.GetData().model_id
        }, SyncDeleteVrObjectMessage.EventKey);

        //vrModels = vrModels.Where(item => item.GetData().model_id != vRModel.GetData().model_id).ToList();
        //vrObjects.Remove(vRModel.gameObject);
    }

    public void DeleteVrModel(int model_id)
    {
        var vRModel = vrModels.Find(item => item.GetData().model_id == model_id);
        vrModels = vrModels.Where(item => item.GetData().model_id != model_id).ToList();
        if (vRModel != null)
        {
            vrObjects.Remove(vRModel.gameObject);
            Destroy(vRModel.gameObject);
        }
    }

    public void RemoveVrModel(VRModelV2 model)
    {
        if (model != null)
        {
            vrModels.Remove(model);
            vrObjects.Remove(model.gameObject);
            Destroy(model.gameObject);
        }
    }

    public void SetTranformModel(int model_id, string translate, string rotation)
    {
        var vRModel = vrModels.Find(item => item.GetData().model_id == model_id);
        if (vRModel != null)
        {
            vRModel.transform.localPosition = ConverStringToVector3(translate);
            vRModel.transform.localEulerAngles = ConverStringToVector3(rotation);
        }
    }

    public void SetScaleModel(int model_id, float scale)
    {
        var vRModel = vrModels.Find(item => item.GetData().model_id == model_id);
        if (vRModel != null)
        {
            vRModel.SetObjectScale(scale);
        }
    }

    public void SetElementScaleModel(int model_id, string scale, int index)
    {
        var vRModel = vrModels.Find(item => item.GetData().model_id == model_id);
        if (vRModel != null)
        {
            vRModel.SetObjectScale(ConverStringToVector3(scale), index);
        }
    }

    public void SetMaterialModel(SyncMaterialModelMessage modelMessage)
    {
        //DebugExtension.LogError("SetMaterialModel" + modelMessage.nameTexture);
        var vRModel = vrModels.Find(item => item.GetData().model_id == modelMessage.id);

        if (vRModel != null)
        {
            VRMaterialData materialData = new VRMaterialData(modelMessage.nameObjRenderer, modelMessage.nameTexture, modelMessage.indexTextureLocal);
            //vRModel.GetData().AddMaterial(materialData);
            vRModel.GetData().ChangeNameTexture(modelMessage.nameTexture);
            vRModel.SetJustCreateNew(false);
            vRModel.UpdateMaterial();
        }
    }

    public void SetMaterialHouse(SyncMaterialHouseMessage modelMessage)
    {
        if (modelMessage.isUpdateHouse)
        {
            if (VrDomeControllerV2.Instance.vrDomeData != null)
            {
                DebugExtension.Log("Update House -------------------------------------------");
                VrDomeControllerV2.Instance.vrDomeData.modelData.UpdateIndexHouse(modelMessage.indexHouse);
                VrDomeControllerV2.Instance.DomeLoadhouse.LoadNewSceneSyncFromServer();
            }
        }
        else
        {
            MaterialDataNetWorkDetail data = new MaterialDataNetWorkDetail(modelMessage.indexMaterialSet, modelMessage.indexMaterialDetail);
            if (VrDomeControllerV2.Instance.vrDomeData != null)
            {
                VrDomeControllerV2.Instance.vrDomeData.modelData.UpdateListMaterialSet(modelMessage.indexHouse, data);
                VrDomeControllerV2.Instance.DomeLoadhouse.UpdateMaterialSyncRuntime(data);
            }
            else
                VrDomeControllerV2.Instance.DomeLoadhouse.UpdateMaterialSyncRuntime(data);
        }
    }
    public void UpdateModel(SyncEditModel Message)
    {
        VRDomeData dome = GameContext.ContentDataCurrent.vr_dome_list.FirstOrDefault(x => x.dome_id == Message.idDome);
        var vRModel = vrModels.Find(item => item.GetData().model_id == Message.model_id);

        if (dome == null) return;
        if (vRModel != null)
        {
            UpdateOutlineModel(vRModel);
        }
    }
    private void UpdateOutlineModel(VRModelV2 vRModel)
    {
        if (vRModel.GetData().isOutline)
        {
            vRModel.SetOutline();
        }
        else
        {
            vRModel.RemoveOutline();
        }
    }
    public void SaveVRObjectData()
    {
        foreach (VRModelV2 model in vrModels)
        {
            model.GetData().model_translate = ConvertVector3ToString(model.transform.localPosition);
            model.GetData().model_rotation = ConvertVector3ToString(model.transform.localEulerAngles);
            model.GetData().model_scale = ConvertVector3ToString(model.transform.localScale);
        }
    }
    public void AddListEditButton()
    {
        ListEditButton.Clear();
        //var editButtons = gameObject.GetComponentsInChildren<VrObjectEditUi>();
        foreach (var d in vrModels)
            ListEditButton.Add(d.UiButtons.gameObject);
    }
    public void ChangeSizeEditButton()
    {
        foreach (var d in ListEditButton)
        {
            distanceEditButtonToPlayer = Vector3.Distance(d.transform.position, Camera.main.transform.position);
            float baseSize1 = (sizeMax - sizeMin) / (distanceMax - distanceMin);
            float baseSize2 = sizeMax - baseSize1 * distanceMax;
            float baseSize = baseSize1 * distanceEditButtonToPlayer + baseSize2;
            //if (distanceEditButtonToPlayer > distanceMax)
            //    d.transform.localScale = Vector3.one * 0.25f;
            //else if (distanceEditButtonToPlayer <= distanceMax && distanceEditButtonToPlayer > distanceMin)
            //    d.transform.localScale = Vector3.one * baseSize * 0.25f;
            //else if (distanceEditButtonToPlayer <= distanceMin)
            //    d.transform.localScale = Vector3.one * sizeMin * 0.25f;
            d.transform.localScale = Vector3.one * 0.25f;
        }
    }

    public void ChangeColorModel()
    {
        foreach (var item in vrModels)
        {
            if (item is VRModelV2 model)
            {
                model.ChangeLight();
            }
        }
    }
}
