using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeUILayout : MonoBehaviour
{
    float fadeTime = 1f;

    private void Awake()
    {
        GetComponent<CanvasGroup>().alpha = 0;
    }

    private void OnEnable()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<CanvasGroup>().DOFade(1.0f, fadeTime);
    }

    private void OnDisable()
    {
        GetComponent<CanvasGroup>().alpha = 0;
    }

    public async void HideUILayout()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().DOFade(0.0f, fadeTime);
        await Task.Delay(1000);
        gameObject.SetActive(false);
    }
}
