using Game.Client;
using System;
using System.Collections.Generic;
using System.IO;
using TWT.Model;
using UnityEngine;
using UnityEngine.UI;

public class VRPlanSaveItemCtrl : MonoBehaviour
{
    enum typeButton
    {
        update, //SAVE normal or override
        load,   //Select, download
        delete,  //Delete
        create  // Create new plan
    }

    [SerializeField] GameObject editPlan;
    [SerializeField] GameObject createPlan;

    [SerializeField] RawImage previewImage;
    [SerializeField] GameObject normal;
    [SerializeField] GameObject hightlight;
    [SerializeField] Outline outLine;
    [SerializeField] Text textTimeCreate;
    [SerializeField] Text nameDome;
    [SerializeField] Text nameDome2;
    [SerializeField] Button selectButton;
    [SerializeField] Image iconSelectButton;
    [SerializeField] Button deleteButton;
    [SerializeField] Image iconDeleteButton;
    [SerializeField] Button updateButton;
    [SerializeField] Image iconUpdatetButton;
    [SerializeField] Button OKButton;

    VRDomeData domeData;
    Action<VRDomeData, Texture> onSelected;
    Action<VRDomeData> onDeleted;
    Action<VRPlanSaveItemCtrl, VRDomeData, Texture2D> onUpdated;
    Action<VRDomeData, Texture2D> onCreate;
    typeButton currentType;
    //string dome_filename;
    public bool IsCreateNew { get; set; }

    private void Start()
    {
        selectButton.onClick.AddListener(() => { SetUI(typeButton.load); });
        deleteButton.onClick.AddListener(() => { SetUI(typeButton.delete); });
        updateButton.onClick.AddListener(() => { SetUI(typeButton.update); });
        OKButton.onClick.AddListener(() =>
        {
            if (IsCreateNew) currentType = typeButton.create;
            switch (currentType)
            {
                case typeButton.update:
                    OnDomeUpdate();
                    break;
                case typeButton.load:
                    OnDomeSelected();
                    break;
                case typeButton.delete:
                    OnDomeDeleted();
                    break;
                case typeButton.create:
                    OnDomeCreate();
                    break;
            }
        });
    }
    void SetUIButton(Button btn, Image icon, bool isActive)
    {
        btn.image.color = isActive ? Color.white : new Color(0.0901f, 0.525f, 0.501f, 1);
        icon.color = isActive ? new Color(0.0901f, 0.525f, 0.501f, 1) : Color.white;
    }
    void SetUI(typeButton type)
    {
        currentType = type;
        switch (type)
        {
            case typeButton.update:
                SetUIButton(selectButton, iconSelectButton, false);
                SetUIButton(deleteButton, iconDeleteButton, false);
                SetUIButton(updateButton, iconUpdatetButton, true);
                nameDome.text = "保存"; // hightlight text
                break;
            case typeButton.load:
                SetUIButton(selectButton, iconSelectButton, true);
                SetUIButton(deleteButton, iconDeleteButton, false);
                SetUIButton(updateButton, iconUpdatetButton, false);
                nameDome.text = "読込"; // hightlight text
                break;
            case typeButton.delete:
                SetUIButton(selectButton, iconSelectButton, false);
                SetUIButton(deleteButton, iconDeleteButton, true);
                SetUIButton(updateButton, iconUpdatetButton, false);
                nameDome.text = "削除"; // hightlight text
                break;
            default:
                break;
        }
        nameDome2.text = nameDome.text; // normal text
    }
    public void SetDome(VRDomeData domeData, Action<VRDomeData, Texture> onSelected, Action<VRDomeData> onDeleted = null, Action<VRPlanSaveItemCtrl, VRDomeData, Texture2D> onUpdated = null)
    {
        IsCreateNew = false;
        editPlan.SetActive(true);
        createPlan.SetActive(false);
        //hightlight?.SetActive(domeData.dome_id == GameContext.CurrentIdDome);
        hightlight.SetActive(true);
        normal.SetActive(false);
        outLine.enabled = domeData.dome_id == GameContext.CurrentIdDome;

        textTimeCreate.text = "id: " + domeData.dome_id; // hightlight text
        //nameDome2.text = nameDome.text; // normal text

        this.domeData = domeData;
        this.onSelected = onSelected;
        this.onDeleted = onDeleted;
        this.onUpdated = onUpdated;

        SetPreview(domeData.id_url);
        SetUI(typeButton.update);
    }

