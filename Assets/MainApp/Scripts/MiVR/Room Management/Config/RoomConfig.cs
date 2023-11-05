namespace jp.co.mirabo.Application.RoomManagement
{
    public class RoomConfig
    {
        public class Room
        {
            public static string chatRoomDomain = "ws://vr-wine-bar.mirabo.tech:2567";
            public static string mainRoomDomain => GameContext.LinkServerSyncRoom;
        }

        public static class SendMessageEvent
        {
            // CHAT TEXT
            public const string SEND_TEXT_MESSAGE = "SEND_TEXT_MESSAGE";

            // WEBRTC VOICE CHAT
            public const string WEBRTC_PUBLISH = "WEBRTC_PUBLISH";
            public const string WEBRTC_SUBSCRIBE = "WEBRTC_SUBSCRIBE";
            public const string WEBRTC_SDP_ANSWER = "WEBRTC_SDP_ANSWER";
            public const string WEBRTC_ICE_UPDATE = "WEBRTC_ICE_UPDATE";
            public const string PLAYER_UPDATE_POSITION = "PLAYER_UPDATE_POSITION";
            public const string MUTE_VOICE = "MUTE_VOICE";

            // WEBRTC TRANSLATION
            public const string CHANGE_LANG = "CHANGE_LANG";
            public const string CHANGE_MUTE_TEXT = "CHANGE_MUTE_TEXT";
        }

        public class ResponseMessageEvent
        {
            // ROOM
            public const string SEND_ERROR = "SEND_ERROR";
            public const string ROOMS_INFO = "ROOMS_INFO";
            public const string RESTART_COUNTDOWN = "RESTART_COUNTDOWN";
            public const string RESTART_VIDEO_TIME = "RESTART_VIDEO";

            // CHAT TEXT
            public const string TEXT_MESSAGE = "TEXT_MESSAGE";
            public const string MESSSAGE_SEND_SUCCESS = "MESSAGE_SEND_SUCCESS";
            public const string MESSAGE_SEND_FAILED = "MESSAGE_SEND_FAIL";

            // WEBRTC VOICE CHAT
            public const string VOICE_MUTED = "VOICE_MUTED";

            // WEBRTC TRANSLATION
            public const string WEBRTC_TRANSLATION = "WEBRTC_TRANSLATION";
        }

        public enum MessageEvent
        {
            WEBRTC_PUBLISH,
            WEBRTC_SDP_ANSWER,
            WEBRTC_NEW_PUBLISHER,
            WEBRTC_SUBSCRIBE,
            WEBRTC_SDP_OFFER,
            WEBRTC_ICE_UPDATE,
            PLAYER_UPDATE_POSITION
        }

        public enum SyncEntity
        {
            SYNC_OBJECT,
            SYNC_HOUSE
        }

        public enum SyncMessage
        {
            CREATE_ENTITY,
            UPDATE_ENTITY,
            REMOVE_ENTITY,
            READY,
            FINISH,
            UPDATE_POSITION,
            START
        }

        public enum SyncDataType
        {
            JSON_OBJECT,
            STRING,
            BOOL,
            NUMBER
        }

        public enum EntityAttribute
        {
            PATH,
            POSITION,
            ROTATION,
            SCALE,
            OPACITY,
            INDEX,
            HOUSE_MATERIALS_DETAIL
        }
    }
}