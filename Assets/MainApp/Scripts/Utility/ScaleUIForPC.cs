using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleUIForPC : MonoBehaviour
{
    public float scale = 1.3f;

    // Start is called before the first frame update
    void Start()
    {
//#if UNITY_STANDALONE
        transform.localScale *= scale;
//#endif
    }

}
