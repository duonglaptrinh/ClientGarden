using System;
using TWT.Model;

namespace TWT.Networking
{
    [Serializable]
    public class GetContentAbsoluteResponse
    {
        public int contentType;
        public string[] absolutePaths;
    }
    
    [Serializable]
    public class GetAllContentDataResponse
    {
        public ContentInfo[] allContents;
    }

    [Serializable]
    public class UpdateVrContentResponse
    {
        public VRContentData contentData;
    }
    
    [Serializable]
    public class UpdateVrContentTitleResponse
    {
        public string contentName;
        public VrContentTitle vrContentTitle;
    }

    [Serializable]
    public class UploadFileResponse
    {
        public string absolutePath;
    }

}