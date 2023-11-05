using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TWT.Model;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Client
{
    public class VrAssetRepository : MonoBehaviour
    {
        private static VrAssetRepository instance = null;
        
        public static VrAssetRepository Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("VrAssetRepository").AddComponent<VrAssetRepository>();
                    //instance.resourceLoader = new LocalResourceLoader();
                    DontDestroyOnLoad(instance.gameObject);
                }

                return instance;
            }
        }

        private VRContentData VrContentData => GameContext.ContentDataCurrent;
        private string VrContentDataName => GameContext.ContentDataCurrent.content_name;

        private IResourceLoader resourceLoader;

        private Dictionary<string, string> UrlsVideoCache { get; } = new Dictionary<string, string>();
        private Texture2D[] Images360Cache { get; set; } 
        private Texture2D[] ImagesArrowCache { get;set; } 
        private Texture2D[] ImagesMarkCache { get;set; }
        private Texture2D[] ImagesDocumentCache { get;set; }

        public async UniTask LoadData()
        {
            //var urlsVideo = await resourceLoader.GetVideos(VrContentDataName);
            //foreach (var url in urlsVideo)
            //{
            //    UrlsVideoCache[Path.GetFileName(url)] = url;
            //}
            
            Images360Cache = await resourceLoader.GetImages(VrContentDataName);
            ImagesArrowCache = await resourceLoader.GetVrArrows(VrContentDataName);
            ImagesMarkCache = await resourceLoader.GetVrMarks(VrContentDataName);
            ImagesDocumentCache = await resourceLoader.GetImages(VrContentDataName, VrDomeAssetResourceNameDefine.DOCUMENT);
        }

        public Texture2D GetAsset(VrContentDataType contentDataType, string fileName)
        {
            switch (contentDataType)
            {
                case VrContentDataType.Json:
                    break;
                case VrContentDataType.Video360:
                    DebugExtension.LogError($"use method GetVideoUrl(string fileName) for load {fileName} instead");
                    break;
                case VrContentDataType.Image360:
                    return Images360Cache.FirstOrDefault(texture2D => texture2D.name == fileName);
                case VrContentDataType.Arrow:
                    return ImagesArrowCache.FirstOrDefault(texture2D => texture2D.name == fileName);
                case VrContentDataType.Mark:
                    return ImagesMarkCache.FirstOrDefault(texture2D => texture2D.name == fileName);
                case VrContentDataType.Document:
                    return ImagesDocumentCache.FirstOrDefault(texture2D => texture2D.name == fileName);
                default:
                    throw new ArgumentOutOfRangeException(nameof(contentDataType), contentDataType, null);
            }

            return null;
        }

        public string GetVideoUrl(string fileName)
        {
            return UrlsVideoCache[fileName];
        }
    }
}