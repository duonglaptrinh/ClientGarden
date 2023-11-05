using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
[CreateAssetMenu(fileName = "OneModelAssetsData", menuName = ("Addressables/OneModelAssetsData"))]
public class OneModelAssetsData : ScriptableObject
{
    /// <summary>
    /// Path addressable thumbnail
    /// </summary>
    public string pathThumb;
    /// <summary>
    /// Path addressable prefab, include all fbx color.
    /// ModelA
    /// ------ModelColor1
    /// ------ModelColor2
    /// ------ModelColor3
    /// </summary>
    public string pathPrefab;
    /// <summary>
    /// Path addressable of list file ColorTexture
    /// </summary>
    public List<string> listPathColorTexture = new List<string>();

    /// <summary>
    /// Load from json
    /// </summary>
    public string NameOnApp;
    public string CategoryParent;
    public string CategoryChild;
    public int Prize = 0;
    public string PrizeUnit = "¥";
    public string Size;
    public int SizeHeight;
    public int SizeWidth;
    public int SizeDepth;
    public string EnglishName;
    public string Description;
    public string Featured;
    public float Weight;

    public OneModelAssetsData(string pathOrigin, string pathPrefab, List<string> pathColorTexture, OneModelJsonData json)
    {
        UpdateData(pathOrigin, pathPrefab, pathColorTexture, json);
    }
    public void UpdateData(string pathOrigin, string pathPrefab, List<string> pathColorTexture, OneModelJsonData json)
    {
        pathThumb = pathOrigin + "/thumb.jpg";
        this.pathPrefab = pathPrefab;
        listPathColorTexture = pathColorTexture;
        return;
        NameOnApp = json.NameOnApp;
        CategoryParent = json.CategoryParent;
        CategoryChild = json.CategoryChild;
        Description = json.Description;
        Prize = json.Price;
        PrizeUnit = json.PriceUnit;
        Size = json.Size;
        GetSize();
    }

    [ContextMenu("Get Size")]
    public void GetSize()
    {
        if (string.IsNullOrEmpty(Size))
        {
            SizeHeight = 0;
            SizeWidth = 0;
            SizeDepth = 0;
            return;
        }
        //Size = "H2700×W4500×D1770"
        try
        {
            string[] arr = Size.Split('×');
            int.TryParse(arr[0].Replace("H", ""), out SizeHeight);
            int.TryParse(arr[1].Replace("W", ""), out SizeWidth);
            int.TryParse(arr[2].Replace("D", ""), out SizeDepth);
            SaveEditor(this);
        }
        catch (Exception e)
        {
            DebugExtension.LogError(NameOnApp + " Error Size = " + Size);
        }
    }
    [ContextMenu("Save Data")]
    public static void SaveEditor(UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(obj);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
#if UNITY_EDITOR
    [ContextMenu("Get Path Thumb And Prefab")]
    public void GetPathThumbAndPrefab()
    {
        string selectedFilePath = AssetDatabase.GetAssetPath(Selection.activeObject);
        string nameFolder = GetNameSelectedFolderPath(selectedFilePath);
        pathThumb = RenameFolder(pathThumb, nameFolder);
        pathPrefab = RenameFolder(pathPrefab, nameFolder);
        SaveEditor(this);
    }
#endif
    public string GetNameSelectedFolderPath(string selectedFilePath)
    {

        if (!string.IsNullOrEmpty(selectedFilePath))
        {
            string[] pathComponents = selectedFilePath.Split('/');
            int indexOfAddressableData = System.Array.IndexOf(pathComponents, "AllModel");

            if (indexOfAddressableData >= 0)
            {
                string rootFolderName = pathComponents[indexOfAddressableData + 1];
                Debug.Log("Selected File's Root Folder Name: " + rootFolderName);
                return rootFolderName;
            }
            else
            {
                Debug.LogWarning("Root folder not found in the path.");
                return null;
            }
        }
        else
        {
            Debug.LogWarning("No file selected.");
            return null;
        }
    }
    string RenameFolder(string oldPath, string newName)
    {
        //string selectedFilePath = "Assets/AddressableData/AllModel/11.照明/ウォールライト9型/data.asset";
        string selectedFilePath = oldPath;
        string[] oldFolderNamePattern = selectedFilePath.Split('/');

        oldFolderNamePattern[3] = newName;
        string newPath = "";
        foreach (string folderName in oldFolderNamePattern)
        {
            newPath += folderName + "/";
        }
        newPath = newPath.Substring(0, newPath.Length - 1);
        Debug.Log("New Path: " + newPath);
        return newPath;
    }
}
