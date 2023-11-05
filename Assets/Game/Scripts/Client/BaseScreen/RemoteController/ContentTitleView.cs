using Game.Client;
using System;
using TWT.Model;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TWT.Client
{
    public class ContentTitleView : ViewComponentBase
    {
        [SerializeField]
        private RawImage iconImage;

        [SerializeField] private InputField titleText;

        [SerializeField] private InputField descriptionText;

        private string iconName;

        public void ShowInfo(ContentInfo content)
        {
            LoadIconSync(content.iconUrlAbsolutePath, content.contentName, content.contentTitle.icon).Forget();
            titleText.text = content.contentTitle.title;
            descriptionText.text = content.contentTitle.description;
            iconName = content.contentTitle.icon;
        }

        public VrContentTitle GetContentTitle()
        {
            return VrContentTitle.CreateInstance(titleText.text, descriptionText.text, iconName);
        }

        private async UniTask LoadIconSync(string url, string contentName, string iconName)
        {
            var loader = GameContext.ResourceLoader as LocalResourceLoader;
            var savePath = loader.GetContentIconPath(contentName, iconName);
            var icon = await Game.Client.Utility.DownloadTextureAsync(url, savePath);

            if (icon)
            {
                iconImage.texture = icon;
            }
        }

        private static async UniTask<Texture> LoadImageAsync(string url)
        {
            try
            {
                return await ObservableUnityWebRequest.GetTexture2DAsObservable(url);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}