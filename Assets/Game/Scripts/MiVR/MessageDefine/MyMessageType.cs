

public class MyMessageType
{
    public static short Role = 1;
    public static short RoleMask = 2;

    public static short START_PLAY = 3;

    public const short VR_MARK_SHOW_DOCUMENT = 4;
    
    public const short DRAW_GAME_CREATE_LINE = 5;
    public const short DRAW_GAME_ADD_POINT_LINE = 6;
    public const short DRAW_GAME_FINISH_LINE = 7;
    public const short DRAW_GAME_CLEAR_BOARD = 8;

    public const short VR_ARROW_NEXT_DOME = 9;

    public const short REQUEST_SYNC_STAGE_DRAW_GAME = 10;
    public const short REQUEST_SYNC_STAGE_VR_MARK= 11;
    
    public const short REQUEST_RESET_DOME = 12;

    public const short REQUEST_TO_FLICK_ALL_VR_OBJECTS = 13;
    public const short REQUEST_TO_SWITCH_ALL_VR_OBJECTS_TO_IDLE_STATE = 14;

    public const short REQUEST_SYNC_TIME_VIDEO = 15;
    public const short REQUEST_PLAY_VIDEO = 16;
    public const short REQUEST_PAUSE_VIDEO = 17;
    public const short REQUEST_TRACK_TIME_VIDEO = 18;
    
    public const short REQUEST_GET_CONTENT_NAME_CURRENT = 19;
    public const short REQUEST_CHANGE_CONTENT_NAME = 20;
    
    public const short CHANGE_CAMERA_DEVICE_MESSAGE = 21;
    public const short CHANGE_TIME_CAPTURE_IMAGE = 22;
    public const short REPORT_PROCESS_DOWNLOAD_IMAGE = 23;
    public const short START_CAPTURE_IMAGE = 24;
    public const short GET_CAMERA_STATUS = 25;
    
    public const short ER_BACK_SECOND_VIDEO = 26;
    public const short ER_NEXT_SECOND_VIDEO = 27;
    public const short ER_RESTART_VIDEO = 28;
    public const short ER_CHANGE_VOLUME_VIDEO = 29;
    public const short ER_CONFIRM_JOIN = 30;
    public const short ER_CONFIRM_LEAVE = 31;
    public const short ER_SCROLL_TEXT = 32;

    public const short SCENARIO_SELECT_READY = 33;
    public const short SCENARIO_SELECTED = 34;
    public const short SCENARIO_EXIT = 35;
    public const short SCENARIO_QUIT = 36;

    public const short REQUEST_CHANGE_VOICE_STATUS = 37;
    public const short REQUEST_SHOW_VROBJECT = 39;
    public const short REQUEST_FLICKER_VROBJECT = 40;
    public const short REQUEST_SHOW_AVATAR = 41;
    public const short REQUEST_CHANGE_DOME_SIZE = 42;
    public const short REQUEST_CHANGE_ENABLE_MOVE = 43;

    public const short REQUEST_SYNC_TIME_VR_MEDIA = 44;
    public const short REQUEST_PLAY_VR_MEDIA = 45;
    public const short REQUEST_PAUSE_VR_MEDIA = 46;
    public const short REQUEST_TRACK_TIME_VR_MEDIA = 47;
    public const short REQUEST_SYNC_ALL_VR_MEDIA = 48;
    public const short REQUEST_VOLUME_VR_MEDIA = 49;

    public const short REQUEST_SYNC_VR_PDF = 50;
    public const short REQUEST_SYNC_ANIMATION_VR_MODEL = 51;
    public const short REQUEST_VOLUME_VR_MEDIA_360 = 52;


}