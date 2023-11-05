using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ExplainRoom
{
    public class ExplainRoom : MonoBehaviour
    {
        [SerializeField]
        private Transform side1;
        [SerializeField]
        private Transform side2;
        [SerializeField]
        private Transform side3;
        [SerializeField]
        private Transform side4;

        public Vector3 ExplainRoomPositionOffset => transform.position;
        
        public enum RoomSide
        {
            Side1,
            Side2,
            Side3,
            Side4,
        }

        private static void SetObjectToSide(Transform obj, Transform side)
        {
            obj.gameObject.SetActive(true);
            obj.parent = side;
            obj.localPosition = Vector3.zero;
            obj.localRotation = Quaternion.identity;
        }

        public static void ClearSide(Transform obj)
        {
            for (int i = 0; i < obj.childCount; i++)
            {
                var child = obj.GetChild(i);
                child.gameObject.SetActive(false);
            }
        }

        public void SetObjectToSide(Transform obj, RoomSide roomSide)
        {
            switch (roomSide)
            {
                case RoomSide.Side1:
                    SetObjectToSide(obj, side1);
                    break;
                case RoomSide.Side2:
                    SetObjectToSide(obj, side2);
                    break;
                case RoomSide.Side3:
                    SetObjectToSide(obj, side3);
                    break;
                case RoomSide.Side4:
                    SetObjectToSide(obj, side4);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(roomSide), roomSide, null);
            }
        }

        public void ClearAllSide()
        {
            ClearSide(side1);
            ClearSide(side2);
            ClearSide(side3);
            ClearSide(side4);
        }

        public void HideAllModel()
        {
            foreach (KeyValuePair<string, GameObject> entry in ExplainRoomLoadModelManager.Instance.SourceLoadedMap)
            {
                entry.Value.SetActive(false);
            }
        }
    }
}