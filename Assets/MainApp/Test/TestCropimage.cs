using System.Collections;
using System.Collections.Generic;
using TWT.Model;
using UnityEngine;
using UnityEngine.UI;

public class TestCropimage : MonoBehaviour
{
    public Image image;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void Capture()
    {
        CaptureScreenIgnoreUI.Instance.CaptureScreenshotWithoutUI(texture =>
        {
            texture = MyUtils.ScaleTextureV2(texture, 128);
            DebugExtension.LogError(texture.width + "  " + texture.height);

            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        });
    }
}
