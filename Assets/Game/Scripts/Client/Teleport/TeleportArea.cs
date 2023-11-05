using UnityEngine;

namespace ClientOnly.Oculus
{
    public class TeleportArea : MonoBehaviour, ITeleportView
    {
        TeleportAreaType ITeleportView.TeleportAreaType => TeleportAreaType.Area;

        void ITeleportView.OnLookAt()
        {
            //Do nothing
        }

        bool ITeleportView.CanTeleport { get; set; } = true;
        
        public void TeleportTo(Vector3 position)
        {
            if (Camera.main != null)
            {
                var cameraRoot = Camera.main.transform.root;

                cameraRoot.transform.position = position;
            }
        }
    }
}