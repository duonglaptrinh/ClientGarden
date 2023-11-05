using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Outline = ThirdOutline.Outline;

public class UIItemTextureModel : UIScrollItemBase
{
    [SerializeField] RawImage icon;
    [SerializeField] Image boder;

    public Texture TextureCurrent => icon.texture;
    public void SetDataTexture(Texture text)
    {
        boder.color = Color.white;
        boder.gameObject.SetActive(true);
        icon.texture = text;
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
        if (boder)
        {
            boder.color = isHighlight ? (new Color(0.05f, 0.63529f, 0.6039f, 1)) : Color.white;
            //DebugExtension.LogError(boder.color + "   name " + name);
        }
    }
}
