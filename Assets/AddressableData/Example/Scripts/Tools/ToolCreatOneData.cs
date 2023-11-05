using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System;

public class ToolCreatOneData
{
#if UNITY_EDITOR
    //[MenuItem("Assets/AddressableImporter: Refactor Root Folder Data")]
    public static void RefactorFolderData()
    {
        HashSet<string> pathsToImport = new HashSet<string>();
        // Folders comes up as Object.
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            var assetPath = AssetDatabase.GetAssetPath(obj); // -> Assets/AddressableData/AllModel
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
                        // Filter out .dirname and dirname~, those are invisible to Unity.
                        if (!dir.StartsWith(".") && !dir.EndsWith("~"))
                        {
                            pathsToImport.Add(dir.Replace('\\', '/'));
                        }
                    }
                }
            }
        }
        foreach (var item in pathsToImport)
        {
            DebugExtension.Log(item);
            CreateAssetData(item);
        }
        //LoadResource();
    }

    //[MenuItem("Assets/AddressableImporter: Refactor Category Folder Data")]
    public static void RefactorCategoryFolderData()
    {
        // Folders comes up as Object.
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            var assetPath = AssetDatabase.GetAssetPath(obj); // -> Assets/AddressableData/AllModel
            // Other assets may appear as Object, so a Directory Check filters directories from folders.
            if (Directory.Exists(assetPath))
            {
                var dirsToAdd = CustomSearcher.GetTopDirectory(assetPath);

                foreach (var item in dirsToAdd)
                {
                    CreateAssetData(item);
                }
            }
        }
        //LoadResource();
    }

    [MenuItem("Assets/AddressableImporter: Refactor One Folder Data")]
    public static void RefactorOneFolderData()
    {
        // Folders comes up as Object.
        foreach (UnityEngine.Object obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
        {
            var assetPath = AssetDatabase.GetAssetPath(obj); // -> Assets/AddressableData/AllModel
            // Other assets may appear as Object, so a Directory Check filters directories from folders.
            if (Directory.Exists(assetPath))
            {
                CreateAssetData(assetPath);
            }
        }
        //LoadResource();
    }
    static void CreateAssetData(string pathModelOrigin)
    {
        pathModelOrigin = pathModelOrigin.Replace('\\', '/');

        var dataThumb = pathModelOrigin + "/thumb.jpg";
        if (!File.Exists(dataThumb))
        {
            DebugExtension.LogError("Thumbnail Image not found Or Name not correct. Shoulb be set name thumb.jpg");
            return;
        }
        var dataPath = pathModelOrigin + "/data.asset";
        //var filesjson = Directory.GetFiles(pathModelOrigin, "*.json", SearchOption.AllDirectories);
        var filesfbx = Directory.GetFiles(pathModelOrigin, "*.fbx", SearchOption.AllDirectories);

        string[] arr = pathModelOrigin.Split('/');
        string nameModel = arr[arr.Length - 1];
        DebugExtension.Log("nameModel = " + nameModel);
        var pathPrefab = pathModelOrigin + "/" + nameModel + ".prefab";
        //Get list file Name Color Texture
        List<string> listNameColorTexture = new List<string>();
        List<GameObject> listGameObjectByColor = new List<GameObject>();
        OneModelJsonData first = null;
        int c = 0;
        if (!File.Exists(pathPrefab))
        {
            DebugExtension.Log("Create prefab " + nameModel);
            GameObject obj = new GameObject(nameModel);
            obj.transform.position = Vector3.zero;
            foreach (var item in filesfbx)
            {
                GameObject t = (GameObject)AssetDatabase.LoadAssetAtPath(item, typeof(GameObject));
                GameObject fbx = GameObject.Instantiate(t, obj.transform);
                listGameObjectByColor.Add(fbx);
                if (c == 0) fbx.SetActive(true);
                else fbx.SetActive(false);

                string file = item.Replace(".fbx", ".json");
                TextAsset te = (TextAsset)AssetDatabase.LoadAssetAtPath(file, typeof(TextAsset));
                OneModelJsonData json = JsonUtility.FromJson<OneModelJsonData>(te.text);
                first = json;
                listNameColorTexture.Add(json.NameTextureInColorTexture);
                c++;
            }
            ModelSupportCreatePrefab pre = obj.AddComponent<ModelSupportCreatePrefab>();
            pre.ListGameObjectbByColor = listGameObjectByColor;
            pre.SetPositionModel();
            //AssetDatabase.CreateAsset(obj, dataPrefab);
            PrefabUtility.SaveAsPrefabAssetAndConnect(obj, pathPrefab, InteractionMode.AutomatedAction);
        }
        else
        {
            List<string> listFileFbxNew = new List<string>();
            GameObject obj = PrefabUtility.LoadPrefabContents(pathPrefab);
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                GameObject child = obj.transform.GetChild(i).gameObject;
                string nameChild = child.name.Replace("(Clone)", "");
                bool isFound = false;
                foreach (var item in filesfbx)
                {
                    if (item.Contains(nameChild))
                    {
                        string file = item.Replace(".fbx", ".json");
                        DebugExtension.Log(file);
                        TextAsset te = (TextAsset)AssetDatabase.LoadAssetAtPath(file, typeof(TextAsset));
                        OneModelJsonData json = JsonUtility.FromJson<OneModelJsonData>(te.text);
                        first = json;

                        listNameColorTexture.Add(json.NameTextureInColorTexture);
                        listGameObjectByColor.Add(child);
                        isFound = true;
                        break;
                    }
                }
                if (!isFound)
                {
                    DebugExtension.LogError("File name fbx and child object not same file " + nameModel + "  childName = " + nameChild);
                }
            }

            //foreach (var item in listFileFbxNew)
            //{
            //    string file = item.Replace(".fbx", ".json");
            //    DebugExtension.Log(file);
            //    TextAsset te = (TextAsset)AssetDatabase.LoadAssetAtPath(file, typeof(TextAsset));
            //    OneModelJsonData json = JsonUtility.FromJson<OneModelJsonData>(te.text);
            //    first = json;
            //    listNameColorTexture.Add(json.NameTextureInColorTexture);

            //    for (int i = 0; i < obj.transform.childCount; i++)
            //    {
            //        GameObject child = obj.transform.GetChild(i).gameObject;
            //        if (string.IsNullOrEmpty(json.NameTextureInColorTexture) || child.name.Contains(json.NameTextureInColorTexture.Replace(".jpg", "")))
            //        {
            //            listGameObjectByColor.Add(child);
            //            break;
            //        }
            //    }
            //}
            ModelSupportCreatePrefab pre = obj.GetComponent<ModelSupportCreatePrefab>();
            pre.ListGameObjectbByColor = listGameObjectByColor;
            // Save contents back to Prefab Asset and unload contents.
            DebugExtension.Log("Update data Index prefab " + pathPrefab);
            //PrefabUtility.ApplyObjectOverride(obj, pathPrefab, InteractionMode.AutomatedAction);
            //PrefabUtility.SaveAsPrefabAssetAndConnect(obj, pathPrefab, InteractionMode.AutomatedAction);
            //PrefabUtility.UnloadPrefabContents(obj);
        }

        OneModelAssetsData dataAsset;
        if (File.Exists(dataPath))
        {
            dataAsset = (OneModelAssetsData)AssetDatabase.LoadAssetAtPath(dataPath, typeof(OneModelAssetsData));
            dataAsset.UpdateData(pathModelOrigin, pathPrefab, GetListPathColorTexture(listNameColorTexture), first);
            DebugExtension.Log("Update file Done " + dataPath);
        }
        else
        {
            dataAsset = new OneModelAssetsData(pathModelOrigin, pathPrefab, GetListPathColorTexture(listNameColorTexture), first);
            AssetDatabase.CreateAsset(dataAsset, pathModelOrigin + "/data.asset");
            DebugExtension.Log("Create file Done " + dataPath);
        }
        if (listGameObjectByColor.Count > 1 && listGameObjectByColor.Count != listNameColorTexture.Count)
            DebugExtension.LogError("Something wrong!! " + nameModel + "   listNameColorTexture not same listGameObjectByColor");
        DebugExtension.Log(" -------------- Refactor Done --------------------");
    }
    static List<string> GetListPathColorTexture(List<string> listNameColorTexture)
    {
        List<string> listPathFileColor = new List<string>();
        string pathColorTexture = "Assets/AddressableData/ColorTexture";
        var filesToAdd = Directory.GetFiles(pathColorTexture, "*", SearchOption.AllDirectories);
        foreach (var item in listNameColorTexture)
        {
            if (string.IsNullOrEmpty(item))
            {
                listPathFileColor.Add("");
                continue;
            }
            foreach (var file in filesToAdd)
            {
                if (item == Path.GetFileName(file))
                {
                    listPathFileColor.Add(file.Replace('\\', '/'));
                    break;
                }
            }
        }
        return listPathFileColor;
    }

    static bool IsAssetIgnored(string assetPath)
    {
        return assetPath.EndsWith(".meta") || assetPath.EndsWith(".DS_Store") || assetPath.EndsWith("~");
    }
    [MenuItem("Assets/AddressableImporter: Load Resources")]
    static void LoadResource()
    {
        string file = "Assets/AddressableData/LoadResourceAddessable.asset";
        LoadResourceAddessable load = (LoadResourceAddessable)AssetDatabase.LoadAssetAtPath(file, typeof(LoadResourceAddessable));
        load.LoadThumbData();
    }
