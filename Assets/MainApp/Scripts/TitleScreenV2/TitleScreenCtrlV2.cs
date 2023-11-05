using Game.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TWT.Model;
using TWT.Networking;
using TWT.Utility;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Shim.Utils;
using VrGardenApi;
using Newtonsoft.Json;
using UnityEngine.AddressableAssets;

namespace TWT.Client.TitleScene
{
    public class TitleScreenCtrlV2 : MonoBehaviour
    {
        public GameObject loginPanel;
        public Button btnLogin;
        public Button intoTheRoomBtn;
        public Button settingBtn;
        public Button backTitlePanelBtn;
        public InputField inputPlayerName;
        public InputField inputPassword;
        public Text txtVersion;
        public Text txtInvalidPassword;
        public GameObject loadingPanel;

        public GameObject loadingDataPanel;
        public Slider loadSlider;
        public Text percentLoaded;
        public Text txtIndex;

        public bool isTestJoinRoom = false;
        bool isDownloadAddressable = false;
        string passRoom => inputPassword.text;
        string roomID => RuntimeData.RoomID;
        string passUserTest = "123456";

        private void Start()
        {
            string key = "App_Version";
            DebugExtension.Log("Version = " + Application.version);
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetString(key, Application.version);
                Caching.ClearCache();
                DebugExtension.Log("Clean Cache .......");
            }
            else
            {
                if (PlayerPrefs.GetString(key) != Application.version)
                {
                    PlayerPrefs.SetString(key, Application.version);
                    //Addressables..CleanBundleCache();
                    Caching.ClearCache();
                    DebugExtension.Log("Clean Cache .......");
                }
            }

            RuntimeData.User = null;
            loadingDataPanel.SetActive(false);
            AddressableDownloadManager.ProgressEvent = OnProgress;
            AddressableDownloadManager.CompletionEvent = OnLoadAddresableDone;
            DebugExtension.Log("SEVER MODE ----- " + GameContext.ServerMode.ToString().ToUpper());

            txtInvalidPassword.enabled = false;
            GameContext.IsEditable = true;
            GameContext.IsOffline = false;
            GameContext.IsDemo = false;
            GameContext.IsDayMode = true;

            loginPanel.SetActive(false);

            backTitlePanelBtn.onClick.AddListener(BackToTitlePanel);
            btnLogin.onClick.AddListener(Login);

            intoTheRoomBtn.onClick.AddListener(IntoTheRoom);

            inputPlayerName.text = PlayerPrefs.GetString(PlayerPrefsConst.PLAYER_NAME, inputPlayerName.text);
            inputPassword.text = PlayerPrefs.GetString(PlayerPrefsConst.PASSWORD_ROOM_ID, inputPassword.text);
            txtVersion.text = string.Format("App.ver: {0}", GameContext.APP_VERSION);
            ClearPlayData();
            SceneConfig.LoadScene(SceneConfig.Scene.JoinRoom, LoadSceneMode.Additive);
            ConnectServer.OnLoginDone += OnLoginDone;
            ConnectServer.OnCheckPassRoom += OnCheckPassRoom;
            ConnectServer.OnJoinRoomDone += OnJoinRoomDone;

            //string a = "\"{\\\"id\\\":12,\\\"username\\\":\\\"test9\\\",\\\"type\\\":1,\\\"visitor\\\":{\\\"userId\\\":12,\\\"name\\\":\\\"test9-name\\\",\\\"email\\\":\\\"test9@test9.vn\\\",\\\"birthDate\\\":\\\"2023-01-19T00:00:00.000Z\\\",\\\"gender\\\":1}}\"";
            //GetUserResponse res = JsonConvert.DeserializeObject<GetUserResponse>(a);
        }
        private void OnDestroy()
        {
            ConnectServer.OnLoginDone -= OnLoginDone;
            ConnectServer.OnCheckPassRoom -= OnCheckPassRoom;
            ConnectServer.OnJoinRoomDone -= OnJoinRoomDone;
        }

