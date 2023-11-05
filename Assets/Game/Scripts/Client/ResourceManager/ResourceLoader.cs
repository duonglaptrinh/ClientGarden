using System;
using Game.Client;
using System.Collections.Generic;
using TWT.Model;
using TWT.Networking;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;
using System.IO;

public class ResourceLoader : IResourceLoader
{
    public UniTask<List<ContentInfo>> GetListContents()
    {
        // if (!PlayerPrefs.HasKey("contents"))
        // {
        //     List<string> contents = new List<string>() { "VRDemo-01" };
        //     PlayerPrefs.SetString("contents", ListContentsToString(contents));
        //     return UniTask.FromResult<List<string>>(contents);
        // }
        // else
        //     return UniTask.FromResult<List<string>>(StringContentsToList(PlayerPrefs.GetString("contents")));
        throw new NotImplementedException();
    }

    public void AddContent(string contentName)
    {
        List<string> contents = StringContentsToList(PlayerPrefs.GetString("contents"));
        contents.Add(contentName);
        PlayerPrefs.SetString("contents", ListContentsToString(contents));
    }

    public void RemoveContent(string contentName)
    {
        List<string> contents = StringContentsToList(PlayerPrefs.GetString("contents"));
        contents.Remove(contentName);
        PlayerPrefs.SetString("contents", ListContentsToString(contents));
    }

    public UniTask<string> GetVrContentJsonData(string contentName)
    {
        string jsonData = "";

        if (PlayerPrefs.HasKey(contentName))
        {
            jsonData = PlayerPrefs.GetString(contentName);
        }
        else
        {
            string path = GetContentPath(contentName) + "/vr-json/";
            TextAsset textAsset = Resources.Load(path + "vr-data") as TextAsset;
            jsonData = textAsset.text;
        }

        return UniTask.FromResult<string>(jsonData);
    }
    public UniTask<string> GetVrContentJsonDataAtRuntimeServer(string contentName)
    {
        string jsonData = "";

        if (PlayerPrefs.HasKey(contentName))
        {
            jsonData = PlayerPrefs.GetString(contentName);
        }
        else
        {
            string path = GetContentPath(contentName) + "/vr-json/";
            TextAsset textAsset = Resources.Load(path + VrContentFileNameConstant.VR_DATA_JSON_TEMP) as TextAsset;
            jsonData = textAsset.text;
        }

        return UniTask.FromResult<string>(jsonData);
    }

    public UniTask<VRContentData> SaveVrContentDataAsync(VRContentData content)
    {
        SaveVrContentData(content);
        return UniTask.FromResult(content);
    }

    public UniTask<VrContentTitle> SaveVrContentTitleAsync(string contentName, VrContentTitle contentTitle)
    {
        throw new System.NotImplementedException();
    }

    public UniTask<string> GetVideoUrl(string contentDataName, string fileName,
                                                                string folderName = VrDomeAssetResourceNameDefine.VIDEO_THUMBNAIL)
    {
        throw new System.NotImplementedException();
    }

    public UniTask<Texture2D> GetImage(string contentName, string domeImageName,
                                    string folderName = VrDomeAssetResourceNameDefine.IMAGE)
    {
        string path = Path.Combine(GetContentPath(contentName), folderName);
        Texture2D texture = Resources.Load(path + CutExtensionFileName(domeImageName)) as Texture2D;
        return UniTask.FromResult<Texture2D>(texture);
    }
    public UniTask<string> GetJson(string path)
    {
        throw new NotImplementedException();
    }

    public UniTask<Texture2D> GetImageThumbnail(string contentDataName, string imageName,
                                    string folderName = VrDomeAssetResourceNameDefine.IMAGE_THUMBNAIL)
    {
        throw new NotImplementedException();
    }

    public UniTask<VideoClip> GetVideo(string contentName, string domeVideoName)
    {
        string path = GetContentPath(contentName) + "/360-movie/";
        VideoClip clip = Resources.Load(path + CutExtensionFileName(domeVideoName)) as VideoClip;
        return UniTask.FromResult<VideoClip>(clip);
    }

    public UniTask<Texture2D> GetVrArrowIcon(string contentName, string arrowName)
    {
        string path = GetContentPath(contentName) + "/vr-objects/vr-move-arrow/";
        Texture2D texture = Resources.Load(path + CutExtensionFileName(arrowName)) as Texture2D;
        return UniTask.FromResult<Texture2D>(texture);
    }

    public UniTask<Texture2D> GetVrMarkIcon(string contentName, string markName)
    {
        string path = GetContentPath(contentName) + "/vr-objects/vr-mark/";
        Texture2D texture = Resources.Load(path + CutExtensionFileName(markName)) as Texture2D;
        return UniTask.FromResult<Texture2D>(texture);
    }

    public UniTask<Texture2D> GetVrDocumentImage(string contentName, string documentName)
    {
        string path = GetContentPath(contentName) + "/vr-documents/";
        Texture2D texture = Resources.Load(path + CutExtensionFileName(documentName)) as Texture2D;
        return UniTask.FromResult<Texture2D>(texture);
    }

    public string GetContentPath(string contentName)
    {
        return "vr-data_root/contents-data_root/" + contentName;
    }

    public string CutExtensionFileName(string fileName)
    {
        return fileName.Split('.')[0];
    }

    private List<string> StringContentsToList(string contents)
    {
        string[] split = contents.Split(',');
        return new List<string>(split);
    }

    private string ListContentsToString(List<string> content)
    {
        string result = "";
        for (int i = 0; i < content.Count; i++)
        {
            if (i < content.Count - 1)
                result += content[i] + ",";
            else
                result += content[i];
        }
        return result;
    }

