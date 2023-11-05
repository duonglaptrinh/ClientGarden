using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFloorSetting : MonoBehaviour
{
    [SerializeField] private float horizontal = 1;
    [SerializeField] private float vertical = 1;

    private float Horizontal => horizontal * 0.1f;

    private float Vertical => vertical * 0.1f;

    private void OnValidate()
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        var scale = new Vector3(Horizontal, 1, Vertical);
        transform.localScale = scale;
    }
}