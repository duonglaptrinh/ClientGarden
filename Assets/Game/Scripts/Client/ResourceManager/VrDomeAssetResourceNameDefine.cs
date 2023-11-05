using System.IO;

namespace Game.Client
{
    public static class VrDomeAssetResourceNameDefine
    {
        public const string VR_DATA_ROOT = "vr-data_root";
        public const string CONTENT_DATA_ROOT = "contents-data_root";
        public const string IMAGE = "360-image";
        public const string VIDEO = "360-movie";
        public const string DOCUMENT = "vr-documents";
        public const string VR_OBJECTS = "vr-objects";
        public const string VR_MARK = "vr-mark";
        public const string VR_MOVE_ARROW = "vr-move-arrow";
        public const string VR_MEDIA_FOLDER = "vr-media";
        public const string VR_IMAGE = "vr-image";
        public const string VR_VIDEO = "vr-video";
        public const string VR_SOUND = "vr-sound";
        public const string VR_JSON = "vr-json";
        public const string VR_MODEL = "vr-model";
        public const string VR_PDF = "vr-pdf";
        public const string VR_JSON_FILENAME = "vr-data.json";

        public const string SYSTEM_DATA_ROOT = "system-data_root";
        public const string SYSTEM_DATA_TITLE = "system-assets";
        public const string PASSWORD_FILENAME = "edit_password.json";
        public const string CONFIG_SERVER_FILENAME = "config_server.json";

        public const string IMAGE_THUMBNAIL = "thumbnail";
        public const string DOCUMENT_THUMBNAIL = "document-thumbnail";
        public const string DOCUMENT_VIDEO_THUMBNAIL = "document-video-thumbnail";
        public const string VIDEO_THUMBNAIL = "360-movie-preview";
        public const string VR_VIDEO_PREVIEW = "vr-video-preview";
        public const string VR_IMAGE_THUMBNAIL = "vr-image-thumbnail";
        public const string VR_MODEL_THUMBNAIL = "vr-model-thumbnail";
        public const string VR_PDF_THUMBNAIL = "vr-pdf-thumbnail";

        public const string CONTENT_TEMPLATE_NAME = "NewContent";
        public static string DEMO_ROOT_PATH => Path.Combine(UnityEngine.Application.streamingAssetsPath, "Demo");

        public static string GetSystemFolderPath(string rootPath) =>
            Path.Combine(rootPath, $"{VR_DATA_ROOT}/{SYSTEM_DATA_ROOT}");

        public static string GetContentFolderPath(string rootPath, string contentName) =>
            Path.Combine(rootPath, $"{VR_DATA_ROOT}/{CONTENT_DATA_ROOT}/{contentName}");
    }
}