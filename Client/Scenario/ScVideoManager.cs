using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Scenario
{
    public class ScVideoManager : MonoBehaviour
    {
        public static ScVideoManager Instance { get; private set; }

        [SerializeField] private ScenarioVideoUICtrl videoUiCtrlPrefab;

        private ScenarioVideoUICtrl VideoUiCtrlPrefab => videoUiCtrlPrefab;

        private Dictionary<string, ScenarioVideoUICtrl> ScenarioVideoUiCtrlMap { get; } =
            new Dictionary<string, ScenarioVideoUICtrl>();

        private void Awake()
        {
            Instance = this;
        }

        private void OnDisable()
        {
            ScenarioVideoUiCtrlMap.Clear();
        }

        public static ScenarioVideoUICtrl LoadVideoUiCtrl(string title, string path)
        {
            ScVideoManager scVideoManager = Instance;

            if (scVideoManager.ScenarioVideoUiCtrlMap.TryGetValue(title, out var videoUi))
            {
                videoUi.gameObject.SetActive(true);
                return videoUi;
            }

            var image = Resources.Load<Texture2D>(path);
            if (image)
            {
                Debug.LogError($"Not find image path {path} to load");
            }

            ScenarioVideoUICtrl scenarioVideoUiCtrl =
                Instantiate(scVideoManager.VideoUiCtrlPrefab, scVideoManager.transform, true);
            scenarioVideoUiCtrl.SetVideoUrl(path);
            scVideoManager.ScenarioVideoUiCtrlMap.Add(title, scenarioVideoUiCtrl);

            return scenarioVideoUiCtrl;
        }

        public static ScenarioVideoUICtrl GetVdieoUiCtrl(string title)
        {
            ScVideoManager scVideoManager = Instance;

            if (!scVideoManager.ScenarioVideoUiCtrlMap.ContainsKey(title))
            {
                throw new NullReferenceException($"Not find image title {title} on cache");
            }

            var scenarioVideoUiCtrl = scVideoManager.ScenarioVideoUiCtrlMap[title];
            //scenarioImageUiCtrl.gameObject.SetActive(true);
            return scenarioVideoUiCtrl;
        }
    }
}
