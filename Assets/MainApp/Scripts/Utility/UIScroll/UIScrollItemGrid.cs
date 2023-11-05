using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class UIScrollItemGrid : UIScrollItemBase
{
    [SerializeField] Text textName;
    [SerializeField] Image icon;
    [SerializeField] Image iconHighLight;
    AssetReferenceTexture2D currentTexture;
    //Image image;
    protected override void Start()
    {
        if (btnClick)
            btnClick.onClick.AddListener(() =>
            {
                OnClickItem?.Invoke(myIndex);
            });
    }
    public void Setup(string nameN, Texture2D texture)
    {
        textName.text = nameN;
        SetIcon(texture, Color.white);
    }
    public void LoadIcon(AssetReferenceTexture2D texture)
    {
        currentTexture?.ReleaseAsset();
        currentTexture = null;
        currentTexture = texture;
        texture.LoadAssetAsync<Texture2D>().Completed += texture =>
        {
            //Image img = Instantiate(prefabThumb, scroll.content);
            Texture2D spr = texture.Result;
            SetIcon(spr, Color.white);
        };
        //icon.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
    }

    public void SetIcon(Texture2D spr, Color color)
    {
        if (icon)
        {
            if (spr)
                icon.sprite = Sprite.Create(spr, new Rect(0, 0, spr.width, spr.height), Vector2.one * 0.5f);
            else icon.sprite = null;
            icon.color = color;
        }
    }

    public override void UnSelect()
    {
        base.UnSelect();
        SetHighlight(isHighlight: false);
    }
    public override void Select()
    {
        base.Select();
        SetHighlight(isHighlight: true);
    }

    void SetHighlight(bool isHighlight)
    {
        //if (!image) image = GetComponent<Image>();
        //Color32 c = image.color;
        //c.a = isHighlight ? (byte)255 : (byte)127;
        //image.color = c;
        iconHighLight.gameObject.SetActive(isHighlight);
    }
    private void OnEnable()
    {
        //SetHighlight(isHighlight: false);
    }
    void OnDisable()
    {
        //note that this may be dangerous, as we are releasing the asset without knowing if the instances still exist.
        // sometimes that's fine, sometimes not.
        currentTexture?.ReleaseAsset();
        currentTexture = null;
    }
}
