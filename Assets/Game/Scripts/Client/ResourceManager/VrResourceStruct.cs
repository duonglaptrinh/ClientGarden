using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Client.Extension;
using Game.Log;
using TWT.Model;
using TWT.Networking;
using TWT.Utility;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;

namespace Game.Client
{
    public partial class VrResourceStruct
    {
        public static VrResourceStruct CreateInstance(string rootPath, bool validate = false)
        {
            var defaultName = VrDomeAssetResourceNameDefine.VR_DATA_ROOT;
            var resourceStruct = new VrResourceStruct(defaultName, Path.Combine(rootPath, defaultName))
            {
                RootPath = rootPath
            };
            #if ADMIN
            resourceStruct.validateSetting = ValidateSetting.Create(rootPath);
            #endif
            if (validate)
                resourceStruct.Validate();
            Newest = resourceStruct;
            return resourceStruct;
        }

        public static VrResourceStruct Newest {get; private set; }= null; 
        public static bool WasValidated {get; set;} = false;

        public string RootPath { get; private set; }
        public string Name { get; }
        ValidateSetting validateSetting;
        int numberFilesValidating = 0;
        int countFilesValidating = 0;
        public float ValidateProgress => countFilesValidating / (float)numberFilesValidating;

        private VrResourceStruct(string name, string path)
        {
            Name = name;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!Directory.Exists(GetContentFolderPath(path)))
            {
                Directory.CreateDirectory(GetContentFolderPath(path));
            }
            if (!Directory.Exists(GetSystemFolderPath(path)))
            {
                Directory.CreateDirectory(GetSystemFolderPath(path));
            }
            Directory.GetDirectories(GetContentFolderPath(path))
                .Select(p => new { folderName = Path.GetFileNameWithoutExtension(p), path = p })
                .ToList()
                .ForEach(obj =>
                {
                    var subFolderList = AllSubFolderListMap[obj.folderName] = new List<Folder>();
                    ContentDataMap[obj.folderName] = Folder.CreateInstance(obj.folderName, obj.path,
                        folders => subFolderList.AddRange(folders));
                });

            var subFolders = AllSubFolderListMap[VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT] = new List<Folder>();
            ContentDataMap[VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT] = Folder.CreateInstance(
                VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT, GetSystemFolderPath(path),
                folders => subFolders.AddRange(folders));
        }

        public static async UniTask ValidateAsync(string path)
        {
            if(WasValidated)
            {
                return;
            }

            List<ContentAndSomeItsFiles> lstFileNotAvailable = new List<ContentAndSomeItsFiles>();
            var resourceStruct = VrResourceStruct.CreateInstance(path);

            //for counting progress in validation
            resourceStruct.countFilesValidating = 0;
            resourceStruct.numberFilesValidating = GetFilesInValidationProgress(path, string.Empty).Length;

            //validate
            resourceStruct.ValidateSystem(lstFileNotAvailable);
            resourceStruct.ValidateContent(lstFileNotAvailable);
            resourceStruct.ValidateImage360Thumbnail(lstFileNotAvailable);
            resourceStruct.ValidateVrObjects(lstFileNotAvailable);
            await resourceStruct.ValidateVideo360PreviewAsync(lstFileNotAvailable);
            resourceStruct.CompleteValidate();
            ShowListFileNotAvailable(lstFileNotAvailable);
            CopyAllContentToServer();

            WasValidated = true;
        }

        public static async UniTask ValidateSpecialContent(string path, string content_name)
        {
            List<ContentAndSomeItsFiles> lstFileNotAvailable = new List<ContentAndSomeItsFiles>();
            var resourceStruct = VrResourceStruct.CreateInstance(path);
            
             resourceStruct.ClearThumbnails(content_name);

            //for counting progress in validation
            resourceStruct.countFilesValidating = 0;
            resourceStruct.numberFilesValidating = GetFilesInValidationProgress(path, content_name).Length;

            //validate
            resourceStruct.ValidateSystem(lstFileNotAvailable);
            resourceStruct.ValidateContent(lstFileNotAvailable, content_name);
            resourceStruct.ValidateImage360Thumbnail(lstFileNotAvailable, content_name);
            resourceStruct.ValidateVrObjects(lstFileNotAvailable, content_name);
            await resourceStruct.ValidateVideo360PreviewAsync(lstFileNotAvailable, content_name);
            resourceStruct.CompleteValidate();
            ShowListFileNotAvailable(lstFileNotAvailable, content_name);
        }

        private void Validate()
        {
            List<ContentAndSomeItsFiles> lstFileNotAvailable = new List<ContentAndSomeItsFiles>();
            ValidateSystem(lstFileNotAvailable);
            ValidateContent(lstFileNotAvailable);
            ValidateImage360Thumbnail(lstFileNotAvailable);
            ValidateVrObjects(lstFileNotAvailable);
            ShowListFileNotAvailable(lstFileNotAvailable);
            ValidateVideo360PreviewAsync(lstFileNotAvailable).Forget();
        }

