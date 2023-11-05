using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TWT.Adapter;

public class Preparer : MonoBehaviour
{
    public Queue<VideoPlayer> videoPlayers = new Queue<VideoPlayer>();
    public VideoPlayer current = null;

    void Update()
    {
        // if(current != null && (current.isPrepared || !current.enabled || !current.gameObject.activeInHierarchy))
        if(current != null && current.isPrepared)
        {
            current = null;
        }

        if(current != null && !current.isPrepared && !current.isEnable)
        {
            videoPlayers.Enqueue(current);
            current = null;
        }

        if(current != null && !current.isPrepared)
        {
            return;
        }

        if(videoPlayers.Count == 0) return;
        current = videoPlayers.Dequeue();
        if(current == null) return;

        if(current.isPrepared)
        {
            current = null;
        }

        else if(current.isEnable)
        {
            current.Prepare();
        }
        else
        {
            videoPlayers.Enqueue(current);
            current = null;
        }
    }
}
