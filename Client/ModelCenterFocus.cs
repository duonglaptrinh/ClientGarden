using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelCenterFocus : MonoBehaviour
{
    public float distance;

    [SerializeField] Stage1ModelLoader modelLoader;
    
    void LateUpdate()
    {
        if(modelLoader!=null && modelLoader.ModelCenter!=null)
        {
            gameObject.transform.position = modelLoader.ModelCenter.position + Vector3.right * distance;
        }
    }
}
