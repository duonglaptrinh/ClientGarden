using Common.VGS;
using Game.Client;

using Player_Management;
using System.Collections;
using System.Collections.Generic;
using TWT.Networking;
using UnityEngine;
using UnityEngine.UI;

public class ListHouseMenu : MonoBehaviour
{
    [SerializeField] UIScrollBase scrollObject;
    [SerializeField] Button backBtn;
    List<HouseDataAsset> houseData;
    MainMenu newMainMenu;

    // Start is called before the first frame update
    private void Awake()
    {
        scrollObject.OnCreateOneItem = CreateOneItemObject;
        scrollObject.OnClickOneItem = OnClickItemObject;
        backBtn.onClick.AddListener(BackToHouseMenu);
    }
    public void BackToHouseMenu()
    {
        BaseScreenUiControllerV2.Instance.EditRemote.SetActive(true);
        newMainMenu.menu.gameObject.SetActive(false);
        newMainMenu.menuHouse.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
    void Start()
    {
        newMainMenu = BaseScreenUiControllerV2.Instance.NewMainMenu;
        houseData = AddressableDownloadManager.ResourcesData.ListPathSceneHouseAddressable;
        LoadSubCategory();
    }
    public void LoadSubCategory()
    {
        List<ItemDataBase> list = new List<ItemDataBase>();

        for (int i = 0; i < houseData.Count; i++)
        {
            list.Add(houseData[i]);
        }
        if (list.Count > 0)
        {
            scrollObject.Initialize(list);
        }

    }
    protected virtual void CreateOneItemObject(UIScrollItemBase item)
    {
        UIItemModel itemgrid = (UIItemModel)item;
        HouseDataAsset data = (HouseDataAsset)itemgrid.CurrentData;
        itemgrid.SetDataStart(data.NameOfTheHouse, data.PathThumbnail);
    }

    protected virtual void OnClickItemObject(int index)
    {
        UIItemModel itemgrid = (UIItemModel)scrollObject.ListItems[index];
        HouseDataAsset data = (HouseDataAsset)itemgrid.CurrentData;
        VRDomeLoadHouse loadModel = VrDomeControllerV2.Instance.GetComponent<VRDomeLoadHouse>();

        if (loadModel.CurrentData.indexHouse == itemgrid.MyIndex) return;

        //VrDomeControllerV2.Instance.vrDomeData.modelData.UpdateIndexHouse(itemgrid.MyIndex);
        loadModel.SetLoading();
        //VrDomeControllerV2.Instance.DomeLoadhouse.LoadNewSceneSyncFromServer();

        HouseManagerSync.Instance.UpdateEntity(
            isUpdateHouse: true,
            indexHouse: itemgrid.MyIndex,
            0, 0);
        //NetworkClient.Send(new SyncMaterialHouseMessage()
        //{
        //    isUpdateHouse = true,
        //    idDome = VrDomeControllerV2.Instance.vrDomeData.dome_id,
        //    indexHouse = itemgrid.MyIndex,
        //    indexMaterialSet = 0,
        //    indexMaterialDetail = 0
        //});
        //gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        PlayerManagerSwitch.isShowMenu = false;
    }
    private void OnEnable()
    {
        PlayerManagerSwitch.isShowMenu = true;

    }

}
