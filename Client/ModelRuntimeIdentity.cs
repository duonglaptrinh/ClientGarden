using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class ModelRuntimeIdentity : MonoBehaviour
{
    [SerializeField] private bool persistence;

    public bool Persistence => persistence;
}