using System.Collections;
using System.Collections.Generic;
using TWT.Client.Interaction;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Client
{
    public class VrInteractController : MonoBehaviour, IControllerInteractable
    {
        public static VrInteractController Instance { get; protected set; }

        [SerializeField] private LayerMask layerMaskDetect;
        [SerializeField] private LayerMask layerMaskDetectCurrent;


        private void Awake()
        {
            Instance = this;
            layerMaskDetectCurrent = layerMaskDetect;
        }

        private void Start()
        {
            //SwitchHandTo(CurvedUIInputModule.Hand.Right);
        }

        IVrInteractable currentInteractable;

        bool isDrag = false;

        private void Update()
        {
            if (BaseScreenTopMenuV2.Instance.isCameraRotating) return;
            if (GameContext.IsEditable) return;

            float length = 100000;

            var myRay = this.GetRay();

            if (Physics.Raycast(myRay, out var hit, length, layerMaskDetectCurrent))
            {
                var interactable = hit.collider.GetComponentInParent<IVrInteractable>();
                if (interactable != null)
                {
                    if (currentInteractable == interactable)
                    {
                    }
                    else
                    {
                        if (currentInteractable != null)
                        {
                            currentInteractable.OnVRRayExit();
                        }

                        currentInteractable = interactable;

                        if (currentInteractable != null)
                            currentInteractable.OnVRRayEnter();
                    }

                    if (currentInteractable != null)
                    {
                        currentInteractable.OnVRRayOn(hit.point);
                        if (
                            #if !UNITY_WEBGL
                            OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.All) ||
                            #endif
                            Input.GetKeyDown(KeyCode.J) ||
                            Input.GetMouseButtonDown(0))
                        {
                            var IsMouseOrTouchOverUI =this.IsMouseOrTouchOverUI();
                            //var Drawable = ((DrawBoard)interactable).tag.Equals("Drawable");
                            var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
                            
                            if ((!IsMouseOrTouchOverUI) && !currentSelectedGameObject)
                            {
                                currentInteractable.OnVRClickDown(hit.point);
                                layerMaskDetectCurrent = interactable.LayerMask;
                            }
                        }
                        else if (
                            #if !UNITY_WEBGL
                                 OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.All) ||
                            #endif
                                 Input.GetKeyUp(KeyCode.J) ||
                                 Input.GetMouseButtonUp(0))
                        {
                            currentInteractable.OnVRClickUp(hit.point);
                            layerMaskDetectCurrent = layerMaskDetect;
                        }

                        if (
                            #if !UNITY_WEBGL
                            OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.All) ||
                            #endif
                            Input.GetKey(KeyCode.J) ||
                            Input.GetMouseButton(0))
                        {
                            currentInteractable.OnVRDragOn(hit.point);
                        }
                    }
                }
                else
                {
                    if (currentInteractable != null)
                        currentInteractable.OnVRRayExit();

                    currentInteractable = null;
                }
            }
            else
            {
                if (currentInteractable != null)
                    currentInteractable.OnVRRayExit();

                currentInteractable = null;
                layerMaskDetectCurrent = layerMaskDetect;
            }
        }

//        void SwitchHandTo(CurvedUIInputModule.Hand newHand)
//        {
//#if UNITY_ANDROID && CURVEDUI_TOUCH
//            CurvedUIInputModule.Instance.UsedHand = newHand;
//            transform.SetParent(CurvedUIInputModule.Instance.OculusTouchUsedControllerTransform);
//            transform.localPosition = Vector3.zero;
//            transform.localRotation = Quaternion.identity;
//            transform.localScale = Vector3.one;
//#endif
//        }

        public Transform Transform => transform;
    }
}