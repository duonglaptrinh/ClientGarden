using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Client;
using Shim.Utils;
using TWT.Model;
using TWT.Networking;
using UIScroll;
using UnityEngine;
using UnityEngine.UI;
using VrGardenApi;

public class MenuSavePositionUI : CommonDialog
{
    public Action OnCloseMenu = null;
    public Action OnBackMenu = null;
    [SerializeField] UIScrollGrid scrollObject;
    [SerializeField] Button btnClose;
    [SerializeField] Button btnBack;
    VRStartPointData vrStartPointData;
    private Action<VRStartPointData> OnPlanSelected = null;
    //private SaveImageApi.GetJsonImageViewResponse getJsonImageViewResponse;
    //private List<SaveImageApi.GetJsonImageViewResponse> _listImageViewResponses;
    //private UIScrollItemBase _uiScrollItemBase;

    private void Awake()
    {
        scrollObject.OnCreateOneItem = CreateOneItemObject;
        scrollObject.OnClickOneItem = OnClickItemObject;
        btnClose.onClick.AddListener(() =>
        {
            OnCloseMenu?.Invoke();
        });
        btnBack.onClick.AddListener(() =>
        {
            OnBackMenu?.Invoke();
        });
    }

    protected virtual void CreateOneItemObject(UIScrollItemBase item)
    {
        ItemDataNewView data = (ItemDataNewView)item.CurrentData;
        if (data.IsCreateNew)
        {
            item.GetComponent<ItemSavePosition>().SetCreateView(data.VRStartPointData, OnCreateNewView);
            item.GetComponent<ItemSavePosition>().CheckCanCreateNewView(scrollObject.ListDatas.Count);
        }
        else
            item.GetComponent<ItemSavePosition>()
                .SetDome(data.VRStartPointData, data.VRStartPointData.id_url, OnSelectOneData, OnDeleteOneData, OnUpdateOneData);
    }
    protected virtual void OnClickItemObject(int index)
    {
        //UIScrollItemBase ui = scrollObject.ListItems[index];
        //ItemDataNewView data = (ItemDataNewView)ui.CurrentData;
        //getJsonImageViewResponse = data.ImageViewResponse;
        //_uiScrollItemBase = ui;
    }
    void OnSelectOneData(int index, VRStartPointData data)
    {
        VRListStartPointData.SendDataSync(EStartPoint.View, data, index);

        //TEST Tạm thời, khi có server sẽ xóa
        //VRObjectSync vrSync = GameObject.FindObjectOfType<VRObjectSync>();
        //vrSync.SyncStartPoint(new SyncStartPointMessage()
        //{
        //    idDome = VrDomeControllerV2.Instance.vrDomeData.dome_id,
        //    type = (int)EStartPoint.View,
        //    json = JsonUtility.ToJson(data),
        //    indexInList = index
        //});
        //LoadData();
        //END TEST

        //BaseScreenUiControllerV2.Instance.NewMainMenu.HideLoading();
    }
    void OnDeleteOneData(int index, VRStartPointData data)
    {
        DeleteOneNewView(data, status =>
        {
            LoadData();
        });
        //VRListStartPointData.SendDataSync(EStartPoint.Delete, data, index);
        //foreach (var item in scrollObject.ListItems)
        //{
        //    item.GetComponent<ItemSavePosition>().CheckCanCreateNewView(scrollObject.ListDatas.Count);
        //}

        //TEST Tạm thời, khi có server sẽ xóa
        //VRObjectSync vrSync = GameObject.FindObjectOfType<VRObjectSync>();
        //vrSync.SyncStartPoint(new SyncStartPointMessage()
        //{
        //    idDome = VrDomeControllerV2.Instance.vrDomeData.dome_id,
        //    type = (int)EStartPoint.Delete,
        //    json = JsonUtility.ToJson(data),
        //    indexInList = index
        //});
        //LoadData();
        //END TEST

    }
    public static void DeleteOneNewView(VRStartPointData data, Action<bool> onDone = null)
    {
        DeleteOneNewView(data.id_url, onDone);
    }
    public static void DeleteOneNewView(string id_url, Action<bool> onDone = null)
    {
        try
        {
            ConnectServer.Instance.DeleteImageByIdView(id_url, response =>
            {
                if (response == null)
                {
                    onDone?.Invoke(false);
                    return;
                }
                if (response.Status)
                {
                    DebugExtension.Log("Delete Image Id = " + id_url);
                    onDone?.Invoke(true);
                }
                else
                {
                    DebugExtension.LogError("Upload Fail");
                    onDone?.Invoke(false);
                }
            });
        }
        catch (Exception e)
        {
            onDone?.Invoke(false);
        }
    }
    void OnUpdateOneData(int index, VRStartPointData data, Texture2D texture)
    {
        BaseScreenUiControllerV2.Instance.NewMainMenu.ShowLoading(MainMenu.TEXT_UPLOAD_SCREEN_SHOT);
        data.position = PlayerManagerSwitch.position;
        data.localScale = PlayerManagerSwitch.localScale;
        data.eulerAngel = PlayerManagerSwitch.eulerAngles;
        data.rotation = PlayerManagerSwitch.rotation;
        //API Upload Image
        UpdateImage(data, texture, (Status, id_url) =>
        {
            if (Status)
            {
                //data.id_url = id_url.ToString();
                //VRListStartPointData.SendDataSync(EStartPoint.Update, data, index);
            }
            LoadData();
            BaseScreenUiControllerV2.Instance.NewMainMenu.HideLoading();
        });
        //End API Upload Image 

        //TEST Tạm thời, khi có server sẽ xóa
        //VRObjectSync vrSync = GameObject.FindObjectOfType<VRObjectSync>();
        //vrSync.SyncStartPoint(new SyncStartPointMessage()
        //{
        //    idDome = VrDomeControllerV2.Instance.vrDomeData.dome_id,
        //    type = (int)EStartPoint.Update,
        //    json = JsonUtility.ToJson(data),
        //    indexInList = index
        //});
        //LoadData();
        //END TEST

    }
    void OnCreateNewView(int index, Texture2D texture)
    {
        BaseScreenUiControllerV2.Instance.NewMainMenu.ShowLoading(MainMenu.TEXT_UPLOAD_SCREEN_SHOT);
        int count = VrDomeControllerV2.Instance.ListIdUrlView.Count;

        VRStartPointData data = new VRStartPointData();
        data.nameView = "View " + VrDomeControllerV2.Instance.MaxIdUrlView;
        data.id_url = "";
        data.position = PlayerManagerSwitch.position;
        data.localScale = PlayerManagerSwitch.localScale;
        data.eulerAngel = PlayerManagerSwitch.eulerAngles;
        data.rotation = PlayerManagerSwitch.rotation;
        //API Upload Image
        UploadImage(data, texture, (Status, id_url) =>
        {
            if (Status)
            {
                //VRListStartPointData.SendDataSync(EStartPoint.Add, data);
            }
            LoadData();
            BaseScreenUiControllerV2.Instance.NewMainMenu.HideLoading();
        });
        //End API Upload Image 


        //TEST Tạm thời, khi có server sẽ xóa
        //VRObjectSync vrSync = GameObject.FindObjectOfType<VRObjectSync>();
        //vrSync.SyncStartPoint(new SyncStartPointMessage()
        //{
        //    idDome = VrDomeControllerV2.Instance.vrDomeData.dome_id,
        //    type = (int)EStartPoint.Add,
        //    json = JsonUtility.ToJson(data),
        //    indexInList = -1
        //});
        //LoadData();
        //END TEST

    }
    private void OnEnable()
    {
        LoadData();
        BaseScreenDataManager.OnDeleteDomeData += LoadData;
        BaseScreenDataManager.OnChangeDome += LoadData;
        VRObjectSync.OnStartPointSync += OnStartPointSync;
    }

