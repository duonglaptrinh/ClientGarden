using Cysharp.Threading.Tasks;
using Game.Client;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TWT.Model;
using TWT.Networking;
using UniRx;
using UnityEngine;

public class DomeListSelect : CommonDialog
{
    [SerializeField] UIScrollBase scrollObject;
    [SerializeField] GameObject loading;
    [SerializeField] GameObject menuPlan;
    [SerializeField] InformationPlanTemplate menuInfomation;

    VRPlanDataTemplate selectedDomeData;
    public static int currentDomeIdPlayerIn;
    public ISubject<VRDomeData> OnDomeSelected { get; } = new Subject<VRDomeData>();

    void Awake()
    {
        scrollObject.OnCreateOneItem = CreateOneItemObject;
        scrollObject.OnClickOneItem = OnClickItemObject;

    }
    protected virtual void CreateOneItemObject(UIScrollItemBase item)
    {
        ItemDataBaseDome data = (ItemDataBaseDome)item.CurrentData;
        item.GetComponent<VRPlanTemplateItem>().SetDome(item.MyIndex, data.planDataTemplate,
               onSelected: (data, sprite) =>
               {
                   selectedDomeData = data;

                   ShowInfomation(item.MyIndex, data);
                   //OnDomeSelected.OnNext(selectedDomeData);

                   //PopupRuntimeManager.Instance.ShowPopup("選択したプランに移動しますか",//"選択したビューへ移動しますか？",
                   //    onClickConfirm: () =>
                   //    {
                   //        OnCloseClick();
                   //        BaseScreenTopMenuV2.Instance.ResetCameraRotate();
                   //        var message = new VrArrowNextDomeMessage()
                   //        {
                   //            DomeId = selectedDomeData.dome_id,
                   //        };

                   //        //Sync
                   //        VrgSyncApi.Send(message, VrArrowNextDomeMessage.EventKey);
                   //    },
                   //    onClickCancel: () =>
                   //    {
                   //        foreach (var vrObject in FindObjectOfType<VRObjectManagerV2>().VrObjects)
                   //        {
                   //            vrObject.ShowMenuUiEdit();
                   //        }
                   //    });
               });
    }

    protected virtual void OnClickItemObject(int index)
    {
        //UIScrollItemBase item = scrollObject.ListItems[index];
        //ItemDataBaseDome data = (ItemDataBaseDome)item.CurrentData;
    }

    void ShowInfomation(int index, VRPlanDataTemplate plan)
    {
        menuPlan.gameObject.SetActive(false);
        menuInfomation.gameObject.SetActive(true);
        menuInfomation.LoadDetail(index, plan);
    }
    void Start()
    {
        menuInfomation.OnBack = OnInfomationBack;
        menuInfomation.OnClose = OnInfomationClose;
        menuInfomation.OnCreatePlan = ApplyPlanTemplate;
    }

    void OnEnable()
    {
        ShowMenuPlan();
        LoadDome();
        BaseScreenDataManager.OnDeleteDomeData += LoadDome;
        BaseScreenDataManager.OnChangeDome += LoadDome;
    }

    void OnDisable()
    {
        BaseScreenDataManager.OnDeleteDomeData -= LoadDome;
        BaseScreenDataManager.OnChangeDome -= LoadDome;
    }

    public void LoadDomeTemplate()
    {
        VRPlanDataTemplate[] domes = GameContext.ContentDataCurrentTemplate.vr_dome_list;
        List<ItemDataBase> list = new List<ItemDataBase>();

        for (int i = 0; i < domes.Length; i++)
        {
            list.Add(new ItemDataBaseDome(domes[i]));
        }
        if (list.Count > 0)
        {
            scrollObject.Initialize(list);
        }
    }
    void LoadDome()
    {
        Action<VRContentDataTemplate> OnContinue = data =>
        {
            loading.gameObject.SetActive(false);
            // menuPlan.gameObject.SetActive(true);
            // menuInfomation.gameObject.SetActive(false);
            LoadDomeTemplate();
        };
        if (GameContext.ContentDataCurrentTemplate == null)
        {
            loading.gameObject.SetActive(true);
            string roomId = PlayerPrefs.GetString(PlayerPrefsConst.ROOM_ID, "");
            ConnectServer.Instance.GetJsonTemplate(roomId, json =>
            {
                GameContext.ContentDataCurrentTemplate = JsonUtility.FromJson<VRContentDataTemplate>(json);
                OnContinue.Invoke(GameContext.ContentDataCurrentTemplate);
            });
        }
        else { OnContinue.Invoke(GameContext.ContentDataCurrentTemplate); }

    }
    public override void OnCloseClick()
    {
        gameObject.SetActive(false);
    }

    
    public void BackToMainMenu()
    {
        BaseScreenUiControllerV2.Instance.ComeBackMainMenu();
    }

    void OnInfomationBack()
    {
        menuPlan.gameObject.SetActive(true);
        menuInfomation.gameObject.SetActive(false);
    }
    void OnInfomationClose()
    {
        gameObject.SetActive(false);
    }
    private void OnHideMenuPlan()
    {
        menuPlan.gameObject.SetActive(false);
    }

    private void ShowMenuPlan()
    {
        menuPlan.gameObject.SetActive(true);
    }

    void ApplyPlanTemplate(VRDomeData plan)
    {
        PopupRuntimeManager.Instance.ShowPopup("選択したサンプルを読み込みますか？",//"選択したビューへ移動しますか？",
            onClickConfirm: () =>
            {
                //OnCloseClick();
                OnHideMenuPlan();
                plan.dome_id = GameContext.CurrentIdDome;
                string json = JsonUtility.ToJson(plan);
                DebugExtension.Log(json);
                BaseScreenTopMenuV2.Instance.ResetCameraRotate();
                VrgSyncApi.Send(new ApplyPlanTemplateMessage()
                {
                    idDome = GameContext.CurrentIdDome,
                    json = json
                }, ApplyPlanTemplateMessage.EventKey);

                //UniTask.Delay(200).ContinueWith(() =>
                //{
                //    MainMenu.SaveData(false);
                //    DebugExtension.Log("Save Data Dome !!!!!!!!!!!");
                //}).Forget();

                UniTask.Delay(200).ContinueWith(() =>
                {
                    var message = new VrArrowNextDomeMessage()
                    {
                        DomeId = GameContext.CurrentIdDome,
                    };
                    //Sync
                    VrgSyncApi.Send(message, VrArrowNextDomeMessage.EventKey);
                    DebugExtension.Log("Send Change Dome !!!!!!!!!!!");
                }).Forget();
            },
            onClickCancel: () =>
            {
                //foreach (var vrObject in FindObjectOfType<VRObjectManagerV2>().VrObjects)
                //{
                //    vrObject.ShowMenuUiEdit();
                //}
            });

    }
}