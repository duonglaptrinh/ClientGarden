using System;
using TWT.Model;

namespace TWT.Networking
{
    [Serializable]
    public class GetContentAbsoluteRequest
    {
        public string contentName;
        public int contentType;
    }

    [Serializable]
    public class UpdateVrContentRequest
    {
        public string contentName;
        public VRContentData contentData;
    }
    
    [Serializable]
    public class UpdateVrContentTitleRequest
    {
        public string contentName;
        public VrContentTitle vrContentTitle;
    }
}