using Shim.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class VrModelEdit : MonoBehaviour
{
    // Start is called before the first frame update
    public Text txtName;
    public Text txtEnglishName;
    //public Text txtWeight;
    public Image thumbModel;
    VRObjectManagerV2 vRObjectManager;

    OneModelAssetsData currentDataModel;
    void Start()
    {
        vRObjectManager = VRObjectManagerV2.Instance;
    }

    public void LoadDetail(OneModelAssetsData data)
    {
        //scroll.content.localPosition = Vector3.zero;


        currentDataModel = data;
        txtName.text = data.NameOnApp?.Replace("$", "");
        txtEnglishName.text = data.EnglishName;
        //txtFeatured.text = data.Featured;
        //if (string.IsNullOrEmpty(data.Featured))
        thumbModel.sprite = null;
        if (!string.IsNullOrEmpty(data.pathThumb))
            Addressables.LoadAssetAsync<Sprite>(data.pathThumb).Completed += sprite =>
            {
                thumbModel.sprite = sprite.Result;
            };
    }
}
