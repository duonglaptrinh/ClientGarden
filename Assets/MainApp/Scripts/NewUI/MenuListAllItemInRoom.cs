using Game.Client;
using Shim.Utils;
using System.Collections;
using System.Collections.Generic;
using TWT.Model;
using TWT.Networking;
using UIScroll;
using UnityEngine;
using UnityEngine.UI;

public class MenuListAllItemInRoom : MonoBehaviour
{
    [SerializeField] List<OneContentInListAllItem> listAllCategory;
    [SerializeField] ScrollRect scrollRect;
    [SerializeField] VerticalLayoutGroup verticalLayoutGroup;
    [SerializeField] Button btnTotalMoney;
    [SerializeField] private GameObject menuListTotalAllItemInRoom;

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
        foreach (var item in listAllCategory)
        {
            item.OnClose = Close;
        }
        btnTotalMoney.onClick.AddListener(ShowMenuListTotalAllItemInRoom);
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
    public void Close()
    {
        BaseScreenUiControllerV2.Instance.HideUIListItemButton();
    }

    private void ShowMenuListTotalAllItemInRoom()
    {
        ShowHideMenuListTotalAllItemInRoom(true);
    }

    public void HideMenuListTotalAllItemInRoom()
    {
        ShowHideMenuListTotalAllItemInRoom(false);
    }

    private void ShowHideMenuListTotalAllItemInRoom(bool isOpenMenu)
    {
        menuListTotalAllItemInRoom.gameObject.SetActive(isOpenMenu);
        this.gameObject.SetActive(!isOpenMenu);
    }
}
