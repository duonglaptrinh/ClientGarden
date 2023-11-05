using System.Collections;
using System.Collections.Generic;
using TWT.Model;
using UnityEngine;

public class MenuEditAutoLookCamera : MonoBehaviour
{
    [SerializeField] LayerMask layer;
    bool isCanUpdate = false;
    BoxCollider colider;
    public Transform myParent { get; set; }
    float distanceBox = 0;
    Vector3 posCenter => myParent.TransformPoint(colider.center);
    //VRModelData currentData;
    float t = 0;
    void Awake()
    {
        //myParent = transform.parent;
    }

    private void OnEnable()
    {
        //StopCoroutine("Loop");
        //StartCoroutine("Loop");
        //if (isCanUpdate) 
        // StartCoroutine(CheckPosCorotine());
        //transform.localScale = Vector3.one / Mathf.Max(myParent.localScale.x, myParent.localScale.y, myParent.localScale.z);
        //   DebugExtension.LogError(" Update Scale Enable ---- " + transform.localScale);
    }

    private void Update()
    {
        t += Time.deltaTime;
        if (t >= 1)
        {
            t = 0;
            //transform.localScale = Vector3.one / Mathf.Max(myParent.localScale.x, myParent.localScale.y, myParent.localScale.z);
            //DebugExtension.LogError(" Update Scale  ---- " + transform.localScale);
        }
        if (colider != null && myParent != null)
            CheckPos();
    }

    public void SetUpdate(BoxCollider co)
    {
        //if (!GameContext.IsEditable) return;
        isCanUpdate = true;
        colider = co;
        CheckPos();
    }
    public void SetCanNotUpdate()
    {
        isCanUpdate = false;
    }
    Vector3 hitPoint; // Only test on Editor
    IEnumerator CheckPosCorotine()
    {
        int i = int.MaxValue;
        do
        {
            CheckPos();
            i--;
            yield return new WaitForSeconds(0.2f);
        } while (i >= 0);
    }

    void CheckPos()
    {
        if (!colider.enabled)
            return;
        Vector3 fwd = Camera.main.transform.position - posCenter;
        RaycastHit hit;

        //if (Physics.Raycast(posCenter, fwd, out hit, 5.0f, layer))      

        if (Physics.Raycast(posCenter + fwd.normalized * distanceBox, -fwd, out hit, 1f, layer))
        {
            if (hit.transform.gameObject != myParent.gameObject)
            {
                distanceBox = distanceBox + 0.2f;
                if (distanceBox >= 5)
                    distanceBox = 0;
                return;
            }
            hitPoint = hit.point;
            Vector3 pos = hit.point + fwd.normalized * 0.3f;
            pos.y += 0.25f;
            transform.position = pos;
            //distanceBox = Mathf.Sqrt(Mathf.Pow(hit.point.x - posCenter.x, 2) + Mathf.Pow(hit.point.y - posCenter.y, 2) + Mathf.Pow(hit.point.z - posCenter.z, 2))+0.01f;
            // DebugExtension.Log(hit.point);
            distanceBox = Vector3.Distance(hit.point, posCenter) + 0.3f;

        }
        else
        {
            distanceBox = distanceBox + 0.2f;
            if (distanceBox >= 5)
                distanceBox = 0;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (myParent && colider)
        {
            Vector3 fwd = Camera.main.transform.position - posCenter;
            Gizmos.color = Color.yellow;
            // Debug.DrawLine(t, Vector3.up);
            Gizmos.DrawLine(posCenter, posCenter + fwd.normalized * 5);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(hitPoint, hitPoint + fwd.normalized * 0.2f);
        }
    }
#endif
}
