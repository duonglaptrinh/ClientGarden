using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CaptureScreenIgnoreUI : MonoBehaviour
{
    public static CaptureScreenIgnoreUI Instance { get; private set; }
    private Action<Texture2D> OnDone = null;
    Camera captureCamera;
    [SerializeField] FlashImage flashImage;
    [SerializeField] GameObject group;
    Coroutine co;
    Texture2D texture = null;

    List<Canvas> listCanvasUI = new List<Canvas>();

    private void Awake()
    {
        Instance = this;
        captureCamera = Camera.main;
    }
    public void CaptureScreenshotWithoutUI(Action<Texture2D> OnDone = null)
    {
        captureCamera.transform.position = Camera.main.transform.position;
        captureCamera.transform.rotation = Camera.main.transform.rotation;
        group.SetActive(true);
        this.OnDone = OnDone;
        if (co != null)
        {
            StopCoroutine(co);
        }
        co = StartCoroutine("Capture");
    }
    IEnumerator Capture()
    {
        OnOffCanvas(isOn: false);
        yield return new WaitForEndOfFrame();
        // Disable UI

        int width = Screen.width;
        int height = Screen.height;
        RenderTexture rt = new RenderTexture(width, height, 24);
        captureCamera.targetTexture = rt;
        captureCamera.Render();

        texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        RenderTexture.active = rt;
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture.Apply();

        RenderTexture.active = null;
        captureCamera.targetTexture = null;
        Destroy(rt);

        //yield return new WaitForSeconds(0.02f);
        yield return new WaitForEndOfFrame();
        // enable UI
        OnOffCanvas(isOn: true);
        //flashImage.gameObject.SetActive(true);
        //flashImage.Flash(0.2f, 0, 1, Color.white);
        //yield return new WaitForSeconds(0.2f);
        //flashImage.gameObject.SetActive(false);
        OnDone?.Invoke(this.texture);
        group.SetActive(false);
    }

    void OnOffCanvas(bool isOn)
    {
        if (!isOn)
            listCanvasUI = FindObjectsOfType<Canvas>().ToList();
        listCanvasUI.ForEach(x =>
        {
            if (x && x.gameObject) x.gameObject.SetActive(isOn);
        });
    }
}