        private static string[] GetFilesInValidationProgress(string rootPath, string contentName)
        {
            string[] files = Directory.GetFiles(Path.Combine(rootPath, VrDomeAssetResourceNameDefine.VR_DATA_ROOT, VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT, contentName), "*", SearchOption.AllDirectories);
            string[] extensionUsed = new string[]
            {
                VRAssetPath.JPG, VRAssetPath.JPEG, VRAssetPath.PNG, VRAssetPath.BMP,
                VRAssetPath.MP4, VRAssetPath.MOV,
                VRAssetPath.WAV, VRAssetPath.MP3,
                ".json"
            };
            string[] folderNotUsed = new string[]
            {
                VrDomeAssetResourceNameDefine.IMAGE_THUMBNAIL,
                VrDomeAssetResourceNameDefine.DOCUMENT_THUMBNAIL,
                VrDomeAssetResourceNameDefine.DOCUMENT_VIDEO_THUMBNAIL,
                VrDomeAssetResourceNameDefine.VIDEO_THUMBNAIL,
                VrDomeAssetResourceNameDefine.VR_VIDEO_PREVIEW,
                VrDomeAssetResourceNameDefine.VR_IMAGE_THUMBNAIL
            };

            List<string> result = new List<string>();
            foreach(var file in files)
            {
                //check is preview or thumbnail
                string[] p = file.Split('/', '\\');
                string folder = p[p.Length - 2];
                if(Array.FindIndex(folderNotUsed, e => folder.Equals(e, StringComparison.OrdinalIgnoreCase)) >= 0)
                    continue;

                //check extension
                string extension = Path.GetExtension(file);
                if(Array.FindIndex(extensionUsed, e => extension.Equals(e, StringComparison.OrdinalIgnoreCase)) >= 0)
                    result.Add(file);
            }

            // string log = "";
            // foreach(var e in result)
            //     log += e + '\n';
            // DebugExtension.Log("Validate files: \n" + log);
            return result.ToArray();
        }

        private void CompleteValidate()
        {
            countFilesValidating = numberFilesValidating;
        }

        private void ValidateOnStep(string fileName)
        {
            countFilesValidating = Mathf.Clamp(countFilesValidating + 1, 0, numberFilesValidating);
        }

        //use when user just active app PC - server Admin
        public static void CheckFilePassword()
        {
            string systemPath = CreateSytemDataRoot();
            ValidateSystemFilePassword(systemPath);
        }
        public static void ValidateSystemFilePassword(string systemPath)
        {
            //Create edit_password.json
            string password_file_path = Path.Combine(systemPath, VrDomeAssetResourceNameDefine.PASSWORD_FILENAME);
            VREditPermissionData content;
            if (!File.Exists(password_file_path))
            {
                content = new VREditPermissionData();
                File.WriteAllText(password_file_path, JsonUtility.ToJson(content));
                DebugExtension.Log($"Auto generate password file");
                //DebugExtension.LogError("Create Json file");
            }
            else
            {
                content = JsonUtility.FromJson<VREditPermissionData>(File.ReadAllText(password_file_path));
            }

            content.version = GameContext.APP_VERSION;
            content.date_realtime = Utility.Encrypt(PlayerPrefs.GetInt(PlayerPrefsConstant.REALTIME_ACTIVED).ToString());
            content.date_training = Utility.Encrypt(PlayerPrefs.GetInt(PlayerPrefsConstant.TRAINING_ACTIVED).ToString());

            File.WriteAllText(password_file_path, JsonUtility.ToJson(content));
            //DebugExtension.LogError("update json file");
        }

        //Path file Password use when user just active app PC - server Admin
        static string CreateSytemDataRoot()
        {
            string path = Application.persistentDataPath;
            path = Path.Combine(path, VrDomeAssetResourceNameDefine.VR_DATA_ROOT);
            path = $"{path}/{VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT}";
            if (!Directory.Exists(path))
            {
                //DebugExtension.LogError("Create Folder");
                Directory.CreateDirectory(path);
            }
            return path;
        }
        private void ValidateSystem(List<ContentAndSomeItsFiles> lstFileNotAvailable)
        {
            if (ContentDataMap.ContainsKey(VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT) && !Directory.Exists(ContentDataMap[VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT].FolderPath))
            {
                Directory.CreateDirectory(ContentDataMap[VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT].FolderPath);
            }

            string systemPath = ContentDataMap[VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT].FolderPath;

            //Create edit_password.json
            ValidateSystemFilePassword(systemPath);

            //Validate system-assets
            VrObjectsInfo vrObjectsInfo = new VrObjectsInfo();
            var contentAndItsFiles = GetOrAddContentAndSomeItsFiles(lstFileNotAvailable, VrDomeAssetResourceNameDefine.SYSTEM_DATA_TITLE);

            string folderPath = Path.Combine(systemPath, VrDomeAssetResourceNameDefine.VR_OBJECTS, VrDomeAssetResourceNameDefine.VR_MOVE_ARROW);
            if(Directory.Exists(folderPath))
            {
                vrObjectsInfo.vrArrowsPath = Directory.GetFiles(folderPath);
                ValidateVrArrows(contentAndItsFiles.files, vrObjectsInfo);
            }
                
            folderPath = Path.Combine(systemPath, VrDomeAssetResourceNameDefine.VR_OBJECTS, VrDomeAssetResourceNameDefine.VR_MARK);
            if(Directory.Exists(folderPath))
            {
                vrObjectsInfo.vrMarksPath = Directory.GetFiles(folderPath);
                ValidateVrMarks(contentAndItsFiles.files, vrObjectsInfo);
            }
        }

