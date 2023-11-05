using Shim.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TWT.Model;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InformationPlanTemplate : MonoBehaviour
{
    public Action<VRDomeData> OnCreatePlan = null;
    public Action OnClose = null;
    public Action OnBack = null;

    public Text txtName;
    public Text txtEnglishName;
    public Text txtCategory;
    public Text txtSubCategory;
    public Text txtPrice;
    public Text txtDescription;
    public Text txtHouse;
    public Text txtTree;
    public Text txtFurniture;
    public Image thumbPlan;
    public Button buttonCreatModel;
    public ScrollRect scroll;
    VRObjectManagerV2 vRObjectManager;

    VRPlanDataTemplate currentData;
    void Start()
    {
        vRObjectManager = VRObjectManagerV2.Instance;
        buttonCreatModel.onClick.AddListener(CreatePlan);
    }

    public void LoadDetail(int index, VRPlanDataTemplate data)
    {
        currentData = data;
        txtName.text = data.name;
        txtEnglishName.text = data.englishName;
        txtCategory.text = data.tag;
        //txtSubCategory;
        txtPrice.text = "(" + data.priceUnit + ")" + MyUtils.FormatCurrency((int)data.price);
        txtDescription.text = data.description.Replace("/n", "\n");
        txtHouse.text = data.typeHouse;
        txtTree.text = data.listExteriorItems.Replace("/n", "\n");
        txtFurniture.text = data.listItemsInterior.Replace("/n", "\n");
        thumbPlan.sprite = LoadResourcesData.Instance.GetThumbnailPlanTemplate(index);
    }
    //public void LoadDetail(OneModelAssetsData data)
    //{

    //    if (data.listPathColorTexture != null && data.listPathColorTexture.Count > 0 && !string.IsNullOrEmpty(data.listPathColorTexture[0]))
    //        Addressables.LoadAssetAsync<Texture2D>(data.listPathColorTexture[0]).Completed += sprite =>
    //        {
    //            Texture2D spr = sprite.Result;
    //            if (spr)
    //            {
    //                //thumbColor.sprite = Sprite.Create(spr, new Rect(0, 0, spr.width, spr.height), Vector2.one * 0.5f);
    //                txtHouse.text = spr.name.Replace(".jpg", "");
    //                txtHouse.text = spr.name.Replace(".png", "");
    //                txtHouse.text = txtHouse.text.Replace("$", "\n");
    //            }
    //        };

    //    thumbPlan.sprite = null;
    //    if (!string.IsNullOrEmpty(data.pathThumb))
    //        Addressables.LoadAssetAsync<Sprite>(data.pathThumb).Completed += sprite =>
    //        {
    //            thumbPlan.sprite = sprite.Result;
    //        };

    //    txtTree.text = data.Size;
    //}
    void CreatePlan()
    {
        OnCreatePlan?.Invoke(new VRDomeData(currentData));
    }
    public void Back()
    {
        gameObject.SetActive(false);
        OnBack?.Invoke();
    }
    public void Close()
    {
        gameObject.SetActive(false);
        OnClose?.Invoke();
    }
}
