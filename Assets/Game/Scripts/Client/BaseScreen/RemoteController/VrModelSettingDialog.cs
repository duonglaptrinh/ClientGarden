using Game.Client;
using Shim.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Common.VGS;
using TWT.Model;
using TWT.Networking;
using System;
using UnityEngine.AddressableAssets;
using SyncRoom.Schemas;

public class VrModelSettingDialog : CommonDialog
{
    // Start is called before the first frame update
    [SerializeField] Text txtName;
    [SerializeField] Text txtEnglishName;
    //public Text txtWeight;
    [SerializeField] Image thumbModel;

    OneModelAssetsData currentDataModel;
    [Header("General UI Comps")]
    [SerializeField] Button buttonClose;
    [SerializeField] Button buttonDelete;
    [SerializeField] Button btnInformation;
    [SerializeField] Button btnScale;
    [SerializeField] Button btnPositionEdit;
    [SerializeField] Button btnRotationEdit;
    [SerializeField] Button btnMaterial;
    [SerializeField] Toggle toggleButtonLock;

    [Header("Transform Settings")]
    [SerializeField] VRObjectRotationSetting rotationSetting;
    [SerializeField] VRObjectPositionSetting positionSetting;
    [SerializeField] VRObjectSizeSetting scaleSetting;
    [SerializeField] MenuEditScaleMode2 scaleSettingMode2;
    [SerializeField] MenuEditSelectTexture selectTexture;

    [Header("Other Ref")]
    [SerializeField] VRObjectTransparentSettingController vRObjectTransparent;
    public VRModelV2 currentEditedVRModel;

    [SerializeField] List<GameObject> listUICanNotShowInViewMode = new List<GameObject>();
    [SerializeField] InformationModelEdit informationModelEdit;
    [SerializeField] GameObject groupButton;
    [SerializeField] GameObject menuScale;
    [SerializeField] GameObject menuPositionEdit;
    [SerializeField] GameObject menuRotationEdit;
    [SerializeField] GameObject menuMaterialEdit;

    void Awake()
    {
        btnInformation.onClick.AddListener(OnClickInformationModelEdit);
        btnPositionEdit.onClick.AddListener(OnClickMenuPositionEdit);
        btnRotationEdit.onClick.AddListener(OnClickMenuRotationEdit);
        btnScale.onClick.AddListener(OnClickMenuScaleEdit);
        btnMaterial.onClick.AddListener(OnClickMaterialEdit);
        informationModelEdit.btnBack.onClick.AddListener(BackToMenuEdit);
        rotationSetting.btnBack.onClick.AddListener(BackToMenuEdit);
        positionSetting.btnBack.onClick.AddListener(BackToMenuEdit);
        //DebugExtension.LogError("VrModelSettingDialog Awake");
        menuScale.GetComponentInChildren<VRObjectSizeSetting>(true).btnBack.onClick.AddListener(BackToMenuEdit);
        //DebugExtension.LogError("VrModelSettingDialog Set Scale Awake");
        buttonClose.onClick.AddListener(OnCloseClick);
        buttonDelete.onClick.AddListener(OnDeleteClick);


        foreach (var item in listUICanNotShowInViewMode)
        {
            item.gameObject.SetActive(GameContext.IsEditable);
        }

        toggleButtonLock.onValueChanged.AddListener(value =>
        {
            buttonDelete.interactable = !buttonDelete.interactable;
            btnScale.interactable = !value;
            btnPositionEdit.interactable = !value;
            btnRotationEdit.interactable = !value;
            btnMaterial.interactable = !value;
            if (currentEditedVRModel) currentEditedVRModel.GetData().isLock = value;
            AlphaObject(buttonDelete, value);
            AlphaObject(btnScale, value);
            AlphaObject(btnPositionEdit, value);
            AlphaObject(btnRotationEdit, value);
            AlphaObject(btnMaterial, value);
        });
        selectTexture.OnBackButtonClick = BackToMenuEdit;
        scaleSettingMode2.OnBackButtonClick = BackToMenuEdit;
    }

    void AlphaObject(Button btn, bool value)
    {
        btn.GetComponent<CanvasGroup>().alpha = value ? .5f : 1f;
    }

    void OnEnable()
    {
        ClearMenuEditTab();
        groupButton.SetActive(true);
        VRObjectTransparentSettingController.instance = vRObjectTransparent;
        PlayerManagerSwitch.isEdit = true;
        //DebugExtension.LogError("OnEnable PlayerController.isEdit = " + PlayerController.isEdit);
        //BaseScreenTopMenuV2.Instance.PlayerController.IsAllowRotate = false;
    }
    private void OnDisable()
    {
        selectTexture.DisableSelectItem();
        PlayerManagerSwitch.isEdit = false;
        //DebugExtension.LogError("OnDisable PlayerController.isEdit = " + PlayerController.isEdit);
        //if (BaseScreenTopMenuV2.Instance && BaseScreenTopMenuV2.Instance.PlayerController)
        //    BaseScreenTopMenuV2.Instance.PlayerController.IsAllowRotate = true;
    }
    public void LoadTitleModel(OneModelAssetsData data)
    {
        //scroll.content.localPosition = Vector3.zero;
        currentDataModel = data;
        txtName.text = currentDataModel.NameOnApp.Replace("$", " ");
        if (!string.IsNullOrEmpty(currentDataModel.EnglishName))
            txtEnglishName.text = currentDataModel.EnglishName.Replace("$", " ");
        informationModelEdit.LoadInformationModel(currentDataModel);
        //txtFeatured.text = data.Featured;
        //if (string.IsNullOrEmpty(data.Featured))
        thumbModel.sprite = null;
        if (!string.IsNullOrEmpty(currentDataModel.pathThumb))
            Addressables.LoadAssetAsync<Sprite>(currentDataModel.pathThumb).Completed += sprite =>
            {
                thumbModel.sprite = sprite.Result;
            };
    }
    //private void OnDropdownDefaultAnimation_ValueChanged(int itemIndex)
    //{
    //    currentEditedVRModel.GetData().model_default_animation = dropdownDefaultAnimation.options[itemIndex].text;
    //}

