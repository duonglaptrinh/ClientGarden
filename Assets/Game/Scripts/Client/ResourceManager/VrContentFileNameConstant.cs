namespace Game.Client
{
    public static class VrContentFileNameConstant
    {
        public const string VERSION_JSON = "version.json";
        public const string VR_DATA_JSON = "vr-data.json";
        public const string VR_DATA_JSON_TEMP = "vr-data-temp.json";
        public const string CONTENT_DATA_TITLE_JSON = "title.json";
        public const string CONTENT_DATA_ICON = "icon.png";

        public static string GetVideo360InfoImagePreviewName(string videoNameWithoutExtension)
        {
            return $"{videoNameWithoutExtension}.png";
        }
        
        public static string GetVideo360InfoJsonName(string videoNameWithoutExtension)
        {
            return $"{videoNameWithoutExtension}-info.json";
        }
    }
}