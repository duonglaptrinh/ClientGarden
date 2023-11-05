using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Client.Extension;
using TWT.Model;
using TWT.Networking;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.Networking;

namespace Game.Client
{
    public class LocalResourceLoader : IResourceLoader
    {
        private VrResourceStruct ResourceRoot => VrResourceStruct.CreateInstance(GetPath());

        public virtual string RootPath { get; }

        protected virtual string ApiUrl => GameContext.Uri;

        private readonly IResourceLoader resourceLoader = new ResourceLoader();

#if UNITY_IOS
        private static string PreFixedFile => "file://";
#else
        private static string PreFixedFile => GameContext.IsDemo ? string.Empty : "file://";
#endif

        public LocalResourceLoader(string rootPath)
        {
            RootPath = rootPath;
        }

        private string GetPath(string path = "")
        {
            DebugExtension.Log("VrResourceStruct GetPath: " + RootPath);
            DebugExtension.Log("Application.persistentDataPath: " + Application.persistentDataPath);
            //#if UNITY_WEBGL
            //            return path;
            //#else
            return Path.Combine(RootPath, path);
            //#endif
        }

        public string GetContentIconPath(string contentName, string iconName)
        {
            return RootPath + GameContext.AbsolutePathVrContentDataIcon(contentName, iconName);
        }

        private string GetUrl(string absolutePath)
        {
            return $"{ApiUrl}{absolutePath}";
        }

        public async UniTask<List<ContentInfo>> GetListContents()
        {
            DebugExtension.LogError("GameContext.IsDemo: " + GameContext.IsDemo);
            if (GameContext.IsDemo)
            {
                string definePathFile = GetPath("content_list.json");
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                    definePathFile = "file://" + definePathFile;
                string defines = await UnityWebRequest.Get(definePathFile).SendWebRequestAsObservable();
                var allContent = JsonUtility.FromJson<GetAllContentDataResponse>(defines);

                return allContent
                    .allContents
                    .ForEach(x =>
                    {
                        string iconUrlAbsolutePath = GetUrl(GameContext.AbsolutePathVrContentDataIcon(x.contentName, x.contentTitle.icon));
                        x.iconUrlAbsolutePath = iconUrlAbsolutePath;
                    })
                    .ToList();
            }

            if (GameContext.IsOffline)
            {
                List<ContentInfo> result = new List<ContentInfo>();
                string contentFolderPath = GetContentFolder();
                var contentFolders = Directory.GetDirectories(contentFolderPath);
                for (int i = 0; i < contentFolders.Length; ++i)
                {
                    string contentFolder = contentFolders[i];
                    string contentName = Path.GetFileName(contentFolder);
                    string titlePath = Path.Combine(contentFolder, VrContentFileNameConstant.CONTENT_DATA_TITLE_JSON);
                    string strTitle = File.ReadAllText(titlePath);
                    var title = JsonUtility.FromJson<VrContentTitle>(strTitle);
                    result.Add(new ContentInfo { contentName = contentName, contentTitle = title });
                }

                return result;
            }

            var url = GetUrl("/contents");
            var responseJson = await ObservableUnityWebRequest.GetAsObservable(url);
            var response = JsonUtility.FromJson<GetAllContentDataResponse>(responseJson);
            return response
                .allContents
                .ForEach(x =>
                {
                    string iconUrlAbsolutePath = GetUrl(GameContext.AbsolutePathVrContentDataIcon(x.contentName, x.contentTitle.icon));
                    x.iconUrlAbsolutePath = iconUrlAbsolutePath;
                })
                .ToList();
        }

        public void AddContent(string contentName)
        {
            ResourceRoot.AddNewContent(contentName);
        }

        public void RemoveContent(string contentName)
        {
            throw new NotImplementedException();
        }

        public async UniTask<string> GetVrContentJsonData(string contentDataName)
        {
            if (GameContext.IsDemo)
                return await GetVrContentJsonDataInDemo(contentDataName);

            if (GameContext.IsOffline)
                return GetVrContentJsonDataInOffline(contentDataName);

            var absolutePath = GameContext.AbsolutePathVrDataJson(contentDataName);
            var url = $"{GameContext.Uri}{absolutePath}";
            DebugExtension.LogError("GetVrContentJsonData: " + url);
            var response = await ObservableUnityWebRequest.GetAsObservable(url);
            SaveVrContentDataToLocal(response, contentDataName);
            return response;
        }

