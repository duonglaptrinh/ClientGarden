using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Game.Client;
using static Game.Client.VrResourceStruct;
using Game.Client.Extension;

public static class ValidateHelper
{
    public class Result
    {
        public string filePath;
        public bool valid;
        public string error;

        public Result() { }
        public Result(bool validSize)
        {
            valid = validSize;
            if (!validSize)
            {
                error = INVALID_SIZE;
                return;
            }
        }

        public string NameWithError => Path.GetFileName(filePath) + " : " + error;
        public string FileName => Path.GetFileName(filePath);


        public const string INVALID_SIZE = "size is invalid";
        public const string INVALID_FORMAT = "format is invalid";
        public const string INVALID_FILE_NAME_LENGTH = "file path is too long (must < 260 characters)";
        public const string INVALID_FILE_NAME_CONTAIN_PLUS = "name contains plus charater";
    }

    const long BYTE_UNIT = 1048576;
    const int MAX_FILE_NAME_LENGHT = 260 - 60; //260 is max, minus 60 for safety
    public static List<Result> ValidateVrArrows(VrObjectsInfo info, ValidateSetting validateSetting)
    {
        return ValidateFiles(info.vrArrowsPath, validateSetting.vr_arrow_max_size);
    }

    public static List<Result> ValidateVrMarks(VrObjectsInfo info, ValidateSetting validateSetting)
    {
        return ValidateFiles(info.vrMarksPath, validateSetting.vr_mark_max_size);
    }

    public static List<Result> ValidateVrDocuments(VrObjectsInfo info, ValidateSetting validateSetting)
    {
        if (string.IsNullOrEmpty(info.documentThumbnailPath)) return new List<Result>();
        if (string.IsNullOrEmpty(info.documentVideoThumbnailPath)) return new List<Result>();
        if (info.documentsPath.IsNullOrEmpty()) return new List<Result>();

        if (!Directory.Exists(info.documentThumbnailPath))
        {
            Directory.CreateDirectory(info.documentThumbnailPath);
        }

        if (!Directory.Exists(info.documentVideoThumbnailPath))
        {
            Directory.CreateDirectory(info.documentVideoThumbnailPath);
        }

        long maxSizeImg = ToByte(validateSetting.document_image_max_size);
        long maxSizeVid = ToByte(validateSetting.document_video_max_size);

        List<Result> result = new List<Result>();
        foreach (var documentPath in info.documentsPath)
        {
            if (!ValidateFileNameLength(documentPath))
            {
                result.Add(new Result { filePath = documentPath, valid = false, error = Result.INVALID_FILE_NAME_LENGTH });
            }
            else if (ValidateStringContainPlusCharacter(documentPath))
            {
                result.Add(new Result { filePath = documentPath, valid = false, error = Result.INVALID_FILE_NAME_CONTAIN_PLUS });
            }
            else if (VrAssetType.Video == VRAssetPath.GetTypeAsset(documentPath))
            {
                result.Add(new Result(FileUtility.ValidateFileSize(documentPath, maxSizeVid)) { filePath = documentPath });
            }
            else if (VrAssetType.Image == VRAssetPath.GetTypeAsset(documentPath))
            {
                result.Add(new Result(FileUtility.ValidateFileSize(documentPath, maxSizeImg)) { filePath = documentPath });
            }
            else
            {
                result.Add(new Result { filePath = documentPath, valid = false, error = Result.INVALID_FORMAT });
            }
        }
        return result;
    }

    public static List<Result> ValidateVrImages(VrObjectsInfo info, ValidateSetting validateSetting, bool is360)
    {
        if (string.IsNullOrEmpty(info.vrImageThumbnailPath)) return new List<Result>();
        if (info.vrImagePaths.IsNullOrEmpty()) return new List<Result>();

        if (!Directory.Exists(info.vrImageThumbnailPath))
        {
            Directory.CreateDirectory(info.vrImageThumbnailPath);
        }

        return ValidateFiles(info.vrImagePaths, VrAssetType.Image, is360 ? validateSetting.vr_360_image_max_size : validateSetting.vr_image_max_size);
    }

    public static List<Result> ValidateVrModel(VrObjectsInfo info, ValidateSetting validateSetting)
    {
        if (info.vrModelPath.IsNullOrEmpty()) return new List<Result>();

        return ValidateFiles(info.vrModelPath, VrAssetType.Model, validateSetting.vr_dont_need_check_size);
    }

    public static List<Result> ValidateVrPdf(VrObjectsInfo info, ValidateSetting validateSetting)
    {
        if (info.vrPdfPath.IsNullOrEmpty()) return new List<Result>();

        return ValidateFiles(info.vrPdfPath, VrAssetType.Pdf, validateSetting.vr_dont_need_check_size);
    }

