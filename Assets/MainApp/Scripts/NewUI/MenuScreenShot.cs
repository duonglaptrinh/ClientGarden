using Game.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UIScroll;
using UnityEngine;
using UnityEngine.UI;
using static VrGardenApi.SaveImageApi;

public class MenuScreenShot : MonoBehaviour
{
    [SerializeField] UIScrollBase scrollObject;
    [SerializeField] GameObject loading;

    [SerializeField] GameObject captureObj;
    [SerializeField] GameObject viewObj;

    [SerializeField] Image imageScreenShot;
    [SerializeField] Button btnDownload;
    [SerializeField] Button btnDelete;
    [SerializeField] Button btnCapture;
    [SerializeField] GameObject SendRequestloading;

    [SerializeField] List<Sprite> listImageTest;
    List<GetJsonImageResponse> listImageData;
    GetJsonImageResponse currentImageResponse;

    UIScrollItemScreenShot currentUISelected;

    int countLoading = 0;
    bool isNeedLoadImage = false;
    void Awake()
    {
        scrollObject.OnCreateOneItem = CreateOneItemObject;
        scrollObject.OnClickOneItem = OnClickItemObject;
    }
    protected virtual void CreateOneItemObject(UIScrollItemBase item)
    {
        ItemDataBaseScreenShot data = (ItemDataBaseScreenShot)item.CurrentData;
        UIScrollItemScreenShot ui = (UIScrollItemScreenShot)item;
        if (isNeedLoadImage)
        {
            //DebugExtension.LogError(ui.IsNeedReload);
            if (ui.IsNeedReload) // if same Image --> dont need download
            {
                LoadImageByUrl(data.imageResponse.Url, item);
            }
            else
            {
                countLoading++;
                CheckLoading();
            }
        }
    }

    void LoadImageByUrl(string url, UIScrollItemBase item)
    {
        try
        {
            ConnectServer.Instance.LoadImageByUrl(url, texture =>
            {
                item.GetComponent<Image>().sprite = VRObjectV2.ConvertTexture2DToSprite(texture);
                countLoading++;
                CheckLoading();
            });
        }
        catch (Exception e)
        {
            DebugExtension.LogError("Load Image screenShot Fail: " + e);
            countLoading++;
            CheckLoading();
        }
    }

    void CheckLoading()
    {
        if (countLoading >= listImageData.Count)
        {
            loading.SetActive(false);
            captureObj.gameObject.SetActive(true);
        }
    }
    protected virtual void OnClickItemObject(int index)
    {
        UIScrollItemScreenShot ui = (UIScrollItemScreenShot)scrollObject.ListItems[index];
        ItemDataBaseScreenShot data = (ItemDataBaseScreenShot)ui.CurrentData;
        currentImageResponse = data.imageResponse;

        ShowHideCaptureAndView(isShowCapture: false);
        Sprite sprite = ui.GetComponent<Image>().sprite;
        imageScreenShot.sprite = sprite;
        imageScreenShot.preserveAspect = true;
        currentUISelected = ui;
    }

    private void OnEnable()
    {
        ShowHideCaptureAndView(isShowCapture: true);
        LoadingListImage(listImageData == null || listImageData.Count == 0);
    }
    private void OnDisable()
    {
    }

    private void Start()
    {
        btnCapture.onClick.AddListener(() =>
        {
            CaptureScreenIgnoreUI.Instance.CaptureScreenshotWithoutUI(texture =>
            {
                string cachePath = Application.persistentDataPath + "/your_image.jpg";
                File.WriteAllBytes(cachePath, texture.EncodeToPNG());

                SendRequestloading.SetActive(true);
                UploadImage(texture);
            });
        });
        btnDelete.onClick.AddListener(() =>
        {
            PopupRuntimeManager.Instance.ShowPopup("この画像を削除してもよろしいですか？", onClickConfirm: () =>
            {
                DeleteOneImage();
            });
        });
        btnDownload.onClick.AddListener(() =>
        {
            ConnectServer.Instance.GetImageById(currentImageResponse.Id, res =>
            {
                DebugExtension.Log("Call Download Function " + res.Url);
                WebGLAdapter.DownloadImageByUrl(res.Url);
            });
        });
    }
    void DeleteOneImage()
    {
        SendRequestloading.SetActive(true);
        ConnectServer.Instance.DeleteImageById(currentImageResponse.Id, response =>
        {
            if (response.Status)
            {
                DebugExtension.Log("Delete ScreenShot Done --------- ");
                listImageData.Remove(currentImageResponse);
                if (currentUISelected)
                {
                    scrollObject.removeItem(currentUISelected);
                    currentUISelected = null;
                }
                ShowHideCaptureAndView(isShowCapture: true);
                CheckButtonCapture();
                scrollObject.CalculateFitContent();
                //LoadingListImage(true);
            }
            else DebugExtension.LogError("Delete Fail");
            SendRequestloading.SetActive(false);
        });
    }
    void UploadImage(Texture2D texture)
    {
        ConnectServer.Instance.UploadImage(texture, response =>
        {
            if (response.Status == 1)
            {
                DebugExtension.Log("Upload ScreenShot Done, Id = " + response.Id);
                //listImageTest.Add(VRObjectV2.ConvertTexture2DToSprite(texture));
                LoadingListImage(true);
            }
            else DebugExtension.LogError("Upload Fail");
            SendRequestloading.SetActive(false);
        });
    }
    //IEnumerator WaitFake()
    //{
    //    yield return new WaitForSeconds(0.5f);
    //    SendRequestloading.SetActive(false);
    //    LoadingListImage();
    //}

    void ShowHideCaptureAndView(bool isShowCapture)
    {
        captureObj.SetActive(isShowCapture);
        viewObj.SetActive(!isShowCapture);
    }
    void LoadingListImage(bool isNeedReloadData = false)
    {
        if (isNeedReloadData)
        {
            isNeedLoadImage = true;
            loading.SetActive(true);
            captureObj.gameObject.SetActive(false);
            ConnectServer.Instance.GetListImage(res =>
            {
                listImageData = res.ListImage;
                ShowListImage();
            });
        }
        else
        {
            isNeedLoadImage = false;
            ShowListImage();
        }
    }
    void ShowListImage()
    {
        countLoading = 0;
        List<ItemDataBase> list = new List<ItemDataBase>();

        for (int i = 0; i < listImageData.Count; i++)
        {
            list.Add(new ItemDataBaseScreenShot(listImageData[i]));
        }
        if (list.Count > 0)
        {
            scrollObject.Initialize(list);
        }
        else
        {
            loading.SetActive(false);
            captureObj.gameObject.SetActive(true);
        }
        CheckButtonCapture();
    }
    void CheckButtonCapture()
    {
        btnCapture.interactable = listImageData.Count < 10;
    }
    public void Close()
    {
        BaseScreenUiControllerV2.Instance.HideUIMenuScreenShot();
    }
}
