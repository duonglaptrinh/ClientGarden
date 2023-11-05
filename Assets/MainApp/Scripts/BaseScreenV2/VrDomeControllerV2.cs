using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Client.Extension;
using TWT.Client;
using TWT.Model;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Common.VGS;
using TWT.Networking;
using VrGardenApi;
using static VrGardenApi.SaveImageApi;

namespace Game.Client
{
    public class VrDomeControllerV2 : MonoBehaviour
    {
        public static VrDomeControllerV2 Instance => BaseScreenCtrlV2.Instance.VrDomeControllerV2;
        [SerializeField] private VRDomeLoadHouse domeLoadhouse;
        public VRDomeLoadHouse DomeLoadhouse => domeLoadhouse;
        private VRObjectManagerV2 vRObjectManager => VRObjectManagerV2.Instance;

        public VRDomeData vrDomeData;
        public List<GetJsonImageViewResponse> ListIdUrlView { get; set; }
        public int MaxIdUrlView => (ListIdUrlView == null || ListIdUrlView.Count == 0) ? 1 : ListIdUrlView.Max(x => x.Id);

        public event Action<VRDomeData> OnChangeDome;

        private void Awake()
        {
            vrDomeData = null;
        }
        public VRStartPointData GetStartPointDataByResponse(GetJsonImageViewResponse res)
        {
            if (res == null) return null;
            if (string.IsNullOrEmpty(res.location)) return VRStartPointData.CreateStart();
            string location = res.location;
            VRStartPointData data = JsonUtility.FromJson<VRStartPointData>(location);
            data.id_url = res.Id.ToString();
            data.nameView = "View " + res.Id.ToString();
            return data;
        }
        public void GetListAllView(Action onDownloadDone = null)
        {
            try
            {
                ConnectServer.Instance.GetListImageView(RuntimeData.RoomIDNumber, GameContext.CurrentIdDome, TypeAPISaveImage.View, response =>
                {
                    ListIdUrlView = response.listView;
                    DebugExtension.Log("List View Of Room = " + response.listView);
                    onDownloadDone?.Invoke();
                });
            }
            catch (Exception e)
            {
                DebugExtension.LogError("Get List View Of Room Fail");
                onDownloadDone?.Invoke();
            }
        }
        public void ChangeDome(VRDomeData vrDomeData)
        {
            DebugExtension.Log("Change Dome Id = " + vrDomeData.dome_id);

            BaseScreenUiControllerV2.Instance.NewMainMenu.ShowLoading(MainMenu.TEXT_LOADING);
            GetListAllView(() =>
            {
                BaseScreenUiControllerV2.Instance.NewMainMenu.HideLoading();
                this.vrDomeData = vrDomeData;
                vRObjectManager.ClearObjects();
                PlayerManagerSwitch.IsAllowMove = false;
                AddressableLoadingHouse.Instance.UnloadScene(() =>
                {
                    //DebugExtension.LogError("Unload Load: Dome id = " + vrDomeData.dome_id);
                    LoadDomeObjects(vrDomeData);
                    ShowDomeView(vrDomeData.modelData);
                    ChangeViewPlayer(vrDomeData.listStartPointData).Forget();
                    //ShowAllVrObject();
                    OnChangeDome?.Invoke(vrDomeData);
                }).Forget();
            });
        }

        public void UpdateDomeMedia(string fileName, string fileType, ModelDataHouseNetwork data)
        {
            vrDomeData.d360_file_name = fileName;
            vrDomeData.d360_file_type = fileType;
            vrDomeData.modelData = data;
        }

        public void SaveOnlyModelData()
        {
            vrDomeData.vr_object_list.vr_model_list = vRObjectManager.vrModels.Select(item => item.GetData()).ToArray();
        }

        public void SaveDomeData()
        {
            vRObjectManager.SaveVRObjectData();
            vrDomeData.vr_object_list.vr_model_list = vRObjectManager.vrModels.Select(item => item.GetData()).ToArray();
            BaseScreenDataManager.UpdateDome(vrDomeData);
        }