        public async UniTask<string> GetVrContentJsonDataAtRuntimeServer(string contentDataName)
        {
            if (GameContext.IsDemo)
                return await GetVrContentJsonDataInDemo(contentDataName);

            if (GameContext.IsOffline)
                return GetVrContentJsonDataInOffline(contentDataName);

            var absolutePath = GameContext.AbsolutePathVrDataJsonTemp(contentDataName);
            var url = $"{GameContext.Uri}{absolutePath}";
            DebugExtension.LogError("GetVrContentJsonData: " + url);
            var response = await ObservableUnityWebRequest.GetAsObservable(url);
            //SaveVrContentDataToLocal(response, contentDataName);
            return response;
        }

        private string GetVrContentJsonDataInOffline(string contentDataName)
        {
            var absolutePath = RootPath + GameContext.AbsolutePathVrDataJson(contentDataName);
            if (File.Exists(absolutePath))
                return File.ReadAllText(absolutePath);
            return string.Empty;
        }

        private async UniTask<string> GetVrContentJsonDataInDemo(string contentDataName)   //get from Streamming Assets
        {
            if (Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.WebGLPlayer)
                return GetVrContentJsonDataInOffline(contentDataName);

            var absolutePath = RootPath + GameContext.AbsolutePathVrDataJson(contentDataName);
            UnityWebRequest webRequest = UnityWebRequest.Get(absolutePath);
            string respone = await webRequest.SendWebRequestAsObservable();
            return respone;
        }

        private async UniTask<Texture2D> GetImageInDemo(string path) //get from Streamming Assets
        {
            Texture2D texture2D = new Texture2D(1, 1);
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_WEBGL)
            var txBytes = await ObservableUnityWebRequest.GetBytesAsObservable(path);
#else
            var txBytes = File.ReadAllBytes(path);
#endif
            texture2D.LoadImage(txBytes as byte[]);
            return texture2D;
        }

        public async UniTask<VRContentData> SaveVrContentDataAsync(VRContentData content)
        {
            var request = JsonUtility.ToJson(new UpdateVrContentRequest()
            {
                contentName = content.content_name,
                contentData = content,
            });
            var url = GetUrl("/adjustContentJson");
            var responseJson = await ObservableUnityWebRequest.PostAsObservable(url, request);
            SaveVrContentDataToLocal(JsonUtility.ToJson(content), content.content_name);
            return JsonUtility.FromJson<UpdateVrContentResponse>(responseJson).contentData;
        }

        public async UniTask<VrContentTitle> SaveVrContentTitleAsync(string contentName, VrContentTitle contentTitle)
        {
            var request = JsonUtility.ToJson(new UpdateVrContentTitleRequest()
            {
                contentName = contentName,
                vrContentTitle = contentTitle,
            });

            var url = GetUrl("/adjustContentTitle");
            var responseJson = await ObservableUnityWebRequest.PostAsObservable(url, request);
            SaveVrContentTitleToLocal(JsonUtility.ToJson(contentTitle), contentName);
            return JsonUtility.FromJson<UpdateVrContentTitleResponse>(responseJson).vrContentTitle;
        }

        public void SaveVrContentTitleToLocal(string content_title, string content_name)
        {
            var absolutePath = GameContext.AbsolutePathVrContentDataTitle(content_name);
            var savePath = $"{Application.persistentDataPath}{absolutePath}";
            File.WriteAllText(savePath, content_title);
        }

        public void SaveVrContentDataToLocal(string content, string content_name)
        {
            var absolutePath = GameContext.AbsolutePathVrDataJson(content_name);
            var savePath = $"{RootPath}{absolutePath}";
            //DebugExtension.LogError(savePath);
            File.WriteAllText(savePath, content);
        }

        public UniTask<VideoClip> GetVideo(string contentDataName, string fileName)
        {
            return resourceLoader.GetVideo(contentDataName, fileName);
        }

        private static VRContentData ContentDataFromJson(string json)
        {
            return JsonUtility.FromJson<VRContentData>(json);
        }

