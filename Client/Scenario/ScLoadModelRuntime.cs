using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Client.Scenario
{
    public class ScLoadModelRuntime : MonoBehaviour
    {
        private static ScLoadModelRuntime instance;
        private Dictionary<string, GameObject> ModelLoadedMap { get; } = new Dictionary<string, GameObject>();

        public static ScLoadModelRuntime Instance
        {
            get
            {
                instance = FindObjectOfType<ScLoadModelRuntime>();
                if (instance == null)
                    instance = new GameObject("ScLoadModelRuntime").AddComponent<ScLoadModelRuntime>();

                return instance;
            }
        }

        private void OnDisable()
        {
            ModelLoadedMap.Clear();
        }

        public static GameObject LoadModel(string modelName, string nameDefine)
        {
            ScLoadModelRuntime loader = Instance;
            if (loader.ModelLoadedMap.TryGetValue(nameDefine, out GameObject obj))
            {
                obj.transform.parent = loader.transform;
                obj.SetActive(true);
                return obj;
            }

            var prefabs = Resources.Load<GameObject>(modelName);
            if (prefabs == null)
            {
                Debug.LogError($"Not find model {modelName} to load");
                return null;
            }
            GameObject modelLoaded = Instantiate(prefabs, Instance.transform, true);
            modelLoaded.name = nameDefine;
            Instance.ModelLoadedMap.Add(nameDefine, modelLoaded);
            return modelLoaded;
        }

        public static GameObject GetModel(string modelName)
        {
            ScLoadModelRuntime loader = Instance;
            if (!loader.ModelLoadedMap.ContainsKey(modelName))
                throw new UnityException($"Not find model {modelName} on cache");

            return loader.ModelLoadedMap[modelName];
        }
    }
}