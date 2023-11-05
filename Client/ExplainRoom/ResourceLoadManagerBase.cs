using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public abstract class ResourceLoadManagerBase<TSourceLoad> : MonoBehaviour where TSourceLoad : Object
    {
        public static ResourceLoadManagerBase<TSourceLoad> Instance { get; private set; }
        
        public Dictionary<string, TSourceLoad> SourceLoadedMap { get; } = new Dictionary<string, TSourceLoad>();


        protected virtual void Awake()
        {
            Instance = this;
        }

        public abstract TSourceLoad Load(string nameDefine, string path);

        public TSourceLoad Get(string nameDefine)
        {
            return SourceLoadedMap.TryGetValue(nameDefine, out var sourceLoad) ? sourceLoad : null;
        }
    }
}