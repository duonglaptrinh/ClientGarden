using Game.Client;
using System;
using TWT.Model;
using UnityEngine;
using UnityEngine.UI;

public class VRPlanTemplateItem : MonoBehaviour
{
    [SerializeField] Image previewImage;
    [SerializeField] GameObject normal;
    [SerializeField] GameObject hightlight;
    [SerializeField] Text nameDome;
    [SerializeField] Text nameDome2;
    [SerializeField] Button selectButton;

    VRPlanDataTemplate domeData;
    Action<VRPlanDataTemplate, Sprite> onSelected;

    private void Start()
    {
        selectButton.onClick.AddListener(OnDomeSelected);
    }

    public void SetDome(int index, VRPlanDataTemplate domeData, Action<VRPlanDataTemplate, Sprite> onSelected)
    {
        //hightlight?.SetActive(domeData.dome_id == GameContext.CurrentIdDome);
        hightlight.SetActive(false);
        normal.SetActive(true);

        //get the name of plan and assign it to the text component
        nameDome.text = domeData.name;
        nameDome2.text = nameDome.text;

        this.domeData = domeData;
        this.onSelected = onSelected;

        previewImage.sprite = LoadResourcesData.Instance.GetThumbnailPlanTemplate(index);
        //SetPreview(domeData.name, domeData.modelData);
    }

    //private void SetPreview(string fileName, ModelDataHouseNetwork data)
    //{
    //    AspectRatioFitter ar = previewImage.gameObject.GetComponent<AspectRatioFitter>();
    //    if (ar) Destroy(ar);

    //    previewImage.texture = LoadResourcesData.Instance.icon_Room;
    //    AspectRatioFitter ar1 = previewImage.gameObject.GetComponent<AspectRatioFitter>();
    //    if (!ar1)
    //        ar1 = previewImage.gameObject.AddComponent<AspectRatioFitter>();
    //    if (ar1) ar1.aspectMode = AspectRatioFitter.AspectMode.HeightControlsWidth;
    //}

    public void OnDomeSelected()
    {
        onSelected?.Invoke(domeData, previewImage.sprite);
        //DebugExtension.LogError("sdfasdf ");
    }
}