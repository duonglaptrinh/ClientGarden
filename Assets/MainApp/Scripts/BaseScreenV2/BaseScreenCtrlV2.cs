using System;
using Game.Client;
using TWT.Model;
using PlayerManager = Player_Management.PlayerManager;
using UnityEngine;
using jp.co.mirabo.Application.RoomManagement;
using System.Linq;
using TWT.Networking;
using VrGardenApi;

public class BaseScreenCtrlV2 : SingletonMonoBehavior<BaseScreenCtrlV2>
{
    /// <summary>
    ///When Data JsonRoom load OK 
    /// </summary>
    public static Action OnChangeDomeDone = null;
    public VrDomeControllerV2 VrDomeControllerV2;
    public VRObjectManagerV2 VRObjectManagerV2;
    public MenuTabControllerV2 MenuTabControllerV2;

    public static bool IsLoadingRoom { get; set; } = false;
    public static VRContentData VrContentData => GameContext.ContentDataCurrent;
    [SerializeField] TextAsset jsonData;
    private void Start()
    {
        GameContext.ContentDataCurrentTemplate = null;
        PlayerManagerSwitch.IsAllowMove = true;

        ConnectServer.OnJoinRoomDone += OnJoinRoomDone;
        RoomManager.OnChangeRoomState += OnChangeRoomState;

        string roomID = PlayerPrefs.GetString(PlayerPrefsConst.ROOM_ID, "");
        string passRoom = PlayerPrefs.GetString(PlayerPrefsConst.PASSWORD_ROOM_ID, "");
        ConnectServer.Instance.JoinRoom(roomID, passRoom);
        BaseScreenUiControllerV2.Instance.HideUiController();

    }
    private void OnDestroy()
    {
        ConnectServer.OnJoinRoomDone -= OnJoinRoomDone;
        RoomManager.OnChangeRoomState -= OnChangeRoomState;
    }
    void OnJoinRoomDone()
    {
        //SceneConfig.LoadScene(SceneConfig.Scene.BaseScreen);
        //LoadContentSceneForEdit().Forget();
        PlayerManager.Instance.CreateAllJoinedPlayer();
        DebugExtension.Log("Join Room Done !!!");
    }

    void OnChangeRoomState(VRContentData contentData)
    {
        if (contentData == null)
        {
            DebugExtension.LogError($"VrContent is null  --> Test Mode Enable");
            contentData = VRContentData.FromJson(jsonData.text);
        }
        else
        {
            DebugExtension.Log("VrContent is not null ----- ");
        }

        // contentData = VRContentData.FromJson(jsonData.text);

        GameContext.ContentDataCurrent = contentData;
        //GameContext.CurrentIdDome = 1;
        DebugExtension.Log("GameContext.CurrentIdDome = " + GameContext.CurrentIdDome);
        RequestChangeDome(GameContext.CurrentIdDome);
    }

    public void RequestChangeDome(int domeId)
    {
        if (VrContentData == null)
        {
            DebugExtension.LogError($"VrContent is null, cannot change dome to dome id {domeId}");
            return;
        }
        IsLoadingRoom = true;
#if UNITY_EDITOR
        //VRContentDataTemplate template = JsonUtility.FromJson<VRContentDataTemplate>(LoadResourcesData.Instance.jsonTemplate.text);
        //GameContext.ContentDataCurrent = new VRContentData(template);
        //domeId = 1;
        //GameContext.ContentDataCurrent.vr_dome_list[0].modelData.UpdateIndexHouse(-1);
#endif
        var dome = VrContentData.vr_dome_list.FirstOrDefault(data => data.dome_id == domeId);
        if (dome == null)
        {
            DebugExtension.LogError($"Not found dome id {domeId} to switch");
            return;
        }
        DebugExtension.Log($"Switch to dome id {domeId}");
        //DebugExtension.LogError(JsonUtility.ToJson(dome));
        VrDomeControllerV2.Instance.ChangeDome(dome);
        OnChangeDomeDone?.Invoke();

#if UNITY_EDITOR
        //VRContentDataTemplate dataTemplateTest = new VRContentDataTemplate(VrContentData);
        //string debug = dataTemplateTest.ToJson(dataTemplateTest);
        //DebugExtension.Log(debug);
#endif

    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            int index = UnityEngine.Random.Range(0, VrContentData.vr_dome_list.Length);
            int dome = VrContentData.vr_dome_list[index].dome_id;
            var message = new VrArrowNextDomeMessage()
            {
                DomeId = dome,
            };
            VRStackDataSync.AddToStack(message);
        }
    }

    [ContextMenu("Delete All Image Start Point")]
    void DeleteAlImage()
    {
        //Debug.Log(JsonUtility.ToJson(VRStartPointData.CreateStart()));
        try
        {
            ConnectServer.Instance.GetListImageView(RuntimeData.RoomIDNumber, GameContext.CurrentIdDome, TypeAPISaveImage.View, response =>
            {
                DebugExtension.Log(response.listView.Count);
                foreach (var item in response.listView)
                {
                    MenuSavePositionUI.DeleteOneNewView(item.Id.ToString());
                }
            });
        }
        catch (Exception e)
        {
            DebugExtension.LogError("Upload Fail Exception");
        }

    }
#endif
}