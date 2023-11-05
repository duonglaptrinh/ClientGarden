using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestRealtimeMode : MonoBehaviour
{
    public GameObject playRemote;
    public Texture2D[] textures;
    [SerializeField] private Material image360Material;
    // Start is called before the first frame update
    void Start()
    {
        image360Material.mainTexture = textures[0];
    }

    // Update is called once per frame
    void Update()
    {
#if !UNITY_WEBGL
        if (OVRInput.GetDown(OVRInput.Button.One) || Input.GetKeyDown(KeyCode.Space))
#else
        if (Input.GetKeyDown(KeyCode.Space))
#endif
        {
            HideShowUiController();
        }
    }

    public void HideShowUiController()
    {
        playRemote.SetActive(!playRemote.activeInHierarchy);
    }

    public void ShowImage1(Toggle toggle)
    {
        if(toggle.isOn)
            image360Material.mainTexture = textures[0];
    }

    public void ShowImage2(Toggle toggle)
    {
        if (toggle.isOn)
            image360Material.mainTexture = textures[1];
    }

    public void ShowImage3(Toggle toggle)
    {
        if (toggle.isOn)
            image360Material.mainTexture = textures[2];
    }

    public void ExitRoom()
    {
        SceneConfig.LoadScene(SceneConfig.Scene.TitleScreen);
    }
}
