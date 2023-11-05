using UnityEngine;

namespace ClientOnly.Oculus
{
    public enum TeleportAreaType
    {
        Point,
        Area,
    }
    
    public interface ITeleportView
    {
        TeleportAreaType TeleportAreaType { get; }
        void OnLookAt();
        bool CanTeleport { get; set; }
        void TeleportTo(Vector3 position);
    }
}