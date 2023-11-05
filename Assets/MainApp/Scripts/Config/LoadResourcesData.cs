using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LoadResourcesData", menuName = ("Assets/LoadResourcesData"))]
public class LoadResourcesData : ScriptableObject
{
    static LoadResourcesData instance;
    public static LoadResourcesData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = (LoadResourcesData)Resources.Load("LoadResourcesData");
            }
            return instance;
        }
    }

    [Header("Setting Sprite")]
    public Texture2D icon_Room;
    public GameObject prefabApiInputTestTablet;
    public Material skybox;
    public Material skyboxNight;
    public List<ItemDataCategory> listThumbnailCategory;
    public List<Sprite> listThumbnailTemplate;
    public TextAsset jsonTemplate;
    public List<Sprite> listSpriteThumbTutorials;
    public List<Sprite> listSpriteBackgroundTutorials;
    public Texture2D icon_Room_Empty;
    //public List<string> listNameCategory;
    /// <summary>
    /// define list category in menu;
    /// </summary>
    public List<MenuNewItemData> listCategoryMenu;

    public Sprite GetSpriteTutorialByName(string nameImage)
    {
        if (listSpriteThumbTutorials == null) return null;
        nameImage = nameImage.Replace(".jpg", "");
        nameImage = nameImage.Replace(".png", "");
        return listSpriteThumbTutorials.FirstOrDefault(x => x.name == nameImage);
    }
    public Sprite GetSpriteBackgroundTutorialByName(int index)
    {
        if (listSpriteBackgroundTutorials == null || index >= listSpriteBackgroundTutorials.Count) return null;
        return listSpriteBackgroundTutorials[index];
    }
    //public ItemDataModel GetDataModelByName(string categoryName)
    //{
    //    return listThumbnailModel.FirstOrDefault(x => x.modelName == categoryName);
    //}
    public ItemDataCategory GetDataCategoryByName(string categoryName)
    {
        return listThumbnailCategory.FirstOrDefault(x => x.nameCategory == categoryName);
    }
    public ItemDataSubCategory GetDataSubCategoryByName(ItemDataCategory category, string nameSubCategory)
    {
        return category.listSubCategoryData.FirstOrDefault(x => x.categorySubName == nameSubCategory);
    }

    //Lấy length value index trong maxnumber mà ko bị trùng nhau
    public static List<int> GetListIndexNotSameType(int lengthList, int minNumber = 0, int maxNumber = 52)
    {
        List<int> list = new List<int>();
        for (int i = minNumber; i < maxNumber; i++)
        {
            list.Add(i);
        }

        List<int> newList = new List<int>();
        for (int i = 0; i < lengthList; i++)
        {
            int index = Random.Range(0, list.Count);
            newList.Add(list[index]);
            list.RemoveAt(index);
        }
        return newList;
    }
    public Sprite GetThumbnailPlanTemplate(int index)
    {
        return listThumbnailTemplate[index % listThumbnailTemplate.Count];
    }
    [ContextMenu("Save Data")]
    public void SaveData()
    {
        SaveEditor(this);
    }
    public static void SaveEditor(UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(obj);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
}

