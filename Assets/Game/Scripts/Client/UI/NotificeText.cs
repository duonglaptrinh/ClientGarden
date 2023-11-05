using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class NotificeText : MonoBehaviour
{
    [SerializeField] private Text contentText;

    private Text ContentText => contentText;
    bool showing;

    public void Show(string content, float time)
    {
        this.gameObject.SetActive(true);
        //MenuGame.instance.OnShowMenu.Subscribe(x =>
        //{
        //    if (x)
        //    {
        //        showing = gameObject.activeInHierarchy;
        //        gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        gameObject.SetActive(true);
        //    }
        //});
        ContentText.text = content;
        StartCoroutine(HideText(time));
    }

    IEnumerator HideText(float time)
    {
        yield return new WaitForSeconds(time);
        this.gameObject.SetActive(false);
        showing = false;
    }
}
