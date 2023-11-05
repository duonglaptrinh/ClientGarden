using Game.Client;
using SyncRoom.Schemas;
using System;
using System.Collections.Generic;
using TWT.Networking;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class MenuEditSelectTexture : MonoBehaviour
{
    public Action OnBackButtonClick = null;
    [SerializeField] UIScrollBase scroll;
    [SerializeField] RawImage imgTextureCurrent;
    [SerializeField] public Text txtNameTextureCurrent;
    [SerializeField] Button btnBackToMenuEdit;

    VRModelV2 vRObject;
    ProductColorController colorController;
    UIItemTextureModel selectItem;

    // Start is called before the first frame update
    void Awake()
    {
        scroll.gameObject.SetActive(true);
        btnBackToMenuEdit.onClick.AddListener(() =>
        {
            OnBackButtonClick?.Invoke();
        });
    }
    public void DisableSelectItem()
    {
        if (selectItem) selectItem.UnSelect();
    }
    public void SetupData(VRModelV2 vRObject, ProductColorController colorController)
    {
        scroll.OnCreateOneItem = CreateOneItem;
        scroll.OnClickOneItem = OnClickSelectItem;
        this.vRObject = vRObject;
        this.colorController = colorController;
        if (colorController)
            LoadNewMode();
        else LoadNormal();
    }
    void LoadNewMode()
    {
        List<ItemDataBase> datas = new List<ItemDataBase>();
        for (int i = 0; i < colorController.ListColorSets.Count; i++)
        {
            datas.Add(new ItemDataTexture(colorController.ListColorSets[i], i));
        }
        scroll.Initialize(datas);
        if (selectItem)
        {
            //scroll.FocusItem((RectTransform)(selectItem.transform));
        }
    }
    void LoadNormal()
    {
        OneModelAssetsData data = vRObject.DataAsset;
        //DebugExtension.LogError(list.Count);
        List<ItemDataBase> datas = new List<ItemDataBase>();
        for (int i = 0; i < data.listPathColorTexture.Count; i++)
        {
            datas.Add(new ItemDataTexture(data.listPathColorTexture[i], i));
        }
        scroll.Initialize(datas);
        if (selectItem)
        {
            //scroll.FocusItem((RectTransform)(selectItem.transform));
        }
    }

    protected void CreateOneItem(UIScrollItemBase item)
    {
        ItemDataTexture m = (ItemDataTexture)item.CurrentData;
        UIItemTextureModel ui = (UIItemTextureModel)item;
        if (colorController)
        {
            ui.SetDataTexture(m.colorSet.ColorThumbnail);
            if (vRObject.GetData().nameTexture == m.indexColor)
            {
                selectItem = ui;
                ui.Select();
            }
            imgTextureCurrent.texture = ui.TextureCurrent;
            if (imgTextureCurrent.texture != null)
                txtNameTextureCurrent.text = m.colorSet.ColorName.Replace("$", "\n");
        }
        else
        {
            if (string.IsNullOrEmpty(m.pathDataTexture))
            {
                if (vRObject.GetData().nameTexture == m.indexColor)
                {
                    selectItem = ui;
                    ui.Select();
                }
                return;
            }
            Addressables.LoadAssetAsync<Texture>(m.pathDataTexture).Completed += sprite =>
            {
                //Image img = Instantiate(prefabThumb, scroll.content);
                Texture spr = sprite.Result;
                ui.SetDataTexture(spr);
                if (vRObject.GetData().nameTexture == m.indexColor)
                {
                    selectItem = ui;
                    ui.Select();
                    imgTextureCurrent.texture = ui.TextureCurrent;
                    if (imgTextureCurrent.texture != null)
                        txtNameTextureCurrent.text = imgTextureCurrent.texture.name.Replace("$", "\n");
                }
            };
        }
        //GameObject icon = item.transform.Find("Icon").gameObject;
        //if (icon) icon.GetComponent<RawImage>().texture = m.texture;
    }

    Texture ConvertSpriteToTexture(Sprite sprite)
    {
        Texture2D texture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;

        Color[] spritePixels = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                        (int)sprite.textureRect.y,
                                                        (int)sprite.textureRect.width,
                                                        (int)sprite.textureRect.height);

        texture.SetPixels(spritePixels);
        texture.Apply();
        return texture;
    }
    protected void OnClickSelectItem(int index)
    {
        UIItemTextureModel item = (UIItemTextureModel)scroll.ListItems[index];
        ItemDataTexture m = (ItemDataTexture)item.CurrentData;

        imgTextureCurrent.texture = item.TextureCurrent;
        if (imgTextureCurrent.texture != null)
        {
            if (colorController)
                txtNameTextureCurrent.text = m.colorSet.ColorName.Replace("$", "\n");
            else
                txtNameTextureCurrent.text = imgTextureCurrent.texture.name.Replace("$", "\n");
        }

        if (selectItem == item) return;
        if (selectItem) selectItem.UnSelect();
        selectItem = item;
        selectItem.Select();

        vRObject.GetData().ChangeNameTexture(m.indexColor);

        vRObject.UpdateMaterial();
        if (!vRObject.IsJustCreatNew)
        {
            SendSyncMaterial();
        }
    }
    public void SendSyncMaterial()
    {
        //DebugExtension.LogError("Change Material = " +  vRObject.GetData().nameTexture);
        VrgSyncApi.Send(new SyncMaterialModelMessage()
        {
            idDome = VrDomeControllerV2.Instance.vrDomeData.dome_id,
            id = vRObject.GetData().model_id,
            nameObjRenderer = "8-8_Not_Use",
            //colorRender = Color.white, //8-8 not use
            indexTextureLocal = 0,
            nameTexture = vRObject.GetData().nameTexture
        }, SyncMaterialModelMessage.EventKey);
    }
}
