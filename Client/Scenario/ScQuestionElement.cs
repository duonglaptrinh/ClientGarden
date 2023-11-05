using Game.Scenario;
using UnityEngine;

namespace Client.Scenario
{
    public class ScQuestionElement : MonoBehaviour
    {
       private IScQuestionView scQuestionView;

       private IScQuestionView ScQuestionView => scQuestionView ?? (scQuestionView = GetComponent<IScQuestionView>());

        public void Initialize(ScQuestionData scQuestionData)
        {
            if (ScQuestionView == null)
            {
                Debug.LogError("init question view is null");
                return;
            }
            
            ScQuestionView.Initialize(scQuestionData);
        }

        public void ShowResult(bool correct)
        {
            ScQuestionView.ShowResult(correct);
        }

        public void SetPlayerAnswerCorrect(int idPlayer)
        {
            ScQuestionView.SetPlayerAnswerCorrect(idPlayer);
        }
    }
}