#endif
}

public class CustomSearcher
{
    public static List<string> GetTopDirectory(string assetPath)
    {
        var dirsToAdd = CustomSearcher.GetDirectories(assetPath, "*", SearchOption.TopDirectoryOnly);
        List<string> pathsToImport = new List<string>();
        // Add sub-folders.
        //var dirsToAdd = Directory.GetDirectories(assetPath, "*", SearchOption.TopDirectoryOnly);
        foreach (var dir in dirsToAdd)
        {
            // Filter out .dirname and dirname~, those are invisible to Unity.
            if (!dir.StartsWith(".") && !dir.EndsWith("~"))
            {
                pathsToImport.Add(dir.Replace('\\', '/'));
            }
        }
        return pathsToImport;
    }
    public static List<string> GetDirectories(string path, string searchPattern = "*",
        SearchOption searchOption = SearchOption.AllDirectories)
    {
        if (searchOption == SearchOption.TopDirectoryOnly)
            return Directory.GetDirectories(path, searchPattern).ToList();

        var directories = new List<string>(GetDirectories(path, searchPattern));

        for (var i = 0; i < directories.Count; i++)
            directories.AddRange(GetDirectories(directories[i], searchPattern));

        return directories;
    }

    private static List<string> GetDirectories(string path, string searchPattern)
    {
        try
        {
            return Directory.GetDirectories(path, searchPattern).ToList();
        }
        catch (UnauthorizedAccessException)
        {
            return new List<string>();
        }
    }
}
