using UnityEngine;


namespace TWT.Networking
{
    public class VrMarkShowDocumentMessage
    {
        public int MarkId;
        public bool IsShowDocument;
    }

    public class VrArrowNextDomeMessage
    {
        public const string EventKey = "SYNC_VR_ARROW_NEXT_DOME";
        public int DomeId;
    }

    public class CreateLineMessage
    {
        public int Id;

        public int IdParent;

        public int SortingLayer;

        public float WidthLine;

        public Color Color;

        public Vector3 LocalPosition;
    }

    public class AddPointLineMessage
    {
        public int Id;

        public Vector3 Point;
    }

    public class FinishLineMessage
    {
        public int Id;
    }

    public class ClearBroadMessage
    {

    }

    public class SyncTimeVideoMessage
    {
        public double time;
    }

    public class ChangeVoiceStatusMessage
    {
        public bool status;
    }

    public class ChangeMoveMode
    {
        public bool status;
    }

    public class DisableAllClient
    {

    }

    public class ChangeStatusMessage
    {
        public bool status;
    }

    public class ChangeDomeSizeMessage
    {
        public double size;
    }

    public class ChangeCameraDeviceMessage
    {
        public string CameraId;
    }
    public class UpdateDataJsonVRContentData
    {
        public string json;
    }

    public class ChangeTimeCaptureImage
    {
        public float TimeInterval;
    }

    public class StartCaptureImage
    {
        public bool IsStart;
    }

    public class ReportProcessDownloadImage
    {
        public int PlayerId;
        public float ProcessValue;
    }

    public class GetCameraStatus
    {

    }

    public class SyncStatusVRMediaMessage
    {
        public double time;
        public int id;
        public int type;
        public bool status;
    }

    public class SyncStatusVRPdfMessage
    {
        public int id;
        public double scrollValue;
        public bool status;
    }

    public class SyncAnimationModel3DMessage
    {
        public int id;
        public string nameAnimation;
    }

    public class SyncAllVrMediaMessage
    {

    }

    public class PlayVideoMessage
    {
        public bool isPlay;
    }

    public class TrackTimeVRMediaMessage
    {
        public double time;
        public int id;
        public int type;
        public bool status;
    }

    public class SyncStageVrMarkMessage
    {

    }

    public class ResetDomeMessage
    {

    }

    public class ChangeEnableMoveMessage
    {
        public bool status;
    }

    public class FlickerAllVrObjectMessage
    {
        public bool status;
    }

    public class AllVrObjectStageMessage
    {
        public bool status;
    }

    public class ShowAvatarStageMessage
    {
        public bool status;
    }

    public class SyncStateDrawGameMessage
    {

    }

    public class SyncStateStageVrMarkMessage
    {

    }

    public class SyncCreateArrowMessage
    {
        public int vr_move_arrow_id;
        public string vr_move_arrow_file_name;
        public int vr_move_arrow_transparent_type;
        public string vr_move_arrow_translate;
        public string vr_move_arrow_rotation;
        public int vr_move_arrow_to_dome_id;
        public float vr_scale;
    }

    public class SyncCreateVrObjectMessage
    {
        public const string EventKey = "CREATE_VR_OBJECT";

        public int idDome;
        public int model_id;
        public string model_url;
        public string model_translate;
        public string model_rotation;
        public string model_scale;
        public string model_default_animation;
        public int vr_model_transparent_type;
        public string index_color;
        public int nameTexture;
        public bool isOutline;
        public string color;
        public bool isLock;
        public string sessionId;
    }

    public class SyncDeleteVrObjectMessage
    {
        public const string EventKey = "DELETE_VR_OBJECT";

        public int idDome;
        public ObjectType type;
        public int id;
    }

    public class SyncTranformVrObjectMessage
    {
        public const string EventKey = "SYNC_TRANSFORM_VR_OBJECT";

        public int idDome;
        public ObjectType type;
        public int id;
        public string translate;
        public string rotation;
    }

    public class SyncNewDomeForArrowMessage
    {
        public const string EventKey = "SYNC_NEW_DOME_FOR_ARROW";
        public int id;
        public int dome_id;
        public string json;
    }
    public class SyncScaleVrObjectMessage
    {
        public int idDome;
        public ObjectType type;
        public int id;
        public float scale;
    }

    public class SyncElementScaleVrObjectMessage
    {
        public const string EventKey = "SYNC_ELEMENT_SCALE_VR_OBJECT";

        public int idDome;
        public ObjectType type;
        public int id;
        public float scale;
        public int index;
        public string localScale;
    }

    public class SyncMaterialModelMessage
    {
        public const string EventKey = "SYNC_MATERIAL_MODEL";

        public int idDome;
        public int id;
        /// <summary>
        /// from 8-8 is not use
        /// </summary>
        public string nameObjRenderer;
        public int nameTexture;
        /// <summary>
        /// from 8-8 is not use
        /// </summary>
        public int indexTextureLocal;
        /// <summary>
        /// from 8-8 is not use
        /// </summary>
        //public Color32 colorRender;
    }

    public class SyncMaterialHouseMessage
    {
        public const string EventKey = "SYNC_MATERIAL_HOUSE";

        public bool isCreateEnity;
        public bool isUpdateHouse;
        public int idDome;
        public int indexHouse;
        public int indexMaterialSet;
        public int indexMaterialDetail;
    }

    public class SaveJsonMessage
    {
        public const string EventKey = "SAVE_JSON";
        public int idDome;
        public bool isNeedUpdateTransform;
        public string translate;
        public string rotation;
        public string scale;
        public int id_url;
    }

    public class DeleteDomePlantMessage
    {
        public const string EventKey = "DELETE_DOME_PLANT";
        public int idDome;
    }

    public class SaveNewPlanJsonMessage
    {
        public const string EventKey = "SAVE_PLAN_JSON";
        public int idDome;
        public string json;
    }
    public class ApplyPlanTemplateMessage
    {
        public const string EventKey = "APPLY_PLAN_TEMPLATE_JSON";
        public int idDome;
        public string json;
    }
    public class SyncLandHouseMessage
    {
        public const string EventKey = "SYNC_LAND_HOUSE_JSON";
        public int idDome;
        public string json;
    }
    public class SyncStartPointMessage
    {
        public const string EventKey = "SYNC_START_POINT_JSON";
        public int idDome;
        public int type; //add, update, delete ----> EStartPoint
        public string json;
        public int indexInList; //only use for update, Delete and view
    }
    public enum EStartPoint
    {
        Add, Update, Delete, View
    }
    public class SyncEditModel
    {
        public const string EventKey = "SYNC_EDIT_MODEL";
        public int idDome;
        public int type;
        public int model_id;
        public string json;
    }
    public enum EditModel
    {
        EditModel
    }
}