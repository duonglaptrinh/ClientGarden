using System;
using TWT.Networking;
using UniRx;
using UnityEngine;
using UnityEngine.UI;


public abstract class VRObjectV2 : MonoBehaviour
{
    [SerializeField] protected GameObject uiButtons;
    public GameObject UiButtons => uiButtons;

    [SerializeField] protected Button editBtn;
    protected virtual void SetPositionEditButton(Vector2 posNew)
    {
        Transform trans = editBtn.transform;
        trans.localPosition = posNew * 1.1f;
    }
    public bool IsAllowDrag { get; set; } = true; // User for model 3D 
    public virtual void OnOffDrag(bool value)
    {
        IsAllowDrag = value;
    }
    public abstract int Id { get; }
    public abstract ObjectType Type { get; }
    public abstract IObservable<Unit> OnSelectAsObservable();

    protected virtual void Awake()
    {
        //DebugExtension.LogError("Awake VRObject " + name);
        animator = GetComponentInChildren<Animator>();
        editBtn.onClick.AddListener(OnEditClick);
    }

    protected virtual void Start()
    {
        //DebugExtension.LogError("Start VRObject " + name);
    }

    public abstract void OnSelectedObject();

    public abstract void ShowMenuUiEdit();

    public abstract int GetObjectTransparent();

    public abstract void SetObjectTransparent(int value);

    public abstract void SetObjectScale(float value);

    public abstract void SetObjectScale(Vector3 value, int index);
    public abstract void SetObjectScale(float value, int index);

    public abstract void SetInteractable(bool isInteractable);

    private Animator animator;

    private static readonly int FlickerAnimationHash = Animator.StringToHash("Flicker");
    private static readonly int IdleAnimationHash = Animator.StringToHash("Idle");

    protected VRObjectManagerV2 VrObjectManager { get; private set; }

    public void Initialize(VRObjectManagerV2 vrObjectManager)
    {
        VrObjectManager = vrObjectManager;
    }

    protected virtual void OnEditClick()
    {
        ShowUiEditsExceptSelf();
    }

    public void ShowUiEditsExceptSelf(bool hideUiButton = true)
    {
        foreach (var vrObject in VrObjectManager.VrObjects)
        {
            if (vrObject.GetInstanceID() != this.GetInstanceID())
            {
                vrObject.ShowMenuUiEdit();
            }
            else
            {
                UiButtons.SetActive(!hideUiButton);
                //DebugExtension.LogError("ShowUiEditsExceptSelf = " + !hideUiButton);
            }
        }
    }

    public void ShowAllUiEdits()
    {
        foreach (var vrObject in VrObjectManager.VrObjects)
        {
            vrObject.ShowMenuUiEdit();
        }
    }

    public void FlickerVrObject(bool status)
    {
        if (animator == null)
            return;
        animator.enabled = true;
        if (status)
            animator.SetTrigger(FlickerAnimationHash);
        else
        {
            animator.SetTrigger(IdleAnimationHash);
            Observable.Timer(TimeSpan.FromSeconds(0.2f)).Subscribe(_ =>
            {
                if (animator)
                    animator.enabled = false;
                SetObjectTransparent(GetObjectTransparent());
            });
        }
    }

    public void SwitchToIdleStage()
    {
        if (animator == null)
            return;
        animator.enabled = true;
        animator.SetTrigger(IdleAnimationHash);
        Observable.Timer(TimeSpan.FromSeconds(0.2f)).Subscribe(_ =>
        {
            if (animator)
                animator.enabled = false;
            SetObjectTransparent(GetObjectTransparent());
        });

    }

    /// <summary>
    /// Call when user is student join room --> if all object is hide --> reset all
    /// </summary>
    public bool CheckShowAllVRObject()
    {
        return true;
    }
    public abstract void ShowVrObject(bool status);

    public abstract void ResetToDefault();

    public static Sprite ConvertTexture2DToSprite(Texture2D texture)
    {
        return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}