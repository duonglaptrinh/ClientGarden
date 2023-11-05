using Game.Client;
using System;
using System.IO;
using TWT.Model;
using TWT.Utility;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ContentItemUICtrl : MonoBehaviour
{
    [SerializeField]
    private Text contentTitleText;
    [SerializeField]
    private Button deleteButton;

    [SerializeField] private RawImage iconImage;
    [SerializeField] private Text descriptionText;
    [SerializeField] private GameObject selectedImage;
    ContentInfo contentInfo;
    bool isEditable = false;

    public Action<string> OnDeleteContent { get; set; }

    private void Start()
    {
        //SetMode();
    }

    void SetMode()
    {
        isEditable = GameContext.IsEditable;
    }

    public void Initialize(ContentInfo contentInfo)
    {
        this.contentInfo = contentInfo;

        if(GameContext.IsDemo)
        {
            LoadLocalIconInDemo(contentInfo.contentName, contentInfo.contentTitle.icon).Forget();
            contentTitleText.text = contentInfo.contentTitle.title;
            return;
        }

        if(GameContext.IsOffline)
        {
            LoadLocalIconInOffline(contentInfo.contentName, contentInfo.contentTitle.icon);
            contentTitleText.text = contentInfo.contentTitle.title;
            return;
        }

#if !ADMIN
        LoadIconAsync(contentInfo.iconUrlAbsolutePath, contentInfo.contentName, contentInfo.contentTitle.icon).Forget();
        descriptionText.text = contentInfo.contentTitle.description;
#else
        LoadLocalIcon(contentInfo.contentName, contentInfo.contentTitle.icon).Forget();
#endif
        contentTitleText.text = contentInfo.contentTitle.title;
    }

    private async UniTask LoadIconAsync(string url, string contentName, string iconName)
    {
        DebugExtension.Log(url);
        var loader = GameContext.ResourceLoader as LocalResourceLoader;
        var savePath = loader.GetContentIconPath(contentName, iconName);
        DebugExtension.Log(savePath);
        var icon = await Utility.DownloadTextureAsync(url, savePath);
        iconImage.texture = icon;
    }

    private async UniTask LoadLocalIcon(string contentName, string iconName)
    {
        string path = string.Empty; 
        if(contentName == VrDomeAssetResourceNameDefine.SYSTEM_DATA_TITLE || contentName == VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT)
            path = Path.Combine(GameContext.VrResourceRootPathOnServer, VrDomeAssetResourceNameDefine.VR_DATA_ROOT,
                                                VrDomeAssetResourceNameDefine.SYSTEM_DATA_ROOT, iconName);
        else
            path = Path.Combine(GameContext.VrResourceRootPathOnServer, VrDomeAssetResourceNameDefine.VR_DATA_ROOT,
                                            VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT, contentName, iconName);
                                            
        Texture2D texture2D = new Texture2D(1, 1);
        var newPath = $"file://{path}";
        DebugExtension.Log(newPath);
        var txBytes = await ObservableUnityWebRequest.GetBytesAsObservable(newPath);
        texture2D.LoadImage(txBytes as byte[]);
        iconImage.texture = texture2D;
    }

    private void LoadLocalIconInOffline(string contentName, string iconName)
    {
        var loader = GameContext.ResourceLoader as LocalResourceLoader;
        var path = Path.Combine(loader.RootPath, VrDomeAssetResourceNameDefine.VR_DATA_ROOT,
                                            VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT, contentName, iconName);
        Texture2D texture2D = new Texture2D(1, 1);
        if(!File.Exists(path)) return;
        var txBytes = File.ReadAllBytes(path);
        texture2D.LoadImage(txBytes as byte[]);
        iconImage.texture = texture2D;
    }

    private async UniTask LoadLocalIconInDemo(string contentName, string iconName) //from StreammingAssets
    {
        var path = Path.Combine(VrDomeAssetResourceNameDefine.DEMO_ROOT_PATH, VrDomeAssetResourceNameDefine.VR_DATA_ROOT,
                                            VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT, contentName, iconName);
        Texture2D texture2D = new Texture2D(1, 1);
        var newPath = $"{path}";
        if (Application.platform != RuntimePlatform.WebGLPlayer)
            newPath = "file://" + newPath;
        DebugExtension.Log(newPath);
        var txBytes = await ObservableUnityWebRequest.GetBytesAsObservable(newPath);
        texture2D.LoadImage(txBytes as byte[]);
        iconImage.texture = texture2D;  
    }

    private static async UniTask<Texture> DownloadTextureAsync(string url)
    {
        try
        {
            return await ObservableUnityWebRequest.GetTexture2DAsObservable(url);
        }
        catch (Exception e)
        {
            DebugExtension.Log(e.Message);
            return null;
        }
    }

    void OnDeleteContentClick()
    {
        OnDeleteContent?.Invoke(contentTitleText.text);
    }

    public void OnPointerEnter()
    {
        if (isEditable)
            deleteButton.gameObject.SetActive(true);
    }

    public void OnPointerExit()
    {
        if (isEditable)
            deleteButton.gameObject.SetActive(false);
    }

    public void OnSelected(bool isSelected)
    {
        selectedImage.SetActive(isSelected);
    }

    public string GetContentName()
    {
        return contentInfo.contentName;
    }
}
