using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Game.Client;
using TWT.Model;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;


namespace TWT.Networking.Client
{
    public class RemoteResourceManager /*: IResourceLoader*/
    {
        public string IpAddress { get; }
        public int Port { get; }

        public RemoteResourceManager(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }

        private string GetUrl(string absolutePath)
        {
            return $"http://{IpAddress}:{Port}{absolutePath}";
        }

        // public async UniTask<List<string>> GetListContents()
        // {
        //     var url = GetUrl("/contents");
        //     var responseJson = await ObservableUnityWebRequest.GetAsObservable(url);
        //     var response = JsonUtility.FromJson<GetAllContentDataResponse>(responseJson);
        //     return response.allContents.ToList();
        // }

        public void AddContent(string contentName)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveContent(string contentName)
        {
            throw new System.NotImplementedException();
        }

        public async UniTask<string> GetVrContentJsonData(string contentDataName)
        {
            var response = await GetContentAbsoluteResponse(contentDataName, VrContentType.VrJson);
            var url = GetUrl(response.absolutePaths.First());
            return await ObservableUnityWebRequest.GetAsObservable(url);
        }
        
        public void SaveVrContentData(VRContentData content)
        {
            SaveVrContentDataAsync(content).Forget();
        }

        public async UniTask<VRContentData> SaveVrContentDataAsync(VRContentData content)
        {
            var request = JsonUtility.ToJson(new UpdateVrContentRequest()
            {
                contentName = content.content_name,
                contentData = content,
            });
            DebugExtension.Log(request);
            var responseJson = await ObservableUnityWebRequest.PostAsObservable(GetUrl("/adjustContentJson"), request);
            DebugExtension.Log(responseJson);
            return JsonUtility.FromJson<UpdateVrContentResponse>(responseJson).contentData;
        }

        public async UniTask<VideoClip> GetDomeVideo(string contentDataName, string fileName)
        {
            throw new NotSupportedException();
        }

        public async UniTask<string> GetDomeVideoUrl(string contentDataName, string fileName)
        {
            var response = await GetContentAbsoluteResponse(contentDataName, VrContentType.Movie360);
            var absolutePath = response.absolutePaths.FirstOrDefault(s => Path.GetFileName(s) == fileName);
            return string.IsNullOrEmpty(absolutePath) ? string.Empty : GetUrl(absolutePath);
        }

        public UniTask<Texture2D> GetDomeImage(string contentDataName, string imageName)
        {
            return GetTexture(contentDataName, VrContentType.Image360, imageName);
        }

        public UniTask<Texture2D> GetVrArrowIcon(string contentDataName, string imageName)
        {
            return GetTexture(contentDataName, VrContentType.VrArrow, imageName);
        }

        public UniTask<Texture2D> GetVrMarkIcon(string contentDataName, string imageName)
        {
            return GetTexture(contentDataName, VrContentType.VrMark, imageName);
        }

        public UniTask<Texture2D> GetVrDocumentImage(string contentDataName, string imageName)
        {
            return GetTexture(contentDataName, VrContentType.Document, imageName);
        }

        public UniTask<VideoClip[]> GetVideos(string contentDataName,
            string folderName = VrDomeAssetResourceNameDefine.VIDEO)
        {
            //var request = CreateRequest(contentDataName, VrContentType.Movie360);
            throw new NotSupportedException();
        }

        public async UniTask<string[]> GetVideoUrls(string contentDataName, string folderName = VrDomeAssetResourceNameDefine.VIDEO)
        {
            var response = await GetContentAbsoluteResponse(contentDataName, VrContentType.Movie360);
            return response.absolutePaths
                .Select(GetUrl)
                .ToArray();
        }

        public UniTask<Texture2D[]> GetImages(string contentDataName,
            string folderName = VrDomeAssetResourceNameDefine.IMAGE)
        {
            return GetTextures(contentDataName, VrContentType.Image360);
        }

        public UniTask<Texture2D[]> GetVrArrows(string contentDataName,
            string folderName = VrDomeAssetResourceNameDefine.VR_MOVE_ARROW)
        {
            return GetTextures(contentDataName, VrContentType.VrArrow);
        }

        public UniTask<Texture2D[]> GetVrMarks(string contentDataName,
            string folderName = VrDomeAssetResourceNameDefine.VR_MARK)
        {
            return GetTextures(contentDataName, VrContentType.VrMark);
        }

        public UniTask<Texture2D[]> GetVrDocuments(string contentDataName,
            string folderName = VrDomeAssetResourceNameDefine.DOCUMENT)
        {
            return GetTextures(contentDataName, VrContentType.Document);
        }

        private async UniTask<GetContentAbsoluteResponse> GetContentAbsoluteResponse(GetContentAbsoluteRequest request)
        {
            var url = GetUrl("/getContent");
            var requestJson = JsonUtility.ToJson(request);
            //DebugExtension.Log(requestJson);
            var responseJson = await ObservableUnityWebRequest.PostAsObservable(url, requestJson);
            //DebugExtension.Log(responseJson);
            return JsonUtility.FromJson<GetContentAbsoluteResponse>(responseJson);
        }

        private async UniTask<GetContentAbsoluteResponse> GetContentAbsoluteResponse(string contentName,
            VrContentType vrContentType)
        {
            var request = CreateRequest(contentName, vrContentType);
            return await GetContentAbsoluteResponse(request);
        }

        private static GetContentAbsoluteRequest CreateRequest(string contentName, VrContentType vrContentType)
        {
            return new GetContentAbsoluteRequest()
            {
                contentName = contentName,
                contentType = (int) vrContentType,
            };
        }

        private static async UniTask<Texture2D> GetTextureSafe(string path, IProgress<float> progress = default)
        {
            var tx = await ObservableUnityWebRequest.GetTexture2DAsObservable(path);
            var txBytes = await ObservableUnityWebRequest.GetBytesAsObservable(path, progress: progress);
            var texture2D = new Texture2D(tx.width, tx.height);
            texture2D.LoadImage(txBytes.ToArray());
            var fileName = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(fileName))
                texture2D.name = fileName;
            return texture2D;
        }
        
        private async UniTask<Texture2D> GetTexture(string contentDataName, VrContentType contentType, string fileName, IProgress<float> progress = default)
        {
            var response = await GetContentAbsoluteResponse(contentDataName, contentType);
            var absolutePath = response.absolutePaths.FirstOrDefault(s => Path.GetFileName(s) == fileName);
            return string.IsNullOrEmpty(absolutePath) ? null : await GetTextureSafe(GetUrl(absolutePath));
        }

        private async UniTask<Texture2D[]> GetTextures(string contentDataName, VrContentType contentType,
            IProgress<float> progress = default)
        {
            var response = await GetContentAbsoluteResponse(contentDataName, contentType);
            var request = response.absolutePaths
                .Select(GetUrl)
                .Select(url => GetTextureSafe(url))
                .ToArray();

            return await UniTask.WhenAll(request);
        }
    }
}