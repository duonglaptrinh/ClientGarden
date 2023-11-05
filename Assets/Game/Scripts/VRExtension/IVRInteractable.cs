using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Client
{
    public interface IVrInteractable
    {
        void OnVRRayEnter();

        void OnVRRayExit();

        void OnVRClickDown(Vector3 position);

        void OnVRClickUp(Vector3 position);

        void OnVRDragOn(Vector3 position);

        void OnVRRayOn(Vector3 position);

        LayerMask LayerMask { get; }
    }
}