using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UIScroll;
using UnityEngine;

public class SubCategoryProducts : MonoBehaviour
{
    public Action<CategoryDataAsset, string, string, bool> OnClickSubCategory = null;
    [SerializeField] UIScrollBase scrollObject;
    CategoryDataAsset categoryData;
    // Start is called before the first frame update
    private void Awake()
    {
        scrollObject.OnCreateOneItem = CreateOneItemObject;
        scrollObject.OnClickOneItem = OnClickItemObject;
    }
    void Start()
    {
    }
    public void LoadSubCategory(string categoryName)
    {
        categoryData = AddressableDownloadManager.ResourcesData.GetCategoryDataAsset(categoryName);
        List<string> listSub = AddressableDownloadManager.ResourcesData.GetNameSubCategoryDataAsset(categoryData);
        //List<OneModelAssetsData> listModel = AddressableDownloadManager.ResourcesData.GetSubCategoryDataAsset(categoryData, listSub[0]);

        ItemDataCategory categoryThumbnail = LoadResourcesData.Instance.GetDataCategoryByName(categoryName);

        List<ItemDataBase> list = new List<ItemDataBase>();

        for (int i = 0; i < categoryThumbnail.listSubCategoryData.Count; i++)
        {
            list.Add(categoryThumbnail.listSubCategoryData[i]);
            //list.Add(LoadResourcesData.Instance.GetDataSubCategoryByName(categoryThumbnail, listSub[i]));
        }
        if (list.Count > 0)
        {
            scrollObject.Initialize(list);
            //LoadMaterialsItem(modelcontroller.ListMaterialSets[0]);
            //selectItemObject = (UIScrollItemGrid)scrollObject.ListItems[0];
            //selectItemObject.Select();
        }

    }
    protected virtual void CreateOneItemObject(UIScrollItemBase item)
    {
        UIItemSubCategory itemgrid = (UIItemSubCategory)item;
        ItemDataSubCategory data = (ItemDataSubCategory)itemgrid.CurrentData;
        var listAssetModel =
            AddressableDownloadManager.ResourcesData.GetTotalCategory(categoryData.NameCategory, data.categorySubName);
        if (string.IsNullOrEmpty(data.categoryNameOnApp))
            itemgrid.SetDataStart(data.categorySubName, data.thumbnai,listAssetModel.ToString(CultureInfo.InvariantCulture));
        else
            itemgrid.SetDataStart(data.categoryNameOnApp, data.thumbnai,listAssetModel.ToString(CultureInfo.InvariantCulture));
    }

    protected virtual void OnClickItemObject(int index)
    {
        UIItemSubCategory itemgrid = (UIItemSubCategory)scrollObject.ListItems[index];
        ItemDataSubCategory data = (ItemDataSubCategory)itemgrid.CurrentData;
        OnClickSubCategory?.Invoke(categoryData, data.categorySubName, data.categoryNameOnApp, false);
        //if (selectItemObject == itemgrid) return;
        //if (selectItemObject) selectItemObject.UnSelect();
        //selectItemObject = itemgrid;
        //selectItemObject.Select();

        //LoadMaterialsItem(data.Data);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
