using System;
using System.Collections.Generic;
using Game.Scenario;
using UnityEngine;

namespace Client.Scenario
{
    public enum ScQuestionType
    {
        AssetQuestion,
        QuestionImageChoice
    }

    public class ScQuestionManager : MonoBehaviour
    {
        [SerializeField] private ScQuestionElement scQuestionAssetPrefab;
        [SerializeField] private ScQuestionElement scQuestionImageChoicePrefab;

        public static ScQuestionManager Instance { get; private set; }


        private ScQuestionElement ScQuestionAssetPrefab => scQuestionAssetPrefab;

        private ScQuestionElement ScQuestionImageChoicePrefab => scQuestionImageChoicePrefab;

        private Dictionary<string, ScQuestionElement> ScQuestionMap { get; } =
            new Dictionary<string, ScQuestionElement>();

        static float defaultPlayerDistance = 3f;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDisable()
        {
            ScQuestionMap.Clear();
        }

        public static ScQuestionElement LoadScQuestion(string title, ScQuestionType questionType, ScQuestionData questionData)
        {
            ScQuestionManager scQuestionManager = Instance;
            if (scQuestionManager.ScQuestionMap.TryGetValue(title, out var question))
            {
                question.gameObject.SetActive(true);
                return question;
            }

            ScQuestionElement questionElement;
            switch (questionType)
            {
                case ScQuestionType.AssetQuestion:
                    questionElement = Instantiate(scQuestionManager.ScQuestionAssetPrefab, scQuestionManager.transform,
                        true);
                    break;
                case ScQuestionType.QuestionImageChoice:
                    questionElement = Instantiate(scQuestionManager.ScQuestionImageChoicePrefab, scQuestionManager.transform,
                        true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(questionType), questionType, null);
            }
            
            questionElement.Initialize(questionData);
            scQuestionManager.ScQuestionMap[title] = questionElement;
            questionElement.gameObject.AddComponent<PlayerFollowCanvas>().distance = defaultPlayerDistance;

            return questionElement;
        }

        public static ScQuestionElement GetQuestion(string title)
        {
            ScQuestionManager scQuestionManager = Instance;

            if (!scQuestionManager.ScQuestionMap.ContainsKey(title))
            {
                throw new NullReferenceException($"Not find question title {title} on cache");
            }

            return scQuestionManager.ScQuestionMap[title];
        }
    }
}