using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Takasho.GLD.VGS
{
    /// <summary>
    /// Implements functionality for Product Prefabs
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ProductController : MonoBehaviour
    {
        private Collider _collider;
        private UIManager _UIManager;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
            OnStartMoving();
        }

        public void InitialezeProduct(UIManager UIManager)
        {
            _UIManager = UIManager;
        }

        public void OnStartMoving()
        {
            _collider.enabled = false;
        }

        public void OnPlaced()
        {
            Debug.Log("released");
            _collider.enabled = true;

            RaycastHit hit;
            Ray ray = new Ray(transform.position, Vector3.down);

            if (Physics.Raycast(ray, out hit, 10f, LayerMask.NameToLayer("Garden")))
            {
                transform.position = hit.point;
            }

        }


        private void OnMouseEnter()
        {
            if (_UIManager != null)
            {
                _UIManager.SetCursor(UIManager.CursorType.PRODUCT);
            }
        }

        private void OnMouseExit()
        {
            if (_UIManager != null)
            {
                if (_UIManager.CurrentMenuState != UIManager.MenuState.GRAB)
                {
                    _UIManager.SetCursor(UIManager.CursorType.DEFAULT);
                }
            }
        }


    }
}