    public void SetDomeCreate(VRDomeData domeData, Action<VRDomeData, Texture2D> onCreate = null)
    {
        IsCreateNew = true;
        editPlan.SetActive(false);
        createPlan.SetActive(true);
        this.onCreate = onCreate;
        this.domeData = domeData;
    }

    public void SetPreview(Texture2D texture)
    {
        AspectRatioFitter ar = previewImage.gameObject.GetComponent<AspectRatioFitter>();
        if (ar) Destroy(ar);

        previewImage.texture = texture;
        AspectRatioFitter ar1 = previewImage.gameObject.GetComponent<AspectRatioFitter>();
        if (!ar1)
            ar1 = previewImage.gameObject.AddComponent<AspectRatioFitter>();
        if (ar1) ar1.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;

    }
    private void SetPreview(int url_image)
    {
        if (url_image < 0)
        {
            SetPreview(LoadResourcesData.Instance.icon_Room);
            return;
        }
        try
        {
            ConnectServer.Instance.GetImageByIdView(url_image, response =>
            {
                if (response == null)
                {
                    SetPreview(LoadResourcesData.Instance.icon_Room);
                    return;
                }
                ConnectServer.Instance.LoadImageByUrl(response.Url, texture =>
                {
                    SetPreview(texture);
                });
            });
        }
        catch (Exception e)
        {
            DebugExtension.LogError("Get API Image View Error " + e);
            SetPreview(LoadResourcesData.Instance.icon_Room);
            //throw;
        }
    }

    public void OnDomeSelected()
    {
        onSelected?.Invoke(domeData, previewImage.texture);
    }

    public void OnDomeDeleted()
    {
        if (domeData.dome_id != GameContext.CurrentIdDome)
        {
            PopupRuntimeManager.Instance.ShowPopup("プランを削除しますか？", onClickConfirm: () => { onDeleted?.Invoke(domeData); });
        }
        else
        {
            PopupRuntimeManager.Instance.ShowPopupOnlyConfirm("選択中のプランは削除できません。");
        }
    }
    public void OnDomeUpdate()
    {
        //PopupRuntimeManager.Instance.ShowPopup("Do you want override this Plan", onClickConfirm: () =>
        //{
        BaseScreenUiControllerV2.Instance.isCaptureNewView = true;
        ProcessCreateScreenShot(domeData, false);
        //});
    }
    public void OnDomeCreate()
    {
        VRDomeData newDome = new VRDomeData(domeData);
        newDome.modelData.indexHouse = -1;
        newDome.modelData.ListHouseMaterialData = new List<HouseMaterialData>();
        newDome.modelData.Land_Setting_FrontOf = 0;
        newDome.modelData.Land_Setting_Behide = 0;
        newDome.modelData.Land_Setting_Left = 0;
        newDome.modelData.Land_Setting_Right = 0;
        newDome.vr_object_list.vr_model_list = new VRModelData[0];
        newDome.listStartPointData = new VRListStartPointData();
        newDome.tranformCamera = new VRTransformData();
        //Array.Clear(newDome.vr_object_list.vr_model_list, 0, newDome.vr_object_list.vr_model_list.Length);
        DebugExtension.Log(JsonUtility.ToJson(newDome));
        PopupRuntimeManager.Instance.ShowPopup("新しいプランを作成しますか？",
            onClickConfirm: () =>
            {
                //ProcessCreateScreenShot(newDome, true); 
                this.onCreate?.Invoke(newDome, MyUtils.ScaleTextureV2(LoadResourcesData.Instance.icon_Room_Empty, 256));
            });
    }
    private void ProcessCreateScreenShot(VRDomeData vrDomeData, bool isCreatePlan)
    {
        //BaseScreenUiControllerV2.Instance.NewMainMenu.ShowLoading("ビューを撮影しています。");
        CaptureScreenIgnoreUI.Instance.CaptureScreenshotWithoutUI(texture =>
        {
            texture = MyUtils.ScaleTextureV2(texture, 256);
            DebugExtension.Log(texture.width + "  " + texture.height);

#if UNITY_EDITOR
            string cachePath = Application.persistentDataPath + "/your_image.jpg";
            File.WriteAllBytes(cachePath, texture.EncodeToPNG());
#endif
            //if (isCreatePlan)
            //    this.onCreate?.Invoke(vrDomeData, texture);
            //else
            this.onUpdated?.Invoke(this, domeData, texture);
        });
    }
}