    public void SaveVrContentData(VRContentData content)
    {
        string contentJson = JsonUtility.ToJson(content);
        DebugExtension.Log("Saved contentTitle: " + contentJson);
        PlayerPrefs.SetString(content.content_name, contentJson);
    }

    public UniTask<VideoClip[]> GetVideos(string contentName, string folderName = "360-movie")
    {
        List<VideoClip> result = new List<VideoClip>();
        string path = GetContentPath(contentName) + "/360-movie/";
        Object[] objects = Resources.LoadAll(path, typeof(VideoClip));
        foreach (Object o in objects)
        {
            result.Add((VideoClip)o);
        }
        return UniTask.FromResult<VideoClip[]>(result.ToArray());
    }

    public UniTask<string[]> GetVideoUrls(string contentDataName, string folderName = VrDomeAssetResourceNameDefine.VIDEO)
    {
        throw new System.NotImplementedException();
    }

    public UniTask<Texture2D[]> GetImages(string contentName, string folderName = "360-image")
    {
        List<Texture2D> result = new List<Texture2D>();
        string path = GetContentPath(contentName) + "/360-image/";
        Object[] objects = Resources.LoadAll(path, typeof(Texture2D));
        foreach (Object o in objects)
        {
            result.Add((Texture2D)o);
        }
        return UniTask.FromResult<Texture2D[]>(result.ToArray());
    }

    public UniTask<Texture2D[]> GetImagesThumbnail(string contentDataName, string folderName = VrDomeAssetResourceNameDefine.IMAGE_THUMBNAIL)
    {
        throw new NotImplementedException();
    }

    public UniTask<Texture2D[]> GetVrArrows(string contentName, string folderName = "vr-move-arrow")
    {
        List<Texture2D> result = new List<Texture2D>();
        string path = GetContentPath(contentName) + "/vr-objects/vr-move-arrow/";
        Object[] objects = Resources.LoadAll(path, typeof(Texture2D));
        foreach (Object o in objects)
        {
            result.Add((Texture2D)o);
        }
        return UniTask.FromResult<Texture2D[]>(result.ToArray());
    }

    public UniTask<Texture2D[]> GetVrMarks(string contentName, string folderName = "vr-mark")
    {
        List<Texture2D> result = new List<Texture2D>();
        string path = GetContentPath(contentName) + "/vr-objects/vr-mark/";
        Object[] objects = Resources.LoadAll(path, typeof(Texture2D));
        foreach (Object o in objects)
        {
            result.Add((Texture2D)o);
        }
        return UniTask.FromResult<Texture2D[]>(result.ToArray());
    }

    public UniTask<Texture2D[]> GetVrDocuments(string contentName, string folderName = "vr-documents")
    {
        List<Texture2D> result = new List<Texture2D>();
        string path = GetContentPath(contentName) + "/vr-documents/";
        Object[] objects = Resources.LoadAll(path, typeof(Texture2D));
        foreach (Object o in objects)
        {
            result.Add((Texture2D)o);
        }
        return UniTask.FromResult<Texture2D[]>(result.ToArray());
    }

    public UniTask<Texture2D> GetVrDocumentThumbnail(string contentDataName, string imageName)
    {
        throw new NotImplementedException();
    }

    public UniTask<Texture2D[]> GetVrDocumentsThumbnail(string contentDataName, string folderName = "document_thumbnail")
    {
        throw new NotImplementedException();
    }

    public UniTask<VrVideo360PreviewInfo> GetVrVideoPreview(string contentDataName, string videoNameWithoutExtension)
    {
        throw new NotImplementedException();
    }

    public UniTask<Texture2D[]> GetVrImages(string contentDataName, string folderName = "vr-image")
    {
        throw new NotImplementedException();
    }

    public UniTask<Texture2D[]> GetVrImagesThumbnail(string contentDataName, string folderName = "vr-image-thumbnail")
    {
        throw new NotImplementedException();
    }

    public UniTask<VideoClip[]> GetVrVideos(string contentDataName, string folderName = "vr-video")
    {
        throw new NotImplementedException();
    }

    public UniTask<string[]> GetVrVideoUrls(string contentDataName, string folderName = "vr-video")
    {
        throw new NotImplementedException();
    }

    public UniTask<Texture2D> GetVrImage(string contentDataName, string imageName)
    {
        throw new NotImplementedException();
    }

    public UniTask<VrVideo360PreviewInfo> GetVideoPreview(string contentDataName, string videoNameWithoutExtension, string folderName = "360-movie-preview")
    {
        throw new NotImplementedException();
    }

    public UniTask<AudioClip> GetVrSound(string contentDataName, string imageName, string folderName = "vr-sound")
    {
        throw new NotImplementedException();
    }

    public UniTask<string[]> GetVrSoundUrls(string contentDataName, string folderName = "vr-sound")
    {
        throw new NotImplementedException();
    }

    public UniTask<string[]> GetModel3DThumbs(string contentDataName, string folderName = "vr-model")
    {
        throw new NotImplementedException();
    }

    public UniTask<string[]> GetPdfThumbs(string contentDataName, string folderName = "vr-pdf")
    {
        throw new NotImplementedException();
    }

    public UniTask<string> GetVrModelUrl(string contentDataName, string modelName, string folderName = "vr-model")
    {
        throw new NotImplementedException();
    }

    public UniTask<string> GetVrPdfUrl(string contentDataName, string pdfName, string folderName = "vr-pdf")
    {
        throw new NotImplementedException();
    }
}