        private static string ContentDataToJson(VRContentData content)
        {
            return JsonUtility.ToJson(content);
        }

        public UniTask<string> GetVideoUrl(string contentDataName, string fileName, string folderName = VrDomeAssetResourceNameDefine.VIDEO)
        {
            var path = GetPathByName(contentDataName, folderName, fileName);
            return UniTask.FromResult<string>(path);
        }

        public async UniTask<VrVideo360PreviewInfo> GetVideoPreview(string contentDataName,
            string videoNameWithoutExtension,
            string folderName = VrDomeAssetResourceNameDefine.VIDEO_THUMBNAIL)
        {
            var imagePreviewName = VrContentFileNameConstant.GetVideo360InfoImagePreviewName(videoNameWithoutExtension);

            var jsonInfoFileName = VrContentFileNameConstant.GetVideo360InfoJsonName(videoNameWithoutExtension);
            var jsonInfoFilePath = GetPathByName(contentDataName, folderName, jsonInfoFileName);

            var imagePreview = await LoadTextureFromFolderByName(contentDataName, folderName, imagePreviewName);

            var jsonInfo = await ObservableUnityWebRequest.GetAsObservable($"{PreFixedFile}{jsonInfoFilePath}");
            return new VrVideo360PreviewInfo(imagePreview, VrResourceStruct.Video360Detail.FromJson(jsonInfo).length);
        }

        public async UniTask<Texture2D> GetImage(string contentDataName, string imageName,
                                                                string folderName = VrDomeAssetResourceNameDefine.IMAGE)
        {
            return await LoadTextureFromFolderByName(contentDataName, folderName, imageName);
        }

        public UniTask<string> GetJson(string path)
        {
            if (File.Exists(path))
                return UniTask.FromResult<string>(File.ReadAllText(path));
            return UniTask.FromResult<string>(string.Empty);
        }

        public async UniTask<Texture2D> GetImageThumbnail(string contentDataName, string imageName,
                                                                string folderName = VrDomeAssetResourceNameDefine.IMAGE_THUMBNAIL)
        {
            return await LoadTextureFromFolderByName(contentDataName, folderName, imageName);
        }

        public async UniTask<Texture2D> GetVrArrowIcon(string contentDataName, string imageName)
        {
            var from_content = await LoadTextureFromFolderByName(contentDataName,
                VrDomeAssetResourceNameDefine.VR_MOVE_ARROW, imageName);
            if (from_content == null)
                return await LoadTextureFromSystemFolderByName(VrDomeAssetResourceNameDefine.VR_MOVE_ARROW, imageName);
            else
                return from_content;
        }

        public async UniTask<Texture2D> GetVrMarkIcon(string contentDataName, string imageName)
        {
            var from_content =
                await LoadTextureFromFolderByName(contentDataName, VrDomeAssetResourceNameDefine.VR_MARK, imageName);
            if (from_content == null)
                return await LoadTextureFromSystemFolderByName(VrDomeAssetResourceNameDefine.VR_MARK, imageName);
            else
                return from_content;
        }

        public async UniTask<Texture2D> GetVrDocumentImage(string contentDataName, string imageName)
        {
            return await LoadTextureFromFolderByName(contentDataName, VrDomeAssetResourceNameDefine.DOCUMENT,
                imageName);
        }

        public async UniTask<Texture2D> GetVrDocumentThumbnail(string contentDataName, string imageName)
        {
            return await LoadTextureFromFolderByName(contentDataName, VrDomeAssetResourceNameDefine.DOCUMENT_THUMBNAIL,
                imageName);
        }

        public async UniTask<AudioClip> GetVrSound(string contentDataName, string fileName, string folderName = VrDomeAssetResourceNameDefine.VR_SOUND)
        {
            var path = GetPathByName(contentDataName, folderName, fileName);
            return await LoadAudioClipFromFolderByName(path);
        }

        public UniTask<string[]> GetVrSoundUrls(string contentDataName,
                                                string folderName = VrDomeAssetResourceNameDefine.VR_SOUND)
        {
            var folder = ResourceRoot.AllSubFolderListMap[contentDataName]
                .FirstOrDefault(fol => fol.Name == folderName);

            var result = folder.PathFiles.Where(file => VrAssetType.Sound == VRAssetPath.GetTypeAsset(file)).ToArray();
            Array.Sort(result);
            return folder != null
                ? UniTask.FromResult(result)
                : UniTask.FromResult<string[]>(Array.Empty<string>());
        }

