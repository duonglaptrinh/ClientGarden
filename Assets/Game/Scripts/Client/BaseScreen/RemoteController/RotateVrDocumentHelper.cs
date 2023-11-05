using TWT.Client.Interaction;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class RotateVrDocumentHelper : MonoBehaviour, IControllerInteractable
{
    [SerializeField] private LayerMask targetLayer;

    private Camera cam;
    private EditAngleVrDocumentHelper objectTrack;
    
    public static RotateVrDocumentHelper Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if(!GameContext.IsEditable) return;
        cam = Camera.main;
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (objectTrack == null)
                    return;

                Ray ray = this.GetRay();

                if (Physics.Raycast(ray, out var hit, Mathf.Infinity, targetLayer))
                {
                    objectTrack.SetPoint(hit.point);

                }
            });
    }

    public void SetTrackObject(EditAngleVrDocumentHelper obj)
    {
        DropObject();
        this.objectTrack = obj;
    }

    public void DropObject()
    {
        if (objectTrack)
        {
            objectTrack.Drop();
            objectTrack = null;
        }
    }

    public Transform Transform => transform;
}