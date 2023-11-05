using Game.Scenario;

namespace Client.Scenario
{
    public interface IScQuestionView
    {
        void Initialize(ScQuestionData questionData);
        void ShowResult(bool correct);
        void SetPlayerAnswerCorrect(int idPlayer);
    }
}