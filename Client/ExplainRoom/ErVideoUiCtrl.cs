using System;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Game.ExplainRoom
{
    public class ErVideoUiCtrl : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private RawImage renderImage;
        [SerializeField] private Button playBtn;
        [SerializeField] private Button pauseBtn;
        [SerializeField] private Button back10sBtn;
        [SerializeField] private Button next10sBtn;
        [SerializeField] private Button restartBtn;
        [SerializeField] private Slider volumeBar;

        Vector3 originPos;

        [HideInInspector] public bool isShow = false;

        private string Id => gameObject.name;

        private const double SECOND_PER_NEXT_OR_BACK = 10;

        public float Volume
        {
            get => volumeBar.value;
            set => volumeBar.value = value;
        }

        private void Awake()
        {
            volumeBar.maxValue = 1;
            volumeBar.value = volumeBar.maxValue;
            
            this.UpdateAsObservable()
                .Select(_ => videoPlayer.isPlaying)
                .DistinctUntilChanged()
                .Subscribe(isPlay =>
                {
                    playBtn.gameObject.SetActive(!isPlay);
                    pauseBtn.gameObject.SetActive(isPlay);
                });

            this.UpdateAsObservable()
                .Select(_ => Volume)
                .DistinctUntilChanged()
                .Subscribe(v => videoPlayer.SetDirectAudioVolume(0, v));
        }

        private void Start()
        {
            playBtn.onClick.AddListener(() =>
            {
                SendRequestToServer(MyMessageType.ER_PLAY_VIDEO, new ErSyncTimeVideo(Id, videoPlayer.time), OnPlayButtonClick);
            });
            pauseBtn.onClick.AddListener(() =>
            {
                SendRequestToServer(MyMessageType.ER_PAUSE_VIDEO, new ErSyncTimeVideo(Id, videoPlayer.time), OnPauseButtonClick);
            });
            back10sBtn.onClick.AddListener(() =>
            {
                SendRequestToServer(MyMessageType.ER_BACK_SECOND_VIDEO, new ErSyncTimeVideo(Id, videoPlayer.time - SECOND_PER_NEXT_OR_BACK));
            });
            next10sBtn.onClick.AddListener(() =>
            {
                SendRequestToServer(MyMessageType.ER_NEXT_SECOND_VIDEO, new ErSyncTimeVideo(Id, videoPlayer.time + SECOND_PER_NEXT_OR_BACK));
            });
            restartBtn.onClick.AddListener(() =>
            {
                SendRequestToServer(MyMessageType.ER_RESTART_VIDEO, new ErSyncTimeVideo(Id, videoPlayer.time));
            });
            
            volumeBar.OnPointerUpAsObservable()
                .Subscribe(_ =>
                {
                    SendRequestToServer(MyMessageType.ER_CHANGE_VOLUME_VIDEO, new ErSyncVolumeVideo(Id, Volume));
                });
        }

        private void OnEnable()
        {
            //PlayVideo();
            originPos = transform.position;
            isShow = true;
        }

        private void OnDisable()
        {
            StopVideo();
            isShow = false;
        }

        public void CreateRenderTexture()
        {
            RenderTexture rt = new RenderTexture(1366, 768, 16, RenderTextureFormat.ARGB32);
            rt.Create();

            videoPlayer.targetTexture = rt;
            renderImage.texture = rt;
        }

        public void SetVideoUrl(string videoUrl)
        {
            if (CheckUrlValid(videoUrl))
                videoPlayer.url = videoUrl;
            else
            {
                var videoClip = ResourceLoadObject.Load<VideoClip>(videoUrl);
                videoPlayer.clip = videoClip;
            }
            videoPlayer.SetDirectAudioVolume(0, 1);
            volumeBar.value = 1;

            var video_size = Utility.SetVideoSizeWithRect(videoPlayer.clip, new Rect(0, 0, 1813, 1020));
            videoPlayer.GetComponent<RectTransform>().sizeDelta = video_size;
            renderImage.rectTransform.sizeDelta = video_size;
        }

        private static bool CheckUrlValid(string source)
        {
            return Uri.TryCreate(source, UriKind.Absolute, out var uriResult) && uriResult.Scheme == Uri.UriSchemeHttp;
        }

        public void OnPlayButtonClick()
        {
            PlayVideo();
        }

        public void OnPauseButtonClick()
        {
            PauseVideo();
        }

        public void PlayVideo()
        {

            videoPlayer.Play();
        }

        public void PauseVideo()
        {
            videoPlayer.Pause();
        }

        public void StopVideo()
        {
            videoPlayer.Stop();
            videoPlayer.targetTexture.Release();
        }

        public void Show()
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            transform.position = originPos;
            isShow = true;
            videoPlayer.SetDirectAudioVolume(0, 1);
        }

        public void Hide()
        {
            originPos = transform.position;
            transform.position = new Vector3(0, -1000, 0);
            isShow = false;
            videoPlayer.SetDirectAudioVolume(0, 0);
        }


        public void SyncTimePlayVideo(double time)
        {
            videoPlayer.time = time;
        }

        public void Restart()
        {
            videoPlayer.time = 0;
            PlayVideo();
        }

        private static void SendRequestToServer(short msgDefine, MessageBase messageBase, Action fallBack = null)
        {
            if (NetworkClient.active)
            {
                NetworkClient.allClients[0].Send(msgDefine, messageBase);
            }
            else
            {
                fallBack?.Invoke();
            }
        }
    }
}