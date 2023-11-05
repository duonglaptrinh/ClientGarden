using DG.Tweening;
using System;
using System.Collections.Generic;
using TWT.Model;
using TWT.Networking;
using TWT.Utility;
using UniRx;
using Cysharp.Threading.Tasks;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Linq;
using System.Threading;
using Newtonsoft.Json.Linq;
using Outline = ThirdOutline.Outline;

public class VRModelV2 : VRObjectV2, IEditableVrObject
{
    MenuEditAutoLookCamera checkPosButtonEdit;
    [SerializeField] private Transform parentModel;
    [Header("Loading Progress")]
    [SerializeField] private GameObject cubeLoading;
    [SerializeField] private Text textLoadingProgress;

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
    private DragObjectManager dragObjectManager;
#endif
    private VRModelData vrModelData;
    //private CancellationTokenSource cancellationTokenSrc;
    private Tweener tween;
    private Animation modelAnimation;
    public BoxCollider colliderComp { get; set; }
    private MeshCollider McolliderComp;
    public LayerMask LayerMaskObject;
    public bool IsJustCreatNew { get; set; }
    public bool IsEditedTransform { get; set; }
    float OldPosY = 0;
    bool OnAnotherObject = true;
    public override int Id => vrModelData != null ? vrModelData.model_id : -1000;
    public override ObjectType Type => ObjectType.Model3D;
    public event Action OnPointerDown;
    public event Action OnDrag;
    public event Action OnPointerUp;
    AsyncOperationHandle<GameObject> addressableHandle;
    GameObject loadedModel;
    public OneModelAssetsData DataAsset { get; set; }
    public List<Collider> ListColliderOfLoadModel { get; set; }
    public List<DicDataMaterial> DicMaterial { get; set; }
    Action OnloadModelDone = null;
    public ProductSizeController SizeControllerComponent { get; set; }
    public ProductColorController ColorControllerComponent { get; set; }

    // Start is called before the first frame update
    public float distanceToModel = 0;
    public bool isCheckClickModel = false;
    void Start()
    {
        checkPosButtonEdit = UiButtons.GetComponent<MenuEditAutoLookCamera>();
        //UiButtonEdit = checkPosButtonEdit.gameObject;
        DOTween.Init();
        //treeBoxPrefab = Resources.Load<GameObject>("UI/CapsuleTree");
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        dragObjectManager = FindObjectOfType<DragObjectManager>();
#endif
    }

    /// <summary>
    /// Set layer to find object can draggable
    /// </summary>
    void SetLayerMask(LayerMask layer)
    {
        int ground = 10,
            avatar = 15,
            domeSurface = 11,
            groundProduct = 16, //nha de xe cong
            table = 17,
            furniture = 18,
            plant = 19;

        LayerMaskObject |= (1 << domeSurface);
        if (layer == groundProduct) //if ((layer & groundProduct) != 0)
        {
            LayerMaskObject |= (1 << ground);
        }
        else if (layer == table) //else if ((layer & table) != 0)
        {
            LayerMaskObject |= (1 << ground);
            LayerMaskObject |= (1 << table);
        }
        else if (layer == furniture)
        {
            LayerMaskObject |= (1 << furniture);
            LayerMaskObject |= (1 << table);
            LayerMaskObject |= (1 << ground);
        }
        else if (layer == plant)
        {
            LayerMaskObject |= (1 << furniture);
            LayerMaskObject |= (1 << table);
            LayerMaskObject |= (1 << ground);
        }
    }

    void OnDestroy()
    {
        Addressables.ReleaseInstance(addressableHandle);
        if (checkPosButtonEdit) Destroy(checkPosButtonEdit.gameObject);

        if (tween.IsPlaying())
            tween.Kill();
    }

    void OnMouseDown()  // Won't be called on VR
    {
        if (!IsAllowDrag) return;
#if UNITY_EDITOR
        if (EventSystem.current.IsPointerOverGameObject())
            return;
#elif !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(0))
            return;
#endif
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        OnPointerDown?.Invoke();
    }

