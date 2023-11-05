using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlCurvedMeshCollider : MonoBehaviour
{
    [SerializeField] private MeshCollider meshCollider;

    private MeshCollider MeshCollider => meshCollider ? meshCollider : meshCollider = GetComponent<MeshCollider>();

    public void EnableMeshCollider(bool value)
    {
        if (MeshCollider != null)
            MeshCollider.enabled = value;
    }
}