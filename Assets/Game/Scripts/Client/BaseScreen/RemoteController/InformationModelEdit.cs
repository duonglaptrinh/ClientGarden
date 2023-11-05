using Shim.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InformationModelEdit : MonoBehaviour
{
    // Start is called before the first frame update
    public Text txtCategory;
    public Text txtSubCategory;
    public Text txtPrice;
    public Text txtDescription;
    public Text txtFeatured;
    //public Text txtWeight;
    public Text txtColor;
    public Text txtScale;
    public Image thumbColor;
    public ScrollRect scroll;
    public Button btnBack;
    VRObjectManagerV2 vRObjectManager;
    [SerializeField] VrModelSettingDialog vrModelSettingDialog;

    OneModelAssetsData currentDataModel;
    void Start()
    {
        //txtColor.text = vrModelSettingDialog.currentEditedVRModel.GetData().listMaterial[int.Parse(vrModelSettingDialog.currentEditedVRModel.GetData().nameTexture)].nameTexture;
        vRObjectManager = VRObjectManagerV2.Instance;
    }

    public void LoadInformationModel(OneModelAssetsData data)
    {
        //scroll.content.localPosition = Vector3.zero;

        RectTransformExtensions.SetLeft(scroll.content, 0);
        RectTransformExtensions.SetRight(scroll.content, 0);
        Vector3 pos = scroll.content.transform.localPosition;
        pos.y = 0;
        scroll.content.transform.localPosition = pos;

        currentDataModel = data;
        txtCategory.text = LoadResourceAddessable.GetNameCategoryOnAppByName(data.CategoryParent).Replace("\n", "");
        txtSubCategory.text = data.CategoryChild;
        txtPrice.text = data.PrizeUnit + MyUtils.FormatCurrency(data.Prize) + "（税込）";

        txtDescription.text = data.Description;

        //txtFeatured.text = data.Featured;
        //if (string.IsNullOrEmpty(data.Featured))
        txtFeatured.text = data.Featured?.Replace("$", "\n");
        txtColor.text = "";

        thumbColor.sprite = null;
        int indexTextureCurrent = vrModelSettingDialog.currentEditedVRModel.GetData().nameTexture;
        if (data.listPathColorTexture != null && data.listPathColorTexture.Count > 0 && !string.IsNullOrEmpty(data.listPathColorTexture[0]))
            Addressables.LoadAssetAsync<Texture2D>(data.listPathColorTexture[indexTextureCurrent]).Completed += sprite =>
            {
                Texture2D spr = sprite.Result;
                if (spr)
                {
                    thumbColor.sprite = Sprite.Create(spr, new Rect(0, 0, spr.width, spr.height), Vector2.one * 0.5f);
                    txtColor.text = spr.name.Replace(".jpg", "");
                    txtColor.text = spr.name.Replace(".png", "");
                    txtColor.text = txtColor.text.Replace("$", "\n");
                }
            };
        //txtColor.text = vrModelSettingDialog.txtNameTextureCurrent.text;


        txtScale.text = data.Size;
    }
}
