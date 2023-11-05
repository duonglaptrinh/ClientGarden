using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using jp.co.mirabo.Application.RoomManagement;

public class AddressableDownloadManager : SingletonMonoBehaviour<AddressableDownloadManager>
{
    public const string preloadLabel = "preload_assets";
    public const string thumbLabel = "label_thumb";
    public const string colorTextureThumb = "label_ColorTexture";
    public const string resourceAsset = "resource_asset";
    public static List<string> listDownload = new List<string>() { preloadLabel, thumbLabel, colorTextureThumb, resourceAsset };

    public static Action OnInitSuccess;
    public static Action<float> ProgressEvent;
    public static Action<bool> CompletionEvent;
    public static Action<bool> CompletionOneItemEvent;

    public int TotalItemDownload => listDownload.Count;
    public int CountItemDownload { get; set; }
    public static LoadResourceAddessable ResourcesData { get; set; }
    [SerializeField] bool isAutoLoad = true;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        DebugExtension.Log("InitializeAsync Addressable !!!");
        Addressables.InitializeAsync().Completed += AddressablesOnComplete;
    }
    private void Awake()
    {
        if (Instance != null && Instance.GetInstanceID() != this.GetInstanceID())
        {
            //DebugExtension.Log("Another instance of " + GetType() + " is already exist! Destroying self...");
            DestroyImmediate(gameObject);
            return;
        }
    }
    public void StartLoadData()
    {
        CountItemDownload = 0;
        StartDownloads(CountItemDownload);
    }

    void AddressablesOnComplete(AsyncOperationHandle<IResourceLocator> obj)
    {
        DebugExtension.Log("InitializeAsync Addressable DONE!!!");
        OnInitSuccess?.Invoke();
        if (isAutoLoad) StartLoadData();
    }

    void StartDownloads(int index)
    {
        CreateDownloader(listDownload[index]);
    }

    void CreateDownloader(string label)
    {
        AddressableMyDownloader download = new GameObject("AddressableMyDownloader").AddComponent<AddressableMyDownloader>();
        download.Download(label, OnProgress, OnComplete);
    }
    void OnProgress(float progress)
    {
        ProgressEvent?.Invoke(progress);
    }
    void OnComplete(bool onComplete)
    {
        CompletionOneItemEvent?.Invoke(onComplete);
        CountItemDownload++;
        if (CountItemDownload == listDownload.Count)
        {
            LoadResourseAsset().Forget();
        }
        else
            StartDownloads(CountItemDownload);
    }
    async UniTask LoadResourseAsset()
    {
        string asset = "Assets/AddressableData/LoadResourceAddessable.asset";
        var assethanlder = Addressables.LoadAssetAsync<LoadResourceAddessable>(asset);
        await assethanlder;
        ResourcesData = assethanlder.Result;
        ResourcesData.PrepareData();
        CompletionEvent?.Invoke(true);
    }
}
