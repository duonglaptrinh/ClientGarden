using System;
using System.Collections;
using UnityEngine;

namespace Game.ExplainRoom
{
    public class ExplainRoomLoadModelManager : ResourceLoadManagerBase<GameObject>
    {
        public override GameObject Load(string nameDefine, string path)
        {
            var loader = this;
            if (loader.SourceLoadedMap.TryGetValue(nameDefine, out GameObject obj))
            {
                obj.transform.parent = loader.transform;
                obj.SetActive(true);
                return obj;
            }

            var prefabs = Resources.Load<GameObject>(path);
            if (prefabs == null)
            {
                Debug.LogError($"Not find model {path} to load");
                return null;
            }
            GameObject modelLoaded = Instantiate(prefabs, loader.transform, true);
            modelLoaded.name = nameDefine;
            loader.SourceLoadedMap.Add(nameDefine, modelLoaded);
            return modelLoaded;
        }
    }
}