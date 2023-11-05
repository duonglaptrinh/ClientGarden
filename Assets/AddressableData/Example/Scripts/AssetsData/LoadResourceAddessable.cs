using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "LoadResourceAddessable", menuName = ("Addressables/LoadResourceAddessable"))]
public class LoadResourceAddessable : ScriptableObject
{
    /// <summary>
    /// Use to setting in the data.asset file
    /// </summary>
    public static List<string> listCategory = new List<string>
    {
        "1.ファサード", "2.フェンス・スクリーン", "3.お庭", "4.車庫まわり", "5.ファニチャー", "6.植栽", "7.その他", "8.床材", "9.壁材", "10.インテリア", "11.照明"
    };

    /// <summary>
    /// Use to Show on list UI
    /// </summary>
    public static List<string> listCategoryNew = new List<string>
        { "ファサード", "スクリーン", "お庭", "車庫まわり", "ファニチャー", "植栽", "その他", "床材", "壁材", "インテリア", "照明" };

    public static List<string> listCategoryNewUpdate = new List<string>
        { "ファサード", "フェンス\nスクリーン", "お庭", "車庫まわり", "ファニチャー", "植栽", "その他", "床材", "壁材", "インテリア", "照明" };


    public List<HouseDataAsset> ListPathSceneHouseAddressable = new List<HouseDataAsset>();
    public List<string> ListThumbPathAddressable = new List<string>();
    public List<OneModelAssetsData> ListAssetModel { get; set; } = new List<OneModelAssetsData>();
    public List<CategoryDataAsset> ListAssetByCategory { get; set; } = new List<CategoryDataAsset>();

    public OneModelAssetsData GetDataAssetByPrefabPath(string pathPrefab)
    {
        //return ListAssetByCategory.Where(x => x.NameCategory == nameCategory).FirstOrDefault();
        return ListAssetModel.FirstOrDefault(x => x.pathPrefab == pathPrefab);
    }
    public CategoryDataAsset GetCategoryDataAsset(string nameCategory)
    {
        //return ListAssetByCategory.Where(x => x.NameCategory == nameCategory).FirstOrDefault();
        return ListAssetByCategory.FirstOrDefault(x => x.NameCategory == nameCategory);
    }
    public float GetTotalSubCategory(string nameCategory)
    {
        var categoryData = AddressableDownloadManager.ResourcesData.GetCategoryDataAsset(nameCategory);
        List<string> listSub = AddressableDownloadManager.ResourcesData.GetNameSubCategoryDataAsset(categoryData);
        float totalSub = 0;
        foreach (var nameSub in listSub)
        {
            totalSub += GetTotalCategory(nameCategory, nameSub);
        }

        return totalSub;
    }

    public List<OneModelAssetsData> GetTotalSubCategory(string nameCategory, string nameSubCategory)
    {
        var listAssetByCategory = ListAssetByCategory.FirstOrDefault(x => x.NameCategory == nameCategory);
        return listAssetByCategory?.ListAssetModel.Where(o => o.CategoryChild == nameSubCategory).Distinct().ToList();
    }

    public float GetTotalCategory(string nameCategory, string nameSubCategory)
    {
        List<OneModelAssetsData> listModel = GetTotalSubCategory(nameCategory, nameSubCategory);
        return listModel.Count;
    }
    
    public CategoryDataAsset GetCategoryDataAssetByNewName(string nameCategory)
    {
        int indexOldCategory = listCategoryNew.IndexOf(nameCategory);
        return ListAssetByCategory.FirstOrDefault(x => x.NameCategory == listCategory[indexOldCategory]);
    }
    public List<string> GetNameSubCategoryDataAsset(CategoryDataAsset category)
    {
        return category.ListAssetModel.Select(o => o.CategoryChild).Distinct().ToList();
    }

    public List<OneModelAssetsData> GetSubCategoryDataAsset(CategoryDataAsset category, string nameSubCategory)
    {
        //string[] arr = nameSubCategory.Split('+');
        //if (arr.Length >= 2)
        //{
        //    List<OneModelAssetsData> list1 = category.ListAssetModel.Where(o => o.CategoryChild == arr[0]).Distinct().ToList();
        //    List<OneModelAssetsData> list2 = category.ListAssetModel.Where(o => o.CategoryChild == arr[1]).Distinct().ToList();
        //    list1.AddRange(list2);
        //    return list1;
        //}
        //else
        //{
        return category.ListAssetModel.Where(o => o.CategoryChild == nameSubCategory).Distinct().ToList();
        //}

    }

    public void PrepareData()
    {
        int count = 0;
        ListAssetModel.Clear();
        foreach (var item in ListThumbPathAddressable)
        {
            //string str = "Assets/AddressableData/AllModel/2.フェンス・スクリーン/トレメッシュフェンス/thumb1.jpg";
            string path = Path.GetDirectoryName(item);
            string pathAsset = Path.Combine(path, "data.asset");
            pathAsset = pathAsset.Replace("\\", "/");
            Addressables.LoadAssetAsync<OneModelAssetsData>(pathAsset).Completed += asset =>
            {
                OneModelAssetsData data = asset.Result;
                ListAssetModel.Add(data);
                count++;
                if (count >= ListThumbPathAddressable.Count)
                {
                    LoadCategory();
                }
            };
        }
    }

