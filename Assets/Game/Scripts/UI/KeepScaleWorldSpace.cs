using System;
using UnityEngine;

namespace Game.Client
{
    public class KeepScaleWorldSpace : MonoBehaviour
    {
        [SerializeField] private Vector3 scaleTarget = Vector3.one;

        private void Update()
        {
            var root = transform.root;
            var scale = root ? root.InverseTransformVector(scaleTarget) : scaleTarget;
            transform.localScale = scale;
        }
    }
}