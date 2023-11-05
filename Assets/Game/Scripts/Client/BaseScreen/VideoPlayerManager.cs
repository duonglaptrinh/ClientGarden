using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TWT.Adapter;
public class VideoPlayerManager
{
    public enum Type
    {
        UNKNOWN = -1,
        VR_VIDEO, DOCUMENT_VIDEO,
        COUNT
    }

    static VideoPlayerManager instance = null;


    Queue<VideoPlayer>[] queues;


    RuntimeSetting setting;

    private VideoPlayerManager()
    {
        queues = new Queue<VideoPlayer>[(int)Type.COUNT]; 
        for(int i = 0; i < queues.Length; ++i)
            queues[i] = new Queue<VideoPlayer>();


        // setting = JsonUtility.FromJson<RuntimeSetting>(System.IO.File.ReadAllText())
        setting = new RuntimeSetting();
    }

    public static void Push(VideoPlayer vd, Type type)
    {
        if(instance == null)
        {
            instance = new VideoPlayerManager();
        }

        if(type < 0 || type >= Type.COUNT) return;

        Queue<VideoPlayer> sourceQueue = instance.queues[(int)type];
        int maxVideoSameTime = 0;
        if(type == Type.DOCUMENT_VIDEO)
            maxVideoSameTime = instance.setting.document_video_play_same_time;
        else if(type == Type.VR_VIDEO)
            maxVideoSameTime = instance.setting.vr_video_play_same_time;

        if(sourceQueue.Contains(vd)) return;

        //check current videos
        List<VideoPlayer> temp = new List<VideoPlayer>();
        while(sourceQueue.Count > 0)
        {
            VideoPlayer video = sourceQueue.Dequeue();
            if(video == null || video.gameObject == null || !video.isPlaying) continue;
            temp.Add(video);
        }

        for(int i = 0; i < temp.Count; ++i)
            sourceQueue.Enqueue(temp[i]);

        //remove oldest video players
        while(sourceQueue.Count >= maxVideoSameTime)
        {
            var video = sourceQueue.Dequeue();
            video.Pause();
        }

        sourceQueue.Enqueue(vd);
    }

    public static void Destroy()
    {
        instance = null;
    }

}

[System.Serializable]
public class RuntimeSetting
{
    public int document_video_play_same_time = 3;
    public int vr_video_play_same_time = 3;
}