    private void OnDeleteClick()
    {
        if (PopupRuntimeManager._IsPopup) return;
        PopupRuntimeManager.Instance.ShowPopup(StringManager.DELETE_VR_MODEL,
            onClickConfirm: () =>
            {
                //BaseScreenCtrl.Instance.vrDomeController.vRObjectManager.DeleteVrModel((VRModel)VRObjectTransparentSettingController.instance.vRObject);
                //Destroy(VRObjectTransparentSettingController.instance.vRObject.gameObject);
                VRObjectManagerV2.Instance.DeleteVrModel(currentEditedVRModel);
                //Destroy(currentEditedVRModel.gameObject);
                currentEditedVRModel = null;
                OnCloseClick();
            });
    }

    void OnDestroy()
    {
        buttonClose.onClick.RemoveAllListeners();
        buttonDelete.onClick.RemoveAllListeners();
        //dropdownDefaultAnimation.onValueChanged.RemoveAllListeners();
    }

    public void SetVRModelToEdit(VRModelV2 vrModel, OneModelAssetsData currentModel)
    {
        currentEditedVRModel = vrModel;

        toggleButtonLock.isOn = currentEditedVRModel.GetData().isLock;

        LoadTitleModel(currentModel);
        // Populate Transform setting data
        rotationSetting.SetVRObjectToEdit(vrModel);
        positionSetting.SetVRObjectToEdit(vrModel);

        if (vrModel.SizeControllerComponent && !vrModel.SizeControllerComponent.IsFreeSize)
        {
            scaleSettingMode2.SetupData(vrModel,
                vrModel.transform.localScale.x,
                vrModel.transform.localScale.y,
                vrModel.transform.localScale.z,
                vrModel.SizeControllerComponent);
        }
        else
        {
            scaleSetting.SetScale(vrModel, vrModel.transform.localScale.x, vrModel.transform.localScale.y, vrModel.transform.localScale.z);
        }
        selectTexture.SetupData(vrModel, vrModel.ColorControllerComponent);
    }

    public void ClearMenuEditTab()
    {
        informationModelEdit.gameObject.SetActive(false);
        groupButton.SetActive(false);
        menuScale.SetActive(false);
        scaleSettingMode2.gameObject.SetActive(false);
        menuPositionEdit.SetActive(false);
        menuRotationEdit.SetActive(false);
        menuMaterialEdit.SetActive(false);
    }
    public void BackToMenuEdit()
    {
        ClearMenuEditTab();
        groupButton.SetActive(true);
    }
    public void OnClickInformationModelEdit()
    {
        ClearMenuEditTab();
        informationModelEdit.gameObject.SetActive(true);
        informationModelEdit.LoadInformationModel(currentDataModel);
    }
    public void OnClickMenuScaleEdit()
    {
        ClearMenuEditTab();
        if (currentEditedVRModel.SizeControllerComponent && !currentEditedVRModel.SizeControllerComponent.IsFreeSize)
        {
            scaleSettingMode2.gameObject.SetActive(true);
            menuScale.SetActive(false);
        }
        else
        {
            scaleSettingMode2.gameObject.SetActive(false);
            menuScale.SetActive(true);
        }
    }
    public void OnClickMaterialEdit()
    {
        ClearMenuEditTab();
        menuMaterialEdit.SetActive(true);
    }

    public void OnClickMenuPositionEdit()
    {
        ClearMenuEditTab();
        menuPositionEdit.SetActive(true);
    }
    public void OnClickMenuRotationEdit()
    {
        ClearMenuEditTab();
        menuRotationEdit.SetActive(true);
    }


    #region OLD
    //protected virtual void CreateOneItem(UIScrollItemBase item)
    //{
    //    UIItemList itemList = (UIItemList)item;

    //    int index = itemList.MyIndex;
    //    ItemDataMaterial m = (ItemDataMaterial)itemList.CurrentData;
    //    itemList.SetDataStart(m.material.name, m.material.mainTexture);
    //    if (index == 0)
    //    {
    //        selectItem = itemList;
    //        itemList.Select();
    //    }

    //    Button btnRow = item.GetComponent<Button>();
    //    if (btnRow)
    //    {
    //        btnRow.onClick.AddListener(() => { OnClickSelectItem(itemList); });
    //    }
    //}

    //protected void OnClickSelectItem(UIItemList item)
    //{
    //    if (selectItem == item) return;
    //    if (selectItem) selectItem.UnSelect();
    //    selectItem = item;
    //    selectItem.Select();
    //}
    #endregion end OLD
}

