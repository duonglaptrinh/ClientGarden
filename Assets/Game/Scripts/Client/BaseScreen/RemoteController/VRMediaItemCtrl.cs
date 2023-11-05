using Game.Client;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using TWT.Model;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VRMediaItemCtrl : MonoBehaviour
{
    [SerializeField] Text name;
    private string nameMediaWithExtension;

    void Start()
    {
    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    public void SetVideoInfo(string videoName)
    {
        nameMediaWithExtension = videoName;
        name.text = Path.GetFileNameWithoutExtension(nameMediaWithExtension);
    }


    Texture CopyTexture(Texture sourceTex)
    {
        var destTex = new Texture2D(sourceTex.width, sourceTex.height, TextureFormat.ARGB32, false);
        destTex.mipMapBias = sourceTex.mipMapBias;
        Graphics.CopyTexture(sourceTex, destTex);
        return destTex;
    }
}