        public UniTask<VideoClip[]> GetVideos(string contentDataName,
            string folderName = VrDomeAssetResourceNameDefine.VIDEO)
        {
            return resourceLoader.GetVideos(contentDataName, folderName);
        }

        public UniTask<string[]> GetVideoUrls(string contentDataName,
            string folderName = VrDomeAssetResourceNameDefine.VIDEO)
        {
            var folder = ResourceRoot.AllSubFolderListMap[contentDataName]
                .FirstOrDefault(fol => fol.Name == folderName);

            if (folder != null)
            {
                var result = folder.PathFiles.Where(file => VrAssetType.Video == VRAssetPath.GetTypeAsset(file)).ToArray();
                Array.Sort(result);
                return UniTask.FromResult(result);
            }
            return UniTask.FromResult<string[]>(Array.Empty<string>());
        }

        public UniTask<Texture2D[]> GetImages(string contentDataName,
            string folderName = VrDomeAssetResourceNameDefine.IMAGE)
        {
            return GetTexturesFromFolder(contentDataName, folderName);
        }

        public UniTask<string[]> GetModel3DThumbs(string contentDataName,
             string folderName = VrDomeAssetResourceNameDefine.VR_MODEL)
        {
            var folder = ResourceRoot.AllSubFolderListMap[contentDataName]
                .FirstOrDefault(fol => fol.Name == folderName);

            List<string> result = new List<string>();
            foreach (var item in folder.Folders)
            {
                result.AddRange(item.PathFiles)/*.ToArray()*/;
            }
            Array.Sort(result.ToArray());
            return folder != null
                ? UniTask.FromResult(result.ToArray())
                : UniTask.FromResult(Array.Empty<string>());
        }
        public UniTask<string[]> GetPdfThumbs(string contentDataName,
            string folderName = VrDomeAssetResourceNameDefine.VR_PDF)
        {
            var folder = ResourceRoot.AllSubFolderListMap[contentDataName]
                .FirstOrDefault(fol => fol.Name == folderName);

            var result = folder.PathFiles/*.ToArray()*/;
            Array.Sort(result);
            return folder != null
                ? UniTask.FromResult(result)
                : UniTask.FromResult(Array.Empty<string>());
        }

        public async UniTask<Texture2D[]> GetVrArrows(string contentDataName,
            string folderName = VrDomeAssetResourceNameDefine.VR_MOVE_ARROW)
        {
            var from_content = await GetTexturesFromFolder(contentDataName, folderName);
            var from_system = await GetTexturesFromFolder(VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT, folderName);

            Array.Resize(ref from_content, from_content.Length + from_system.Length);
            Array.Copy(from_system, 0, from_content, from_content.Length - from_system.Length, from_system.Length);

            Array.Sort(from_content, new SortTextureByName());
            return from_content;
        }

        public async UniTask<Texture2D[]> GetVrMarks(string contentDataName,
            string folderName = VrDomeAssetResourceNameDefine.VR_MARK)
        {
            var from_content = await GetTexturesFromFolder(contentDataName, folderName);
            var from_system = await GetTexturesFromFolder(VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT, folderName);

            Array.Resize(ref from_content, from_content.Length + from_system.Length);
            Array.Copy(from_system, 0, from_content, from_content.Length - from_system.Length, from_system.Length);

            Array.Sort(from_content, new SortTextureByName());
            return from_content;
        }

        private async UniTask<Texture2D[]> GetTexturesFromFolder(string contentDataName, string folderName)
        {
            DebugExtension.Log("GetTexturesFromFolder: " + contentDataName + "    " + folderName);
            var folder = ResourceRoot.AllSubFolderListMap[contentDataName]
                .FirstOrDefault(fol => fol.Name == folderName);
            if (folder == null) return Array.Empty<Texture2D>();
            var requests = folder.PathFiles.Select(path =>
            {
                DebugExtension.Log("GetTexturesFromFolder: " + GameContext.Uri + " " + path);
                var newPath = path.Replace(Application.persistentDataPath, "");
                newPath = $"{GameContext.Uri}{newPath}";
                DebugExtension.Log("GetTexturesFromFolder 11111: " + newPath);
                return GetTextureFromLocalFile(newPath);
            }).ToArray();

            var result = await UniTask.WhenAll(requests);
            Array.Sort(result, new SortTextureByName());
            return result;
        }