        private void ValidateContent(List<ContentAndSomeItsFiles> lstFileNotAvailable, string content_name = null)
        {
            AllSubFolderListMap.Keys
                .Where(x => (string.IsNullOrEmpty(content_name) && !x.Equals(content_name)) ||
                                                    (!string.IsNullOrEmpty(content_name) && x.Equals(content_name)))
                .Select(contentName => new
                {
                    contentName,
                    versionPath = Path.Combine(ContentDataMap[contentName].FolderPath,
                        VrContentFileNameConstant.VERSION_JSON),
                    titlePath = Path.Combine(ContentDataMap[contentName].FolderPath,
                        VrContentFileNameConstant.CONTENT_DATA_TITLE_JSON),
                    contentFolderPath = ContentDataMap[contentName].FolderPath
                })
                .ForEach(x =>
                {
                    ValidateContentName(lstFileNotAvailable, x.contentName);

                    if (!File.Exists(x.versionPath))
                    {
                        File.WriteAllText(x.versionPath, JsonUtility.ToJson(VrContentVersion.CreateFirstVersion()));
                        DebugExtension.Log($"Auto generate version for content: {x.contentName} => {x.versionPath}");
                    }

                    ValidateOnStep(x.versionPath);

                    PlayerPrefsConstant.SetVersionContent(x.contentName, JsonUtility.FromJson<VrContentVersion>(File.ReadAllText(x.versionPath)).version);

                    if (x.contentName == VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT) return;

                    string iconName = VrContentFileNameConstant.CONTENT_DATA_ICON;
                    try
                    {
                        var contentTitle = JsonUtility.FromJson<VrContentTitle>(File.ReadAllText(x.titlePath));
                        contentTitle.icon = FindIconName(x.contentFolderPath);
                        File.WriteAllText(x.titlePath, JsonUtility.ToJson(contentTitle));
                        iconName = contentTitle.icon;
                    }
                    catch(Exception e)
                    {
                        DebugExtension.LogError($"[VrResourceStruct] Load VrContentTitle of content {x.contentName} failed. Automatically fixing.");
                        iconName = FindIconName(x.contentFolderPath);
                        File.WriteAllText(x.titlePath, JsonUtility.ToJson(VrContentTitle.CreateInstance(x.contentName, string.Empty, iconName)));
                    }

                    //check icon is exist
                    string iconPath = Path.Combine(x.contentFolderPath, iconName);
                    if(!File.Exists(iconPath))
                    {
                        GetOrAddContentAndSomeItsFiles(lstFileNotAvailable, x.contentName).files.Add(new ValidateHelper.Result
                        {
                            valid = false,
                            error = "is not found",
                            filePath = Path.Combine(x.contentFolderPath, iconPath)
                        });
                    }

                    ValidateOnStep(x.titlePath);
                    ValidateOnStep("content icon file");

                    if (!Directory.Exists(this.GetVrJsonFolder(x.contentName)))
                    {
                        Directory.CreateDirectory(this.GetVrJsonFolder(x.contentName));
                    }

                    ValidateVrJson(x.contentName);
                });
        }

        private static string FindIconName(string contentFolderPath)
        {
            const string partialName = "icon";
            DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(contentFolderPath);
            FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + partialName + "*.*");
            string iconName = VrContentFileNameConstant.CONTENT_DATA_ICON;
            foreach (FileInfo foundFile in filesInDir)
            {
                iconName = Path.GetFileName(foundFile.FullName);
                break;
            }

