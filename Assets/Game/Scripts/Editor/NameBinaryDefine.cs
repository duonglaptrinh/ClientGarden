namespace Game.Build
{
    public static class NameBinaryDefine
    {
        public const string ADMIN_SERVER = "Server";
        public const string GAME_ROOM_SERVER = "Roomserver";
        public const string OCULUS_GO = "app_Go";
        public const string OCULUS_QUEST = "app_Quest";
        public const string OCULUS_PICO = "app_Pico";
        public const string OCULUS_TABLET = "app_tablet";
        public const string BRIDGER = "app-bridge";

        public static string AdminServerExe = $"{ADMIN_SERVER}.exe";
        public static string GameRoomServerExe = $"{GAME_ROOM_SERVER}.exe";
        public static string OculusGoApk = $"{OCULUS_GO}.apk";
        public static string OculusQuestApk = $"{OCULUS_QUEST}.apk";
        public static string PicoApk = $"{OCULUS_PICO}.apk";
        public static string TabletApk = $"{OCULUS_TABLET}.apk";
        public static string BridgerExe = $"{BRIDGER}.exe";
    }
}