    public static string GetNameCategoryOnAppByName(string nameCategoryInDataAsset)
    {
        for (int i = 0; i < listCategory.Count; ++i)
        {
            string item = listCategory[i];
            if (nameCategoryInDataAsset == item) return listCategoryNewUpdate[i];
        }
        return nameCategoryInDataAsset;
    }

    void LoadCategory()
    {
        ListAssetByCategory.Clear();
        //foreach (var item in listCategory)
        for (int i = 0; i < listCategory.Count; ++i)
        {
            string item = listCategory[i];
            List<OneModelAssetsData> list = ListAssetModel.Where(x => x.CategoryParent == item).ToList();
            ListAssetByCategory.Add(
                new CategoryDataAsset(listCategoryNew[i], list)
                );
        }
    }

    [ContextMenu("Load Data Addressable")]
    public void LoadThumbData()
    {
#if UNITY_EDITOR
        ListThumbPathAddressable.Clear();
        // Folders comes up as Object.
        var assetPath = "Assets/AddressableData/AllModel";
        // Other assets may appear as Object, so a Directory Check filters directories from folders.
        if (Directory.Exists(assetPath))
        {
            var dirsToAdd = CustomSearcher.GetTopDirectory(assetPath);

            foreach (var item in dirsToAdd)
            {
                List<string> listTemp = CustomSearcher.GetTopDirectory(item);
                // Add sub-folders.
                foreach (var dir in listTemp)
                {
                    var path = dir.Replace('\\', '/');
                    var dataPath = path + "/data.asset";
                    OneModelAssetsData dataAsset = (OneModelAssetsData)AssetDatabase.LoadAssetAtPath(dataPath, typeof(OneModelAssetsData));
                    ListThumbPathAddressable.Add(dataAsset.pathThumb);
                }
            }
        }
        else
        {
            DebugExtension.LogError("Folder asset model " + assetPath + "not Exist!");
        }
#endif
    }

    [ContextMenu("Load Data Scene House")]
    public void LoadSceneHouseData()
    {
#if UNITY_EDITOR
        ListPathSceneHouseAddressable.Clear();
        // Folders comes up as Object.
        var assetPath = "Assets/AddressableData/HouseData";
        // Other assets may appear as Object, so a Directory Check filters directories from folders.
        if (Directory.Exists(assetPath))
        {
            var dirsToAdd = CustomSearcher.GetTopDirectory(assetPath);

            foreach (var item in dirsToAdd)
            {
                if (Path.GetFileName(item).Contains("House"))
                {
                    string[] dirs = Directory.GetFiles(Path.Combine(item, "Daytime"), $"*.unity");
                    string[] dirsNight = Directory.GetFiles(Path.Combine(item, "Night"), $"*.unity");

                    string nameFolder = Path.GetFileNameWithoutExtension(item);

                    string thumb = Path.Combine(item, "Thumb.png");
                    if (!File.Exists(thumb))
                    {
                        thumb = Path.Combine(item, "Thumb.jpg");
                        if (!File.Exists(thumb))
                        {
                            DebugExtension.LogError("Missing thumbnail in " + nameFolder + " house.");
                        }
                    }

                    ListPathSceneHouseAddressable.Add(
                        new HouseDataAsset(
                            dirs[0],
                            dirsNight[0],
                            thumb,
                            nameFolder
                        )
                    );
                }
            }
        }
        else
        {
            DebugExtension.LogError("Folder asset model " + assetPath + "not Exist!");
        }
#endif
    }

    [ContextMenu("Save Data")]
    public static void SaveEditor(UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(obj);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
}

[System.Serializable]
public class CategoryDataAsset
{
    public string NameCategory;
    public List<OneModelAssetsData> ListAssetModel = new List<OneModelAssetsData>();
    public CategoryDataAsset(string NameCategory, List<OneModelAssetsData> ListAssetModel)
    {
        this.NameCategory = NameCategory;
        this.ListAssetModel = ListAssetModel;
    }
}

[System.Serializable]
public class HouseDataAsset : ItemDataBase
{
    public string PathSceneDay;
    public string PathSceneNight;
    public string PathThumbnail;
    public string NameOfTheHouse;
    public HouseDataAsset() { }
    public HouseDataAsset(string pathSceneDay, string pathSceneNight, string pathThumbnail, string namehouse)
    {
        this.PathSceneDay = pathSceneDay.Replace("\\", "/");
        this.PathSceneNight = pathSceneNight.Replace("\\", "/");
        this.PathThumbnail = pathThumbnail.Replace("\\", "/");
        //this.NameOfTheHouse = namehouse;
    }
}

[System.Serializable]
public class HouseDataJson
{
    public string NameOnApp;
    public string Category;
    public string Thumbnail;
    public float SizeX;
    public float SizeZ;
    public string Price;
    public string PriceUnit;
    public string Description;
}


