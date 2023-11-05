using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Scenario
{
    public class ScImageManager : MonoBehaviour
    {
        public static ScImageManager Instance { get; private set; }

        [SerializeField] private ScenarioImageUICtrl imageUiCtrlPrefab;

        private ScenarioImageUICtrl ImageUiCtrlPrefab => imageUiCtrlPrefab;

        private Dictionary<string, ScenarioImageUICtrl> ScenarioImageUiCtrlMap { get; } =
            new Dictionary<string, ScenarioImageUICtrl>();

        private void Awake()
        {
            Instance = this;
        }

        private void OnDisable()
        {
            ScenarioImageUiCtrlMap.Clear();
        }

        public static ScenarioImageUICtrl LoadImageUiCtrl(string title, string path, float scale)
        {
            ScImageManager scImageManager = Instance;

            if (scImageManager.ScenarioImageUiCtrlMap.TryGetValue(title, out var imageUi))
            {
                imageUi.gameObject.SetActive(true);
                return imageUi;
            }

            var image = Resources.Load<Texture2D>(path);
            if (!image)
            {
                Debug.LogError($"Not find image path {path} to load");
            }

            ScenarioImageUICtrl scenarioImageUiCtrl =
                Instantiate(scImageManager.ImageUiCtrlPrefab, scImageManager.transform, true);
            scenarioImageUiCtrl.SetImage(image, scale);
            scImageManager.ScenarioImageUiCtrlMap.Add(title, scenarioImageUiCtrl);

            return scenarioImageUiCtrl;
        }

        public static ScenarioImageUICtrl GetImageUiCtrl(string title)
        {
            ScImageManager scImageManager = Instance;

            if (!scImageManager.ScenarioImageUiCtrlMap.ContainsKey(title))
            {
                throw new NullReferenceException($"Not find image title {title} on cache");
            }

            var scenarioImageUiCtrl = scImageManager.ScenarioImageUiCtrlMap[title];
            //scenarioImageUiCtrl.gameObject.SetActive(true);
            return scenarioImageUiCtrl;
        }
    }
}