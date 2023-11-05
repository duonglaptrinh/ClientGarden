using System;
using UnityEngine;

namespace Game.Client
{
    public partial class VrResourceStruct
    {
        [Serializable]
        public class Video360Detail
        {
            public double length;

            public string ToJson()
            {
                return JsonUtility.ToJson(this);
            }

            public static Video360Detail FromJson(string json)
            {
                return JsonUtility.FromJson<Video360Detail>(json);
            }
        }

        public class VrObjectsInfo
        {
            public string contentName;
            public string documentThumbnailPath;
            public string documentVideoThumbnailPath;
            public string[] documentsPath;
            public string[] vrMarksPath;
            public string[] vrArrowsPath;
            public string vrVideoThumbnailPath;
            public string[] vrVideoPaths;
            public string vrImageThumbnailPath;
            public string[] vrImagePaths;
            public string[] vrSoundPaths;
            public string[] vrModelPath;
            public string[] vrPdfPath;

            public VrObjectsInfo()
            { }

            public VrObjectsInfo(string contentName, string documentThumbnailPath, string documentVideoThumbnailPath, string[] documentsPath, string[] vrMarksPath, string[] vrArrowsPath,
                                string vrVideoThumbnailPath, string[] vrVideoPaths, string vrImageThumbnailPath, string[] vrImagePaths, string[] vrSoundPaths, string[] vrModelPath, string[] vrPdfPath)
            {
                this.contentName = contentName;
                this.documentThumbnailPath = documentThumbnailPath;
                this.documentVideoThumbnailPath = documentVideoThumbnailPath;
                this.documentsPath = documentsPath;
                this.vrArrowsPath = vrArrowsPath;
                this.vrMarksPath = vrMarksPath;
                this.vrVideoThumbnailPath = vrVideoThumbnailPath;
                this.vrVideoPaths = vrVideoPaths;
                this.vrImageThumbnailPath = vrImageThumbnailPath;
                this.vrImagePaths = vrImagePaths;
                this.vrSoundPaths = vrSoundPaths;
                this.vrModelPath = vrModelPath;
                this.vrPdfPath = vrPdfPath;
            }
        }

    }
}