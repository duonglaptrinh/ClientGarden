using Game.Client;
using Shim.Utils;
using SyncRoom.Schemas;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using jp.co.mirabo.Application.RoomManagement;
using Player_Management;
using TWT.Networking;
using UIScroll;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class OneContentInListAllItem : MonoBehaviour
{
    public Action OnClose = null;
    [SerializeField] Text textTitle;
    [SerializeField] GridLayoutGroup verticalLayout;
    [SerializeField] UIItemInRoom prefabOneItemUI;
    const float heightTitle = 50;
    List<VRModelV2> listModel = new List<VRModelV2>();

    List<UIItemInRoom> listAllObject = new List<UIItemInRoom>();
    public void ClearData()
    {
        foreach (var item in listAllObject)
        {
            Destroy(item.gameObject);
        }
        listAllObject.Clear();
    }
    public float Setupdata(MenuNewItemData data)
    {
        listModel = GetListModel(data);
        return Setupdata(data.nameTitle, data.nameCategory);
    }
    public float Setupdata(string title, string category)
    {
        bool isHouse = category == "House";
        int length = listModel.Count;
        if (isHouse)
            length = 1;
        for (int i = 0; i < length; i++)
        {
            UIItemInRoom item = Instantiate(prefabOneItemUI, verticalLayout.transform);
            if (isHouse)
                item.SetupHouse(VrDomeControllerV2.Instance.DomeLoadhouse.CurrentHouse.NameOfTheHouse);
            else
            {
                VRModelV2 m = listModel[i];
                item.Setup(m);
            }
            item.OnClick = OnClickItem;
            item.OnClickHouse = OnClickHouse;
            listAllObject.Add(item);
        }
        textTitle.text = title;

        //caculator Size
        float spacing = verticalLayout.spacing.y;
        RectOffset pading = verticalLayout.padding;
        float height = ((RectTransform)prefabOneItemUI.transform).sizeDelta.y;

        RectTransform rectContent = (RectTransform)verticalLayout.transform;
        float totalHeight = pading.top + pading.bottom + (height + spacing) * length;
        rectContent.sizeDelta = new Vector2(rectContent.rect.width, totalHeight);

        RectTransformExtensions.SetLeft(rectContent, 0);
        RectTransformExtensions.SetRight(rectContent, 0);


        rectContent = (RectTransform)transform;
        totalHeight += heightTitle;
        rectContent.sizeDelta = new Vector2(rectContent.rect.width, totalHeight);

        //RectTransformExtensions.SetLeft(rectContent, 0);
        //RectTransformExtensions.SetRight(rectContent, 0);

        return totalHeight;
    }

    void OnClickItem(VRModelV2 model)
    {
        //DebugExtension.LogError("Click Item " + model.name);
        // VRObjectManagerV2 manager = VRObjectManagerV2.Instance;
        // if (!manager.IsAllowShowUIEdit)
        // {
        //     //DebugExtension.LogError("Active Button Edit All ");
        //     manager.IsAllowShowUIEdit = true;
        //     //DebugExtension.LogError("OnOffEditMode = " + manager.IsAllowShowUIEdit);
        //     foreach (var item in manager.vrModels)
        //     {
        //         item.UiButtons.gameObject.SetActive(manager.IsAllowShowUIEdit);
        //         item.OnOffDrag(manager.IsAllowShowUIEdit);
        //     }
        // }
        //DebugExtension.LogError("Show menu Edit ");
        if (model.GetData().isLock)
        {
            PopupRuntimeManager.Instance.ShowPopupOnlyConfirm("選択したモデルは現在編集中です!");
        }
        else
        {
            foreach (var item in listModel)
            {
                if (item == model)
                {
                    MenuTabControllerV2.Instance.ShowVrModelSettingDialog(model, model.DataAsset);
                    MenuTabControllerV2.Instance.ShowVRObjectSettingTab(model);
                    ClickModel3D.SendSyncEditModel(EditModel.EditModel, model, true, MyUtils.GetColorString(model.GetComponent<Renderer>()), true, RoomManager.Instance.GameRoom.SessionId);
                }
                else
                {
                    if (item.GetData().isLock && item.GetData().sessionId == RoomManager.Instance.GameRoom.SessionId)
                    {
                        ClickModel3D.SendSyncEditModel(EditModel.EditModel, item, false, MyUtils.GetColorString(item.GetComponent<Renderer>()), false, "");
                    }
                }
            }
        }
    }
    void OnClickHouse()
    {
        OnClose?.Invoke();
        BaseScreenUiControllerV2.Instance.NewMainMenu.ForceShowHideHouseMenu(true);
    }

    List<VRModelV2> GetListModel(MenuNewItemData data)
    {
        List<VRModelV2> list = VRObjectManagerV2.Instance.vrModels;
        string floor = "床材";
        switch (data.type)
        {
            case ETypeMenuNewItem.Out_Side:
                string tree = LoadResourcesData.Instance.listCategoryMenu[(int)ETypeMenuNewItem.Tree].nameCategory;
                string inside = LoadResourcesData.Instance.listCategoryMenu[(int)ETypeMenuNewItem.In_Side].nameCategory;
                List<string> listExpect = new List<string>() {
                    LoadResourcesData.Instance.listCategoryMenu[(int)ETypeMenuNewItem.Floor_Wall].nameCategory,
                    LoadResourcesData.Instance.listCategoryMenu[(int)ETypeMenuNewItem.Car].nameCategory,
                    LoadResourcesData.Instance.listCategoryMenu[(int)ETypeMenuNewItem.Around].nameCategory
                };
                List<VRModelV2> newlist = list.Where(x => !listExpect.Contains(x.DataAsset.CategoryChild)).ToList();
                var newListNoInside = newlist.Where(x => !x.DataAsset.CategoryParent.Contains(inside)).ToList();
                var newListNoInsideAndFloor = newListNoInside.Where(x => !x.DataAsset.CategoryParent.Contains(floor)).ToList();
                return newListNoInsideAndFloor.Where(x => !x.DataAsset.CategoryParent.Contains(tree)).ToList();

            case ETypeMenuNewItem.Tree:
                return list.Where(x => x.DataAsset.CategoryParent.Contains(data.nameCategory)).ToList();//.Select(x => x.DataAsset).ToList();
            case ETypeMenuNewItem.In_Side:
                return list.Where(x => x.DataAsset.CategoryParent.Contains(data.nameCategory)).ToList();
            case ETypeMenuNewItem.Floor_Wall:
                return list.Where(x => x.DataAsset.CategoryParent.Contains(floor)).ToList();
            case ETypeMenuNewItem.Car:
            case ETypeMenuNewItem.Around:
                return list.Where(x => x.DataAsset.CategoryChild == data.nameCategory).ToList();
        }
        return new List<VRModelV2>();
    }
}
