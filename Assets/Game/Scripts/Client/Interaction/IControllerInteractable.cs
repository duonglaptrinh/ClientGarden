using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TWT.Client.Interaction
{
    public interface IControllerInteractable
    {
        Transform Transform { get; }
    }

    public static class IInteractableExtension
    {
        /// <summary>
        ///   get direction of the controller
        /// </summary>
        /// <returns></returns>
        public static Ray GetRay(this IControllerInteractable controllerInteractable)
        {
            var myRay = new Ray(controllerInteractable.Transform.position, controllerInteractable.Transform.forward);

            if (GameContext.IsMobileClient)
            {
                myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            }

#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_WEBGL
            myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
#endif
            return myRay;
        }

        public static bool IsMouseOrTouchOverUI(this IControllerInteractable controllerInteractable)
        {
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
            return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#else
            return EventSystem.current.IsPointerOverGameObject();
#endif
        }
    }
}