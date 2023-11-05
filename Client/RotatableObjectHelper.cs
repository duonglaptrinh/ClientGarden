using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class RotatableObjectHelper : MonoBehaviour
{
    public IObservable<RotatableObject> OnChangeObjectRotatableAsObservable()
    {
        const float length = 10000;
        const int myLayerMask = -1;
        return this.UpdateAsObservable()
            .Select(_ =>
            {
                var myRay = new Ray(transform.position, transform.forward);

                RaycastHit hit;
                return Physics.Raycast(myRay, out hit, length, myLayerMask)
                    ? hit.collider.GetComponent<RotatableObject>()
                    : null;
            }).Where(x => x != null);
    }
}