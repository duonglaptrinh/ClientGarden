using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Client;
using Game.Client.Extension;
using TWT.Utility;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TWT.Networking.Client
{
    public class RemoteVrResourceLoader
    {
        public string IpAddress { get; }
        public int Port { get; }
        public string RootPathToSave { get; }

        private GameObject ObjectExecute { get; }

        public RemoteVrResourceLoader(string ipAddress, int port, string rootPathToSave, GameObject objectExecute = null)
        {
            IpAddress = ipAddress;
            Port = port;
            RootPathToSave = rootPathToSave;
            ObjectExecute = objectExecute;
        }

        public string GetUrl(string absolutePath = "")
        {
            return $"{GameContext.Uri}{absolutePath}";
        }

        public async UniTask LoadContentFromRemote(string contentName, IProgress<float> progress = null)
        {
            var absolutePaths = await GetAbsolutePathsContentsData(contentName);

            var count = absolutePaths.Count();
            var processValues = new float[count];

            var requests = absolutePaths
                .Select((absolutePath, i) => SaveFileFromRequest(GetUrl(), absolutePath, new Progress<float>(p =>
                {
                    if (!Game.Client.Utility.GetServerStatus())
                        return;
                    processValues[i] = p;
                    progress?.Report(processValues.Sum() / count);
                })));

            foreach (var request in requests)
            {
                if (!Game.Client.Utility.GetServerStatus())
                    return;
                var path = await request;
                DebugExtension.Log($"Save to {path}");
            }

            progress?.Report(1);
        }

        public async UniTask<string[]> GetAbsolutePathsContentsData(string contentName,
            IProgress<float> progress = null)
        {
            var urlList = new List<string>();

            var types = Enum.GetValues(typeof(VrContentType))
                .OfType<VrContentType>();

            var vrContentTypes = types as VrContentType[] ?? types.ToArray();
            var count = vrContentTypes.Count();
            var processValues = new float[count];

            var requests = vrContentTypes
                .Select((type, i) =>
                        GetAbsolutePathsContentData(contentName, type, new Progress<float>(p =>
                        {
                            processValues[i] = p;
                            progress?.Report(processValues.Sum() / count);
                        }
                        ))
                )
                .ToArray();

            var responseJson = await UniTask.WhenAll(requests);
            responseJson.ForEach(urls => urlList.AddRange(urls));
            return urlList.ToArray();
        }

        private async UniTask<string[]> GetAbsolutePathsContentData(string contentName, VrContentType vrContentType,
            IProgress<float> progress)
        {
            var requestJson = JsonUtility.ToJson(new GetContentAbsoluteRequest()
            {
                contentName = contentName,
                contentType = (int)vrContentType,
            });

            DebugExtension.Log($"request {requestJson}");

            var request = PostAsync(GetUrl("/getContent"), requestJson, progress);

            var responseJson = await request;

            DebugExtension.Log($"response {responseJson}");

            return JsonUtility.FromJson<GetContentAbsoluteResponse>(responseJson).absolutePaths;
        }


        public async UniTask<string> SaveFileFromRequest(string baseUrl, string absolutePath, IProgress<float> progress = null)
        {
            var rootPathSave = Application.persistentDataPath;
            var path = Path.Combine(rootPathSave, absolutePath.Substring(1)).Replace(@"\", "/");
            var folderPath = path.Replace($"/{Path.GetFileName(path)}", "");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            if ('\\' != absolutePath.First())
                absolutePath = absolutePath.Insert(0, "\\");
            var url = Path.Combine(baseUrl, absolutePath.Substring(1)).Replace(@"\", "/");
#else
            if ('\\' != absolutePath.First())
                absolutePath = absolutePath.Insert(0, "\\");
            var url = Path.Combine(baseUrl, absolutePath.Substring(1));
#endif
            //DebugExtension.LogError(url);
            try
            {
                float timeout = GameContext.DOWNLOAD_CONTENT_TIMEOUT;
                if (ObjectExecute)
                    await ObservableUnityWebRequest.DownloadFileAsObservable(url, path, progress: progress).TakeUntilDestroy(ObjectExecute);
                else
                    await ObservableUnityWebRequest.DownloadFileAsObservable(url, path, progress: progress);
            }
            catch (Exception ex)
            {
                throw;
            }

            return path;
        }

        private static async UniTask<string> PostAsync(string uri, string payload, IProgress<float> progress = null)
        {
            return await ObservableUnityWebRequest.PostAsObservable(uri, payload, progress: progress);
        }

        private static async UniTask<IEnumerable<byte>> GetAsync(string uri, IProgress<float> progress = null)
        {
            return await ObservableUnityWebRequest.GetBytesAsObservable(uri, progress: progress);
        }

    }
}