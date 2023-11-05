using Game.Client;
using TWT.Client;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using TWT.Networking;
using Common.VGS;
using TWT.Model;
using jp.co.mirabo.Application.RoomManagement;
using System;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    public const string TEXT_UPLOAD_SCREEN_SHOT = "スクリーンショットがアップロードしています";
    public const string TEXT_LOADING = "読み込み中...";

    public HomeMenuUI menu;
    public MenuHouseUI menuHouse;
    public GameObject menuNewItem;
    public MenuSaveUI menuSave;
    public MenuTutorialUI menuTutorial;
    public MenuSavePositionUI menuSavePositionUI;
    [SerializeField] private GameObject loading;
    [SerializeField] private Text textProgress;

    public Button productBtn;
    public Button exitBtn;
    public Button PlanButton;
    public Button houseButton;
    public Button btnSetting;

    GameObject editRemote => BaseScreenUiControllerV2.Instance.EditRemote;
    MenuTabControllerV2 menuTab => MenuTabControllerV2.Instance;

    public static bool _IsPopup = false;

    [SerializeField] private ContentTitleController contentTitleController;

    //public bool IsNeedResetMenuWhenEnable { get; set; } = true;
    private void OnEnable()
    {
        //if (IsNeedResetMenuWhenEnable)
        if (!BaseScreenUiControllerV2.Instance.isCaptureNewView)
            ResetMenu();
    }
    public void ResetMenu()
    {
        menu.gameObject.SetActive(true);
        menuHouse.gameObject.SetActive(false);
        menuNewItem.SetActive(false);
        menuSave.gameObject.SetActive(false);
        menuTutorial.gameObject.SetActive(false);
        menuSavePositionUI.gameObject.SetActive(false);
    }
    public void HideAllMenu()
    {
        menu.gameObject.SetActive(false);
        menuHouse.gameObject.SetActive(false);
        menuNewItem.SetActive(false);
        menuSave.gameObject.SetActive(false);
        menuTutorial.gameObject.SetActive(false);
        menuSavePositionUI.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        ResetMenu();

        productBtn.onClick.AddListener(OnVrObjectClick);
        exitBtn.onClick.AddListener(OnExitClick);
        btnSetting.onClick.AddListener(OnSettingClick);
        houseButton.onClick.AddListener(OnHouseButtonClick);

        PlanButton.onClick.AddListener(OnDomeClick);
        menuSave.OnClickChangeDome = OnClickChangeDome;

        menuHouse.OnClickBtnListHouse = ShowListHouse;
        menuHouse.OnClickBtnMaterial = ShowMenuChangeMaterial;
        menuHouse.OnCloseMenu = OnCloseMainMenu;

        menuTutorial.OnClickViewContent = ShowMenuTabTutorial;
        menuTutorial.OnCloseMenu = OnCloseMainMenu;
        menuTutorial.OnBackMenu = OnBackToMainMenu;

        menuSavePositionUI.OnCloseMenu = OnCloseMainMenu;
        menuSavePositionUI.OnBackMenu = OnBackToMainMenu;
    }
    public void OnOffEditMode(bool isActive)
    {
        menu.OnOffEditMode(isActive);
    }
    public void OnSettingClick()
    {
        menuTab.ShowSettingTab();
        OnCloseMainMenu();
    }
    public void ForceShowHideHouseMenu(bool isShowMenu)
    {
        editRemote.gameObject.SetActive(isShowMenu);
        ShowHideHouseMenu(true);
    }
    public void ShowHideHouseMenu(bool isShowMenu)
    {
        HideAllMenu();
        menuHouse.gameObject.SetActive(isShowMenu);
    }
    public void ShowHideMenuNewItem(bool isShowMenu)
    {
        HideAllMenu();
        menuNewItem.SetActive(isShowMenu);
    }
    public void ShowHideMenuSave(bool isShowMenu)
    {
        HideAllMenu();
        menuSave.gameObject.SetActive(isShowMenu);
    }
    public void OnBackToMainMenu()
    {
        menu.gameObject.SetActive(true);
        menuHouse.gameObject.SetActive(false);
        menuNewItem.SetActive(false);
        menuSave.gameObject.SetActive(false);
        menuTutorial.gameObject.SetActive(false);
        menuSavePositionUI.gameObject.SetActive(false);
        BaseScreenUiControllerV2.Instance.isCaptureNewView = false;
    }
    public void OnCloseMainMenu()
    {
        //DebugExtension.LogError("" + editRemote + "  " + menu + " " + menuHouse + " " + menuNewItem + " " + menuSave + " " + menuTutorial);
        editRemote.SetActive(false);
        menu.gameObject.SetActive(true);
        menuHouse.gameObject.SetActive(false);
        menuNewItem.SetActive(false);
        menuSave.gameObject.SetActive(false);
        menuTutorial.gameObject.SetActive(false);
        menuSavePositionUI.gameObject.SetActive(false);
        BaseScreenUiControllerV2.Instance.isCaptureNewView = false;
    }
    private void OnClickChangeDome()
    {
        editRemote.SetActive(true);
        menu.gameObject.SetActive(false);
        menuHouse.gameObject.SetActive(false);
        menuNewItem.SetActive(false);
        menuSave.gameObject.SetActive(true);
        menuTutorial.gameObject.SetActive(false);
        menuSavePositionUI.gameObject.SetActive(false);
        BaseScreenUiControllerV2.Instance.isCaptureNewView = false;
    }

    public void OnDomeClick()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            menuTab.ShowDomeTab();
            OnCloseMainMenu();
        }
    }

    public void ShowMenuTutorial()
    {
        HideAllMenu();
        menuTutorial.gameObject.SetActive(true);
    }

    public void ShowMenuSavePosition()
    {
        HideAllMenu();
        menuSavePositionUI.gameObject.SetActive(true);
    }
    public void ShowMenuTabTutorial(TutorialData data)
    {
        menuTab.ShowMenuTabTutorial(data);
        OnCloseMainMenu();
    }

    #region MENU HOUSE
    void OnHouseButtonClick()
    {
        ShowHideHouseMenu(isShowMenu: true);
    }
    public void ShowMenuChangeMaterial()
    {
        menuTab.ShowChangeMaterialTab();
        OnCloseMainMenu();
    }
    public void ShowListHouse()
    {
        menuTab.ShowListHouseTab();
        OnCloseMainMenu();
    }
    #endregion END MENU HOUSE

    #region MENU NEW ITEM
    public void OnVrObjectClick()
    {
        ShowHideMenuNewItem(isShowMenu: true);
    }
    public void ShowVRObjectByType(ETypeMenuNewItem type)
    {
        menuTab.ShowVRObjectTab(type);
        OnCloseMainMenu();
    }
    public void ShowFloor()
    {
        ShowVRObjectByType(ETypeMenuNewItem.Floor_Wall);
    }
    public void ShowOutSide()
    {
        ShowVRObjectByType(ETypeMenuNewItem.Out_Side);
    }
    public void ShowInSide()
    {
        ShowVRObjectByType(ETypeMenuNewItem.In_Side);
    }
    public void ShowTree()
    {
        ShowVRObjectByType(ETypeMenuNewItem.Tree);
    }
    public void ShowCar()
    {
        ShowVRObjectByType(ETypeMenuNewItem.Car);
    }
    public void ShowAround()
    {
        ShowVRObjectByType(ETypeMenuNewItem.Around);
    }
    #endregion END NEW MENU ITEM

    public void OnExitClick()
    {
        if (PopupRuntimeManager._IsPopup) return;
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            PopupRuntimeManager.Instance.ShowPopup("退出しますか？",
            onClickConfirm: () =>
            {
                RoomManager.Instance.Disconnect();
                SceneConfig.LoadScene(SceneConfig.Scene.TitleScreenV2);
            }, null);
        }
    }

    public static void SaveData(int id_url)
    {
        Action OnSave = async () =>
        {
            await SaveNewModelAndSync();
            VrgSyncApi.Send(new SaveJsonMessage()
            {
                idDome = GameContext.CurrentIdDome,
                isNeedUpdateTransform = false,
                rotation = VRObjectManagerV2.ConvertVector3ToString(Vector3.zero),
                scale = VRObjectManagerV2.ConvertVector3ToString(Vector3.zero),
                translate = VRObjectManagerV2.ConvertVector3ToString(Vector3.zero),
                id_url = id_url
            }, SaveJsonMessage.EventKey);
            DebugExtension.Log("Save Data !!");
        };
        //if (!isNeedAsk)
        //{
        OnSave.Invoke();
        //    return;
        //}
        //if (PopupRuntimeManager._IsPopup) return;
        //PopupRuntimeManager.Instance.ShowPopup("保存しますか？", onClickConfirm: OnSave);

    }

    public void SaveDataT()
    {
        ShowHideMenuSave(true);
        //SaveData();
    }

    public void SaveStartPoint()
    {
        if (PopupRuntimeManager._IsPopup) return;
        PopupRuntimeManager.Instance.ShowPopup("開始地点を保存しますか？",
          onClickConfirm: async () =>
          {
              await SaveNewModelAndSync();
              string rotation = VRObjectManagerV2.ConvertVector3ToString(Camera.main.transform.parent.eulerAngles);
              string scale = VRObjectManagerV2.ConvertVector3ToString(Camera.main.transform.parent.localScale);
              string translate = VRObjectManagerV2.ConvertVector3ToString(Camera.main.transform.parent.position);
              VrgSyncApi.Send(new SaveJsonMessage()
              {
                  idDome = GameContext.CurrentIdDome,
                  isNeedUpdateTransform = true,
                  rotation = rotation,
                  scale = scale,
                  translate = translate,
                  id_url = VrDomeControllerV2.Instance.vrDomeData.id_url
              }, SaveJsonMessage.EventKey);

              //Update local
              GameContext.ContentDataCurrent.UpdateStartPointData(GameContext.CurrentIdDome, translate, rotation, scale);
              DebugExtension.Log("Save Data Point !!" + Camera.main.transform.parent.eulerAngles);
          });
    }

    static async UniTask SaveNewModelAndSync()
    {
        List<VRModelV2> list = VRObjectManagerV2.Instance.vrModels;

        foreach (var item in GameContext.ContentDataCurrent.vr_dome_list)
        {
            if (item.dome_id == GameContext.CurrentIdDome)
            {
                var needToDestroyItems = new List<VRModelV2>();

                foreach (var modelObject in list)
                {
                    if (modelObject.IsJustCreatNew)
                    {
                        modelObject.SetJustCreateNew(false);
                        var data = modelObject.GetData();
                        data.model_rotation = VRObjectManagerV2.ConvertVector3ToString(modelObject.transform.localEulerAngles);
                        data.model_scale = VRObjectManagerV2.ConvertVector3ToString(modelObject.transform.localScale);
                        data.model_translate = VRObjectManagerV2.ConvertVector3ToString(modelObject.transform.localPosition);
                        VRModelData.SendDataCreateNewModel(data, item.dome_id);
                        DebugExtension.Log("Save Model ID = " + data.model_id);

                        needToDestroyItems.Add(modelObject);
                    }
                }

                foreach (var needToDestroyItem in needToDestroyItems)
                {
                    VRObjectManagerV2.Instance?.RemoveVrModel(needToDestroyItem);
                }
            }
            else
            {
                //Check and send All Data model
                item.SendAllDataModelOtherDome(item.dome_id);
            }
            //Clear Data Model after send
            item.ClearAllNewModel();
        }

        await UniTask.Delay(200);
    }

    public void ShowLoading(string loadingText)
    {
        loading.SetActive(true);
        textProgress.text = loadingText;

        //Invoke(nameof(HideLoading), displayTime);
    }

    public void HideLoading()
    {
        loading.gameObject.SetActive(false);
    }

    public void UpdateTextLand()
    {
        menuHouse.UpdateTextLand();
    }
    // Update is called once per frame
    void Update()
    {

    }
    //private void OnEnable()
    //{
    //    PlayerController.isShowMenu = true;
    //}
    //private void OnDisable()
    //{
    //    PlayerController.isShowMenu = false;
    //}
}