        private async UniTask<Texture2D> LoadTextureFromFolderByName(string contentData, string folderName,
            string fileName)
        {
            var path = GetPathByName(contentData, folderName, fileName);
            //DebugExtension.Log("A: " + path);
            if (string.IsNullOrEmpty(path))
            {
                fileName = Path.ChangeExtension(fileName, VRAssetPath.BMP);
                path = GetPathByName(contentData, folderName, fileName);
                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }
            }
            if (RootPath.Equals(VrDomeAssetResourceNameDefine.DEMO_ROOT_PATH))
                return await GetImageInDemo(path);

            return await GetTextureFromLocalFile(path);
        }

        private async UniTask<Texture2D> LoadTextureFromSystemFolderByName(string folderName, string fileName)
        {
            var path = GetSystemPathByName(Path.Combine(folderName, fileName));
            if (string.IsNullOrEmpty(path))
            {
                fileName = Path.ChangeExtension(fileName, VRAssetPath.BMP);
                path = GetSystemPathByName(Path.Combine(folderName, fileName));
                if (string.IsNullOrEmpty(path))
                {
                    return null;
                }
            }
            return await GetTextureFromLocalFile(path);
        }

        private static async UniTask<Texture2D> GetTextureFromLocalFile(string path,
            IProgress<float> progress = default)
        {
            Texture2D texture2D = new Texture2D(1, 1);

#if UNITY_WEBGL
            var newPath = path.Replace("\\", "/");
            DebugExtension.Log("aaaaa: " + newPath);

            var txBytes = await ObservableUnityWebRequest.GetBytesAsObservable(newPath, progress: progress);
            texture2D.LoadImage(txBytes as byte[]);
#else
            if (File.Exists(path))
            {
                if (Path.GetExtension(path).Equals(VRAssetPath.BMP, StringComparison.OrdinalIgnoreCase))
                {
                    var newPath = Path.ChangeExtension(path, VRAssetPath.BMP);
                    texture2D = Utility.LoadTextureFromBMPImage(newPath);
                }
                else
                {

                    var newPath = $"{PreFixedFile}{path}";
                    var txBytes = await ObservableUnityWebRequest.GetBytesAsObservable(newPath, progress: progress);
                    texture2D.LoadImage(txBytes as byte[]);
                }
            }
#endif
            var fileName = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(fileName))
                texture2D.name = fileName;
            return texture2D;
        }

        private async UniTask<AudioClip> LoadAudioClipFromFolderByName(string fileName)
        {
            var newPath = $"{PreFixedFile}{fileName}";
            var extension = Path.GetExtension(newPath);
#if !UNITY_WEBGL
            if (extension.Equals(VRAssetPath.MP3, StringComparison.OrdinalIgnoreCase))
            {
#if ADMIN
                var mp3_data = await ObservableUnityWebRequest.GetBytesAsObservable(newPath);
                return NAudioPlayer.FromMp3Data(mp3_data.ToArray());
#else
                return await ObservableUnityWebRequest.GetAudioClipAsObservable(newPath, AudioType.MPEG);
#endif
            }
            else if (extension.Equals(VRAssetPath.WAV, StringComparison.OrdinalIgnoreCase))
            {
                return await ObservableUnityWebRequest.GetAudioClipAsObservable(newPath, AudioType.WAV);
            }
#endif
            var audioClip = await ObservableUnityWebRequest.GetAudioClipAsObservable(newPath, AudioType.WAV);
            return audioClip;
        }

        private string GetPathByName(string contentData, string folderName, string fileName)
        {
#if !UNITY_WEBGL
            //StreammingAssets or DataPath
            if(RootPath.Equals(VrDomeAssetResourceNameDefine.DEMO_ROOT_PATH))
            {
#endif
            if (
                folderName.Equals(VrDomeAssetResourceNameDefine.VR_VIDEO) ||
                folderName.Equals(VrDomeAssetResourceNameDefine.VR_IMAGE) ||
                folderName.Equals(VrDomeAssetResourceNameDefine.VR_SOUND)
            )
                folderName = Path.Combine(VrDomeAssetResourceNameDefine.VR_MEDIA_FOLDER, folderName);

            else if (
                folderName.Equals(VrDomeAssetResourceNameDefine.VR_MARK) ||
                folderName.Equals(VrDomeAssetResourceNameDefine.VR_MOVE_ARROW)
            )
                folderName = Path.Combine(VrDomeAssetResourceNameDefine.VR_OBJECTS, folderName);
            else if (
                folderName.Equals(VrDomeAssetResourceNameDefine.VR_IMAGE_THUMBNAIL))
                folderName = Path.Combine(VrDomeAssetResourceNameDefine.VR_MEDIA_FOLDER, VrDomeAssetResourceNameDefine.VR_IMAGE, folderName);
            else if (
                folderName.Equals(VrDomeAssetResourceNameDefine.DOCUMENT_VIDEO_THUMBNAIL))
                folderName = Path.Combine(VrDomeAssetResourceNameDefine.DOCUMENT, folderName);
            else if (
                folderName.Equals(VrDomeAssetResourceNameDefine.VR_VIDEO_PREVIEW))
                folderName = Path.Combine(VrDomeAssetResourceNameDefine.VR_MEDIA_FOLDER, VrDomeAssetResourceNameDefine.VR_VIDEO, folderName);
            return GetPathByNameDirectly(contentData, folderName, fileName);
#if !UNITY_WEBGL
        }
#endif

            //Default using directory reader
            var folder = ResourceRoot.AllSubFolderListMap[contentData].FirstOrDefault(f => f.Name == folderName);
            return folder?.PathFiles.FirstOrDefault(p => Path.GetFileName(p).Equals(fileName, StringComparison.OrdinalIgnoreCase));
        }

        private string GetPathByNameDirectly(string contentDataName, string folderName, string fileName)
        {
            return Path.Combine(GetContentFolder(), contentDataName, folderName, fileName);
        }

        private string GetSystemPathByName(string fileName)
        {
#if UNITY_WEBGL
            string url = Path.Combine(ApiUrl, VrDomeAssetResourceNameDefine.VR_DATA_ROOT,
                VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT, VrDomeAssetResourceNameDefine.VR_OBJECTS, fileName);
            DebugExtension.LogError(url);
            return url;
#else
            return Path.Combine(RootPath, VrDomeAssetResourceNameDefine.VR_DATA_ROOT,
                VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT, VrDomeAssetResourceNameDefine.VR_OBJECTS, fileName);
#endif
        }

        private string GetContentFolder()
        {
#if !UNITY_WEBGL
            return Path.Combine(RootPath, VrDomeAssetResourceNameDefine.VR_DATA_ROOT, VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT);
#else
            //DebugExtension.LogError("ApiUrl --------- " + ApiUrl);
            return Path.Combine(ApiUrl, VrDomeAssetResourceNameDefine.VR_DATA_ROOT, VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT);
#endif
        }

        public UniTask<string> GetVrModelUrl(string contentDataName, string modelName, string folderName = VrDomeAssetResourceNameDefine.VR_MODEL)
        {

            var path = GetPathByName(contentDataName, folderName, modelName);
            DebugExtension.Log(path);
#if UNITY_WEBGL
            path = path.Replace("\\", "/");
#endif
            return UniTask.FromResult<string>(path);
        }

        public UniTask<string> GetVrPdfUrl(string contentDataName, string pdfName, string folderName = VrDomeAssetResourceNameDefine.VR_PDF)
        {
            var path = GetPathByName(contentDataName, folderName, pdfName);
            DebugExtension.Log(path);
#if UNITY_WEBGL
            path = path.Replace("\\", "/");
#endif
            return UniTask.FromResult<string>(path);
        }
    }

    #region Comparer Class
    internal class SortTextureByName : IComparer<Texture>
    {
        public int Compare(Texture x, Texture y)
        {
            return x.name.CompareTo(y.name);
        }
    }
    #endregion
}