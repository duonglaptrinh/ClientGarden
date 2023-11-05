using System.Collections.Generic;
using UnityEngine;

namespace TWT.light
{
    [System.Serializable]
    public class VRLightObjectData
    {
        public int light_id;
        public LightType type;
        public Color color;
        public LightmapBakeType lightmapBakeType;
        public float bounceIntensity;
        public float intensity;
        public LightShadows shadows;
        public Vector3 localPosition;
        public Vector3 localEulerangle;

        public VRLightObjectData()
        {
            this.light_id = 0;
            type = LightType.Point;
            color = Color.white;
            lightmapBakeType = LightmapBakeType.Realtime;
            bounceIntensity = 1;
            intensity = 1;
            shadows = LightShadows.Soft;
            localPosition = Vector3.zero;
            localEulerangle = Vector3.zero;
        }
    }
}