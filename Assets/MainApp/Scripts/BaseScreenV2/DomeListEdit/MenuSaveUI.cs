using Game.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TWT.Model;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TWT.Networking;
using VrGardenApi;

public class MenuSaveUI : CommonDialog
{
    public Action OnClickChangeDome = null;
    [SerializeField] UIScrollBase scrollObject;

    VRDomeData selectedDomeData;
    public static int currentDomeIdPlayerIn;
    public Action<VRDomeData> OnPlanSelected = null;

    void Awake()
    {
        scrollObject.OnCreateOneItem = CreateOneItemObject;
        scrollObject.OnClickOneItem = OnClickItemObject;

    }
    protected virtual void CreateOneItemObject(UIScrollItemBase item)
    {
        ItemDataBaseDome data = (ItemDataBaseDome)item.CurrentData;
        if (data.IsCreateNew)
            item.GetComponent<VRPlanSaveItemCtrl>().SetDomeCreate(data.domeData, OnCreateNewPlan);
        else
            item.GetComponent<VRPlanSaveItemCtrl>().SetDome(data.domeData, OnSelectOnePlan, OnDeleteOnePlan, OnUpdateOnePlan);
    }

    void OnSelectOnePlan(VRDomeData plan, Texture thumbnail)
    {
        DebugExtension.Log("Load New Plan");
        selectedDomeData = plan;

        OnPlanSelected?.Invoke(selectedDomeData);

        PopupRuntimeManager.Instance.ShowPopup("選択したプランを読み込みますか？",//"選択したビューへ移動しますか？",
            onClickConfirm: () =>
            {
                OnCloseClick();
                var message = new VrArrowNextDomeMessage()
                {
                    DomeId = selectedDomeData.dome_id,
                };
                //Sync
                VrgSyncApi.Send(message, VrArrowNextDomeMessage.EventKey);
                OnClickChangeDome?.Invoke();
            },
            onClickCancel: () =>
            {
                foreach (var vrObject in FindObjectOfType<VRObjectManagerV2>().VrObjects)
                {
                    vrObject.ShowMenuUiEdit();
                }
            });
    }
    void OnDeleteOnePlan(VRDomeData plan)
    {
        foreach (var startPointData in plan.listStartPointData.listStartPoint)
        {
            MenuSavePositionUI.DeleteOneNewView(startPointData);
        }
        MenuSavePositionUI.DeleteOneNewView(plan.id_url.ToString());

        DebugExtension.Log("Delete New Plan");
        VrgSyncApi.Send(new DeleteDomePlantMessage()
        {
            idDome = plan.dome_id
        }, DeleteDomePlantMessage.EventKey);
    }
    void OnUpdateOnePlan(VRPlanSaveItemCtrl item, VRDomeData plan, Texture2D texture)
    {
        if (PopupRuntimeManager._IsPopup) return;

        Action<Action> OnUploadImage = action =>
        {
            BaseScreenUiControllerV2.Instance.NewMainMenu.ShowLoading(MainMenu.TEXT_UPLOAD_SCREEN_SHOT);
            UpdateImage(plan.id_url, plan.dome_id, texture, (Status, id_url) =>
            {
                BaseScreenUiControllerV2.Instance.NewMainMenu.HideLoading();
                if (!Status)
                {
                    return;
                }
                plan.id_url = (int)id_url;
                action?.Invoke();
            });
        };

        if (plan.dome_id == GameContext.CurrentIdDome)
        {
            DebugExtension.Log("Update Current Plan");
            PopupRuntimeManager.Instance.ShowPopup("保存しますか？", () =>
            {
                OnUploadImage.Invoke(() =>
                {
                    item.SetPreview(texture);
                    MainMenu.SaveData(plan.id_url);
                });
            });
        }
        else
        {
            DebugExtension.Log("Update Into New Plan");
            PopupRuntimeManager.Instance.ShowPopup("選択したプランに上書き保存しますか？", onClickConfirm: () =>
            {
                OnUploadImage.Invoke(() =>
                {
                    item.SetPreview(texture);

                    VRDomeData overrideData = new VRDomeData(VrDomeControllerV2.Instance.vrDomeData);
                    overrideData.dome_id = plan.dome_id;
                    overrideData.id_url = plan.id_url;
                    //override data of current slot into select slot
                    string json = JsonUtility.ToJson(overrideData);
                    VrgSyncApi.Send(new ApplyPlanTemplateMessage()
                    {
                        idDome = plan.dome_id,
                        json = json
                    }, ApplyPlanTemplateMessage.EventKey);
                });
            });
        }
    }
    void OnCreateNewPlan(VRDomeData plan, Texture2D texture)
    {
        BaseScreenUiControllerV2.Instance.NewMainMenu.ShowLoading(MainMenu.TEXT_UPLOAD_SCREEN_SHOT);
        UploadImage(plan.dome_id, texture, (Status, id_url) =>
        {
            BaseScreenUiControllerV2.Instance.NewMainMenu.HideLoading();
            if (!Status)
            {
                return;
            }
            DebugExtension.Log("Create New Plan ");

            plan.id_url = (int)id_url;
            VrgSyncApi.Send(new SaveNewPlanJsonMessage()
            {
                idDome = plan.dome_id,
                json = JsonUtility.ToJson(plan)
            }, SaveNewPlanJsonMessage.EventKey);
            UniTask.Delay(200).ContinueWith(() =>
            {
                var message = new VrArrowNextDomeMessage()
                {
                    DomeId = plan.dome_id,
                };
                //Sync
                VrgSyncApi.Send(message, VrArrowNextDomeMessage.EventKey);
                DebugExtension.Log("Send Change Dome !!!!!!!!!!!");
            }).Forget();

        });
    }

