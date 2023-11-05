using Shim.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UIScroll;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class MenuModels : MonoBehaviour
{
    // Start is called before the first frame update
    public Action<CategoryDataAsset, string, string> OnClickInformationModel = null;
    [SerializeField] UIScrollGrid scrollObject;
    CategoryDataAsset categoryData;
    [SerializeField] CategoryProducts categoryProduct;
    [SerializeField] InformationModel informationModel;

    string nameSubCategory;
    int indexModel = 0;
   
    private void Awake()
    {
        scrollObject.OnCreateOneItem = CreateOneItemObject;
        scrollObject.OnClickOneItem = OnClickItemObject;
    }
    void Start()
    {
        //informationModel = gameObject.transform.parent.GetComponentInChildren<InformationModel>(true);
    }
    public void LoadModel(CategoryDataAsset category, string subName)
    {
        RectTransformExtensions.SetLeft(scrollObject.Scroll.content, 0);
        RectTransformExtensions.SetRight(scrollObject.Scroll.content, 0);
        Vector3 pos = scrollObject.Scroll.content.localPosition;
        pos.y = 0;
        scrollObject.Scroll.content.localPosition = pos;

        this.categoryData = category;
        nameSubCategory = subName;
        List<OneModelAssetsData> listModel = AddressableDownloadManager.ResourcesData.GetSubCategoryDataAsset(categoryData, nameSubCategory);
        List<ItemDataBase> list = new List<ItemDataBase>();
        if (listModel.Count == 0)
        {
            scrollObject.ClearAll();
        }
        else
        {
            for (int i = 0; i < listModel.Count; i++)
            {
                list.Add(new ItemDataModel(listModel[i]));
                //list.Add(LoadResourcesData.Instance.GetDataSubCategoryByName(categoryThumbnail, listSub[i]));
            }
            if (list.Count > 0)
            {
                scrollObject.ClearAll();
                scrollObject.Initialize(list, 2);
                //LoadMaterialsItem(modelcontroller.ListMaterialSets[0]);
                //selectItemObject = (UIScrollItemGrid)scrollObject.ListItems[0];
                //selectItemObject.Select();
            }
        }
    }
    protected virtual void CreateOneItemObject(UIScrollItemBase item)
    {
        UIItemModel itemgrid = (UIItemModel)item;
        ItemDataModel data = (ItemDataModel)itemgrid.CurrentData;
        itemgrid.SetDataStart(data.Data.NameOnApp, data.Data.pathThumb);
    }

    protected virtual void OnClickItemObject(int index)
    {
        UIItemModel itemgrid = (UIItemModel)scrollObject.ListItems[index];
        ItemDataModel data = (ItemDataModel)itemgrid.CurrentData;
        categoryProduct.OnClickMenuModel();

        informationModel.LoadDetail(data.Data);

        indexModel = index;
    }
    // Update is called once per frame

    void Update()
    {

    }
}
