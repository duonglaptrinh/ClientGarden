using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

public interface IRotatableObject
{
    void Rotate(Vector3 rotate);
}

public class RotatableObject : MonoBehaviour, IRotatableObject
{
    public float MinRotationX { private get; set; }

    public float MaxRotationX { private get; set; }

    public float MinRotationY { private get; set; }

    public float MaxRotationY { private get; set; }

    public float MinRotationZ { private get; set; }

    public float MaxRotationZ { private get; set; }

    private float RotationX { get; set; }
    private float RotationY { get; set; }
    private float RotationZ { get; set; }

    public void Rotate(Vector3 rotate)
    {
        RotationX += rotate.x;
        RotationY += rotate.y;
        RotationZ += rotate.z;

        RotationX = Mathf.Clamp(RotationX, MinRotationX, MaxRotationX);
        RotationY = Mathf.Clamp(RotationY, MinRotationY, MaxRotationY);
        RotationZ = Mathf.Clamp(RotationZ, MinRotationZ, MaxRotationZ);

        var euler = Quaternion.Euler(RotationX, RotationY, RotationZ);
        transform.rotation = euler;
    }

    public void ResetRotation()
    {
        MinRotationX = 0;
        MaxRotationX = 0;
        MinRotationY = 0;
        MaxRotationY = 0;
        MinRotationZ = 0;
        MaxRotationZ = 0;
    }
}