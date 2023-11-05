using System;
using System.Collections.Generic;
using TWT.Model;
using TWT.Networking;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace Game.Client
{
    public interface IResourceLoader
    {
        UniTask<List<ContentInfo>> GetListContents();
        void AddContent(string contentName);
        void RemoveContent(string contentName);
        UniTask<string> GetVrContentJsonData(string contentDataName);
        UniTask<string> GetVrContentJsonDataAtRuntimeServer(string contentDataName);
        UniTask<VRContentData> SaveVrContentDataAsync(VRContentData content);
        UniTask<VrContentTitle> SaveVrContentTitleAsync(string contentName, VrContentTitle contentTitle);
        
        UniTask<VideoClip> GetVideo(string contentDataName, string fileName);
        UniTask<string> GetVideoUrl(string contentDataName, string fileName,
                                                                string folderName = VrDomeAssetResourceNameDefine.VIDEO);
        UniTask<VrVideo360PreviewInfo> GetVideoPreview(string contentDataName, string videoNameWithoutExtension, 
                                                                string folderName = VrDomeAssetResourceNameDefine.VIDEO_THUMBNAIL);
        UniTask<Texture2D> GetImage(string contentDataName, string imageName,
                                                                string folderName = VrDomeAssetResourceNameDefine.IMAGE);
        UniTask<string> GetJson(string path);
        UniTask<Texture2D> GetImageThumbnail(string contentDataName, string imageName,
                                                                string folderName = VrDomeAssetResourceNameDefine.IMAGE_THUMBNAIL);
        UniTask<Texture2D> GetVrArrowIcon(string contentDataName, string imageName);
        UniTask<Texture2D> GetVrMarkIcon(string contentDataName, string imageName);
        UniTask<Texture2D> GetVrDocumentImage(string contentDataName, string imageName);
        UniTask<Texture2D> GetVrDocumentThumbnail(string contentDataName, string imageName);

        UniTask<AudioClip> GetVrSound(string contentDataName, string imageName, string folderName = VrDomeAssetResourceNameDefine.VR_SOUND);

        UniTask<VideoClip[]> GetVideos(string contentDataName, string folderName = VrDomeAssetResourceNameDefine.VIDEO);
        UniTask<string[]> GetVideoUrls(string contentDataName, string folderName = VrDomeAssetResourceNameDefine.VIDEO);
        UniTask<string[]> GetModel3DThumbs(string contentDataName, string folderName = VrDomeAssetResourceNameDefine.VR_MODEL);
        UniTask<string[]> GetPdfThumbs(string contentDataName, string folderName = VrDomeAssetResourceNameDefine.VR_PDF);
        UniTask<Texture2D[]> GetImages(string contentDataName, string folderName = VrDomeAssetResourceNameDefine.IMAGE);
        UniTask<Texture2D[]> GetVrArrows(string contentDataName, string folderName = VrDomeAssetResourceNameDefine.VR_MOVE_ARROW);
        UniTask<Texture2D[]> GetVrMarks(string contentDataName, string folderName = VrDomeAssetResourceNameDefine.VR_MARK);
        UniTask<string> GetVrModelUrl(string contentDataName, string modelName, string folderName = VrDomeAssetResourceNameDefine.VR_MODEL);
        UniTask<string> GetVrPdfUrl(string contentDataName, string pdfName, string folderName = VrDomeAssetResourceNameDefine.VR_PDF);

    }
}