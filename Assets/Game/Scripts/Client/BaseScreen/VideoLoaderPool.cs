using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using TWT.Adapter;

public class VideoLoaderPool
{
    public static Preparer preparer = null;
    public static void Add(VideoPlayer videoPlayer)
    {
        if(preparer == null)
        {
            GameObject gameobj = new GameObject("VideoLoader");
            preparer = gameobj.AddComponent<Preparer>();
        }

        DebugExtension.Log("VideoLoaderPool Added: " + videoPlayer.url);
        preparer.videoPlayers.Enqueue(videoPlayer);
    }
}
