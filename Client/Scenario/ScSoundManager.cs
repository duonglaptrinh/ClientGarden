using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Scenario
{
    public class ScSoundManager : MonoBehaviour
    {
        private static ScSoundManager instance;
        
        public static ScSoundManager Instance
        {
            get
            {
                instance = FindObjectOfType<ScSoundManager>();
                if (instance == null)
                    instance = new GameObject("ScSoundManager").AddComponent<ScSoundManager>();

                return instance;
            }
        }
        
        private Dictionary<string, ScWavElement> ScWavLoadedMap { get; } = new Dictionary<string, ScWavElement>();

        private void OnDisable()
        {
            ScWavLoadedMap.Clear();
        }

        public static ScWavElement LoadWavSc(string title, string path, bool loop)
        {
            ScSoundManager scSoundManager = Instance;
            
            if (scSoundManager.ScWavLoadedMap.TryGetValue(title, out ScWavElement scWav))
            {
                return scWav;
            }
            
            var scWavElement = new GameObject(title).AddComponent<ScWavElement>();
            scWavElement.Initialize(path, loop);
            scWavElement.transform.parent = scSoundManager.transform;
            scSoundManager.ScWavLoadedMap.Add(title, scWavElement);
            return scWavElement;
        }

        public static ScWavElement GetWav(string title)
        {
            ScSoundManager scSoundManager = Instance;

            if (!scSoundManager.ScWavLoadedMap.ContainsKey(title))
            {
                throw new NullReferenceException($"the wav {title} had not loaded");
            }

            return scSoundManager.ScWavLoadedMap[title];
        }
    }
}