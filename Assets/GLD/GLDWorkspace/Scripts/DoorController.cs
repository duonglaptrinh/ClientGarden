using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Takasho.GLD.VGS
{
    public class DoorController : MonoBehaviour
    {
        [SerializeField] private Transform _door;

        [SerializeField] float angle = 90f;
        [SerializeField] Vector3 axis = Vector3.up;
        [SerializeField] float interpolant = 0.2f;

        Quaternion targetRot;
        Quaternion returnRot;
        List<MeshCollider> list = new List<MeshCollider>();
        private bool _isOpen = false;
        public bool _isClose
        {
            get { return _isOpen; }
            set { _isOpen = !value; }
        }

        private void Awake()
        {
            foreach (var item in GetComponentsInChildren<MeshCollider>())
            {
                list.Add(item);
            }
        }

        private void Start()
        {
            targetRot = Quaternion.AngleAxis(angle, axis) * _door.rotation;
            returnRot = Quaternion.AngleAxis(0f, axis) * _door.rotation;
        }

        /// <summary>
        /// need refactoring
        /// </summary>
        private void Update()
        {
            if (_isOpen)
            {
                _door.rotation = Quaternion.Lerp(_door.rotation, targetRot, interpolant);
            }
            else
            {
                _door.rotation = Quaternion.Lerp(_door.rotation, returnRot, interpolant);
            }
            
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isOpen = true;
                SetTriggerMeshChildrent();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isOpen = false;
                SetUnTriggerMeshChildrent();
            }
        }
        void SetTriggerMeshChildrent()
        {
            foreach (var item in list)
            {
                item.convex = true;
                item.isTrigger = true;
            }
        }
        void SetUnTriggerMeshChildrent()
        {
            foreach (var item in list)
            {
                item.isTrigger = false;
                item.convex = false;
            }
        }
    }
}

