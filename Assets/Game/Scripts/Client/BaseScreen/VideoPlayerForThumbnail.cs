using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TWT.Adapter;
public class VideoPlayerForThumbnail : VideoPlayer
{
    [SerializeField] GameObject[] documentVideoRenderers;
    [SerializeField] GameObject[] dome360VideoRenderers;
    [SerializeField] RenderTexture renderTextureFor360;

    // public override string url 
    // { 
    //     get => base.url;
    //     set
    //     {
    //         base.url = value;
    //     } 
    // }

    public override RenderTexture targetTexture
    {
        get
        {
            return renderTextureFor360;
        }
    }


    public void Switch360Preview()
    {
        ClearRenderTexture();
        //universalMediaPlayer.RenderingObjects = dome360VideoRenderers;
        videoPlayer.targetTexture = renderTextureFor360;
    }

    public void SwitchDocumentVideoPreview()
    {
        ClearRenderTexture();
        //universalMediaPlayer.RenderingObjects = documentVideoRenderers;
        videoPlayer.targetTexture = renderTextureFor360;
    }

    public void ClearRenderTexture()
    {
        RenderTexture rt = UnityEngine.RenderTexture.active;
        UnityEngine.RenderTexture.active = renderTextureFor360;
        GL.Clear(true, true, Color.clear);
        UnityEngine.RenderTexture.active = rt;
    }
}
