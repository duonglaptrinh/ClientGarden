using System.Collections;
using System.Collections.Generic;
using UIScroll;
using UnityEngine;
using UnityEngine.UI;
using Outline = ThirdOutline.Outline;
using Common.VGS;
using TWT.Networking;

using Game.Client;
using Player_Management;

public class MenuChangeMaterial : MonoBehaviour
{
    [SerializeField] Button backBtn;
    [SerializeField] UIScrollGrid scrollObject;
    [SerializeField] UIScrollGrid scrollMaterial;
    MainMenu newMainMenu;
    MaterialController modelcontroller;
    UIScrollItemGrid selectItemObject;
    UIScrollItemGrid selectItem;
    List<Outline> listOutline = new List<Outline>();
    VRDomeLoadHouse loadModel;
    // Start is called before the first frame update
    void Awake()
    {
        loadModel = VrDomeControllerV2.Instance.DomeLoadhouse;
        scrollObject.OnCreateOneItem = CreateOneItemObject;
        scrollObject.OnClickOneItem = OnClickItemObject;

        scrollMaterial.OnCreateOneItem = CreateOneItemMaterial;
        scrollMaterial.OnClickOneItem = OnClickItemMaterial;
    }
    void Start()
    {
        //LoadObject();
        newMainMenu = BaseScreenUiControllerV2.Instance.NewMainMenu;
        backBtn.onClick.AddListener(BackToHouseMenu);
    }
    public void BackToHouseMenu()
    {
        BaseScreenUiControllerV2.Instance.EditRemote.SetActive(true);
        newMainMenu.menu.gameObject.SetActive(false);
        newMainMenu.menuHouse.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    protected virtual void CreateOneItemObject(UIScrollItemBase item)
    {
        UIScrollItemGrid itemgrid = (UIScrollItemGrid)item;
        ItemDataMaterialSet data = (ItemDataMaterialSet)itemgrid.CurrentData;
        Material m = data.Data.targetDefault.GetComponent<Renderer>().material;
        itemgrid.UnSelect();
        itemgrid.Setup(data.Data.ItemName, (Texture2D)m.mainTexture);
    }

    protected virtual void OnClickItemObject(int index)
    {
        UIScrollItemGrid itemgrid = (UIScrollItemGrid)scrollObject.ListItems[index];

        if (selectItemObject == itemgrid) return;
        if (selectItemObject) selectItemObject.UnSelect();
        selectItemObject = itemgrid;
        selectItemObject.Select();

        ItemDataMaterialSet data = (ItemDataMaterialSet)itemgrid.CurrentData;
        LoadMaterialsItem(data.Data);
    }

    protected virtual void CreateOneItemMaterial(UIScrollItemBase item)
    {
        UIScrollItemGrid itemgrid = (UIScrollItemGrid)item;
        ItemDataMaterialSetDetail data = (ItemDataMaterialSetDetail)itemgrid.CurrentData;
        itemgrid.UnSelect();
        itemgrid.LoadIcon(data.Data.Icon);
    }

    protected virtual void OnClickItemMaterial(int index)
    {
        UIScrollItemGrid itemgrid = (UIScrollItemGrid)scrollMaterial.ListItems[index];

        if (selectItem == itemgrid) return;
        if (selectItem) selectItem.UnSelect();
        selectItem = itemgrid;
        selectItem.Select();

        ItemDataMaterialSetDetail data = (ItemDataMaterialSetDetail)itemgrid.CurrentData;
        SetMaterialToObject(data);

        if (selectItemObject)
        {
            Material m = data.DataSet.targetDefault.GetComponent<Renderer>().material;
            selectItemObject.SetIcon((Texture2D)m.mainTexture, m.color);

            if (loadModel.CurrentData == null) return;

            //VrDomeController.Instance.vrDomeData.modelData.UpdateListMaterialSet(
            //    loadModel.CurrentData.indexHouse,
            //    new MaterialDataNetWorkDetail(selectItemObject.MyIndex, index)
            //    );

            HouseManagerSync.Instance.UpdateEntity(
                isUpdateHouse: false,
                indexHouse: loadModel.CurrentData.indexHouse,
                indexMaterialSet: selectItemObject.MyIndex,
                indexMaterialDetail: index);

            //NetworkClient.Send(new SyncMaterialHouseMessage()
            //{
            //    isUpdateHouse = false,
            //    idDome = VrDomeController.Instance.vrDomeData.dome_id,
            //    indexHouse = loadModel.CurrentData.indexHouse,
            //    indexMaterialSet = selectItemObject.MyIndex,
            //    indexMaterialDetail = index
            //});
        }
    }

    public void LoadObject()
    {
        List<ItemDataBase> list = new List<ItemDataBase>();

        foreach (var item in modelcontroller.ListMaterialSets)
        {
            list.Add(new ItemDataMaterialSet(item));
        }
        if (list.Count > 0)
        {
            scrollObject.Initialize(list, 1);
            DebugExtension.Log("Load Done");
            LoadFirstItem();
        }
    }
    void LoadFirstItem()
    {
        //DebugExtension.LogError("Load sdfsd");
        LoadMaterialsItem(modelcontroller.ListMaterialSets[0]);
        selectItemObject = (UIScrollItemGrid)scrollObject.ListItems[0];
        selectItemObject.Select();
    }
    public void LoadMaterialsItem(MaterialSet itemMaterialSet)
    {
        if (selectItem) selectItem.UnSelect();
        selectItem = null;
        List<ItemDataBase> list = new List<ItemDataBase>();

        foreach (var item in itemMaterialSet.ListMaterialsData)
        {
            list.Add(new ItemDataMaterialSetDetail(itemMaterialSet.ItemName, item, itemMaterialSet));
        }
        if (list.Count > 0)
        {
            scrollMaterial.Initialize(list);
            Material matCurrent = itemMaterialSet.targetDefault.GetComponent<Renderer>().material;
            for (int i = 0; i < itemMaterialSet.ListMaterialsData.Count; i++)
            {
                if (matCurrent.name.Replace(" (Instance)", "") == itemMaterialSet.ListMaterialsData[i].Material.name)
                {
                    selectItem = (UIScrollItemGrid)scrollMaterial.ListItems[i];
                    selectItem.Select();
                    break;
                }
            }
        }

        SetOutlineToObject(itemMaterialSet);
    }

    public void SetMaterialToObject(ItemDataMaterialSetDetail detail)
    {
        foreach (var item in detail.DataSet.TargetObjects)
        {
            item.GetComponent<Renderer>().material = detail.Data.Material;
            //Material[] materials = item.GetComponent<Renderer>().materials;
            //foreach (var mat in materials)
            //{
            //}
        }
    }
    public void SetOutlineToObject(MaterialSet itemMaterialSet)
    {
        RemoveOutLine();
        foreach (var item in itemMaterialSet.TargetObjects)
        {
            SetOutline(item.GetComponent<Renderer>());
        }
    }
    void SetOutline(Renderer renderer)
    {
        Outline outline = renderer.GetComponent<Outline>();
        if (!outline) outline = renderer.gameObject.AddComponent<Outline>();
        outline.OutlineWidth = 12;
        outline.OutlineColor = Color.yellow;
        listOutline.Add(outline);
    }
    void RemoveOutLine()
    {
        foreach (var item in listOutline)
        {
            Destroy(item);
        }
        listOutline.Clear();
    }
    void OnDisable()
    {
        PlayerManagerSwitch.isShowMenu = false;
        RemoveOutLine();
        if (selectItemObject) selectItemObject.UnSelect();
        selectItemObject = null;
        if (selectItem) selectItem.UnSelect();
        selectItem = null;
    }
    private void OnEnable()
    {
        GetHouseModel();
        PlayerManagerSwitch.isShowMenu = true;
        if (modelcontroller)
            LoadObject();
        else
        {
            scrollObject.ClearAll();
            scrollMaterial.ClearAll();
        }
        //if (scrollObject.ListDatas.Count > 0)
        //{
        //    foreach (var item in scrollObject.ListItems)
        //    {
        //        item.UnSelect();
        //    }
        //    foreach (var item in scrollMaterial.ListItems)
        //    {
        //        item.UnSelect();
        //    }
        //    LoadFirstItem();
        //}
    }
    void GetHouseModel()
    {
        modelcontroller = loadModel.Modelcontroller;
    }
}
