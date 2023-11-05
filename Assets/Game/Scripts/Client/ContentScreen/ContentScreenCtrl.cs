using Game.Client;
using System;
using System.Collections.Generic;
using Game.Client.Extension;
using TWT.Client.ContentScreen;
using TWT.Model;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TWT.Utility;
using LitJson;

public class ContentScreenCtrl : MonoBehaviour
{
    [SerializeField] TextAsset jsonData;
    public GameObject title;
    public GameObject loadingContentPanel;
    public GameObject listContentPanel;
    public GameObject loadingDataPanel;

    public Transform container;
    public GameObject contentItemPrefab;

    public Button addContentButton;
    public GameObject createContentPanel;

    public ScrollRect contentsScroll;
    public Button nextContentButton;
    public Button backContentButton;

    public Button backButton;

    public Slider loadSlider;
    public Text percentLoaded;
    public Text txtIndex;

    IResourceLoader resourceLoader;

    List<ContentInfo> listContents;
    int pageIndex;
    int pageNumber;
    int numberItemPerPage = 3;

    public event Action<string> OnSelectContent;
    string contentName;
    string jsonContentDataFromServer;

    void Start()
    {
        AddressableDownloadManager.ProgressEvent = OnProgress;
        AddressableDownloadManager.CompletionEvent = OnLoadDone;
        SetText(0);

        resourceLoader = new LocalResourceLoader(GameContext.IsDemo ? VrDomeAssetResourceNameDefine.DEMO_ROOT_PATH : Application.persistentDataPath);

        addContentButton.onClick.AddListener(OnAddContentClick);
        nextContentButton.onClick.AddListener(OnNextListContent);
        backContentButton.onClick.AddListener(OnBackListContent);
        backButton.onClick.AddListener(ReturnToTitle);
        if (GameContext.IsOffline)
            LoadContents();
    }
    void OnProgress(float progress)
    {
        SetText(progress);
    }

    void SetText(float progress)
    {
        percentLoaded.text = "Loading " + string.Format("Value: {0:P2}.", progress);// string.Format("Value: {0:%}.", (progress * 100));// + "%";
        loadSlider.value = progress;
    }

    void Update()
    {
        int count = AddressableDownloadManager.Instance.CountItemDownload;
        if (count > AddressableDownloadManager.Instance.TotalItemDownload)
            count = AddressableDownloadManager.Instance.TotalItemDownload;
        if (txtIndex) txtIndex.text = "Item Download: " + count + "/" + AddressableDownloadManager.Instance.TotalItemDownload;
        // percentLoaded.text = Mathf.Ceil(((float)loadSlider.value / 1) * 100).ToString() + "%";
    }

    void SetMode()
    {
        if (GameContext.IsEditable)
        {
            addContentButton.gameObject.SetActive(true);
        }
        else
        {
            addContentButton.gameObject.SetActive(false);
        }
    }

    void ClearListContents()
    {
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }

    [ContextMenu("load content")]
    public void LoadContents()
    {
        //LoadDatasAsync("Mirabo-Demo-v2").Forget();

        LoadContentsAsync().Forget();
    }

    private async UniTask LoadContentsAsync()
    {
        //loadingContentPanel.SetActive(true);
        listContentPanel.SetActive(false);

        //string[] listContents = await resourceLoader.GetAllContentDataName();
        try
        {
            listContents = await resourceLoader.GetListContents();
        }
        catch (Exception e)
        {
            listContents = new List<ContentInfo>();
            //DebugExtension.LogError(e);
            //PopupRuntimeManager.Instance.ShowPopupOnlyConfirm($"{e}", ReturnToTitle);
        }

        pageNumber = (int)Mathf.Ceil((float)listContents.Count / numberItemPerPage);
        pageIndex = 1;
        ShowContentsByPage(pageIndex);

        Observable.Timer(TimeSpan.FromSeconds(0.5f)).Subscribe(_ =>
        {
            loadingContentPanel.SetActive(false);
            listContentPanel.SetActive(true);
        });

    }

