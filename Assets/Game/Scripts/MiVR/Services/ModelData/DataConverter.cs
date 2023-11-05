using SyncRoom.Schemas;
using System.Collections;
using System.Collections.Generic;
using TWT.Model;
using UnityEngine;
using System.Linq;

public static class DataConverter
{
    public static VRContentData ToVRContentData(this SynsDataObject source)
    {
        var vrContentData = new VRContentData();
        vrContentData.content_name = source.content_name;
        vrContentData.vr_dome_list = source.vr_dome_list.items.Select(x => x.Value.ToVRDomeData()).ToArray();

        return vrContentData;
    }

    public static VRDomeData ToVRDomeData(this VRDome source)
    {
        var vrDomeData = new VRDomeData();

        vrDomeData.dome_id = Mathf.RoundToInt(source.dome_id);
        vrDomeData.d360_file_name = source.d360_file_name;
        vrDomeData.modelData = source.modelData.ToModelDataHouseNetwork();
        vrDomeData.d360_file_type = source.d360_file_type;
        vrDomeData.vr_object_list = source.vr_object_list.ToVRObjectList();
        vrDomeData.dome_size = source.dome_size;
        vrDomeData.dome_rotate = source.dome_rotate;
        vrDomeData.d360_video_loop = source.d360_video_loop;
        vrDomeData.tranformCamera = source.tranformCamera.ToVRTransformData();
        vrDomeData.listStartPointData = source.listStartPointData.ToVRListStartPointDataNetwork();
        vrDomeData.id_url = (int) source.id_url;

        return vrDomeData;
    }

    public static VRTransformData ToVRTransformData(this SyncRoom.Schemas.TranformCamera source)
    {
        var vrTransformData = new VRTransformData();

        return vrTransformData;
    }

    public static VRObjectList ToVRObjectList(this SyncRoom.Schemas.VRObject source)
    {
        var vrObjectList = new VRObjectList();
        //vrObjectList.vr_mark_list = new List<VRMarkData>().ToArray();
        //vrObjectList.vr_move_arrow_list = new List<VRArrowData>().ToArray();
        //vrObjectList.vr_video_list = new List<VRVideoData>().ToArray();
        //vrObjectList.vr_sound_list = new List<VRSoundData>().ToArray();
        //vrObjectList.vr_image_list = new List<VRImageData>().ToArray();
        vrObjectList.vr_model_list = source.vr_model_list.items.Select(x => x.Value.ToVRModelData()).ToArray();
        //vrObjectList.vr_pdf_list = new List<VRPdfData>().ToArray();
        return vrObjectList;
    }

    public static VRModelData ToVRModelData(this SyncRoom.Schemas.VRModel source)
    {
        var vrModelData = new VRModelData();

        vrModelData.model_id = Mathf.RoundToInt(source.model_id);
        vrModelData.model_url = source.model_url;
        vrModelData.model_translate = source.model_translate;
        vrModelData.model_rotation = source.model_rotation;
        vrModelData.model_scale = source.model_scale;
        vrModelData.model_default_animation = source.model_default_animation;
        vrModelData.vr_model_transparent_type = Mathf.RoundToInt(source.vr_model_transparent_type);
        vrModelData.listMaterial = source.listMaterial.items.Select(x => x.Value.ToVRMaterialData()).ToList();
        vrModelData.nameTexture = (int)source.nameTexture;
        vrModelData.isOutline = source.isOutline;
        vrModelData.color = source.color;
        vrModelData.isLock = source.isLock;
        vrModelData.sessionId = source.sessionId;
        return vrModelData;
    }

    public static VRMaterialData ToVRMaterialData(this ObjectUnlocate source)
    {
        var vrMaterialData = new VRMaterialData();

        return vrMaterialData;
    }

    public static ModelDataHouseNetwork ToModelDataHouseNetwork(this ModelData source)
    {
        var modelDataHouseNetwork = new ModelDataHouseNetwork();

        modelDataHouseNetwork.indexHouse = Mathf.RoundToInt(source.indexHouse);
        modelDataHouseNetwork.model_translate = source.model_translate;
        modelDataHouseNetwork.model_rotation = source.model_rotation;
        modelDataHouseNetwork.model_scale = source.model_scale;
        modelDataHouseNetwork.ListHouseMaterialData = source.ListHouseMaterialData.items.Select(x => x.Value.ToHouseMaterialData()).ToList();
        modelDataHouseNetwork.Land_Setting_Behide = source.Land_Setting_Behide;
        modelDataHouseNetwork.Land_Setting_FrontOf = source.Land_Setting_FrontOf;
        modelDataHouseNetwork.Land_Setting_Left = source.Land_Setting_Left;
        modelDataHouseNetwork.Land_Setting_Right = source.Land_Setting_Right;

        return modelDataHouseNetwork;
    }

    public static HouseMaterialData ToHouseMaterialData(this SyncRoom.Schemas.HouseMaterialData source)
    {
        var houseMaterialData = new HouseMaterialData();

        houseMaterialData.indexHouse = Mathf.RoundToInt(source.indexHouse);
        houseMaterialData.ListMaterialSet = source.ListMaterialSet.items.Select(x => x.Value.ToMaterialDataNetWorkDetail()).ToList();

        return houseMaterialData;
    }

    public static MaterialDataNetWorkDetail ToMaterialDataNetWorkDetail(this SyncRoom.Schemas.MaterialSet source)
    {
        var materialDataNetworkDetail = new MaterialDataNetWorkDetail();

        materialDataNetworkDetail.indexMaterialSet = Mathf.RoundToInt(source.indexMaterialSet);
        materialDataNetworkDetail.indexMaterialDetail = Mathf.RoundToInt(source.indexMaterialDetail);

        return materialDataNetworkDetail;
    }

    public static VRListStartPointData ToVRListStartPointDataNetwork(this ListStartPointPlayer source)
    {
        var listStartPointData = new VRListStartPointData();

        listStartPointData.indexStartPoint = Mathf.RoundToInt(source.indexStartPoint);
        listStartPointData.listStartPoint = source.listStartPoint.items.Select(x => x.Value.ToVRStartPointData()).ToList();
        return listStartPointData;
    }
    public static VRStartPointData ToVRStartPointData(this SyncRoom.Schemas.StartPointPlayer source)
    {
        var startPointData = new VRStartPointData();

        startPointData.nameView = source.nameView;
        startPointData.id_url = source.id_url;
        startPointData.position = source.position.ToVector3();
        startPointData.localScale = source.localScale.ToVector3();
        startPointData.eulerAngel = source.eulerAngel.ToVector3();
        startPointData.rotation = source.rotation.ToQuaternion();

        return startPointData;
    }
    public static Vector3 ToVector3(this SyncRoom.Schemas.XYZ source)
    {
        var vector3 = new Vector3();

        vector3.x = source.x;
        vector3.y = source.y;
        vector3.z = source.z;

        return vector3;
    }
    public static Quaternion ToQuaternion(this SyncRoom.Schemas.XYZW source)
    {
        var quaternion = new Quaternion();

        quaternion.x = source.x;
        quaternion.y = source.y;
        quaternion.z = source.z;
        quaternion.w = source.w;

        return quaternion;
    }
}
