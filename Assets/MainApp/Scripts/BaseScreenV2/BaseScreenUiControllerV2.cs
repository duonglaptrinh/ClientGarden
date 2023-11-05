using Common.VGS;
using Game.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class BaseScreenUiControllerV2 : SingletonMonoBehavior<BaseScreenUiControllerV2>
{
    [SerializeField] private GameObject editRemote;
    [SerializeField] public MenuListAllItemInRoom menuListAllItemInRoom;
    [SerializeField] private MenuListTotalAllItemInRoom menuListTotalAllItemInRoom;
    [SerializeField] private MenuScreenShot menuScreenShot;
    [SerializeField] GameObject menuUI;
    public bool isCheckOpenInsize = false;
    public bool isCaptureNewView = false;
    public GameObject EditRemote => editRemote;

    public bool IsShow = false;
    public VRObjectManagerV2 VrManager => VRObjectManagerV2.Instance;
    MainMenu mainMenu = null;
    public MainMenu NewMainMenu
    {
        get
        {
            if (mainMenu == null)
            {
                mainMenu = editRemote.GetComponentInChildren<MainMenu>(true);
            }
            return mainMenu;
        }
    }
    private void Awake()
    {
    }

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (VRDomeLoadHouse.IsLoadingHouse) return;
        HideShowMenuButton();
        if (!MenuTabControllerV2.Instance.vrModelSettingDialog.activeSelf)
        {
            if (VrManager.IsAllowShowUIEdit && !PlayerManagerSwitch.isDrag)
                HideShowUiEditButton(true);
            //VrDomeController.Instance.DisableActionsWhenShowMenu(false);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HideShowUiController();
        }

        if (editRemote.activeInHierarchy != IsShow)
        {
            IsShow = editRemote.activeInHierarchy;
            //VrDomeController.Instance.DisableActionsWhenShowMenu(IsShow);
        }

        if ((editRemote.activeSelf ||
            menuListAllItemInRoom.gameObject.activeSelf ||
            menuScreenShot.gameObject.activeSelf ||
            MenuTabControllerV2.Instance.vRObjectTablet.activeSelf ||
            MenuTabControllerV2.Instance.DomeListTablet.activeSelf ||
            MenuTabControllerV2.Instance.SettingTab.activeSelf ||
            MenuTabControllerV2.Instance.ChangeMaterialTab.activeSelf ||
            //MenuTabControllerV2.Instance.TabTutorial.activeSelf ||
            MenuTabControllerV2.Instance.ListHouseTab.activeSelf) &&
            !MenuTabControllerV2.Instance.vrModelSettingDialog.activeSelf)
        {
            HideShowUiEditButton(false);
            //VrDomeController.Instance.DisableActionsWhenShowMenu(true);
        }
        DontDragModel();
    }
    public void HideShowMenuButton()
    {
        if (MenuTabControllerV2.Instance.ChangeMaterialTab.activeSelf ||
            //MenuTabControllerV2.Instance.TabTutorial.activeSelf ||
            MenuTabControllerV2.Instance.ListHouseTab.activeSelf ||
            MenuTabControllerV2.Instance.SettingTab.activeSelf ||
            MenuTabControllerV2.Instance.vRObjectTablet.activeSelf ||
            MenuTabControllerV2.Instance.DomeListTablet.activeSelf ||
            MenuTabControllerV2.Instance.vrModelSettingDialog.activeSelf ||
            menuListAllItemInRoom.gameObject.activeSelf ||
            menuListTotalAllItemInRoom.gameObject.activeSelf||
            menuScreenShot.gameObject.activeSelf ||
            editRemote.activeSelf)
            menuUI.SetActive(false);
        else
            menuUI.SetActive(true);
    }
    public void HideShowUiEditButton(bool status)
    {
        //Tasks 785 Jira
        //foreach (VRModelV2 md in VrManager.vrModels)
        //{
        //    if (md.UiButtonEdit)
        //        md.UiButtonEdit.SetActive(status);
        //    //foreach(Collider corllider in md.gameObject.GetComponentsInChildren<Collider>())
        //    //{
        //    //    corllider.enabled = status;
        //    //}
        //}
    }
    public void DontDragModel()
    {
        bool _isDrag = !((editRemote.activeSelf ||
                        menuListAllItemInRoom.gameObject.activeSelf ||
                        menuScreenShot.gameObject.activeSelf ||
                        MenuTabControllerV2.Instance.vRObjectTablet.activeSelf ||
                        MenuTabControllerV2.Instance.DomeListTablet.activeSelf ||
                        MenuTabControllerV2.Instance.SettingTab.activeSelf ||
                        MenuTabControllerV2.Instance.ChangeMaterialTab.activeSelf ||
                        //MenuTabControllerV2.Instance.TabTutorial.activeSelf ||
                        MenuTabControllerV2.Instance.ListHouseTab.activeSelf) &&
                        !MenuTabControllerV2.Instance.vrModelSettingDialog.activeSelf);

        //Task 785
        //foreach (VRModelV2 md in VrManager.vrModels)
        //{
        //    foreach (Collider corllider in md.gameObject.GetComponentsInChildren<Collider>())
        //    {
        //        corllider.enabled = _isDrag;
        //    }
        //}

    }

    public void ComeBackMainMenu()
    {
        MenuTabControllerV2.Instance.CloseAllTabs();
        editRemote.SetActive(true);
        if (NewMainMenu.menuHouse.gameObject.activeSelf || NewMainMenu.menuNewItem.activeSelf ||
                NewMainMenu.menuSave.gameObject.activeSelf || NewMainMenu.menuTutorial.gameObject.activeSelf)
        {
            NewMainMenu.ResetMenu();
        }
    }

    public void HideShowUiController()
    {
        if (editRemote.activeSelf || 
            menuListAllItemInRoom.gameObject.activeSelf ||
            menuScreenShot.gameObject.activeSelf || 
            MenuTabControllerV2.Instance.vrModelSettingDialog.activeSelf)
        {
            MenuTabControllerV2.Instance.CloseAllTabs();
            editRemote.SetActive(false);
        }
        else if (MenuTabControllerV2.Instance.vRObjectTablet.activeSelf ||
                 MenuTabControllerV2.Instance.SettingTab.activeSelf ||
                 MenuTabControllerV2.Instance.ChangeMaterialTab.activeSelf ||
                 MenuTabControllerV2.Instance.ListHouseTab.activeSelf ||
                 //MenuTabControllerV2.Instance.TabTutorial.activeSelf ||
                 MenuTabControllerV2.Instance.DomeListTablet.activeSelf ||
                 NewMainMenu.menuHouse.gameObject.activeSelf ||
                 NewMainMenu.menuSave.gameObject.activeSelf ||
                 NewMainMenu.menuTutorial.gameObject.activeSelf ||
                 NewMainMenu.menuNewItem.activeSelf)
            ComeBackMainMenu();
        else
        {
            editRemote.SetActive(!editRemote.activeInHierarchy);
            if (editRemote.activeInHierarchy)
                BaseScreenTopMenuV2.Instance.SetCameraRotate(false, false);
            else
                BaseScreenTopMenuV2.Instance.ResetCameraRotate();
        }
    }
    public void HideShowListItemButton()
    {
        if (editRemote.activeSelf ||
            menuListAllItemInRoom.gameObject.activeSelf ||
            menuScreenShot.gameObject.activeSelf ||
            MenuTabControllerV2.Instance.vrModelSettingDialog.activeSelf)
        {
            MenuTabControllerV2.Instance.CloseAllTabs();
            menuListAllItemInRoom.gameObject.SetActive(false);
        }
        else
        {
            menuListAllItemInRoom.gameObject.SetActive(!menuListAllItemInRoom.gameObject.activeInHierarchy);
        }
    }
    public void HideShowCameraButton()
    {
        if (editRemote.activeSelf ||
            menuListAllItemInRoom.gameObject.activeSelf || 
            menuScreenShot.gameObject.activeSelf || 
            MenuTabControllerV2.Instance.vrModelSettingDialog.activeSelf)
        {
            MenuTabControllerV2.Instance.CloseAllTabs();
            menuScreenShot.gameObject.SetActive(false);
        }
        else
        {
            menuScreenShot.gameObject.SetActive(!menuScreenShot.gameObject.activeInHierarchy);
        }
    }
    public void HideUIListItemButton()
    {
        menuListAllItemInRoom.gameObject.SetActive(false);
        menuListTotalAllItemInRoom.gameObject.SetActive(false);
    }
    public void HideUIMenuScreenShot()
    {
        menuScreenShot.gameObject.SetActive(false);
    }
    public void HideUiController()
    {
        IsShow = false;
        editRemote.SetActive(false);
        BaseScreenTopMenuV2.Instance.ResetCameraRotate();
    }
}