    void ShowContentsByPage(int pageIndex)
    {
        ClearListContents();
        if (pageNumber == 0)
            return;
        txtIndex.text = string.Format("{0}/{1}", pageIndex, pageNumber);

        ShowButton();

        List<ContentInfo> showContents;
        int fromIndex = (pageIndex - 1) * numberItemPerPage;
        if (fromIndex + numberItemPerPage > listContents.Count)
        {
            showContents = listContents.GetRange(fromIndex, listContents.Count - fromIndex);
        }
        else
        {
            showContents = listContents.GetRange(fromIndex, numberItemPerPage);
        }

        foreach (var contentInfo in showContents)
        {
            //Join the first content on list
            OnSelectContent?.Invoke(contentInfo.contentName);
            LoadDatasAsync(contentInfo.contentName).Forget();
            return;

            GameObject contentItem = Instantiate(contentItemPrefab, container);
            contentItem.transform.localPosition = Vector3.zero;
            contentItem.transform.localRotation = Quaternion.Euler(Vector3.zero);
            contentItem.transform.localScale = Vector3.one;
            contentItem.GetComponent<ContentItemUICtrl>().Initialize(contentInfo);
            contentItem.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                GameContext.ContentInfoCurrent = contentInfo;
                OnContentClick(contentInfo.contentName);
            });
            contentItem.GetComponent<ContentItemUICtrl>().OnDeleteContent = DeleteContent;
        }
    }

    public async UniTask LoadDatasAsync(string contentName, string jsonContentDataFromServer = "")
    {
        SetText(0);
        loadingContentPanel.SetActive(false);
        loadingDataPanel.SetActive(true);
        title.SetActive(false);
        listContentPanel.SetActive(false);
        this.contentName = contentName;

        if (jsonData != null)
        {
            DebugExtension.LogError("Load Json From file Test");
            jsonContentDataFromServer = jsonData.text;
        }
        else
            jsonContentDataFromServer = await GameContext.ResourceLoader.GetVrContentJsonDataAtRuntimeServer(GameContext.ContentName);
        //GameContext.ContentDataCurrent = JsonUtility.FromJson<VRContentData>(vrDataJson);
        this.jsonContentDataFromServer = jsonContentDataFromServer;
        AddressableDownloadManager.Instance.StartLoadData();
        await UniTask.Delay(10);
    }
    async void OnLoadDone(bool isDone)
    {
        if (!isDone) return;
        try
        {
            await loadingDataPanel.GetComponent<LoadingDataController>().LoadContentAndSystemContentAsync(contentName);
        }
        catch
        {
            PopupRuntimeManager.Instance.ShowPopupOnlyConfirm(
                "しばらくお待ちください",
                () =>
            {
                ReturnToTitle();
            });
        }

        if (!GameContext.IsOffline)
        {
            if (!Game.Client.Utility.GetServerStatus())
                return;
        }
        var vrDataJson = "";
        DebugExtension.LogError("jsonContentDataFromServer = " + jsonContentDataFromServer);
        if (string.IsNullOrEmpty(jsonContentDataFromServer))
        {
            vrDataJson = await resourceLoader.GetVrContentJsonData(contentName);
            DebugExtension.LogError("vrDataJson = " + vrDataJson);
        }
        else
            vrDataJson = jsonContentDataFromServer;
        GameContext.ContentDataCurrent = JsonUtility.FromJson<VRContentData>(vrDataJson);

        await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        if (!GameContext.IsOffline)
        {
            if (!Game.Client.Utility.GetServerStatus()) return;
        }

        SceneConfig.LoadScene(SceneConfig.Scene.BaseScreen);
    }

    public void CreateNewContent(string newContentName)
    {
        resourceLoader.AddContent(newContentName);
        LoadContents();
    }

    public void DeleteContent(string contentName)
    {
        resourceLoader.RemoveContent(contentName);
        LoadContents();
    }

    public void OnContentClick(string contentName)
    {
        DebugExtension.LogError(contentName);
        OnSelectContent?.Invoke(contentName);
        LoadDatasAsync(contentName).Forget();
    }

    public void OnAddContentClick()
    {
        createContentPanel.SetActive(true);
    }

    public void OnNextListContent()
    {
        if (pageIndex == pageNumber)
            return;

        pageIndex++;
        ShowContentsByPage(pageIndex);
    }

    public void OnBackListContent()
    {
        if (pageIndex == 1)
            return;

        pageIndex--;
        ShowContentsByPage(pageIndex);
    }

    public void ReturnToTitle()
    {
#if ADMIN
            SceneConfig.LoadScene(SceneConfig.Scene.AdminScreen);
            return;
#endif
        SceneConfig.LoadScene(SceneConfig.Scene.TitleScreen);
    }

    void ShowButton()
    {
        nextContentButton.gameObject.SetActive(pageIndex < pageNumber);
        backContentButton.gameObject.SetActive(pageIndex > 1);
    }
}