    protected virtual void OnClickItemObject(int index)
    {
        //None - not use
    }

    void Start()
    {

    }

    void OnEnable()
    {
        LoadDome();
        BaseScreenDataManager.OnDeleteDomeData += LoadDome;
        BaseScreenDataManager.OnChangeDome += LoadDome;
    }

    void OnDisable()
    {
        BaseScreenDataManager.OnDeleteDomeData -= LoadDome;
        BaseScreenDataManager.OnChangeDome -= LoadDome;
    }

    public void LoadDomeSave()
    {
        VRDomeData[] domes = GameContext.ContentDataCurrent.vr_dome_list;
        List<ItemDataBase> list = new List<ItemDataBase>();

        int max = domes.Max(x => x.dome_id);
        //DebugExtension.LogError(max);

        VRDomeData currentDome = null;
        for (int i = 0; i < domes.Length; i++)
        {
            if (domes[i].dome_id == GameContext.CurrentIdDome)
            {
                currentDome = new VRDomeData(domes[i]);
                currentDome.dome_id = max + 1;
            }
            list.Add(new ItemDataBaseDome(domes[i], false));
        }
        //if (domes.Length < 5)
        // Add new Item plus to create New Save
        list.Add(new ItemDataBaseDome(currentDome, true));
        if (list.Count > 0)
        {
            scrollObject.Initialize(list);
        }
    }
    void LoadDome()
    {
        LoadDomeSave();
    }
    private void UpdateImage(int id_url, int domeId, Texture2D texture, Action<bool, long> onDone)
    {
        try
        {
            ConnectServer.Instance.UpdateImageView(id_url, RuntimeData.RoomIDNumber, domeId, TypeAPISaveImage.ThumbDome, location: "", texture, response =>
            {
                if (response == null)
                {
                    onDone?.Invoke(false, 0);
                    return;
                }
                if (response.Status == 1)
                {
                    DebugExtension.Log("Update NewView Done, Id = " + response.Id);
                    onDone?.Invoke(true, response.Id);
                }
                else
                {
                    DebugExtension.LogError("Update Fail");
                    onDone?.Invoke(false, 0);
                }
            });
        }
        catch (Exception e)
        {
            DebugExtension.LogError("Update Fail Exception");
            onDone?.Invoke(false, 0);
        }
    }
    private void UploadImage(int domeId, Texture2D texture, Action<bool, long> onDone)
    {
        try
        {
            ConnectServer.Instance.UploadImageView(RuntimeData.RoomIDNumber, domeId, TypeAPISaveImage.ThumbDome, location: "", texture, response =>
            {
                if (response == null)
                {
                    onDone?.Invoke(false, 0);
                    return;
                }
                if (response.Status == 1)
                {
                    DebugExtension.Log("Upload NewView Done, Id = " + response.Id);
                    onDone?.Invoke(true, response.Id);
                }
                else
                {
                    DebugExtension.LogError("Upload Fail");
                    onDone?.Invoke(false, 0);
                }
            });
        }
        catch (Exception e)
        {
            DebugExtension.LogError("Upload Fail Exception");
            onDone?.Invoke(false, 0);
        }
    }
}