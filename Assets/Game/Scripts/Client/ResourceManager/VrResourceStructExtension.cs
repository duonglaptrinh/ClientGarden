using System.IO;
using System.Linq;
using UnityEngine;
using static Game.Client.VrResourceStruct;

namespace Game.Client
{
    public static class VrResourceStructExtension
    {
        public static string GetVrJsonPath(this VrResourceStruct vrResourceStruct, string contentName)
        {
            return Path.Combine(GetVrJsonFolder(vrResourceStruct, contentName), VrContentFileNameConstant.VR_DATA_JSON);
        }

        public static string GetVrJsonFolder(this VrResourceStruct vrResourceStruct, string contentName)
        {
            var absolutePath =
                $"{VrDomeAssetResourceNameDefine.VR_DATA_ROOT}/{VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT}/{contentName}/{VrDomeAssetResourceNameDefine.VR_JSON}";
            var path = Path.Combine(vrResourceStruct.RootPath, absolutePath);

            return path;
        }

        public static string GetVrContentFolderPath(this VrResourceStruct vrResourceStruct, string contentName)
        {
            return GetFolderPath(vrResourceStruct, contentName);
        }

        public static string GetVrContentTitlePath(this VrResourceStruct vrResourceStruct, string contentName)
        {
            var folderPath = vrResourceStruct.GetVrContentFolderPath(contentName);
            return string.IsNullOrEmpty(folderPath)
                ? string.Empty
                : Path.Combine(folderPath, VrContentFileNameConstant.CONTENT_DATA_TITLE_JSON);
        }

        public static string GetVrContentIconPath(this VrResourceStruct vrResourceStruct, string contentName)
        {
            var folderPath = vrResourceStruct.GetVrContentFolderPath(contentName);
            return string.IsNullOrEmpty(folderPath)
                ? string.Empty
                : Path.Combine(folderPath, VrContentFileNameConstant.CONTENT_DATA_ICON);
        }

        public static string GetFolderPath(this VrResourceStruct vrResourceStruct, string contentName, string folder1 = "", string folder2 = "")
        {
            var folder = vrResourceStruct.AllSubFolderListMap[contentName].FirstOrDefault(f => f.Name == contentName);
            return folder != null ? Path.Combine(folder.FolderPath, folder1, folder2) : string.Empty;
        }
    }
}