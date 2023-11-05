using UnityEngine;

namespace Game.ExplainRoom
{
    public static class ResourceLoadObject
    {
        public static T Load<T>(string path) where T : Object
        {
            var load = Resources.Load<T>(path);
            if (!load)
            {
                Debug.LogError($"Not find path {path} to load");
            }

            return load;
        }
    }
}