using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Game.Client;
using Shim.Utils;
using TWT.Model;
using TWT.Networking;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MenuListTotalAllItemInRoom : MonoBehaviour
{
    [SerializeField] List<OneContentInTotalListAllItem> listAllCategory;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] private Button backMenu;
    [SerializeField] private Text sumTotalCategory;
    public double totalCategory;
    public double sumHouse;
    public double Floor_Wall;
    public double OutSide;
    public double InSide ;
    public double Tree;
    public double Car;
    public double Around;

    private void OnEnable()
    {
        ShowListCategory();
        VRDomeLoadHouse.OnChangeHouseModel += OnChangeHouseModel;
        VRObjectSync.OnCreateNewModel += OnCreateNewModel;
        VRObjectSync.OnDeleteNewModel += OnDeleteNewModel;
    }
    private void OnDisable()
    {
        VRDomeLoadHouse.OnChangeHouseModel -= OnChangeHouseModel;
        VRObjectSync.OnCreateNewModel -= OnCreateNewModel;
        VRObjectSync.OnDeleteNewModel -= OnDeleteNewModel;
    }
    void OnChangeHouseModel(Transform transform)
    {
        ShowListCategory();
    }
    void OnCreateNewModel(SyncCreateVrObjectMessage msg, VRModelData data)
    {
        ShowListCategory();
    }
    void OnDeleteNewModel(SyncDeleteVrObjectMessage msg)
    {
        ShowListCategory();
    }
    private void Start()
    {
       backMenu.onClick.AddListener(BaseScreenUiControllerV2.Instance.menuListAllItemInRoom.HideMenuListTotalAllItemInRoom);
    }
    private void Update()
    {
        sumHouse = listAllCategory[0].isCheckTotalPrice ? listAllCategory[0].sumTotalCategory : 0;
        Floor_Wall = listAllCategory[1].isCheckTotalPrice ? listAllCategory[1].sumTotalCategory : 0;
        OutSide = listAllCategory[2].isCheckTotalPrice ? listAllCategory[2].sumTotalCategory : 0;
        InSide = listAllCategory[3].isCheckTotalPrice ? listAllCategory[3].sumTotalCategory : 0;
        Tree = listAllCategory[4].isCheckTotalPrice ? listAllCategory[4].sumTotalCategory : 0;
        Car = listAllCategory[5].isCheckTotalPrice ? listAllCategory[5].sumTotalCategory : 0;
        Around = listAllCategory[6].isCheckTotalPrice ? listAllCategory[6].sumTotalCategory : 0;
        totalCategory = sumHouse + Floor_Wall + OutSide + InSide + Tree + Car + Around;
        sumTotalCategory.text = "¥" + $"{totalCategory:N0}";
    }

    void ShowListCategory()
    {
        float height = 0;
        //setup list House
        listAllCategory[0].ClearData();
        height += listAllCategory[0].Setupdata("建物", "House");

        //Setup list category
        for (int i = 1; i < listAllCategory.Count; i++)
        {
            listAllCategory[i].ClearData();
            MenuNewItemData data = LoadResourcesData.Instance.listCategoryMenu[i - 1];
            height += listAllCategory[i].Setupdata(data);
        }
        RectTransform rectContent = (RectTransform)scrollRect.content.transform;
        height += verticalLayoutGroup.padding.top + verticalLayoutGroup.padding.bottom + verticalLayoutGroup.spacing * listAllCategory.Count;
        rectContent.sizeDelta = new Vector2(rectContent.rect.width, height);
        RectTransformExtensions.SetLeft(rectContent, 0);
        RectTransformExtensions.SetRight(rectContent, 0);
    }
}
