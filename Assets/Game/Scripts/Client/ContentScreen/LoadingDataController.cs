using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Client;
using TWT.Model;
using TWT.Networking;
using TWT.Networking.Client;
using TWT.Utility;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TWT.Client.ContentScreen
{
    public class LoadingDataController : UiControllerBase<LoadingDataView>
    {
        private RemoteVrResourceLoader loader;

        private RemoteVrResourceLoader Loader => loader != null
            ? loader
            : loader = new RemoteVrResourceLoader(GameContext.IpAddress, GameContext.API_PORT, Application.persistentDataPath, this.gameObject);

        protected override void Initialize()
        {
        }

        public async UniTask LoadContentAndSystemContentAsync(string contentName)
        {
            if(GameContext.IsOffline)
            {
                UpdateProcess(1);
                return;
            }

            float time = Time.time;
            UpdateTimeout().Forget();

            var processValue = new float[2];
            float previous = processValue.Sum();
            var tasks = new[]
            {
                LoadSystemContentAsync(new Progress<float>((v =>
                {
                    processValue[0] = v;
                    if(processValue.Sum() > previous)
                        timeOut = GameContext.DOWNLOAD_CONTENT_TIMEOUT;

                    previous = processValue.Sum();
                    UpdateProcess(processValue.Sum() / 2);
                }))),
                LoadContentAsync(contentName, new Progress<float>((v =>
                {
                    processValue[1] = v;
                    if(processValue.Sum() > previous)
                        timeOut = GameContext.DOWNLOAD_CONTENT_TIMEOUT;

                    previous = processValue.Sum();
                    UpdateProcess(processValue.Sum() / 2);
                }))),
            }.AsEnumerable();

            foreach (var uniTask in tasks)
            {
                await uniTask;
            }

            #if !ADMIN
            await SaveToLocalTitleContent(contentName);
            #endif
            isContentFinish = true;

            DebugExtension.Log("Download time (s): " + (Time.time - time));

            // if(GameContext.IsTeacher || GameContext.IsEditable)
            //     await SaveToLocalTitleContent();
        }

        float timeOut = 10f;
        bool isContentFinish = false;
        bool errorTimeout = false;
        public async UniTask UpdateTimeout()
        {
            isContentFinish = false;
            timeOut = GameContext.DOWNLOAD_CONTENT_TIMEOUT;
            errorTimeout = false;
            while(!isContentFinish && enabled)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                DebugExtension.Log("Content Timeout: " + timeOut);
                if(isContentFinish)
                {
                    break;
                } 

                timeOut -= 1f;
                if(timeOut <= 0)
                {
                    errorTimeout = true;
                    PopupRuntimeManager.Instance.ShowPopupOnlyConfirm(
                    "しばらくお待ちください",
                    () =>
                    {
                        SceneConfig.LoadScene(SceneConfig.Scene.TitleScreen);
                    });
                    break;
                }
            }
        }

        private async UniTask LoadContentAsync(string contentName, IProgress<float> progress = null)
        {
            var response = await CheckVersionContentAsync(contentName);
            DebugExtension.Log($"Check content {contentName} version latest {response.LatestVersion} {response.IsLatest}");
            if (response.IsLatest)
            {
                DebugExtension.Log($"Content {contentName} pass version {response.LatestVersion}");
                progress?.Report(1);
                await UniTask.Yield();
            }
            else
            {
                var path = contentName != VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT
                    ? VrDomeAssetResourceNameDefine.GetContentFolderPath(Loader.RootPathToSave, contentName)
                    : VrDomeAssetResourceNameDefine.GetSystemFolderPath(Loader.RootPathToSave);
                if (Directory.Exists(path))
                {
                    DeleteAllFile(path);
                    DebugExtension.Log($"Remove {contentName} old version success ...");
                }

                DebugExtension.Log($"Downloading {contentName} version {response.LatestVersion} latest...");
                await Loader.LoadContentFromRemote(contentName, progress);
                if (!Game.Client.Utility.GetServerStatus())
                    return;
                PlayerPrefsConstant.SetVersionContent(contentName, response.LatestVersion);
            }
        }

        private async UniTask SaveToLocalTitleContent(string contentName)
        {
            string contentPath = '\\' + Path.Combine(
                                VrDomeAssetResourceNameDefine.VR_DATA_ROOT, 
                                VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT,
                                contentName);
            var path = await Loader.SaveFileFromRequest(Loader.GetUrl(), Path.Combine(contentPath, VrContentFileNameConstant.CONTENT_DATA_TITLE_JSON));
            DebugExtension.Log("Title json was saved to " + path);
            VrContentTitle title = JsonUtility.FromJson<VrContentTitle>(File.ReadAllText(path));
            path = await Loader.SaveFileFromRequest(Loader.GetUrl(), Path.Combine(contentPath, title.icon)); 
            DebugExtension.Log("Icon was saved to " + path);
        }

        //Unused...
        // private async UniTask SaveToLocalTitleContent()
        // {
        //     var contentInfo = GameContext.ContentInfoCurrent;
        //     var contentTitle = contentInfo.contentTitle;

        //     //Save icon
        //     var iconSavePath = Path.Combine(VrDomeAssetResourceNameDefine.GetContentFolderPath(Loader.RootPathToSave, contentInfo.contentName), contentTitle.icon);
        //     Texture2D iconTexture = await Game.Client.Utility.DownloadTextureAsync(contentInfo.iconUrlAbsolutePath, iconSavePath) as Texture2D;
        //     using (FileStream fs = File.Create(iconSavePath))
        //     {
        //         string iconExtension = Path.GetExtension(iconSavePath);
        //         byte[] iconBytes;
        //         if(iconExtension.ToLower().Equals(".jpg")) iconBytes = iconTexture.EncodeToJPG();
        //         else if(iconExtension.ToLower().Equals(".tga")) iconBytes = iconTexture.EncodeToTGA();
        //         else iconBytes = iconTexture.EncodeToPNG(); //png is default

        //         fs.Write(iconBytes, 0, iconBytes.Length);
        //     } 

        //     //Save title json
        //     string contentFilePath = Path.Combine(VrDomeAssetResourceNameDefine.GetContentFolderPath(Loader.RootPathToSave, contentInfo.contentName), VrContentFileNameConstant.CONTENT_DATA_TITLE_JSON);
        //     var strJson = JsonUtility.ToJson(contentTitle);
        //     using (FileStream fs = File.Create(contentFilePath))
        //     {
        //         byte[] info = new System.Text.UTF8Encoding(true).GetBytes(strJson);
        //         fs.Write(info, 0, info.Length);
        //     }
        // }

        private async UniTask LoadSystemContentAsync(IProgress<float> progress = null)
        {
            await LoadContentAsync(VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT, progress);
        }


        private async UniTask<CheckVersionContentResponse> CheckVersionContentAsync(string contentName)
        {
            DebugExtension.Log($"Check {contentName} version...");
            var versionRemote = await GameContext.GetVersionContentAsync(contentName);
            var versionClient = PlayerPrefsConstant.GetVersionContent(contentName);
            return new CheckVersionContentResponse(versionClient == versionRemote, versionRemote);
        }

        private void UpdateProcess(float value)
        {
            View.UpdateProcess(value);
        }

        private void DeleteAllFile(string path)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private class CheckVersionContentResponse
        {
            public bool IsLatest { get; }
            public string LatestVersion { get; }

            public CheckVersionContentResponse(bool isLatest, string latestVersion)
            {
                IsLatest = isLatest;
                LatestVersion = latestVersion;
            }
        }

    }
}