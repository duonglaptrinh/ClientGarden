using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
//using UMP;

namespace TWT.Adapter
{
    public class VideoPlayer : MonoBehaviour
    {
        //[SerializeField] protected UniversalMediaPlayer universalMediaPlayer;
        [SerializeField] protected UnityEngine.Video.VideoPlayer videoPlayer;


        public bool IsUseUMP { get; set; } = false;

        public bool isPlaying => IsUseUMP; //? universalMediaPlayer.IsPlaying : videoPlayer.isPlaying;
        // public bool isPrepared => IsUseUMP ? (universalMediaPlayer.AbleToPlay && !universalMediaPlayer.IsParsing) : videoPlayer.isPrepared;
        public bool isPrepared => IsUseUMP; //? universalMediaPlayer.IsReady : videoPlayer.isPrepared;
        public bool isEnable => IsUseUMP; //? (universalMediaPlayer.enabled && universalMediaPlayer.gameObject.activeInHierarchy) : (videoPlayer.enabled && videoPlayer.gameObject.activeInHierarchy);
        public bool isFirstFrameAvailable => IsUseUMP;// ? universalMediaPlayer.IsReady : videoPlayer.isPrepared;

        public double time
        {
            //get => IsUseUMP ? universalMediaPlayer.Time / 1000f : videoPlayer.time;
            get => videoPlayer.time;
            set
            {
                //if (IsUseUMP)
                //    universalMediaPlayer.Time = (long)(value * 1000f);
                //else
                videoPlayer.time = value;
            }
        }
        public virtual string url
        {
            //get => IsUseUMP ? universalMediaPlayer.Path : videoPlayer.url;
            get => videoPlayer.url;
            set
            {
                //if(universalMediaPlayer != null)
                //    universalMediaPlayer.Stop();
                videoPlayer.Stop();
                IsUseUMP = Application.platform == RuntimePlatform.Android && System.IO.Path.GetExtension(value).Equals(Game.Client.VRAssetPath.MOV, System.StringComparison.OrdinalIgnoreCase);
                // universalMediaPlayer.enabled = IsUseUMP;
                // videoPlayer.enabled = !IsUseUMP;
                if (IsUseUMP)
                {
                    if (GameContext.IsDemo && Application.platform == RuntimePlatform.Android)
                    {
                        value = "file:///" + value.Substring(Application.streamingAssetsPath.Length + 1);
                    }
                    //universalMediaPlayer.Path = value;
                }
                else
                    videoPlayer.url = value;

                videoPlayer.targetMaterialProperty = "_Tex";
#if ADMIN
                videoPlayer.SetDirectAudioVolume(0, 0);
#endif

                DebugExtension.Log("SetVideo Url: " + value + " and use UMP: " + IsUseUMP);
            }
        }
        //public float length => IsUseUMP ? universalMediaPlayer.Length / 1000f : (float)videoPlayer.length;
        public float length => (float)videoPlayer.length;
        public int width => (int)videoPlayer.width;
        //public int width => IsUseUMP ? universalMediaPlayer.VideoWidth : (int)videoPlayer.width;
        public int height => (int)videoPlayer.height;
        //public int height => IsUseUMP ? universalMediaPlayer.VideoHeight : (int)videoPlayer.height;

        public VideoPlayerManager.Type videoType = VideoPlayerManager.Type.UNKNOWN;

        //TODO: in the future...
        [HideInInspector]
        public virtual RenderTexture targetTexture
        {
            get => IsUseUMP ? null : videoPlayer.targetTexture;
            set
            {
                if (IsUseUMP) return;
                videoPlayer.targetTexture = value;
            }
        }

        [HideInInspector]
        public VideoClip clip
        {
            get => IsUseUMP ? null : videoPlayer.clip;
            set
            {
                if (IsUseUMP) return;
                videoPlayer.clip = clip;
            }
        }

        public System.Action<VideoPlayer> loopPointReached;

        void Start()
        {
            //if (universalMediaPlayer != null && universalMediaPlayer.EventManager != null)
            //    universalMediaPlayer.EventManager.PlayerEndReachedListener += () =>
            //    {
            //        if (!universalMediaPlayer.Loop)
            //            Prepare();

            //        if (IsUseUMP)
            //            loopPointReached?.Invoke(this);
            //    };

            if (videoPlayer != null)
                videoPlayer.loopPointReached += (videoplayer) =>
                {
                    if (!IsUseUMP)
                        loopPointReached?.Invoke(this);
                    //Replay();
                };
        }
        public void Replay()
        {
            time = 0.1f;
            StepForward();
            Play();
        }
        public void Prepare()
        {
            if (string.IsNullOrEmpty(url)) return;

            //if (IsUseUMP)
            //    universalMediaPlayer.Prepare();
            //else
            videoPlayer.Prepare();
        }

        public void Pause()
        {
            //if (IsUseUMP)
            //    universalMediaPlayer.Pause();
            //else
            videoPlayer.Pause();
        }

        public void Play()
        {
            if (string.IsNullOrEmpty(url)) return;

            //if (IsUseUMP)
            //    universalMediaPlayer.Play();
            //else
            videoPlayer.Play();

            VideoPlayerManager.Push(this, videoType);
        }

        public void StepForward()
        {
            if (IsUseUMP) ;
            else
                videoPlayer.StepForward();
        }

        public void Stop()
        {
            //if (IsUseUMP)
            //    universalMediaPlayer.Stop();
            //else
            videoPlayer.Stop();
        }

        public void SetDirectAudioVolume(ushort trackIndex, float volume)
        {
            //if (IsUseUMP)
            //    universalMediaPlayer.Volume = volume;
            //else
            videoPlayer.SetDirectAudioVolume(trackIndex, volume);
        }

        public void SetLoop(bool loop)
        {
            //if (IsUseUMP)
            //    universalMediaPlayer.Loop = loop;
            //else
            videoPlayer.isLooping = loop;
        }
    }
}

