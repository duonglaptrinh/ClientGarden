using System;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Scenario
{
    public class ScTextManager : MonoBehaviour
    {
        [SerializeField] private ScenarioTextUICtrl scenarioTextUiCtrlPrefab;
        public static ScTextManager Instance { get; private set; }
        
        private Dictionary<string, ScenarioTextUICtrl> ScenarioTextUiCtrlMap { get; } = new Dictionary<string, ScenarioTextUICtrl>();

        private ScenarioTextUICtrl ScenarioTextUiCtrlPrefab => scenarioTextUiCtrlPrefab;

        static float defaultPlayerDistance = 3f;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDisable()
        {
            ScenarioTextUiCtrlMap.Clear();
        }

        public static ScenarioTextUICtrl LoadTextUiCtrl(string title, string content, Vector3 position, bool inWorldSpace)
        {
            ScTextManager scTextManager = Instance;
            
            if (scTextManager.ScenarioTextUiCtrlMap.TryGetValue(title, out var textUi))
            {
                textUi.SetText(content);
                return textUi;
            }

            ScenarioTextUICtrl scenarioTextUiCtrl = Instantiate(scTextManager.ScenarioTextUiCtrlPrefab, scTextManager.transform, true);
            scenarioTextUiCtrl.SetText(content);
            scTextManager.ScenarioTextUiCtrlMap.Add(title, scenarioTextUiCtrl);

            scenarioTextUiCtrl.transform.position = position;
            if(!inWorldSpace)
            {
                scenarioTextUiCtrl.gameObject.AddComponent<PlayerFollowCanvas>().distance = defaultPlayerDistance;
            }

            return scenarioTextUiCtrl;
        }

        public static ScenarioTextUICtrl GetTextUiCtrl(string title)
        {
            ScTextManager scTextManager = Instance;

            if (!scTextManager.ScenarioTextUiCtrlMap.ContainsKey(title))
            {
                throw  new NullReferenceException($"not find text title --{title}-- on cache");
            }

            return scTextManager.ScenarioTextUiCtrlMap[title];
        }
    }
}