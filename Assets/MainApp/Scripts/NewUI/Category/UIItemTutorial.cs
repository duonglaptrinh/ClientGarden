using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Outline = ThirdOutline.Outline;

public class UIItemTutorial : UIScrollItemBase
{
    [SerializeField] protected Text textName;
    [SerializeField] protected Text textTime;
    [SerializeField] protected Image icon;
    //Outline outline;
    //Image image;
    public void SetDataStart(TutorialData dataContent)
    {
        textName.text = dataContent.name;
        UpdateIcon(LoadResourcesData.Instance.GetSpriteTutorialByName(dataContent.thumb));
        textTime.text = dataContent.time;
        //Addressables.LoadAssetAsync<Sprite>(urlThumb).Completed += sprite =>
        //{
        //};
    }
    public void UpdateIcon(Sprite texture)
    {
        if (icon)
            icon.sprite = texture;
    }
    public override void UnSelect()
    {
        //base.UnSelect();
        //if (outline) Destroy(outline);
        //SetHighlight(isHighlight: false);
        //btnClick.gameObject.SetActive(false);
    }
    public override void Select()
    {

    }

}
