using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MaterialController : MonoBehaviour
{
    [SerializeField] private List<MaterialSet> listMaterialSets = new List<MaterialSet>();
    public List<MaterialSet> ListMaterialSets => listMaterialSets;
    public MaterialSet GetMaterialSetByIndex(int index)
    {
        if (index >= listMaterialSets.Count) return null;
        return listMaterialSets[index];
    }

    [ContextMenu("Copy Data")]
    void MenuCopyData()
    {
        foreach (var item in FindObjectsOfType<MaterialController>())
        {
            if (item.ListMaterialSets != null && item.ListMaterialSets.Count > 0)
            {
                CopyData(item.ListMaterialSets, gameObject);
                break;
            }
        }
        LoadResourceAddessable.SaveEditor(this);
    }

    void CopyData(List<MaterialSet> list, GameObject parentTarget)
    {
        listMaterialSets = new List<MaterialSet>();
        foreach (var item in list)
        {
            listMaterialSets.Add(new MaterialSet(item, parentTarget));
        }
    }
}

[Serializable]
public class MaterialSet
{
    [SerializeField] string itemName;
    public string ItemName => itemName;

    [SerializeField] private List<GameObject> targetObjects;
    public List<GameObject> TargetObjects => targetObjects;

    public GameObject targetDefault => targetObjects.Count > 0 ? targetObjects[0] : null;

    [SerializeField] private List<MaterialData> listMaterialsData;
    public List<MaterialData> ListMaterialsData => listMaterialsData;
    public MaterialData GetMaterialDataByIndex(int index)
    {
        if (index >= listMaterialsData.Count) return null;
        return listMaterialsData[index];
    }
    public MaterialSet()
    {
        itemName = "";
        listMaterialsData = new List<MaterialData>();
        targetObjects = new List<GameObject>();
    }
    public MaterialSet(MaterialSet item, GameObject parentObject)
    {
        itemName = item.itemName;

        targetObjects = new List<GameObject>();
        Renderer[] listRender = parentObject.GetComponentsInChildren<Renderer>();
        foreach (var obj in item.targetObjects)
        {
            var target = listRender.FirstOrDefault(x => x.name == obj.name).gameObject;
            targetObjects.Add(target);
        }

        listMaterialsData = new List<MaterialData>();
        foreach (var mat in item.listMaterialsData)
        {
            listMaterialsData.Add(MaterialData.CopyData(mat));
        }
    }
}
[Serializable]
public class MaterialData
{
    [SerializeField] private Material material;
    public Material Material => material;

    [SerializeField] private AssetReferenceTexture2D icon;
    public AssetReferenceTexture2D Icon => icon;

    public MaterialData()
    {
        material = null;
        icon = null;
    }
    public MaterialData(Material mat, AssetReferenceTexture2D icon)
    {
        material = mat;
        this.icon = icon;
    }
    public static MaterialData CopyData(MaterialData item)
    {
        return new MaterialData(item.material, item.icon);
    }
}

[Serializable]
public class MaterialDataNetWorkDetail
{
    public int indexMaterialSet;
    public int indexMaterialDetail;
    public MaterialDataNetWorkDetail() { }
    public MaterialDataNetWorkDetail(int indexSet, int indexDetail)
    {
        indexMaterialSet = indexSet;
        indexMaterialDetail = indexDetail;
    }
    public MaterialDataNetWorkDetail(MaterialDataNetWorkDetail oldData)
    {
        indexMaterialSet = oldData.indexMaterialSet;
        indexMaterialDetail = oldData.indexMaterialDetail;
    }
}

[Serializable]
public class HouseMaterialData
{
    public int indexHouse;
    public List<MaterialDataNetWorkDetail> ListMaterialSet = new List<MaterialDataNetWorkDetail>();