    public static List<Result> ValidateVrVideos(VrObjectsInfo info, ValidateSetting validateSetting, bool is360)
    {
        if (string.IsNullOrEmpty(info.vrVideoThumbnailPath)) return new List<Result>();
        if (info.vrVideoPaths.IsNullOrEmpty()) return new List<Result>();

        if (!Directory.Exists(info.vrVideoThumbnailPath))
        {
            Directory.CreateDirectory(info.vrVideoThumbnailPath);
        }

        return ValidateFiles(info.vrVideoPaths, VrAssetType.Video, is360 ? validateSetting.vr_360_video_max_size : validateSetting.vr_video_max_size, assetTypeExtension: VrAssetType.Model);
    }

    public static List<Result> ValidateVrSounds(VrObjectsInfo info, ValidateSetting validateSetting)
    {
        return ValidateFiles(info.vrSoundPaths, VrAssetType.Sound, validateSetting.vr_sound_max_size);
    }

    static List<Result> ValidateFiles(string[] filePaths, VrAssetType assetType, float maxSizeInMb, VrAssetType assetTypeExtension = VrAssetType.None)
    {
        List<Result> output = new List<Result>();
        long sizeInByte = ToByte(maxSizeInMb);
        foreach (var path in filePaths)
        {
            Result result = new Result { filePath = path, valid = true };
            if (!ValidateFileNameLength(path))
            {
                result.valid = false;
                result.error = Result.INVALID_FILE_NAME_LENGTH;
            }
            else if (ValidateStringContainPlusCharacter(path))
            {
                result.valid = false;
                result.error = Result.INVALID_FILE_NAME_CONTAIN_PLUS;
            }
            else if (assetType != VRAssetPath.GetTypeAsset(path))
            {
                if (assetTypeExtension == VrAssetType.None)
                {
                    result.valid = false;
                    result.error = Result.INVALID_FORMAT;
                }
                else
                {
                    if (assetTypeExtension != VRAssetPath.GetTypeAsset(path))
                    {
                        result.valid = false;
                        result.error = Result.INVALID_FORMAT;
                    }
                }
            }
            else if (!FileUtility.ValidateFileSize(path, sizeInByte))
            {
                result.valid = false;
                result.error = Result.INVALID_SIZE;
            }

            output.Add(result);
        }
        return output;
    }

    static List<Result> ValidateFiles(string[] filePaths, float maxSizeInMb)
    {
        List<Result> result = new List<Result>();
        long sizeInByte = ToByte(maxSizeInMb);
        foreach (var e in filePaths)
        {
            if (!ValidateFileNameLength(e))
                result.Add(new Result { filePath = e, valid = false, error = Result.INVALID_FILE_NAME_LENGTH });
            else if (ValidateStringContainPlusCharacter(e))
            {
                result.Add(new Result { filePath = e, valid = false, error = Result.INVALID_FILE_NAME_CONTAIN_PLUS });
            }
            else
                result.Add(new Result(FileUtility.ValidateFileSize(e, sizeInByte)) { filePath = e });
        }
        return result;
    }

    static bool ValidateFileNameLength(string filePath)
    {
        return filePath.Length < MAX_FILE_NAME_LENGHT;
    }

    static long ToByte(float inMb)
    {
        return (long)(inMb * (double)BYTE_UNIT); ;
    }

    public static bool ValidateStringContainPlusCharacter(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        return fileInfo.Name.Contains("+");
    }
}



[Serializable]
public class ValidateSetting
{
    //all unit is represented by MB
    public float vr_arrow_max_size = 1;
    public float vr_mark_max_size = 1;

    public float document_image_max_size = 3;
    public float document_video_max_size = 300;

    public float vr_image_max_size = 20;
    public float vr_video_max_size = 300;

    public float vr_360_image_max_size = 20;
    public float vr_360_video_max_size = 2000;

    public float vr_sound_max_size = 100;

    public float vr_model_max_size = 100;
    public float vr_pdf_max_size = 100;

    public float vr_dont_need_check_size = -1;

    //if pathFile = null, it will use default path was setup
    public static ValidateSetting Create(string rootPath)
    {
        string pathFile = Path.Combine(rootPath, VrDomeAssetResourceNameDefine.VR_DATA_ROOT,
                                    VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT, "validate-setting.json");
        if (File.Exists(pathFile))
            return CreateDirectlyWithPath(pathFile);
        else
        {
            var defaultValidate = new ValidateSetting();
            File.WriteAllText(pathFile, JsonUtility.ToJson(defaultValidate));
            return defaultValidate;
        }

    }

    public static ValidateSetting CreateDirectlyWithPath(string filePath)
    {
        return JsonUtility.FromJson<ValidateSetting>(File.ReadAllText(filePath));
    }
}