        public void ShowDomeView(ModelDataHouseNetwork modelData = null)
        {
            if (modelData != null)
            {
                Camera.main.clearFlags = CameraClearFlags.Skybox;
                domeLoadhouse.SetData(modelData);
            }
        }
        public void ChangeViewPlayer()
        {
            DebugExtension.LogError("ChangeViewPlayer = " + JsonUtility.ToJson(vrDomeData.listStartPointData.CurrentData));
            ChangeViewPlayer(vrDomeData.listStartPointData, false).Forget();
        }
        public async UniTask ChangeViewPlayer(VRListStartPointData data = null, bool isFirst = true)
        {
            VRStartPointData dataStart = VRStartPointData.CreateStart();
            //if (data != null)
            //{
            //    dataStart = data.CurrentData;
            //    if (dataStart == null)
            //    {
            //        dataStart = VRStartPointData.CreateStart();
            //    }
            //}
            if (data != null && data.indexStartPoint >= 0)
            {
                if (ListIdUrlView != null && ListIdUrlView.Count > 0)
                {
                    GetJsonImageViewResponse item = null;
                    if (isFirst)
                        item = ListIdUrlView[0];
                    else
                        item = ListIdUrlView.FirstOrDefault(x => data.indexStartPoint == x.Id);
                    data.indexStartPoint = item.Id;
                    if (item != null)
                    {
                        dataStart = GetStartPointDataByResponse(item);
                        if (dataStart == null) dataStart = VRStartPointData.CreateStart();
                    }
                }
            }

            PlayerManagerSwitch.IsAllowMove = false;
            PlayerManagerSwitch.DisableCharacter();
            PlayerManagerSwitch.SetPosition(dataStart.position);
            PlayerManagerSwitch.SetRotation(dataStart.rotation);
            PlayerManagerSwitch.EnableCharacter();
            await UniTask.Delay(500);
            PlayerManagerSwitch.IsAllowMove = true;
        }

        public void LoadDomeObjects(VRDomeData vrDomeData)
        {
            vRObjectManager.LoadVrObjectList(vrDomeData.vr_object_list);
        }

        void ShowAllVrObject(bool status)
        {
            foreach (var vrObject in vRObjectManager.VrObjects)
            {
                vrObject.ShowVrObject(!status);
                //vrObject.SwitchToIdleStage();
            }
        }

        public bool IsPopupShowing()
        {
            return BaseScreenUiControllerV2.Instance.EditRemote.activeSelf || MenuTabControllerV2.Instance.IsPopupShowing(); 
        }

        public void RequestNewData()
        {
            DebugExtension.Log("RequestNewData !!");
            var message = new UpdateDataJsonVRContentData()
            {
                json = "need update json"
            };
            VrgSyncApi.Send(message);
        }
        public async void UpdateVRContentData()
        {
            DebugExtension.LogError("Update Data for all client --------------------------");

            //Save temp all new model just create
            Dictionary<int, List<VRModelData>> listNewData = new Dictionary<int, List<VRModelData>>();
            int i = 0;
            foreach (var item in GameContext.ContentDataCurrent.vr_dome_list)
            {
                List<VRModelData> listModel = item.GetAllNewModel();
                listNewData.Add(item.dome_id, listModel);
                i++;
            }

            string vrDataJson = await GameContext.ResourceLoader.GetVrContentJsonData(GameContext.ContentName);
            GameContext.ContentDataCurrent = JsonUtility.FromJson<VRContentData>(vrDataJson);
            DebugExtension.LogError("Update Data for all client Done--------------------------");
            i = 0;
            foreach (var item in GameContext.ContentDataCurrent.vr_dome_list)
            {
                if (item.dome_id == listNewData.ElementAt(i).Key)
                {
                    item.SetAllNewModel(listNewData.ElementAt(i).Value);
                }
                i++;
            }
        }
    }
}