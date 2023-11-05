using Extension;
using UnityEngine;

namespace Game.ExplainRoom
{
    public class ErPlayWav : MonoBehaviour
    {
        private AudioSource AudioSource { get; set; }

        [HideInInspector]
        public bool isShow = false;

        public bool IsLoop
        {
            get { return AudioSource.loop; }
            set { AudioSource.loop = value; }
        }

        public void Initialize(string pathSound, bool loop)
        {
            AudioSource = gameObject.GetOrAddComponent<AudioSource>();
            AudioSource.playOnAwake = false;

            var clip = Resources.Load(pathSound, typeof(AudioClip)) as AudioClip;
            AudioSource.clip = clip;
            AudioSource.loop = loop;
        }

        private void OnEnable()
        {
            isShow = true;
        }

        private void OnDisable()
        {
            isShow = false;
        }

        public void Play()
        {
            AudioSource.Play();
        }
        
        public void Play(bool loop)
        {
            AudioSource.loop = loop;
            AudioSource.Play();
        }

        public void Stop()
        {
            AudioSource.Stop();
        }

        public void Show()
        {
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);

            isShow = true;
            AudioSource.volume = 1;
        }

        public void Hide()
        {
            isShow = false;
            AudioSource.volume = 0;
        }
    }
}