using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Game.Client;
using TWT.Model;
using UnityEngine;
using UnityEngine.UI;
using VrGardenApi;

public class ItemSavePosition : MonoBehaviour
{
    [SerializeField] GameObject view;
    [SerializeField] GameObject createView;
    [SerializeField] RawImage previewImage;
    [SerializeField] Text text;
    [SerializeField] private Button deleteView;
    [SerializeField] private Button updateView;
    [SerializeField] private Button selectView;
    [SerializeField] private Button createButton;
    VRStartPointData vrStartPointData;
    [SerializeField] Outline outLine;

    Action<int, VRStartPointData> onSelected;
    Action<int, VRStartPointData> onDeleted;
    Action<int, VRStartPointData, Texture2D> onUpdated;
    Action<int, Texture2D> onCreate;

    private int currentIndex = 0;
    private string idUrlImage;
    public bool IsCreateNew { get; set; }

    private void Start()
    {
        deleteView.onClick.AddListener(OnDeletedView);
        updateView.onClick.AddListener(OnUpdateView);
        selectView.onClick.AddListener(OnSelectedView);
        createButton.onClick.AddListener(OnCreateView);
    }
    public void SetDome(VRStartPointData vrStartPointData, string id_url, Action<int, VRStartPointData> onSelected, Action<int, VRStartPointData> onDeleted = null, Action<int, VRStartPointData, Texture2D> onUpdated = null)
    {
        IsCreateNew = false;
        view.SetActive(true);
        createView.SetActive(false);

        this.currentIndex = int.Parse(vrStartPointData.id_url);
        outLine.enabled = VrDomeControllerV2.Instance.vrDomeData.listStartPointData.indexStartPoint == currentIndex;
        this.vrStartPointData = vrStartPointData;
        this.idUrlImage = id_url;
        this.onSelected = onSelected;
        this.onDeleted = onDeleted;
        this.onUpdated = onUpdated;

        text.text = vrStartPointData.nameView;
        SetPreview(id_url);
    }

    public void SetCreateView(VRStartPointData vrStartPointData, Action<int, Texture2D> onCreate = null)
    {
        IsCreateNew = true;
        view.SetActive(false);
        createView.SetActive(true);

        this.vrStartPointData = vrStartPointData;
        //currentIndex = int.Parse(vrStartPointData.id_url);
        this.onCreate = onCreate;
    }
    public void CheckCanCreateNewView(int maxList)
    {
        if (IsCreateNew)
            createButton.interactable = maxList <= 8;
    }

    private void SetPreview(string id_url)
    {
        AspectRatioFitter ar = previewImage.gameObject.GetComponent<AspectRatioFitter>();
        if (ar) Destroy(ar);
        try
        {
            ConnectServer.Instance.GetImageByIdView(int.Parse(id_url), response =>
            {
                if (response == null)
                {
                    return;
                }
                //Load Image
                ConnectServer.Instance.LoadImageByUrl(response.Url, texture =>
                {
                    previewImage.texture = texture;// LoadResourcesData.Instance.icon_Room;
                    AspectRatioFitter ar1 = previewImage.gameObject.GetComponent<AspectRatioFitter>();
                    if (!ar1)
                        ar1 = previewImage.gameObject.AddComponent<AspectRatioFitter>();
                    if (ar1) ar1.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
                });
            });
        }
        catch (Exception e)
        {
            DebugExtension.LogError("Get API Image View Error");
        }
    }

    private void OnCreateView()
    {
        PopupRuntimeManager.Instance.ShowPopup("新しいビューを作成しますか？",
            onClickConfirm: () =>
            {
                BaseScreenUiControllerV2.Instance.isCaptureNewView = true;
                ProcessCreateScreenShot(isCreateNew: true);
                createButton.interactable = false;
            },
            onClickCancel: () =>
            {

            });
    }

    private void ProcessCreateScreenShot(bool isCreateNew)
    {       
        CaptureScreenIgnoreUI.Instance.CaptureScreenshotWithoutUI(texture =>
        {
            texture = MyUtils.ScaleTextureV2(texture, 128);
            DebugExtension.Log(texture.width + "  " + texture.height);
#if UNITY_EDITOR
            string cachePath = Application.persistentDataPath + "/your_image.jpg";
            File.WriteAllBytes(cachePath, texture.EncodeToPNG());
#endif
            if (isCreateNew)
                this.onCreate?.Invoke(currentIndex, texture);
            else
                this.onUpdated?.Invoke(currentIndex, vrStartPointData, texture);
        });
    }

    private void OnDeletedView()
    {
        if (VrDomeControllerV2.Instance.vrDomeData.listStartPointData.indexStartPoint != currentIndex)
        {
            PopupRuntimeManager.Instance.ShowPopup("選択したビューを削除しますか？",
                onClickConfirm: () =>
                {
                    this.onDeleted?.Invoke(currentIndex, vrStartPointData);
                },
                onClickCancel: () =>
                {

                });
        }
        else
        {
            PopupRuntimeManager.Instance.ShowPopupOnlyConfirm("選択中のプランは削除できません。");
        }

    }

    private void OnUpdateView()
    {
        PopupRuntimeManager.Instance.ShowPopup("選択したビューを更新しますか？",
            onClickConfirm: () =>
            {
                BaseScreenUiControllerV2.Instance.isCaptureNewView = true;
                ProcessCreateScreenShot(isCreateNew: false);
            },
            onClickCancel: () =>
            {

            });
    }

    private void OnSelectedView()
    {
        PopupRuntimeManager.Instance.ShowPopup("選択したビューに移動しますか？",
            onClickConfirm: () =>
            {
                this.onSelected?.Invoke(currentIndex, vrStartPointData);
            },
            onClickCancel: () =>
            {

            });
    }
}
