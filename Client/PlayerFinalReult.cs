using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFinalReult : MonoBehaviour
{
    private int s1Score;
    private int s2Score;

    [SerializeField] Text stage1ScoreText;

    [SerializeField] Text stage2ScoreText;

    [SerializeField] Text totalScoreText;

    public void Clear()
    {
        s1Score = 0;
        s2Score = 0;

        stage1ScoreText.text = string.Empty;
        stage2ScoreText.text = string.Empty;
        totalScoreText.text = string.Empty;
    }

    public void SetScore(int s1, int s2)
    {
        s1Score = s1;
        s2Score = s2;

        stage1ScoreText.text = s1Score.ToString();
        stage2ScoreText.text = s2Score.ToString();
        totalScoreText.text = (s1 + s2).ToString();
    }

    public void SetSoreS1(int s1)
    {
        s1Score = s1;
        stage1ScoreText.text = s1Score.ToString();
        totalScoreText.text = (s1Score + s2Score).ToString();
    }

    public void SetScoreS2(int s2)
    {
        s2Score = s2;
        stage2ScoreText.text = s2Score.ToString();
        totalScoreText.text = (s1Score + s2Score).ToString();
    }

}
