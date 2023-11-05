using UnityEngine;

public class LookAtPosition : MonoBehaviour
{
    [SerializeField] private Vector3 point;

    private void OnEnable()
    {
        transform.forward = -(point - transform.position).normalized;
    }

    private void Start()
    {
        transform.forward = -(point - transform.position).normalized;
    }
    
    private void LateUpdate()
    {
        if(GameContext.IsEditable)
           transform.forward = -(Camera.main.transform.position - transform.position).normalized;
    }
}