    public HouseMaterialData()
    {

    }
    public HouseMaterialData(int indexHouse)
    {
        this.indexHouse = indexHouse;
        this.ListMaterialSet = new List<MaterialDataNetWorkDetail>() {
                new MaterialDataNetWorkDetail(0, 0)
            };
    }
    public HouseMaterialData(HouseMaterialData oldData)
    {
        this.indexHouse = oldData.indexHouse;
        this.ListMaterialSet = new List<MaterialDataNetWorkDetail>();
        foreach (var item in oldData.ListMaterialSet)
        {
            this.ListMaterialSet.Add(new MaterialDataNetWorkDetail(item));
        }
    }
    public HouseMaterialData(int indexHouse, List<MaterialDataNetWorkDetail> listDetail)
    {
        this.indexHouse = indexHouse;
        this.ListMaterialSet = listDetail;
    }
    public void UpdateListMaterialSet(MaterialDataNetWorkDetail newUpdate)
    {
        for (int i = 0; i < ListMaterialSet.Count; i++)
        {
            MaterialDataNetWorkDetail item = ListMaterialSet[i];
            if (item.indexMaterialSet == newUpdate.indexMaterialSet)
            {
                ListMaterialSet[i] = newUpdate;
                return;
            }
        }
        ListMaterialSet.Add(newUpdate);
    }
}

[Serializable]
public class ModelDataHouseNetwork
{
    public int indexHouse;
    public string model_translate;
    public string model_rotation;
    public string model_scale;
    public List<HouseMaterialData> ListHouseMaterialData;
    public float Land_Setting_FrontOf;
    public float Land_Setting_Behide;
    public float Land_Setting_Left;
    public float Land_Setting_Right;
    public ModelDataHouseNetwork()
    {
        this.indexHouse = 0;
        this.model_translate = "0, -1.5, 0";
        this.model_rotation = "0, 0, 0";
        this.model_scale = "1, 1, 1";
        this.ListHouseMaterialData = new List<HouseMaterialData>();
        for (int i = 0; i < 6; i++)
        {
            this.ListHouseMaterialData.Add(new HouseMaterialData(i));
        }
        this.Land_Setting_FrontOf = 5;
        this.Land_Setting_Behide = 2;
        this.Land_Setting_Left = 3;
        this.Land_Setting_Right = 3;
    }
    public ModelDataHouseNetwork(ModelDataHouseNetwork oldData)
    {
        this.indexHouse = oldData.indexHouse;
        this.model_translate = oldData.model_translate;
        this.model_rotation = oldData.model_rotation;
        this.model_scale = oldData.model_scale;
        this.ListHouseMaterialData = new List<HouseMaterialData>();
        foreach (var item in oldData.ListHouseMaterialData)
        {
            this.ListHouseMaterialData.Add(new HouseMaterialData(item));
        }
        this.Land_Setting_Behide = oldData.Land_Setting_Behide;
        this.Land_Setting_FrontOf = oldData.Land_Setting_FrontOf;
        this.Land_Setting_Left = oldData.Land_Setting_Left;
        this.Land_Setting_Right = oldData.Land_Setting_Right;
    }

    public void UpdateIndexHouse(int keyIndexHouse)
    {
        indexHouse = keyIndexHouse;
    }
    public void UpdateListMaterialSet(int keyIndexHouse, MaterialDataNetWorkDetail newUpdate)
    {
        for (int i = 0; i < ListHouseMaterialData.Count; i++)
        {
            HouseMaterialData item = ListHouseMaterialData[i];
            if (item.indexHouse == keyIndexHouse)
            {
                ListHouseMaterialData[i].UpdateListMaterialSet(newUpdate);
                return;
            }
        }
        HouseMaterialData newHouse = new HouseMaterialData(ListHouseMaterialData.Count);
        newHouse.UpdateListMaterialSet(newUpdate);
        ListHouseMaterialData.Add(newHouse);
    }
    public void UpdateSizeLand(TypeLandDirection type, float size)
    {
        switch (type)
        {
            case TypeLandDirection.frontOf:
                this.Land_Setting_FrontOf = size;
                break;
            case TypeLandDirection.behide:
                this.Land_Setting_Behide = size;
                break;
            case TypeLandDirection.left:
                this.Land_Setting_Left = size;
                break;
            case TypeLandDirection.right:
                this.Land_Setting_Right = size;
                break;
        }
    }
}

[Serializable]
public class JsonLandHouse
{
    public TypeLandDirection type;
    public float size;
    public JsonLandHouse(TypeLandDirection type, float size)
    {
        this.type = type;
        this.size = size;
    }
    public static JsonLandHouse Convert(string jsonLand)
    {
        return JsonUtility.FromJson<JsonLandHouse>(jsonLand);
    }
}