    void OnMouseDrag()  // Won't be called on VR
    {
        if (!IsAllowDrag) return;
#if UNITY_EDITOR
        if (EventSystem.current.IsPointerOverGameObject())
            return;
#elif !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(0))
            return;
#endif
        OnDrag?.Invoke();
    }

    void OnMouseUp()  // Won't be called on VR
    {
        if (!IsAllowDrag) return;
#if UNITY_EDITOR
        if (EventSystem.current.IsPointerOverGameObject())
            return;
#elif !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(0))
            return;
#endif
        OnPointerUp?.Invoke();
    }

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
    void Update()
    { 
        if (!IsAllowDrag) return;
        if (XRSettings.enabled)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || (OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger)))
            {
                Ray ray = new Ray(dragObjectManager.transform.position, dragObjectManager.transform.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("VRObject", "UI", "Default")))
                {
                    DebugExtension.Log(hit.transform.name);
                    if (hit.transform == this.transform)
                        OnPointerDown?.Invoke();
            }
        }
    }
#endif
    public void SetButtonEdit()
    {
        if (checkPosButtonEdit == null)
            checkPosButtonEdit = UiButtons.GetComponent<MenuEditAutoLookCamera>();
        checkPosButtonEdit.transform.parent = VRObjectManagerV2.ParentEditMenu;
    }
    public void SetJustCreateNew(bool isNew)
    {
        IsJustCreatNew = isNew;
        if (vrModelData != null)
            vrModelData.IsJustCreatNew = isNew;
    }
    public async UniTask SetData(VRModelData data, bool isFirstTimeCreateData = false, bool isUpdateMaterial = false)
    {
        string model_url = data.model_url;
        DebugExtension.Log(model_url);
        this.vrModelData = data;
        if (textLoadingProgress)
        {
            textLoadingProgress.fontSize = 26;
            textLoadingProgress.text = "Loading";
        }

        Transform parentCubeLoading = cubeLoading.transform.parent;
        parentCubeLoading.gameObject.SetActive(true);

        parentCubeLoading.rotation = Quaternion.identity;
        parentCubeLoading.localScale = Vector3.one;//GameObjectUtility.StringToVector3(data.model_scale).x;
        // Spin the cube around to indicate that the model is being loaded
        tween = cubeLoading.transform.DOLocalRotate(new Vector3(0, 360, 0), 1.5f, RotateMode.LocalAxisAdd)
                                     .SetEase(Ease.InOutQuint)
                                     .SetLoops(10000, LoopType.Restart);
        DataAsset = AddressableDownloadManager.ResourcesData.GetDataAssetByPrefabPath(model_url);
        if (DataAsset == null) return;
        //DebugExtension.Log("DatacurrentModel    " + DataAsset.NameOnApp);
        try
        {
            addressableHandle = Addressables.InstantiateAsync(DataAsset.pathPrefab);
            await addressableHandle.Task;

            parentCubeLoading.gameObject.SetActive(false);   // Actually destroying its parent gameObject
            tween.Kill();
            textLoadingProgress.text = "";
            loadedModel = addressableHandle.Result;
            loadedModel.transform.SetParent(parentModel);
            loadedModel.transform.localPosition = Vector3.zero;
            loadedModel.transform.localEulerAngles = Vector3.zero;
            //DebugExtension.Log("InitializeAsync Model Done !!!");
            LoadModelDone(data, isFirstTimeCreateData, isUpdateMaterial);
        }
        catch (Exception e)
        {
            //Destroy(gameObject);
        }
        //};
    }
    void LoadModelDone(VRModelData data, bool isFirstTimeCreateData = false, bool isUpdateMaterial = false)
    {
        ChangeColorWhenSwitchDayNightMode changeColor = GetComponentInChildren<ChangeColorWhenSwitchDayNightMode>(true);
        changeColor?.ChangeColor();

        bool isNeedCreateMesh = loadedModel.GetComponent<ModelSupportCreatePrefab>().IsNeedAutoCreateMeshCollider;
        DicMaterial = new List<DicDataMaterial>();
        Renderer[] arr = loadedModel.GetComponentsInChildren<Renderer>(true);
        foreach (var render in arr)
        {
            if (isNeedCreateMesh)
            {
                render.gameObject.AddComponent<MeshCollider>();
                //render.gameObject.layer = 9;// LayerMask.NameToLayer("Default");
            }
            foreach (var mat in render.materials)
            {
                DicMaterial.Add(new DicDataMaterial(render, mat, mat.color));
            }
        }
        ListColliderOfLoadModel = loadedModel.GetComponentsInChildren<Collider>(true).ToList();
        ChangeLight(isChangeWhenStart: true);
        //loadedModel.layer = 9;
        SetLayerMask(loadedModel.layer);

        this.transform.localEulerAngles = GameObjectUtility.StringToVector3(data.model_rotation);

        // Gradually show the loaded object
        loadedModel.transform.DOScale(1, .7f)
                             .SetEase(Ease.OutQuart)
                             .onComplete = () =>
                             {
                                 // Add BoxCollider for VRModel gameObject
                                 colliderComp = gameObject.AddComponent<BoxCollider>();
                                 colliderComp.isTrigger = true;
                                 Bounds modelBounds = GameObjectUtility.CalculateLocalBounds(this.gameObject);
                                 loadedModel.transform.localPosition = new Vector3(-modelBounds.center.x / 100, 0, -modelBounds.center.z / 100);
                                 distanceToModel = Vector3.Distance(Vector3.zero, modelBounds.center);
                                 if (IsJustCreatNew && isFirstTimeCreateData)
                                 {
                                     var forward = Camera.main.transform.forward.normalized * (2f + distanceToModel);
                                     transform.position = Camera.main.transform.position + new Vector3(forward.x, -1f, forward.z);
                                 }
                                 //colliderComp.center = modelBounds.center / this.transform.localScale.x;
                                 colliderComp.center = new Vector3(-modelBounds.center.x / 100, modelBounds.size.y / 2, -modelBounds.center.z / 100);
                                 colliderComp.size = new Vector3(modelBounds.size.x / this.transform.localScale.x, modelBounds.size.y / this.transform.localScale.y, modelBounds.size.z / this.transform.localScale.z);
                                 if (CheckShowAllVRObject())
                                     ShowHideModel(true);
                                 else
                                     ShowHideModel(false);
                                 //SetObjectTransparent(0);
                                 checkPosButtonEdit.myParent = transform;
                                 checkPosButtonEdit.SetUpdate(colliderComp);
                                 gameObject.layer = 0;
                                 //if (!isUpdateMaterial)
                                 Vector3 scale = GameObjectUtility.StringToVector3(data.model_scale);
                                 if (SizeControllerComponent != null)
                                     SizeControllerComponent.ShowObjectSize(scale.y, scale.x, scale.z);
                                 else
                                     transform.localScale = scale;
                                 ChangeTilingTexture();
                                 OnloadModelDone?.Invoke();
                                 OnloadModelDone = null;
                                 //SaveOldPositionY();
                                 InstanceBoxTree();
                                 UiButtons.gameObject.SetActive(VRObjectManagerV2.Instance.IsAllowShowUIEdit);
                                 //DebugExtension.LogError("Load Model Done = " + VRObjectManagerV2.Instance.IsAllowShowUIEdit);
                             };

        if (data.isOutline)
        {
            SetOutline();
            isCheckClickModel = true;
        }
        IsAllowDrag = VRObjectManagerV2.Instance.IsAllowShowUIEdit;
        SizeControllerComponent = loadedModel.GetComponentInChildren<ProductSizeController>();
        ColorControllerComponent = loadedModel.GetComponentInChildren<ProductColorController>();
        ColorControllerComponent?.Setup(arr);
        UpdateMaterial();
    }

    public override void OnOffDrag(bool value)
    {
        //base.OnOffDrag(value);
    }
    public void PlayAnimation(string animationName)
    {
        if (modelAnimation != null)
        {
            if (string.IsNullOrEmpty(animationName))
                modelAnimation.Stop();
            else
                modelAnimation.Play(animationName);
        }
        bool canAccess = GameContext.IsTeacher && !GameContext.IsEditable;
        if (!canAccess) return;

        var message = new SyncAnimationModel3DMessage()
        {
            id = Id,
            nameAnimation = animationName
        };
        VrgSyncApi.Send(message);
    }

    public VRModelData GetData()
    {
        return vrModelData;
    }

    public Animation GetModelAnimation()
    {
        return modelAnimation;
    }

    public override int GetObjectTransparent()
    {
        return vrModelData.vr_model_transparent_type;
    }

    public override IObservable<Unit> OnSelectAsObservable()
    {
        throw new NotImplementedException();
    }

    public override void OnSelectedObject() { }

    public override void ResetToDefault()
    {
        SetObjectTransparent(GetObjectTransparent());
    }

    public override void SetObjectScale(float value)
    {
        transform.localScale = new Vector3(value, value, value);
        ChangeTilingTexture();
    }

    //set scale when recieve message from server
    public override void SetObjectScale(Vector3 value, int index)
    {
        if (SizeControllerComponent != null)
            SizeControllerComponent.ShowObjectSize(value.y, value.x, value.z);
        else
            transform.localScale = value;
        ChangeTilingTexture();
    }
    //Set Scale for myself
    public override void SetObjectScale(float value, int index)
    {
        DebugExtension.Log("SetObjectScale có index");
        Vector3 newScale = transform.localScale;
        switch (index)
        {
            case 1:
                newScale.x = value;
                break;
            case 2:
                newScale.y = value;
                break;
            case 3:
                newScale.z = value;
                break;
            default:
                newScale.z = value;
                break;
        }
        transform.localScale = newScale;
        ChangeTilingTexture();
    }
    public override void ShowMenuUiEdit()
    {
        //if (GameContext.IsEditable)
        //{
        //UiButtons.GetComponent<VrObjectEditUi>().ResetToDefault();
        UiButtons.gameObject.SetActive(VRObjectManagerV2.Instance.IsAllowShowUIEdit);
        // foreach (var item in VrObjectManager.vrModels)
        // {
        //     if (item.isCheckClickModel)
        //     {
        //         item.UiButtons.gameObject.SetActive(true);
        //     }
        // }

        //DebugExtension.LogError("ShowMenuUiEdit = " + VRObjectManagerV2.Instance.IsAllowShowUIEdit);
        //canvasUIEditModeRT.gameObject.SetActive(true);
        //BaseScreenCtrlV2.Instance.HidePickerDialog();
        //}
    }

    public override void ShowVrObject(bool status)
    {
        if (status)
        {
            //SetObjectTransparent(GetObjectTransparent());
            ShowHideModel(true);
        }
        else
        {
            //SetObjectTransparent(0);
            ShowHideModel(false);
        }
    }

    public override void SetInteractable(bool isInteractable)
    {
        if (colliderComp)
            colliderComp.enabled = isInteractable;

        if (McolliderComp)
            McolliderComp.enabled = isInteractable;
        foreach (var meshCollider in GetComponentsInChildren<MeshCollider>(true))
        {
            if (meshCollider != null)
            {
                meshCollider.enabled = isInteractable;
            }
        }
    }

    public override void SetObjectTransparent(int value)
    {
        if (value > 125)
        {
            ShowHideModel(true);
        }
        else
        {
            ShowHideModel(false);
        }
    }

    void ShowHideModel(bool isShow)
    {
        loadedModel?.gameObject.SetActive(isShow);
    }

    protected override void OnEditClick()
    {
        base.OnEditClick();
        if (!Input.GetKeyDown(KeyCode.Space))
        {
            MenuTabControllerV2.Instance.ShowVrModelSettingDialog(this, DataAsset);
        }
        MenuTabControllerV2.Instance.ShowVRObjectSettingTab(this); //----------------------------============================================
        //checkPosButtonEdit.gameObject.SetActive(false);
        //Task 785 - dont need hide button Edit
        //uiButtons.gameObject.SetActive(false);
    }

    public void UpdateMaterialForLoadModel(int indexColor)
    {
        if (!loadedModel) return;
        if (ColorControllerComponent)
            ColorControllerComponent.UpdateMaterial(indexColor);
        else
            loadedModel.GetComponent<ModelSupportCreatePrefab>().ShowModel(indexColor);
    }
    public void UpdateMaterial()
    {
        if (this.vrModelData.nameTexture < 0)
        {
            return;
        }
        int indexColor = vrModelData.nameTexture;
        //try
        //{
        //int.TryParse(vrModelData.nameTexture, out indexColor);
        UpdateMaterialForLoadModel(indexColor);
        //}
        //catch (Exception e)
        //{
        //    DebugExtension.LogError(loadedModel.name + " Error Index Color = " + vrModelData.nameTexture);
        //}
        //SetData(this.vrModelData, false, true).Forget();
    }
    public void SaveOldPositionY()
    {
        if (IsJustCreatNew && !IsEditedTransform) return;
        OldPosY = transform.position.y;
    }
    public void ResetPositionY()
    {
        if (IsJustCreatNew && !IsEditedTransform) return;
        Vector3 pos = transform.position;
        pos.y = OldPosY;
        transform.position = pos;
    }

    public void SetOnAnotherObject(bool status)
    {
        OnAnotherObject = status;
    }

    public bool GetOnAnotherObject()
    {
        return OnAnotherObject;
    }
    public void InstanceBoxTree()
    {
        //foreach (CapsuleCollider m in GetComponentsInChildren<CapsuleCollider>())
        //{
        //    if (m.gameObject.tag == "Tree")
        //    {
        //        CapsuleCollider thisCollider = Instantiate(treeBoxPrefab, m.transform.parent).GetComponent<CapsuleCollider>();
        //        thisCollider.gameObject.layer = 19;
        //        thisCollider.transform.localPosition = Vector3.zero;
        //        thisCollider.center = new Vector3(m.center.x, 1.5f, m.center.z);
        //        thisCollider.radius = m.radius;
        //        thisCollider.height = 3;
        //    }
        //}
    }
    public void ChangeLayer(int layer)
    {
        foreach (var item in ListColliderOfLoadModel)
        {
            item.gameObject.layer = layer;
        }
    }
    public void ChangeLight(bool isChangeWhenStart = false)
    {
        if (isChangeWhenStart && GameContext.IsDayMode) return;
        Color32 dark = new Color(0.654f, 0.654f, 0.654f, 1);
        foreach (var item in DicMaterial)
        {
            item.ChangeMetalic(GameContext.IsDayMode);
            //    item.material.color = item.colorOrigin;
            //else item.material.color = dark;
        }
    }
    public void ChangeTilingTexture()
    {
        //Vector3 scale = transform.localScale;
        //Vector2 newTiling = new Vector2(scale.x, scale.y);
        //foreach (var item in DicMaterial)
        //{
        //    var mat = item.material;
        //    mat.SetTextureScale("_MainTex", newTiling);
        //}
    }
    public void SetOutline()
    {
        Renderer[] arr = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (var render in arr)
        {
            Outline outline = render.GetComponent<Outline>();
            if (!outline) outline = render.gameObject.AddComponent<Outline>();
            outline.OutlineWidth = 12;
            outline.OutlineColor = Color.yellow;
        }
    }
    public void RemoveOutline()
    {
        Renderer[] arr = gameObject.GetComponentsInChildren<Renderer>(true);
        foreach (var render in arr)
        {
            var outline = render.GetComponent<Outline>();
            if (outline)
            {
                Destroy(outline);
            }
        }
    }

    #region Test Delete
    [ContextMenu("DeleteThisModel")]
    void DeleteThisModel()
    {
        VRObjectManagerV2.Instance.DeleteVrModel(this);
    }
    #endregion
}
