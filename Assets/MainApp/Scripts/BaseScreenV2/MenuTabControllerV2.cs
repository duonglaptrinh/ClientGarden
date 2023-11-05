using Game.Client;
using System.Collections;
using System.Collections.Generic;
using TWT.Client;
using UnityEngine;
using UnityEngine.UI;

public class MenuTabControllerV2 : MonoBehaviour
{
    public static MenuTabControllerV2 Instance => BaseScreenCtrlV2.Instance.MenuTabControllerV2;
    [SerializeField] GameObject domeListTablet;
    [SerializeField] GameObject vrObjectTab;
    [SerializeField] GameObject settingTab;
    [SerializeField] GameObject changeMaterialTab;
    [SerializeField] GameObject listHouseTab;
    [SerializeField] public VrModelSettingDialog vrModelSettingTab;
    [SerializeField] MenuTabTutorial menuTabTutorial;
    public GameObject DomeListTablet => domeListTablet;
    public GameObject vrModelSettingDialog => vrModelSettingTab.gameObject;
    public GameObject vRObjectTablet => vrObjectTab;
    public GameObject SettingTab => settingTab;
    public GameObject ChangeMaterialTab => changeMaterialTab;
    public GameObject ListHouseTab => listHouseTab;
    public GameObject TabTutorial => menuTabTutorial.gameObject;
    public float menuTabDistance = 1.5f;

    public VrObjectEditSelectHelperV2 GetObjectSelected()
    {
        return vrModelSettingTab.currentEditedVRModel.gameObject.GetComponent<VrObjectEditSelectHelperV2>();
    }

    public bool IsPopupShowing()
    {
        if (domeListTablet.activeSelf ||
        vrObjectTab.activeSelf ||
        settingTab.activeSelf ||
        changeMaterialTab.activeSelf ||
        listHouseTab.activeSelf ||
        TabTutorial.activeSelf ||
        vrModelSettingTab.gameObject.activeSelf)
            return true;

        return false;
    }
    private void Awake()
    {

    }
    private void Start()
    {
        if (OSPlatform.Platform == EOSPlatform.Android_Occulus_Quest)
        {
            SettingTabTest();
        }
    }

    void SettingTabTest()
    {
#if UNITY_EDITOR
        vrObjectTab.transform.localScale = Vector3.one * 2;
        settingTab.transform.localScale = Vector3.one * 2;
        vrModelSettingTab.transform.localScale = Vector3.one * 2;
#endif

    }
    public void CloseAllTabs()
    {
        BaseScreenTopMenuV2.Instance.SetCameraRotate(false, false);
        vrObjectTab.SetActive(false);
        settingTab.SetActive(false);
        changeMaterialTab.SetActive(false);
        listHouseTab.SetActive(false);
        vrModelSettingDialog.SetActive(false);
        domeListTablet.SetActive(false);
        //TabTutorial.SetActive(false);
    }

    void UpdateMenuPosition()
    {
        //transform.position = Camera.main.transform.position + Camera.main.transform.forward * menuTabDistance;
        //transform.rotation = Quaternion.Euler(new Vector3(Camera.main.transform.rotation.eulerAngles.x, Camera.main.transform.rotation.eulerAngles.y, 0));
    }
    public void ShowDomeTab()
    {
        UpdateMenuPosition();
        CloseAllTabs();
        domeListTablet.SetActive(true);
    }
    public void ShowVRObjectTab(ETypeMenuNewItem type)
    {
        UpdateMenuPosition();
        CloseAllTabs();
        vrObjectTab.SetActive(true);
        vrObjectTab.GetComponent<CategoryProducts>().ShowCategoryByNewItem(type);
    }

    public void ShowSettingTab()
    {
        UpdateMenuPosition();
        CloseAllTabs();
        settingTab.SetActive(true);
    }
    public void ShowChangeMaterialTab()
    {
        UpdateMenuPosition();
        CloseAllTabs();
        changeMaterialTab.SetActive(true);
    }
    public void ShowListHouseTab()
    {
        UpdateMenuPosition();
        CloseAllTabs();
        listHouseTab.SetActive(true);
    }
    public void ShowMenuTabTutorial(TutorialData data)
    {
        UpdateMenuPosition();
        CloseAllTabs();
        TabTutorial.SetActive(true);
        menuTabTutorial.LoadData(data);
    }

    public void ShowVrModelSettingDialog(VRModelV2 vrModel, OneModelAssetsData currentModel)
    {
        UpdateMenuPosition();
        CloseAllTabs();
        vrModelSettingTab.gameObject.SetActive(true);
        vrModelSettingTab.GetComponent<VrModelSettingDialog>().SetVRModelToEdit(vrModel, currentModel);
        BaseScreenUiControllerV2.Instance.NewMainMenu.OnCloseMainMenu();
        BaseScreenUiControllerV2.Instance.HideUIListItemButton();
        BaseScreenUiControllerV2.Instance.HideUIMenuScreenShot();
    }

    public void ShowVRObjectSettingTab(VRObjectV2 settingObject)
    {
        //UpdateMenuPosition();
        //CloseAllTabs();
        if (VRObjectTransparentSettingController.instance)
            VRObjectTransparentSettingController.instance.OpenWithVRobject(settingObject);
    }
}
