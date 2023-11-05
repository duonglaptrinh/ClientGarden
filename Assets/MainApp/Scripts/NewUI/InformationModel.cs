using Shim.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using jp.co.mirabo.Application.RoomManagement;
using Player_Management;
using TWT.Networking;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InformationModel : MonoBehaviour
{
    // Start is called before the first frame update
    public Text txtName;
    public Text txtEnglishName;
    public Text txtCategory;
    public Text txtSubCategory;
    public Text txtPrice;
    public Text txtDescription;
    public Text txtFeatured;
    //public Text txtWeight;
    public Text txtColor;
    public Text txtScale;
    public Image thumbModel;
    public Image thumbColor;
    public Button buttonCreatModel;
    public ScrollRect scroll;
    VRObjectManagerV2 vRObjectManager;

    OneModelAssetsData currentDataModel;
    void Start()
    {
        vRObjectManager = VRObjectManagerV2.Instance;
        buttonCreatModel.onClick.AddListener(CreateModel);
    }

    public void LoadDetail(OneModelAssetsData data)
    {
        //scroll.content.localPosition = Vector3.zero;

        RectTransformExtensions.SetLeft(scroll.content, 0);
        RectTransformExtensions.SetRight(scroll.content, 0);
        Vector3 pos = scroll.content.transform.localPosition;
        pos.y = 0;
        scroll.content.transform.localPosition = pos;

        currentDataModel = data;
        txtName.text = data.NameOnApp?.Replace("$", "");

        txtEnglishName.text = data.EnglishName;

        txtCategory.text = LoadResourceAddessable.GetNameCategoryOnAppByName(data.CategoryParent).Replace("\n", "");
        txtSubCategory.text = data.CategoryChild;
        txtPrice.text = data.PrizeUnit + $"{data.Prize:N0}" + "（税込）";

        txtDescription.text = data.Description;

        //txtFeatured.text = data.Featured;
        //if (string.IsNullOrEmpty(data.Featured))
        txtFeatured.text = data.Featured?.Replace("$", "\n");
        txtColor.text = "";

        thumbColor.sprite = null;
        if (data.listPathColorTexture != null && data.listPathColorTexture.Count > 0 && !string.IsNullOrEmpty(data.listPathColorTexture[0]))
            Addressables.LoadAssetAsync<Texture2D>(data.listPathColorTexture[0]).Completed += sprite =>
            {
                Texture2D spr = sprite.Result;
                if (spr)
                {
                    thumbColor.sprite = Sprite.Create(spr, new Rect(0, 0, spr.width, spr.height), Vector2.one * 0.5f);
                    txtColor.text = spr.name.Replace(".jpg", "");
                    txtColor.text = spr.name.Replace(".png", "");
                    txtColor.text = txtColor.text.Replace("$", "\n");
                }
            };

        thumbModel.sprite = null;
        if (!string.IsNullOrEmpty(data.pathThumb))
            Addressables.LoadAssetAsync<Sprite>(data.pathThumb).Completed += sprite =>
            {
                thumbModel.sprite = sprite.Result;
            };

        txtScale.text = data.Size;
    }
    void CreateModel()
    {
        DisableLockModel();
        MainMenu newMainMenu = BaseScreenUiControllerV2.Instance.NewMainMenu;
        newMainMenu.OnOffEditMode(true);
        // Code Sinh Model
        //DebugExtension.Log(scrollObject.ListItems[index]);
        var forward = Camera.main.transform.forward.normalized * 2.5f;
        Vector3 startPos = Camera.main.transform.position + new Vector3(forward.x, -1f, forward.z);
        GameObject newModel = vRObjectManager.AddNewVrModel3D(currentDataModel.pathPrefab, startPos);
        ClickModel3D.SetCheckClickModel();
        //dragObjectManager.SetDraggableObject(newModel);
        //MenuTabControllerV2.Instance.vRObjectTablet.gameObject.SetActive(false);
        //BaseScreenUiControllerV2.Instance.HideUiController();
    }

    private void DisableLockModel()
    {
        foreach (var model in VRObjectManagerV2.Instance.vrModels.Where(model => model.isCheckClickModel))
        {
            if (model.GetData().sessionId != RoomManager.Instance.GameRoom.SessionId) continue;
            model.isCheckClickModel = false;
            ClickModel3D.SendSyncEditModel(EditModel.EditModel, model, false,
                MyUtils.GetColorString(model.GetComponent<Renderer>()), false, "");
        }
    }
}
