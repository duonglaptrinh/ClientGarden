using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

public class EditAngleVrDocumentHelper : MonoBehaviour
{
    [SerializeField] private float distance = 2.2f;

    [SerializeField] private Transform transformCompare;

    [SerializeField] public RawImage image, video;
    
    private RotateVrDocumentHelper rotateVrDocumentHelper => RotateVrDocumentHelper.Instance;
    
    private bool IsSelected { get; set; }
    private float default_distance = 2;

    private void Start()
    {
        if (!GameContext.IsEditable) return;
        ObserverEdit();
    }

    private void OnEnable()
    {
        SetPoint(transform.position);
    }

    private void ObserverEdit()
    {
        image.OnPointerDownAsObservable()
            .Subscribe(_ =>
            {
                if (!IsSelected)
                {
                    rotateVrDocumentHelper.SetTrackObject(this);
                }
                else
                {
                    rotateVrDocumentHelper.DropObject();
                }

                IsSelected = !IsSelected;
            });

        video.OnPointerDownAsObservable()
            .Subscribe(_ =>
            {
                if (!IsSelected)
                {
                    rotateVrDocumentHelper.SetTrackObject(this);
                }
                else
                {
                    rotateVrDocumentHelper.DropObject();
                }

                IsSelected = !IsSelected;
            });

        image.OnPointerUpAsObservable()
            .Subscribe(_ =>
            {
                rotateVrDocumentHelper.DropObject();

                IsSelected = !IsSelected;
            });

        video.OnPointerUpAsObservable()
            .Subscribe(_ =>
            {
                rotateVrDocumentHelper.DropObject();

                IsSelected = !IsSelected;
            });
    }

    public void SetPoint(Vector3 pointRaw)
    {
        transform.position = GetPointToSet(pointRaw);
    }
    
    private Vector3 GetPointToSet(Vector3 pointRaw)
    {
        var position = transformCompare.position;
        var ray = new Ray(position, (pointRaw - position).normalized);
        var point = ray.GetPoint(distance);
        return point;
    }

    public void Drop()
    {
        //ObserverEdit();
    }

    public void ChangeDistance(float value)
    {
        distance = default_distance + value;
    } 
}