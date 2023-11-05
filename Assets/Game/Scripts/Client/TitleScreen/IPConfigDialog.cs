using Game.Client;
using System;
using System.IO;
using TWT.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace TWT.Client.TitleScene
{
    public class IPConfigDialog : MonoBehaviour
    {
        [SerializeField] public InputField ipInput;
        [SerializeField] private Button confirmBtn;

        public event Action<string> OnChangeIp;

        private void Start()
        {
            //confirmBtn.image.sprite = LoadResourcesData.Instance.btn_etc;
            confirmBtn.onClick.AddListener(() =>
            {
                var current_ip = GameContext.IpFixed;// PlayerPrefs.GetString(PlayerPrefsConstant.IP_CONFIG, GameContext.IpFixed);
                var ip = ipInput.text;
                PlayerPrefs.SetString(PlayerPrefsConstant.IP_CONFIG, ip);
                OnChangeIp?.Invoke(ip);

                //Remove all content of old IP
                if (!current_ip.Equals(ip))
                {
                    string path = Path.Combine(VrDomeAssetResourceNameDefine.VR_DATA_ROOT,
                                                    VrDomeAssetResourceNameDefine.CONTENT_DATA_ROOT);
                    FileUtility.DeleteAllCacheFile(path);
                }

            });

            //var change_ip = gameObject.AddComponent<TurnOnOffConnection>();
            //change_ip.ipConfigDialog = this;
        }

        private void OnEnable()
        {
            ipInput.text = GameContext.IpFixed;// PlayerPrefs.GetString(PlayerPrefsConstant.IP_CONFIG, GameContext.IpFixed);
        }
    }
}