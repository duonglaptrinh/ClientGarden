using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Outline = ThirdOutline.Outline;

public class UIItemList : UIScrollItemBase
{
    [SerializeField] Text textName;
    [SerializeField] RawImage icon;
    Outline outline;
    Image image;
    public void SetDataStart(string nameT, Texture texture)
    {
        btnClick.gameObject.SetActive(false);
        textName.text = nameT.Length > 26 ? (nameT.Substring(0, 25) + "...") : nameT;
        UpdateIcon(texture);
    }
    public void UpdateIcon(Texture texture)
    {
        icon.texture = texture;
    }
    public override void UnSelect()
    {
        base.UnSelect();
        if (outline) Destroy(outline);
        SetHighlight(isHighlight: false);
        btnClick.gameObject.SetActive(false);
    }
    public override void Select()
    {
        base.Select();
        ItemDataMaterial m = (ItemDataMaterial)CurrentData;
        outline = m.renderer.GetComponent<Outline>();
        if (!outline) outline = m.renderer.gameObject.AddComponent<Outline>();
        outline.OutlineWidth = 5;
        outline.OutlineColor = Color.yellow;

        SetHighlight(isHighlight: true);
        btnClick.gameObject.SetActive(true);
    }
    void SetHighlight(bool isHighlight)
    {
        if (!image) image = GetComponent<Image>();
        Color32 c = image.color;
        c.a = isHighlight ? (byte)127 : (byte)1;
        image.color = c;
    }
}
