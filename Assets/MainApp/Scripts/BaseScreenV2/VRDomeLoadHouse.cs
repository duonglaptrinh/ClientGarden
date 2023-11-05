using ClientOnly.Oculus;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TWT.Model;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Game.Client
{
    public class VRDomeLoadHouse : MonoBehaviour
    {
        public static Action<Transform> OnChangeHouseModel = null;
        [SerializeField] private GameObject loading;
        [SerializeField] private Transform parentModel;
        [SerializeField] private Text textLoadingProgress;
        [SerializeField] private GameObject Land;
        [SerializeField] List<Renderer> renderersLand = new List<Renderer>();
        [SerializeField] List<Renderer> renderersGrid = new List<Renderer>();
        [SerializeField] private GameObject objGrid;
        [SerializeField] private GameObject parentGridPosition;

        List<DicDataMaterial> listMaterialsLand = new List<DicDataMaterial>();
        List<DicDataMaterial> listMaterialsGrid = new List<DicDataMaterial>();

        public static bool IsLoadingHouse { get; set; }
        public Transform CurrentModel => parentModel;
        List<ITeleportView> listTelePort = new List<ITeleportView>();
        public ModelDataHouseNetwork CurrentData => VrDomeControllerV2.Instance.vrDomeData.modelData;
        public MaterialController Modelcontroller { get; set; }
        public HouseDataAsset CurrentHouse;
        public HouseDataJson DataJson { get; set; }

        List<MaterialDataNetWorkDetail> stackData = new List<MaterialDataNetWorkDetail>();
        int stackDataIndexHouse = -1;
        int currentIndexHouseLoading = -1;

        private void Awake()
        {
            textLoadingProgress.text = "建物データを読み込んでいます…";
        }
        private void OnEnable()
        {
            AddressableLoadingHouse.OnLoadSceneDone += OnLoadSceneDone;
        }
        private void OnDisable()
        {
            AddressableLoadingHouse.OnLoadSceneDone -= OnLoadSceneDone;
        }
        private void Start()
        {
            foreach (var item in renderersLand)
            {
                foreach (var mat in item.materials)
                {
                    listMaterialsLand.Add(new DicDataMaterial(item, mat, mat.color));
                }
            }

            foreach (var item in renderersGrid)
            {
                foreach (var mat in item.materials)
                {
                    listMaterialsGrid.Add(new DicDataMaterial(item, mat, mat.color));
                }
            }

            StartCoroutine("AutoCheckStack");
        }
        public void ResetModel()
        {
            if (parentModel)
            {
                parentModel.position = Vector3.zero;
                parentModel.localScale = Vector3.one;
                parentModel.localEulerAngles = Vector3.zero;
            }
        }
        //private Tweener tween;

        public void SetData(ModelDataHouseNetwork data)
        {
            IsLoadingHouse = false;
            stackData = new List<MaterialDataNetWorkDetail>();
            if (data.indexHouse < 0)
            {
                loading.gameObject.SetActive(false);
                CheckDomeStack();
                return;
            }
            FirstLoadNewHouse(data.indexHouse);
        }
        public void FirstLoadNewHouse(int indexHouse)
        {
            //DebugExtension.LogError("Load House  DomeID = " + GameContext.CurrentIdDome);
            stackDataIndexHouse = -1;
            if (CurrentHouse != null) // when change Dome
            {
                LoadNewSceneSyncFromServer();
                return;
            }

            IsLoadingHouse = true;
            loading.gameObject.SetActive(true);
            HouseDataAsset houseData = AddressableDownloadManager.ResourcesData.ListPathSceneHouseAddressable[indexHouse];
            CurrentHouse = houseData;
            currentIndexHouseLoading = indexHouse;
            AddressableLoadingHouse.Instance.LoadSceneAsync(GameContext.IsDayMode ? CurrentHouse.PathSceneDay : CurrentHouse.PathSceneNight).Forget();
        }
        public void SetLoading()
        {
            //IsLoadingHouse = true;
            loading.gameObject.SetActive(true);
        }
        public void LoadNewSceneSyncFromServer()
        {
            if (IsLoadingHouse) //When Loading House1 but other player change to House2 --> after load house1 Done need auto load House2
            {
                stackDataIndexHouse = CurrentData.indexHouse;
                return;
            }
            HouseDataAsset houseData = AddressableDownloadManager.ResourcesData.ListPathSceneHouseAddressable[CurrentData.indexHouse];
            CurrentHouse = houseData;
            currentIndexHouseLoading = CurrentData.indexHouse;
            ChangeDayOrNight(GameContext.IsDayMode);
        }
        public void ChangeDayOrNight(bool isChangeDay)
        {
            if (CurrentData.indexHouse < 0)
            {
                loading.gameObject.SetActive(false);
                return;
            }
            IsLoadingHouse = true;
            loading.gameObject.SetActive(true);
            AddressableLoadingHouse.Instance.UnloadScene(() =>
            {
                //DebugExtension.LogError("Load House Day Or Night DomeID = " + GameContext.CurrentIdDome);
                AddressableLoadingHouse.Instance.LoadSceneAsync(isChangeDay ? CurrentHouse.PathSceneDay : CurrentHouse.PathSceneNight).Forget();
            }).Forget();
        }
        void OnLoadSceneDone(GameObject[] roots)
        {
            IsLoadingHouse = false;
            if (stackDataIndexHouse >= 0 && stackDataIndexHouse != currentIndexHouseLoading)
            {
                LoadNewSceneSyncFromServer();
                return;
            }
            else
            {
                stackDataIndexHouse = -1;
            }
            loading.gameObject.SetActive(false);
            foreach (var item in roots)
            {
                Modelcontroller = item.GetComponent<MaterialController>();
                if (Modelcontroller)
                {
                    parentModel = item.transform;
                    OnChangeHouseModel?.Invoke(parentModel);
                    break;
                }
            }
            LoadMaterialSync(CurrentData.indexHouse, CurrentData.ListHouseMaterialData);
            LoadDataLand(CurrentHouse.PathSceneDay).Forget();
            OnOffGridMode();
            LandUpdateLight();
            CheckDomeStack();
        }
        void CheckDomeStack()
        {
            BaseScreenCtrlV2.IsLoadingRoom = false;
            //DebugExtension.LogError("===============================================================================");
            //DebugExtension.LogError("Load House DONE  DomeID = " + GameContext.CurrentIdDome);
            VRStackDataSync.CheckNextDomeStack(GameContext.CurrentIdDome);
        }
        public void OnOffTeleport(bool isEnable)
        {
            foreach (var item in listTelePort)
            {
                item.CanTeleport = isEnable;
            }
        }
        [ContextMenu("Set Data Test")]
        public void TestSetData()
        {
            List<MaterialDataNetWorkDetail> test = new List<MaterialDataNetWorkDetail>()
            {
                new MaterialDataNetWorkDetail(0, 3),
                new MaterialDataNetWorkDetail(1, 2),
                new MaterialDataNetWorkDetail(2, 1),
                new MaterialDataNetWorkDetail(3, 1),
                new MaterialDataNetWorkDetail(4, 2)
            };
            //LoadMaterialSync(test);
        }

        public void LoadMaterialSync(int indexHouse, List<HouseMaterialData> listHouse)
        {
            if (!Modelcontroller || listHouse == null || listHouse.Count == 0) return;
            if (indexHouse >= listHouse.Count) return;
            List<MaterialDataNetWorkDetail> listData = listHouse[indexHouse].ListMaterialSet;
            foreach (var item in listData)
            {
                UpdateMaterialSync(item);
            }
        }

        public void UpdateMaterialSyncRuntime(MaterialDataNetWorkDetail detail)
        {
            if (Modelcontroller)
                UpdateMaterialSync(detail);
            else
                stackData.Add(detail);
        }

        public void UpdateMaterialSync(MaterialDataNetWorkDetail detail)
        {
            if (!Modelcontroller || detail == null) return;
            MaterialSet set = Modelcontroller.GetMaterialSetByIndex(detail.indexMaterialSet);
            if (set == null) return;
            MaterialData material = set.GetMaterialDataByIndex(detail.indexMaterialDetail);
            if (material == null) return;

            foreach (var obj in set.TargetObjects)
            {
                obj.GetComponent<Renderer>().material = material.Material;
            }
        }

        IEnumerator AutoCheckStack()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.5f);
                //DebugExtension.LogError("Stack Call");
                if (stackData.Count > 0)
                {
                    for (int i = 0; i < stackData.Count; i++)
                    {
                        UpdateMaterialSync(stackData[i]);
                    }
                    stackData.Clear();
                }
            }
        }
        async UniTask LoadDataLand(string pathHouse)
        {
            string parentPath = Path.GetDirectoryName(pathHouse); // Get parent path
            string houseName = Path.GetFileName(parentPath); // Get last folder name
            // remove last folder name
            string desiredPath = parentPath.Substring(0, parentPath.Length - houseName.Length);
            //string finalPath = Path.Combine(desiredPath, "1.png").Replace("\\", "/");
            string finalPath = Path.Combine(desiredPath, "HouseData.json").Replace("\\", "/");
            Debug.Log(finalPath);
            //Assets\AddressableData\HouseData\House1\1.png

            //var model = Addressables.LoadAssetAsync<Sprite>(finalPath);
            var model = Addressables.LoadAssetAsync<TextAsset>(finalPath);
            await model.Task;

            DataJson = JsonUtility.FromJson<HouseDataJson>(model.Result.text);
            UpdateLand();
        }
        public void UpdateLand()
        {
            if (Modelcontroller == null) return;
            UpdateLandTilingMaterial();
            SetGridPosition();
        }
        void UpdateLandTilingMaterial()
        {
            if (DataJson == null) return;
            float scaleX = GameContext.Land_Setting_Left + DataJson.SizeX + GameContext.Land_Setting_Right;
            float scaleZ = GameContext.Land_Setting_FrontOf + DataJson.SizeZ + GameContext.Land_Setting_Behide;

            Vector3 v = Land.transform.localScale;
            Land.transform.localScale = new Vector3(scaleX, v.y, scaleZ);

            Vector3 p = Land.transform.position;
            MaskingPointPivotHouse maskingPointPivotHouse = Modelcontroller.GetComponentInChildren<MaskingPointPivotHouse>(true);
            Transform pivotHouse = maskingPointPivotHouse.PointPivot;
            //Land.transform.position = new Vector3((scaleX / 2 - DataJson.SizeX / 2), p.y, (scaleZ / 2 - DataJson.SizeZ / 2));
            Land.transform.position = new Vector3((pivotHouse.position.x + GameContext.Land_Setting_Left), p.y, (pivotHouse.position.z + GameContext.Land_Setting_FrontOf));
            parentGridPosition.transform.position = Land.transform.position;

            Vector2 newTiling = new Vector2(scaleX, scaleZ);
            foreach (var item in listMaterialsLand)
            {
                item.material.SetTextureScale("_MainTex", newTiling);
            }
            foreach (var item in listMaterialsGrid)
            {
                item.material.SetTextureScale("_MainTex", newTiling);
            }
        }
        public void OnOffGridMode()
        {
            objGrid.gameObject.SetActive(GameContext.IsGridMode);
        }
        List<GameObject> listObjectPosition = new List<GameObject>();
        //List<GameObject> listSideObjectPositioin = new List<GameObject>();
        GameObject objectDrag = null;
        int row;
        int col;
        void SetGridPosition()
        {
            if (DataJson == null) return;

            listObjectPosition.Clear();
            float scaleX = GameContext.Land_Setting_Left + DataJson.SizeX + GameContext.Land_Setting_Right;
            float scaleZ = GameContext.Land_Setting_FrontOf + DataJson.SizeZ + GameContext.Land_Setting_Behide;
            int j = 0, i = 0;
            row = (int)scaleZ;
            col = (int)scaleX;
            for (i = 0; i < scaleX; i++)
            {
                for (j = 0; j < scaleZ; j++)
                {
                    listObjectPosition.Add(CreateOneObjectPosition(i, j));
                }
            }
            if (scaleZ > j - 1)
            {
                for (i = 0; i < scaleX; i++)
                {
                    listObjectPosition.Add(CreateOneObjectPosition(i, scaleZ));
                }
            }
            if (scaleX > i - 1)
            {
                for (i = 0; i < scaleZ; i++)
                {
                    listObjectPosition.Add(CreateOneObjectPosition(scaleX, i));
                }
            }
            listObjectPosition.Add(CreateOneObjectPosition(scaleX, scaleZ));
            objectDrag = CreateOneObjectPosition(0, 0);
        }
        GameObject CreateOneObjectPosition(float x, float z)
        {
            GameObject obj = new GameObject("pos" + x + "" + z);
            obj.transform.SetParent(parentGridPosition.transform);
            obj.transform.localPosition = new Vector3(-x, 0, -z);
            return obj;
        }
        public Vector3 CheckGridSnap(Vector3 position, Vector3 oldPosition)
        {
            if (!GameContext.IsSnapGrid) return position;
            objectDrag.transform.position = position;
            int x = (int)objectDrag.transform.localPosition.x;
            int z = (int)objectDrag.transform.localPosition.z;
            //DebugExtension.LogError(x + "  " + z);
            if (x <= 0 && z <= 0 && x > -col && z > -row)
            {
                //    return listObjectPositioin[Math.Abs(x * row + col)].transform.position;
                //}
                //else
                //{
                float min = int.MaxValue;
                Vector3 minVec = Vector3.zero;
                foreach (var item in listObjectPosition)
                {
                    float distance = Vector3.Distance(objectDrag.transform.position, item.transform.position);
                    if (distance < min)
                    {
                        min = distance;
                        minVec = item.transform.position;
                    }
                }
                if (min <= 1) return minVec;
                else return oldPosition;
            }
            else return oldPosition;
        }
        void LandUpdateLight()
        {
            foreach (var item in listMaterialsLand)
            {
#if UNITY_EDITOR
                float m = 0.9f;
#else
                    float m = 0.99f;
#endif
                float newMetallicValue = GameContext.IsDayMode ? item.metallic : m;
                item.material.SetFloat("_Metallic", newMetallicValue);
                float newSmoothness = GameContext.IsDayMode ? item.smoothness : 0.1f;
                item.material.SetFloat("_Glossiness", newSmoothness);

            }
        }
    }
}