            return iconName;
        }

        private void ValidateVrJson(string content_name)
        {
            var jsonPath = this.GetVrJsonPath(content_name);

            if (!File.Exists(jsonPath))
            {
                File.WriteAllText(jsonPath, VRContentData.ToJson(new VRContentData(content_name)));
                DebugExtension.Log($"Auto generate vr data json for content: {content_name} => {jsonPath}");
                ValidateOnStep(jsonPath);
                return;
            }
            
            string systemPath = GetSystemFolderPath(Path.Combine(RootPath, VrDomeAssetResourceNameDefine.VR_DATA_ROOT));
            string contentPath = Path.Combine(GetContentFolderPath(Path.Combine(RootPath, VrDomeAssetResourceNameDefine.VR_DATA_ROOT)), content_name);
            DebugExtension.Log("ContentPath: " + contentPath);
            bool CheckFilePath(string folder, string fileName, bool includeSystemRoot)
            {
                bool available = false;
                if(includeSystemRoot)
                    available = File.Exists(Path.Combine(systemPath, folder, fileName));

                if(!available)
                    available = File.Exists(Path.Combine(contentPath, folder, fileName));

                if(!available)
                    DebugExtension.LogError($"Vr-Json {Path.Combine(folder, fileName)} of {content_name} was not found");

                return available;
            }

            //Validate files in vrJson
            VRContentData contentData = VRContentData.FromJson(File.ReadAllText(jsonPath));
            foreach(var vrDome in contentData.vr_dome_list)
            {
                //check 360 image/video
                if(VrAssetType.Video == VRAssetPath.GetTypeAsset(vrDome.d360_file_name))
                    CheckFilePath(VrDomeAssetResourceNameDefine.VIDEO, vrDome.d360_file_name, false);
                else if(VrAssetType.Video == VRAssetPath.GetTypeAsset(vrDome.d360_file_name))
                    CheckFilePath(VrDomeAssetResourceNameDefine.IMAGE, vrDome.d360_file_name, false);
                
                foreach (var vrObject in vrDome.vr_object_list.vr_model_list)
                {
                    CheckFilePath(
                      VrDomeAssetResourceNameDefine.VR_MODEL,
                      vrObject.model_url, false);
                }
            }

            ValidateOnStep(jsonPath);
        }

        private void ValidateImage360Thumbnail(List<ContentAndSomeItsFiles> lstFileNotAvailable, string content_name = null)
        {
            AllSubFolderListMap.Keys
                .Where(x => (string.IsNullOrEmpty(content_name) && !x.Equals(content_name)) ||
                                                    (!string.IsNullOrEmpty(content_name) && x.Equals(content_name)))
                .Select(contentName =>
                {
                    var image360Folder = AllSubFolderListMap[contentName]
                        .FirstOrDefault(f => f.Name == VrDomeAssetResourceNameDefine.IMAGE);
                    return new
                    {
                        contentName,
                        thumbnailPath = image360Folder != null
                            ? Path.Combine(image360Folder.FolderPath, VrDomeAssetResourceNameDefine.IMAGE_THUMBNAIL)
                            : string.Empty,
                        imagesPath = image360Folder != null ? image360Folder.PathFiles : Array.Empty<string>()
                    };
                })
                .ForEach(x =>
                {
                    ValidateVrImages(GetOrAddContentAndSomeItsFiles(lstFileNotAvailable, x.contentName).files, new VrObjectsInfo
                    {
                        vrImagePaths = x.imagesPath,
                        vrImageThumbnailPath = x.thumbnailPath
                    }, true);

                    DebugExtension.Log(
                        $"Auto generate thumbnail for content: {x.contentName} => {x.thumbnailPath}, count {x.imagesPath.Length} images ");
                });
        }

        private async UniTask ValidateVideo360PreviewAsync(List<ContentAndSomeItsFiles> lstFileNotAvailable, string content_name = null)
        {
            var contents = AllSubFolderListMap.Keys
                .Where(x => x != VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT &&
                                                    ((string.IsNullOrEmpty(content_name) && !x.Equals(content_name)) ||
                                                    (!string.IsNullOrEmpty(content_name) && x.Equals(content_name))))
                .Select(contentName =>
                {
                    var video360Folder = AllSubFolderListMap[contentName]
                        .FirstOrDefault(f => f.Name == VrDomeAssetResourceNameDefine.VIDEO);
                    return new
                    {
                        contentName,
                        thumbnailFolderPath = video360Folder != null
                            ? Path.Combine(video360Folder.FolderPath, VrDomeAssetResourceNameDefine.VIDEO_THUMBNAIL)
                            : string.Empty,
                        videosPath = video360Folder != null ? video360Folder.PathFiles : Array.Empty<string>()
                    };
                });

            foreach (var x in contents)
            {
                await ValidateVrVideos(GetOrAddContentAndSomeItsFiles(lstFileNotAvailable, x.contentName).files, new VrObjectsInfo
                {
                    vrVideoThumbnailPath = x.thumbnailFolderPath,
                    vrVideoPaths = x.videosPath
                }, true);
            }
        }

        private void ValidateVrObjects(List<ContentAndSomeItsFiles> lstFileNotAvailable, string content_name = null)
        {
            // List<string> lstFileNotAvailable = new List<string>();
            AllSubFolderListMap.Keys
                .Where(x => x != VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT &&
                                                    ((string.IsNullOrEmpty(content_name) && !x.Equals(content_name)) ||
                                                    (!string.IsNullOrEmpty(content_name) && x.Equals(content_name))))
                .Select(contentName =>
                {
                    var vrMarkFolder = AllSubFolderListMap[contentName]
                        .FirstOrDefault(f => f.Name == VrDomeAssetResourceNameDefine.VR_MARK);
                    var vrArrowFolder = AllSubFolderListMap[contentName]
                        .FirstOrDefault(f => f.Name == VrDomeAssetResourceNameDefine.VR_MOVE_ARROW);
                    var documentFolder = AllSubFolderListMap[contentName]
                        .FirstOrDefault(f => f.Name == VrDomeAssetResourceNameDefine.DOCUMENT);
                    var vrVideoFolder = AllSubFolderListMap[contentName]
                        .FirstOrDefault(f => f.Name == VrDomeAssetResourceNameDefine.VR_VIDEO);
                    var vrImageFolder = AllSubFolderListMap[contentName]
                        .FirstOrDefault(f => f.Name == VrDomeAssetResourceNameDefine.VR_IMAGE);
                    var vrSoundFolder = AllSubFolderListMap[contentName]
                        .FirstOrDefault(f => f.Name == VrDomeAssetResourceNameDefine.VR_SOUND);
                    var vrModelFolder = AllSubFolderListMap[contentName]
                        .FirstOrDefault(f => f.Name == VrDomeAssetResourceNameDefine.VR_MODEL);
                    var vrPdfFolder = AllSubFolderListMap[contentName]
                        .FirstOrDefault(f => f.Name == VrDomeAssetResourceNameDefine.VR_PDF);

                    return new VrObjectsInfo(
                        contentName,
                        documentFolder != null
                            ? Path.Combine(documentFolder.FolderPath, VrDomeAssetResourceNameDefine.DOCUMENT_THUMBNAIL)
                            : string.Empty,
                        documentFolder != null
                            ? Path.Combine(documentFolder.FolderPath, VrDomeAssetResourceNameDefine.DOCUMENT_VIDEO_THUMBNAIL)
                            : string.Empty,
                        documentFolder != null ? documentFolder.PathFiles : Array.Empty<string>(),
                        vrMarkFolder != null ? vrMarkFolder.PathFiles : Array.Empty<string>(),
                        vrArrowFolder != null ? vrArrowFolder.PathFiles : Array.Empty<string>(),
                        vrVideoFolder != null
                            ? Path.Combine(vrVideoFolder.FolderPath, VrDomeAssetResourceNameDefine.VR_VIDEO_PREVIEW)
                            : string.Empty,
                        vrVideoFolder != null ? vrVideoFolder.PathFiles : Array.Empty<string>(),
                        vrImageFolder != null
                            ? Path.Combine(vrImageFolder.FolderPath, VrDomeAssetResourceNameDefine.VR_IMAGE_THUMBNAIL)
                            : string.Empty,
                        vrImageFolder != null ? vrImageFolder.PathFiles : Array.Empty<string>(),
                        vrSoundFolder != null ? vrSoundFolder.PathFiles : Array.Empty<string>(),
                        vrModelFolder != null ? vrModelFolder.PathFiles : Array.Empty<string>(),
                        vrPdfFolder != null ? vrPdfFolder.PathFiles : Array.Empty<string>()
                    );
                })
                .ForEach(x =>
                {
                    var contentAndItsFiles = GetOrAddContentAndSomeItsFiles(lstFileNotAvailable, x.contentName);
                    ValidateVrDocuments(contentAndItsFiles.files, x);
                    ValidateVrArrows(contentAndItsFiles.files, x);
                    ValidateVrMarks(contentAndItsFiles.files, x);
                    ValidateVrImages(contentAndItsFiles.files, x, false);
                    ValidateVrSounds(contentAndItsFiles.files, x);
                    ValidateVrVideos(contentAndItsFiles.files, x, false).Forget();
                    ValidateVrModels(contentAndItsFiles.files, x);
                    ValidateVrPdfs(contentAndItsFiles.files, x);
                });

        }

        private ContentAndSomeItsFiles GetOrAddContentAndSomeItsFiles(List<ContentAndSomeItsFiles> list, string contentName)
        {
            int index = list.FindIndex(n => n.contentName == contentName);
            if(index < 0) 
            {
                list.Add(new ContentAndSomeItsFiles{contentName = contentName});
                index = list.Count - 1;
            }

            return list[index];
        }

        private void ValidateContentName(List<ContentAndSomeItsFiles> lstFileNotAvailable, string content_name)
        {
            if (ValidateHelper.ValidateStringContainPlusCharacter(content_name))
            {
                GetOrAddContentAndSomeItsFiles(lstFileNotAvailable, content_name).files.Add(new ValidateHelper.Result
                {
                    valid = false,
                    error = ValidateHelper.Result.INVALID_FILE_NAME_CONTAIN_PLUS,
                    filePath = StringManager.CONTENT_NAME_INVALID
                });
            }
        }

        private void ValidateVrMarks(List<ValidateHelper.Result> lstFileNotAvailable, VrObjectsInfo x)
        {
            var dict = ValidateHelper.ValidateVrMarks(x, validateSetting);
            foreach(var e in dict)
            {
                string path = e.filePath;
                if(!e.valid)
                {
                    lstFileNotAvailable.Add(e);
                    FileUtility.ChangeFileNameToNotAvailable(path);
                }

                ValidateOnStep(path);
            }
        }

        private void ValidateVrArrows(List<ValidateHelper.Result> lstFileNotAvailable, VrObjectsInfo x)
        {
            var dict = ValidateHelper.ValidateVrArrows(x, validateSetting);
            foreach(var e in dict)
            {
                string path = e.filePath;
                if(!e.valid)
                {
                    lstFileNotAvailable.Add(e);
                    FileUtility.ChangeFileNameToNotAvailable(path);
                }

                ValidateOnStep(path);
            }
        }

        private void ValidateVrDocuments(List<ValidateHelper.Result> lstFileNotAvailable, VrObjectsInfo x)
        {
            var dict = ValidateHelper.ValidateVrDocuments(x, validateSetting);
            foreach(var e in dict)
            {
                string path = e.filePath;
                if(!e.valid)
                {
                    lstFileNotAvailable.Add(e);
                    FileUtility.ChangeFileNameToNotAvailable(path);
                    ValidateOnStep(path);
                    return;
                }

                if(VrAssetType.Video ==  VRAssetPath.GetTypeAsset(path))
                {
                    SaveThumbnailVideoAsync(path, x.documentVideoThumbnailPath).Forget();
                }
                else //image
                {
                    var pathToSave = Path.Combine(x.documentThumbnailPath, e.FileName);
                    if (!File.Exists(pathToSave))
                    {
                        ResizeImageAndSave(path, pathToSave);
                    }
                }

                ValidateOnStep(path);
            }
        }

        private void ValidateVrImages(List<ValidateHelper.Result> lstFileNotAvailable, VrObjectsInfo x, bool is360)
        {
            var dict = ValidateHelper.ValidateVrImages(x, validateSetting, is360);
            foreach(var e in dict)
            {
                string path = e.filePath;
                if(e.valid)
                {
                    var pathToSave = Path.Combine(x.vrImageThumbnailPath, e.FileName);
                    if (!File.Exists(pathToSave))
                    {
                        ResizeImageAndSave(path, pathToSave);
                    }
                }
                else
                {
                    lstFileNotAvailable.Add(e);
                    FileUtility.ChangeFileNameToNotAvailable(path);
                }

                ValidateOnStep(path);
            };
        }

        private async UniTask ValidateVrVideos(List<ValidateHelper.Result> lstFileNotAvailable, VrObjectsInfo x, bool is360)
        {
           // UnityLogCustom.DebugLog( $"List video and model = : {lstFileNotAvailable.Count}");
            var dict = ValidateHelper.ValidateVrVideos(x, validateSetting, is360);
            foreach(var e in dict)
            {
                string path = e.filePath;
                // UnityLogCustom.DebugLog($"path video and md = : {path}");
                if (e.valid)
                {
                    VrAssetType type = VRAssetPath.GetTypeAsset(path);
                    if (type == VrAssetType.Video || type == VrAssetType.Image)
                        await SaveThumbnailVideoAsync(path, x.vrVideoThumbnailPath);
                }
                else
                {
                    lstFileNotAvailable.Add(e);
                    FileUtility.ChangeFileNameToNotAvailable(path);
                }

                ValidateOnStep(path);
            }
        }

        private void ValidateVrSounds(List<ValidateHelper.Result> lstFileNotAvailable, VrObjectsInfo x)
        {
            var dict = ValidateHelper.ValidateVrSounds(x, validateSetting);
            foreach(var e in dict)
            {
                string path = e.filePath;
                if (!e.valid)
                {
                    lstFileNotAvailable.Add(e);
                    FileUtility.ChangeFileNameToNotAvailable(path);
                }

                ValidateOnStep(path);
            }
        }

        private void ValidateVrModels(List<ValidateHelper.Result> lstFileNotAvailable, VrObjectsInfo x)
        {
            var dict = ValidateHelper.ValidateVrModel(x, validateSetting);
            foreach (var e in dict)
            {
                string path = e.filePath;
                if (!e.valid)
                {
                    lstFileNotAvailable.Add(e);
                    FileUtility.ChangeFileNameToNotAvailable(path);
                }

                ValidateOnStep(path);
            }
        }
        private void ValidateVrPdfs(List<ValidateHelper.Result> lstFileNotAvailable, VrObjectsInfo x)
        {
            var dict = ValidateHelper.ValidateVrPdf(x, validateSetting);
            foreach (var e in dict)
            {
                string path = e.filePath;
                if (!e.valid)
                {
                    lstFileNotAvailable.Add(e);
                    FileUtility.ChangeFileNameToNotAvailable(path);
                }

                ValidateOnStep(path);
            }
        }

        private void ClearThumbnails(string contentName)
        {
            string pathToContent = Path.Combine(RootPath, 
                                VrDomeAssetResourceNameDefine.VR_DATA_ROOT,
                                VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT,
                                contentName);
            string[] thumbnailFolders = new string[]
            {
                Path.Combine(pathToContent, VrDomeAssetResourceNameDefine.IMAGE, VrDomeAssetResourceNameDefine.IMAGE_THUMBNAIL),
                Path.Combine(pathToContent, VrDomeAssetResourceNameDefine.VIDEO, VrDomeAssetResourceNameDefine.VIDEO_THUMBNAIL),
                Path.Combine(pathToContent, VrDomeAssetResourceNameDefine.DOCUMENT, VrDomeAssetResourceNameDefine.DOCUMENT_THUMBNAIL),
                Path.Combine(pathToContent, VrDomeAssetResourceNameDefine.DOCUMENT, VrDomeAssetResourceNameDefine.DOCUMENT_VIDEO_THUMBNAIL),
                Path.Combine(pathToContent, VrDomeAssetResourceNameDefine.VR_MEDIA_FOLDER, VrDomeAssetResourceNameDefine.VR_IMAGE, VrDomeAssetResourceNameDefine.VR_IMAGE_THUMBNAIL),
                Path.Combine(pathToContent, VrDomeAssetResourceNameDefine.VR_MEDIA_FOLDER, VrDomeAssetResourceNameDefine.VR_VIDEO, VrDomeAssetResourceNameDefine.VR_VIDEO_PREVIEW)
            };

            foreach(var folder in thumbnailFolders)
            {
                if(!Directory.Exists(folder)) continue;
                
                DebugExtension.Log("Cleared thumbnail path: " + folder);
                Directory.Delete(folder, true);
            }
        }

        private static void ResizeImageAndSave(string imagePath, string pathToSave)
        {
            var texture = new Texture2D(1, 1);

            var extension = Path.GetExtension(imagePath);

            if (extension.Equals(VRAssetPath.BMP, StringComparison.OrdinalIgnoreCase))
            {
                texture = Utility.LoadTextureFromBMPImage(imagePath);
                pathToSave = Path.ChangeExtension(pathToSave, VRAssetPath.PNG);
            }
            else
            {
                var imageBytes = File.ReadAllBytes(imagePath);
                texture.LoadImage(imageBytes);
            }

            var newWidth = 512;
            var newHeight = 512;
            if (texture.width > texture.height)
            {
                newHeight = (int)((texture.height * 1.0f / texture.width) * 512);
            }
            else
            {
                newWidth = (int)((texture.width * 1.0f / texture.height) * 512);
            }

            var newTexture = texture.ScaleTexture(newWidth, newHeight);

            UnityEngine.Object.Destroy(texture);

            File.WriteAllBytes(pathToSave, newTexture.EncodeToPNG());

            UnityEngine.Object.Destroy(newTexture);
        }

        private static RenderTexture renderTextureTemplate;

        private static RenderTexture RenderTextureTemplate => renderTextureTemplate
            ? renderTextureTemplate
            : renderTextureTemplate = Resources.Load<RenderTexture>("UI/RenderTexture/VideoThumbnailTemplate");

        private static async UniTask SaveThumbnailVideoAsync(string videoPath, string pathFolderToSave)
        {
            var nameVideoWithoutExtension = Path.GetFileNameWithoutExtension(videoPath);
            var pathThumbnail = Path.Combine(pathFolderToSave, VrContentFileNameConstant.GetVideo360InfoImagePreviewName(nameVideoWithoutExtension));
            var pathInfoVideoJson = Path.Combine(pathFolderToSave, VrContentFileNameConstant.GetVideo360InfoJsonName(nameVideoWithoutExtension));

            if (File.Exists(pathThumbnail)) return;

            var videoPlayer = new GameObject("VrResourceStruct-ValidateVideo").AddComponent<VideoPlayer>();
            Object.DontDestroyOnLoad(videoPlayer.gameObject);
            videoPlayer.SetDirectAudioVolume(0, 0);
            var renderTexture = new RenderTexture(RenderTextureTemplate);
            videoPlayer.targetTexture = renderTexture;
            videoPlayer.url = videoPath;
            videoPlayer.sendFrameReadyEvents = true;
            videoPlayer.frameReady += (source, frameIdx) =>
            {
                var thumbnail = renderTexture.GetRTPixels();
                File.WriteAllBytes(pathThumbnail, thumbnail.EncodeToPNG());
                Object.Destroy(thumbnail);
            };

            videoPlayer.Play();
            await UniTask.WaitUntil(() => videoPlayer.isPlaying);
            await UniTask.WaitUntil(() => videoPlayer.targetTexture.IsCreated());

            var videoLength = videoPlayer.length;
            var video360Detail = new Video360Detail()
            {
                length = videoLength
            };

            File.WriteAllText(pathInfoVideoJson, video360Detail.ToJson());

            Observable.Timer(TimeSpan.FromSeconds(2f))
                .Subscribe(_ =>
                {
                    renderTexture.Release();
                    Object.Destroy(videoPlayer.targetTexture);
                    Object.Destroy(videoPlayer.gameObject);
                });
        }

        private static void ShowListFileNotAvailable(List<ContentAndSomeItsFiles> lstFileNotAvailable, string content_name = null)
        {
            string folderInvalid = Path.Combine($"{Application.dataPath}/../", "Logs", "invalid_files");
            if(!Directory.Exists(folderInvalid))
            {
                Directory.CreateDirectory(folderInvalid);
            }

            //function add text to file
            void WriteLogListFile(string contentName, string text = "")
            {
                string filePath = $"{folderInvalid}/{contentName}_invalid_files.log";
                if(text == "")
                {
                    if(File.Exists(filePath)) File.Delete(filePath);
                    return;
                }

                using (var sw = File.CreateText(filePath))
                {
                    sw.Write(text);
                }
            }

// #if ADMIN
            //there are not any contents -> remove all invalid files logs
            if(lstFileNotAvailable.Count == 0)
            {
                Directory.Delete(folderInvalid, true);
                Directory.CreateDirectory(folderInvalid);
                return;
            }

            string notifice_file_not_available = StringManager.LIST_NOT_AVAILABLE_FILE;
            
            foreach(var content in lstFileNotAvailable)
            {
                if(content.files.Count == 0) 
                {
                    WriteLogListFile(content.contentName, "");
                    continue;
                }

                string logText = "";
                //add content name string
                notifice_file_not_available = string.Concat(notifice_file_not_available, content.contentName, "\n");

                //add list files is not available
                for (int i = 0, j = 0; i < content.files.Count && j < 10; i++, j++)
                {
                    notifice_file_not_available = string.Concat(notifice_file_not_available, content.files[i].FileName, "\n");
                    logText = string.Concat(logText, content.files[i].filePath + FileUtility.PREFIX_FILE_INVALID_REASON + content.files[i].error, "\n");
                }
                WriteLogListFile(content.contentName, logText);
            }

            if(notifice_file_not_available != StringManager.LIST_NOT_AVAILABLE_FILE)
            {
                PopupRuntimeManager.Instance.ShowPopupOnlyConfirm(notifice_file_not_available);
            }
            
// #endif
        }

        public static void CopyContentToServer(string content_name)
        {
            var path = GameContext.VrResourceRootPathOnServer;

            if (Directory.Exists(path))
            {
                var resourceLocalPath =
                    Path.Combine(Application.persistentDataPath, VrDomeAssetResourceNameDefine.VR_DATA_ROOT,
                                                    VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT, content_name);
                var resourceServerPath = Path.Combine(path, VrDomeAssetResourceNameDefine.VR_DATA_ROOT,
                                                    VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT, content_name);

                FileUtility.DeleteAllFile(resourceLocalPath);

                if (Directory.Exists(resourceLocalPath))
                {
                    Directory.Delete(resourceLocalPath);
                }

                FileUtility.DirectoryCopy(resourceServerPath, resourceLocalPath, true);
            }
        }

        public static void CopyAllContentToServer()
        {
            var path = GameContext.VrResourceRootPathOnServer;
            if (Directory.Exists(path))
            {
                CopyToServer(path, VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT);
                CopyToServer(path, VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT);
            }
        }

        public static void CopyToServer(string path, string folder)
        {
            var localPath =
                    Path.Combine(Application.persistentDataPath, VrDomeAssetResourceNameDefine.VR_DATA_ROOT, folder);
            var serverPath = Path.Combine(path, VrDomeAssetResourceNameDefine.VR_DATA_ROOT, folder);

            FileUtility.DeleteAllFile(localPath);

            if (Directory.Exists(localPath))
            {
                Directory.Delete(localPath);
            }

            FileUtility.DirectoryCopy(serverPath, localPath, true);
        }

        public static string GetSystemFolderPath(string path) =>
            $"{path}/{VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT}";

        public static string GetContentFolderPath(string path) =>
            $"{path}/{VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT}";

        public Dictionary<string, List<Folder>> AllSubFolderListMap { get; } =
            new Dictionary<string, List<Folder>>();

        private Dictionary<string, Folder> ContentDataMap { get; } = new Dictionary<string, Folder>();

        public void AddNewContent(string contentFolderPath)
        {
            DebugExtension.LogError(contentFolderPath);
            if (Directory.Exists(contentFolderPath))
            {
                PopupRuntimeManager.Instance.ShowPopupOnlyConfirm(StringManager.FOLDER_EXIST);
                return;
            }
            string sourceFile = System.IO.Path.Combine(Application.streamingAssetsPath, VrDomeAssetResourceNameDefine.CONTENT_TEMPLATE_NAME);
            FileUtility.DirectoryCopy(sourceFile, contentFolderPath, true);
        }

        public class Folder
        {
            internal static Folder CreateInstance(string name, string path, Action<Folder[]> returnFolders)
            {
                return new Folder(name, path, returnFolders);
            }

            public string Name { get; }
            public string[] PathFiles { get; }
            public string[] PathSubFolder { get; }

            public Folder[] Folders { get; }

            public string FolderPath { get; }

            private Folder(string name, string path, Action<Folder[]> returnFolders)
            {
                Name = name;
                FolderPath = path;
                if (Directory.Exists(path))
                {
                    PathFiles = Directory.GetFiles(path)
                        .Where(f => Path.GetExtension(f) != ".meta" && Path.GetExtension(f) != ".DS_Store").ToArray();
                    PathSubFolder = Directory.GetDirectories(path);
                    Folders = PathSubFolder.Select(p => CreateInstance(Path.GetFileName(p), p, returnFolders))
                        .ToArray();
                    returnFolders?.Invoke(Folders);
                }
            }
        }
    
        public class ContentAndSomeItsFiles
        {
            public string contentName;
            public List<ValidateHelper.Result> files = new List<ValidateHelper.Result>();
        }
    }
}