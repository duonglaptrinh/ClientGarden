using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Game.Client;
using Shim.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class OneContentInTotalListAllItem : MonoBehaviour
{
    [SerializeField] Text textTitle;
    [SerializeField] GridLayoutGroup verticalLayout;
    [SerializeField] UITotalItemInRoom prefabOneItemUI;
    const float heightTitle = 50;
    List<VRModelV2> listModel = new List<VRModelV2>();

    List<UITotalItemInRoom> listAllObject = new List<UITotalItemInRoom>();
    public List<string> namesList = new List<string>();
    public List<double> ListTotal = new List<double>();
    Dictionary<string, int> nameCounts = new Dictionary<string, int>();
    private bool isHouse;
    private double _total;
    private double sumTotal;
    private string Price;
    private string PriceHouse;
    private string PrizeUnit = "¥";
    private string NameOfTheHouse;
    [SerializeField] private Text totalQuanlyti;
    [SerializeField] private Text totalPrice;
    [SerializeField] private Toggle toggleCheckPrice;
    public double sumTotalCategory;
    public bool isCheckTotalPrice;
    
    private void Start()
    {
        //QuanlityPriceCategory();
        toggleCheckPrice.onValueChanged.AddListener(CheckTotalPrice);
    }

    public void ClearData()
    {
        foreach (var item in listAllObject)
        {
            Destroy(item.gameObject);
        }
        listAllObject.Clear();
        nameCounts.Clear();
        namesList.Clear();
        ListTotal.Clear();
        sumTotal = 0.0;
    }
    public float Setupdata(MenuNewItemData data)
    {
        listModel = GetListModel(data);
        return Setupdata(data.nameTitle, data.nameCategory);
    }
   
    public float Setupdata(string title, string category)
    {
        isHouse = category == "House";
        int length = listModel.Count;
      
        if (isHouse)
        {
            length = 1;
        }
        for (int i = 0; i < length; i++)
        {
            if (isHouse)
            {
                namesList.Add(VrDomeControllerV2.Instance.DomeLoadhouse.CurrentHouse.NameOfTheHouse);
            }
            else
            {
                VRModelV2 m = listModel[i];
                namesList.Add(m.DataAsset.NameOnApp.Replace("$", " "));
            }
        }
        foreach (string name in namesList)
        {
            if (nameCounts.ContainsKey(name))
            {
                nameCounts[name]++;
            }
            else
            {
                nameCounts[name] = 1;
            }
        }
        namesList.Clear();
        foreach (KeyValuePair<string, int> kvp in nameCounts)
        {
            for (int j = 0; j < kvp.Value; j++)
            {
                namesList.Add(kvp.Key);
            }
        }
        foreach (var name in nameCounts)
        {
            UITotalItemInRoom item = Instantiate(prefabOneItemUI, verticalLayout.transform);
            if (isHouse && name.Value == 1)
            {
                PriceHouse = PrizeUnit + $"{double.Parse(VrDomeControllerV2.Instance.DomeLoadhouse.DataJson.Price):N0}";
                item.SetupHouse(name.Key, name.Value,PriceHouse);
                totalPrice.text = PriceHouse;
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    VRModelV2 m = listModel[i];
                    if (name.Key == m.DataAsset.NameOnApp.Replace("$", " "))
                    {
                        _total = name.Value * m.DataAsset.Prize;
                        Price = PrizeUnit + $"{_total:N0}";
                    }
                }
                ListTotal.Add(_total);
                item.Setup(name.Key,name.Value,Price);
            }
            listAllObject.Add(item);
        }

        textTitle.text = title;
        QuanlityPriceCategory();
        SumTotalCategory();
        if (namesList.Count > 0)
        {
            isCheckTotalPrice = toggleCheckPrice.isOn;
        }
        else
        {
            toggleCheckPrice.isOn = false;
        }

        //caculator Size
        float spacing = verticalLayout.spacing.y;
        RectOffset pading = verticalLayout.padding;
        float height = ((RectTransform)prefabOneItemUI.transform).sizeDelta.y;

        RectTransform rectContent = (RectTransform)verticalLayout.transform;
        float totalHeight = pading.top + pading.bottom + (height + spacing) * nameCounts.Count;
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

    private void QuanlityPriceCategory()
    {
        totalQuanlyti.text = namesList.Count.ToString();
        if (!isHouse)
        {
            if (ListTotal.Count > 0)
            {
                foreach (var value in ListTotal)
                {
                    sumTotal += value;
                    totalPrice.text = PrizeUnit + $"{sumTotal:N0}";
                }
            }
            else
            {
                totalPrice.text = PrizeUnit + 0;
            }
        }
    }

    private void CheckTotalPrice(bool isCheck)
    {
        isCheckTotalPrice = isCheck;
    }

    private void SumTotalCategory()
    {
        sumTotalCategory = isHouse ? double.Parse(VrDomeControllerV2.Instance.DomeLoadhouse.DataJson.Price) : sumTotal;
    }
}
