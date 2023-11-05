using Game.Client;
using Game.Client.Extension;
using System;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;

public class VRObjectItemCtrl : MonoBehaviour
{
    [SerializeField] RawImage icon;
    [SerializeField] Text txtName;
    Rect defaultRect;
    Action OnSelectedVRObject;
    Action OnDropVRObject;

    public void Instantiate(Texture2D texture, Action onSelectedVRObject, Action onDropVRObject)
    {
        if (texture != null)
        {
            defaultRect = icon.rectTransform.rect;
            //icon.texture = texture;
            GraphicExtension.SetRawImageWithRect(texture, icon, defaultRect);
        }
        OnSelectedVRObject = onSelectedVRObject;
        OnDropVRObject = onDropVRObject;
    }


    public void Instantiate(string name, Action onSelectedVRObject, Action onDropVRObject)
    {
        txtName.text = name;
        OnSelectedVRObject = onSelectedVRObject;
        OnDropVRObject = onDropVRObject;
    }

    public void InstantiateFoModel3d(string name, string urlThumb, Action onSelectedVRObject, Action onDropVRObject)
    {
        txtName.text = name;
        //Texture2D texture = Resources.Load<Texture2D>("PrefabVRObject/IconModels");
        //var texture = await BaseScreenCtrl.resourceLoader.GetImage(BaseScreenCtrl.VrContentData.content_name,
        //                                                       urlThumb, VrDomeAssetResourceNameDefine.VR_MODEL);
        Addressables.LoadAssetAsync<Sprite>(urlThumb).Completed += sprite =>
        {
            //Image img = Instantiate(prefabThumb, scroll.content);
            Sprite spr = sprite.Result;
            if (icon)
            {
                icon.texture = spr.texture;
                icon.color = Color.white;
            }
        };
        OnSelectedVRObject = onSelectedVRObject;
        OnDropVRObject = onDropVRObject;
    }
    public void InstantiateForPdf(string name, Action onSelectedVRObject, Action onDropVRObject)
    {
        txtName.text = name;
        Texture2D texture = Resources.Load<Texture2D>("PrefabVRObject/IconPdf");
        if (texture != null)
            icon.texture = texture;
        OnSelectedVRObject = onSelectedVRObject;
        OnDropVRObject = onDropVRObject;
    }

    public void OnVRObjectHold()
    {
        OnSelectedVRObject?.Invoke();
    }

    public void OnVRObjectRelease()
    {
        OnDropVRObject?.Invoke();
    }
}
