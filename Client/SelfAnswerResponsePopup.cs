using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfAnswerResponsePopup : MonoBehaviour
{

    [SerializeField] GameObject correct;

    [SerializeField] GameObject incorrect;

    public void ShowResponseOnTime(bool isCorrect, float time = 5f)
    {
        this.gameObject.SetActive(true);
        StartCoroutine(DoShowResponseOnTime(isCorrect, time));

    }

    IEnumerator DoShowResponseOnTime(bool isCorrect, float time = 5f)
    {
        correct.SetActive(isCorrect);
        incorrect.SetActive(!isCorrect);

        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
    }

    public bool IsInCorrectShowing()
    {
        return gameObject.activeInHierarchy && incorrect.activeSelf;
    }

    public void HideIt()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(false);
    }

}