    private void OnDisable()
    {
        BaseScreenDataManager.OnDeleteDomeData -= LoadData;
        BaseScreenDataManager.OnChangeDome -= LoadData;
        VRObjectSync.OnStartPointSync -= OnStartPointSync;
    }
    void OnStartPointSync(SyncStartPointMessage message)
    {
        //DebugExtension.LogError("OnStartPointSync load data ");
        LoadData();
    }
    private void LoadDataSave()
    {
        scrollObject.Scroll.content.SetLeft(0);
        scrollObject.Scroll.content.SetRight(0);
        Vector3 pos = scrollObject.Scroll.content.localPosition;
        pos.y = 0;
        scrollObject.Scroll.content.localPosition = pos;

        Action OnloadDone = () =>
        {
            VRStartPointData vrStartPointData = null;
            List<ItemDataBase> list = new List<ItemDataBase>();
            if (VrDomeControllerV2.Instance.ListIdUrlView != null && VrDomeControllerV2.Instance.ListIdUrlView.Count > 0)
            {
                list = VrDomeControllerV2.Instance.ListIdUrlView
                   .Select(response => new ItemDataNewView(VrDomeControllerV2.Instance.GetStartPointDataByResponse(response), false))
                   .Cast<ItemDataBase>().ToList();
            }
            list.Add(new ItemDataNewView(vrStartPointData, true));
            if (list.Count > 0)
            {
                scrollObject.Initialize(list);
            }
            else
            {
                scrollObject.ClearAll();
            }
        };
        VrDomeControllerV2.Instance.GetListAllView(OnloadDone);
    }

    private void LoadData()
    {
        LoadDataSave();
    }

    private void UpdateImage(VRStartPointData data, Texture2D texture, Action<bool, long> onDone)
    {
        try
        {
            string location = JsonUtility.ToJson(data);
            ConnectServer.Instance.UpdateImageView(int.Parse(data.id_url), RuntimeData.RoomIDNumber, GameContext.CurrentIdDome, TypeAPISaveImage.View, location, texture, response =>
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
    private void UploadImage(VRStartPointData data, Texture2D texture, Action<bool, long> onDone)
    {
        try
        {
            string location = JsonUtility.ToJson(data);
            ConnectServer.Instance.UploadImageView(RuntimeData.RoomIDNumber, GameContext.CurrentIdDome, TypeAPISaveImage.View, location, texture, response =>
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