        public void Login()
        {
            txtInvalidPassword.enabled = false;
            loadingPanel.SetActive(true);
            string user = inputPlayerName.text;
            PlayerPrefs.SetString(PlayerPrefsConst.PLAYER_NAME, user);
            PlayerPrefs.SetString(PlayerPrefsConst.PASSWORD, passUserTest);
            PlayerPrefs.SetString(PlayerPrefsConst.ROOM_ID, roomID);
            PlayerPrefs.SetString(PlayerPrefsConst.PASSWORD_ROOM_ID, passRoom);
            PlayerPrefs.Save();
            if (RuntimeData.User != null)
            {
                ConnectServer.Instance.CheckRoom(roomID, passRoom);
            }
            else
            {
                ConnectServer.Instance.StartLogin(user, passUserTest, ex =>
                {
                    loadingPanel.SetActive(false);
                    txtInvalidPassword.enabled = true;
                    RuntimeData.User = null;
                });
            }
        }
        void OnLoginDone()
        {
            ConnectServer.Instance.CheckRoom(roomID, passRoom);
        }
        void OnCheckPassRoom(bool isPassCorrect)
        {
            loadingPanel.SetActive(false);
            if (isPassCorrect)
            {
                DebugExtension.Log("Room correct!");
                if (isTestJoinRoom)
                    OnLoadAddresableDone(true);
                else
                    LoadAddressable();
            }
            else
            {
                RuntimeData.User = null;
                txtInvalidPassword.enabled = true;
                //string message = "このユーザーはアクセスを制限されています。";
            }
        }
        void OnLoadAddresableDone(bool isDone)
        {
            isDownloadAddressable = false;
            if (!isDone) return;
            SceneConfig.LoadScene(SceneConfig.Scene.BaseScreenV2);
        }
        void OnJoinRoomDone()
        {
            //SceneConfig.LoadScene(SceneConfig.Scene.BaseScreen);
            //LoadContentSceneForEdit().Forget();
            DebugExtension.Log("Join Room Done !!!");
        }
        void LoadAddressable()
        {
            isDownloadAddressable = true;
            SetText(0);
            loadingDataPanel.SetActive(true);
            AddressableDownloadManager.Instance.StartLoadData();
        }
        void OnProgress(float progress)
        {
            SetText(progress);
        }
        void SetText(float progress)
        {
            percentLoaded.text = "Loading " + string.Format("Value: {0:P2}.", progress);// string.Format("Value: {0:%}.", (progress * 100));// + "%";
            loadSlider.value = progress;
        }
        void Update()
        {
            if (!isDownloadAddressable) return;
            int count = AddressableDownloadManager.Instance.CountItemDownload;
            if (count > AddressableDownloadManager.Instance.TotalItemDownload)
                count = AddressableDownloadManager.Instance.TotalItemDownload;
            if (txtIndex) txtIndex.text = "Item Download: " + count + "/" + AddressableDownloadManager.Instance.TotalItemDownload;
        }
        private void ClearPlayData()
        {
            GameContext.ContentDataCurrent = null;
        }

        private async UniTask<VREditPermissionData> GetPasswordForEditContent()
        {
            loadingPanel.SetActive(true);
            var permission = await GameContext.GetPasswordForEditContentAsync();
            loadingPanel.SetActive(false);
            return permission;
        }


        private async UniTask LoadContentSceneForEdit()
        {
            loadingPanel.SetActive(true);
            VREditPermissionData permission;
            PlayerPrefs.SetString(PlayerPrefsConstant.PLAYER_NAME, inputPlayerName.text);
            PlayerPrefs.SetString(PlayerPrefsConstant.PASSWORD, inputPassword.text);

            permission = await GetPasswordForEditContent();
            loadingPanel.SetActive(false);
            if (permission == null)
                return;

            if (inputPassword.text.Equals(permission.password))// && GameContext.APP_VERSION.Equals(permission.version))
            {
                GameContext.GameMode = GameMode.TrainingMode;
                GameContext.IsEditable = true;
                SceneConfig.LoadScene(SceneConfig.Scene.ContentListScreen);
            }
            else
            {
                txtInvalidPassword.enabled = true;
                //string message = "このユーザーはアクセスを制限されています。";
            }
        }

        public void BackToTitlePanel()
        {
            loginPanel.SetActive(false);
        }
        public void IntoTheRoom()
        {
            loginPanel.SetActive(true);
        }
    }
}