using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CategoryProducts : MonoBehaviour
{
    [SerializeField] List<OnActionCategoryBtn> listButtonCategory;
    [SerializeField] List<OnActionCategoryBtn> listSubButtonCategory;
    [SerializeField] OnActionCategoryBtn categoryBtnPrefab;
    [SerializeField] SubCategoryProducts subCategoryProducts;
    [SerializeField] MenuModels menuModels;

    public Button backBtn;
    GameObject editRemote;
    public int indexCategory = 0;
    public Text txtTitleSubCategory;
    public Text txtTitleListModel;
    public Text txtPrice;
    public Text txtMaterial;
    public Text txtStructure;
    public Text txtWeight;
    public Text txtColor;
    public Text txtScale;
    public Text txtNameProducts;

    [SerializeField] Transform parCategoryBtn;
    [SerializeField] Transform parSubCategoryBtn;
    [SerializeField] List<Sprite> listCategoryImg;
    [SerializeField] List<Sprite> listSubCategoryFloorWallImg;
    [SerializeField] List<Sprite> listIconImg;
    [SerializeField] GameObject menuCategory;
    [SerializeField] GameObject menuSubCategory;
    [SerializeField] GameObject informationModel;

    [Header("SubCartegory Title")]
    [SerializeField] Image iconChangeTreeInsize;
    [SerializeField] Image iconsprite;
    //[SerializeField] Sprite spriteAround;
    [SerializeField] Image iconSpriteChange;
    //[SerializeField] Sprite spriteCar;
    //[SerializeField] Sprite spriteInSide;

    List<string> listCategory => LoadResourceAddessable.listCategoryNew;
    List<string> listCategoryUpdate => LoadResourceAddessable.listCategoryNewUpdate;
    string currentSubName = null;
    private void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        editRemote = BaseScreenUiControllerV2.Instance.EditRemote;

        backBtn.onClick.AddListener(BackToMainMenu);
        // catagory1Btn.onClick.AddListener(ShowSubCategory);
        //CategoryDataAsset a = AddressableDownloadManager.ResourcesData.GetCategoryDataAsset(listCategory[indexCategory]);
        //listSub = AddressableDownloadManager.ResourcesData.GetNameSubCategoryDataAsset(a);
        //List<OneModelAssetsData> listModel = AddressableDownloadManager.ResourcesData.GetSubCategoryDataAsset(a, listSub[indexCategory]);
    }

    // Update is called once per frame
    public void BackToMainMenu()
    {
        MainMenu newMainMenu = BaseScreenUiControllerV2.Instance.NewMainMenu;
        //newMainMenu.IsNeedResetMenuWhenEnable = false;
        editRemote.SetActive(true);
        newMainMenu.ShowHideMenuNewItem(true);
        //newMainMenu.IsNeedResetMenuWhenEnable = true;
        gameObject.SetActive(false);
        BaseScreenUiControllerV2.Instance.isCheckOpenInsize = false;
    }
    private void OnEnable()
    {
        menuCategory.SetActive(true);
        menuSubCategory.gameObject.SetActive(false);
        menuModels.gameObject.SetActive(false);
        informationModel.SetActive(false);
    }
    public void BackToMenuCategory()
    {
        if (indexCategory == listCategoryUpdate.Count - 6 || BaseScreenUiControllerV2.Instance.isCheckOpenInsize)
        {
            BackToMainMenu();
            return;
        }
        menuCategory.SetActive(true);
        menuSubCategory.SetActive(false);
        menuModels.gameObject.SetActive(false);
        informationModel.SetActive(false);

    }
    public void BackToMenuSubCategory()
    {
        if (currentSubName == Sub_Category_Light)
        {
            BackToMenuCategory();
            return;
        }
        if (indexCategory == listCategoryUpdate.Count - 5)
        {
            BackToMainMenu();
            return;
        }
        menuCategory.SetActive(false);
        menuSubCategory.SetActive(true);
        menuModels.gameObject.SetActive(false);
        informationModel.SetActive(false);
    }
    public void BackToMenuModel()
    {
        menuCategory.SetActive(false);
        menuSubCategory.SetActive(false);
        menuModels.gameObject.SetActive(true);
        informationModel.SetActive(false);

    }
    public void ClickCategoryButton(int indexCategory)
    {
        iconChangeTreeInsize.gameObject.SetActive(false);
        // if (indexCategory == listCategoryUpdate.Count - 1)
        // {
        //     ShowLight();
        //     return;
        // }
        currentSubName = null;
        this.indexCategory = indexCategory;
        menuCategory.SetActive(false);
        menuSubCategory.SetActive(true);
        menuModels.gameObject.SetActive(false);
        informationModel.SetActive(false);

        subCategoryProducts.LoadSubCategory(listCategory[indexCategory]);
        txtTitleSubCategory.text = listCategoryUpdate[indexCategory];
    }

    void OnClickSubCategory(CategoryDataAsset category, string subName, string subNameOnApp, bool forceUseSubNameForTitle)
    {
        currentSubName = subName;
        menuCategory.SetActive(false);
        menuSubCategory.SetActive(false);
        menuModels.gameObject.SetActive(true);
        informationModel.SetActive(false);

        menuModels.LoadModel(category, subName);
        if (forceUseSubNameForTitle)
        {
            iconsprite.gameObject.SetActive(true);
            string title = subName;
            if (subName == Sub_Category_Around)
            {
                title = LoadResourcesData.Instance.listCategoryMenu[(int)ETypeMenuNewItem.Around].nameTitle;// "周辺環境";
                iconsprite.sprite = LoadResourcesData.Instance.listCategoryMenu[(int)ETypeMenuNewItem.Around].iconImage; ;// spriteAround;
            }
            else if (subName == Sub_Category_Car)
            {
                title = LoadResourcesData.Instance.listCategoryMenu[(int)ETypeMenuNewItem.Car].nameTitle;// "乗物";
                iconsprite.sprite = LoadResourcesData.Instance.listCategoryMenu[(int)ETypeMenuNewItem.Car].iconImage; ;// spriteCar;
            }
            else if (subName == Sub_Category_InSide)
            {
                iconsprite.sprite = LoadResourcesData.Instance.listCategoryMenu[(int)ETypeMenuNewItem.In_Side].iconImage;// spriteInSide;
            }
            else if (subName == Sub_Category_Floor_Wall)
            {
                iconsprite.sprite = LoadResourcesData.Instance.listCategoryMenu[(int)ETypeMenuNewItem.Floor_Wall].iconImage; ;// spriteFloorWall;
            }
            else if (subName == Sub_Category_Light)
            {
                iconsprite.gameObject.SetActive(false);
            }
            txtTitleListModel.text = title;
        }
        else
        {
            iconsprite.gameObject.SetActive(false);
            if (string.IsNullOrEmpty(subNameOnApp))
                txtTitleListModel.text = subName;
            else
            {
                subNameOnApp = subNameOnApp.Replace("$", "\n");
                txtTitleListModel.text = subNameOnApp;
            }
        }
    }
    public void OnClickMenuModel()
    {
        menuCategory.SetActive(false);
        menuSubCategory.SetActive(false);
        menuModels.gameObject.SetActive(false);
        informationModel.SetActive(true);
    }

    public void ShowCategoryByNewItem(ETypeMenuNewItem type)
    {
        switch (type)
        {
            case ETypeMenuNewItem.Floor_Wall:
                ShowFloorWall();
                break;
            case ETypeMenuNewItem.Out_Side:
                ShowOutSide();
                break;
            case ETypeMenuNewItem.In_Side:
                ShowInSide();
                break;
            case ETypeMenuNewItem.Tree:
                ShowTree();
                break;
            case ETypeMenuNewItem.Car:
                ShowCar();
                break;
            case ETypeMenuNewItem.Around:
                ShowAround();
                break;
        }
    }
    void ShowOutSide()
    {
        foreach (var item in listButtonCategory)
        {
            Destroy(item.gameObject);
        }
        listButtonCategory.Clear();
        iconSpriteChange.sprite = listIconImg[0];
        txtNameProducts.text = "エクステリア";
        indexCategory = 0;

        List<string> list = listCategoryUpdate.GetRange(0, listCategoryUpdate.Count - 6);
        foreach (var nameC in list)
        {
            OnActionCategoryBtn item = Instantiate(categoryBtnPrefab, parCategoryBtn);
            float totalSub = 0;
            totalSub = AddressableDownloadManager.ResourcesData.GetTotalSubCategory(nameC == list[1] ? listCategory[1] : nameC);
            item.SetData(indexCategory, nameC, listCategoryImg[indexCategory],totalSub.ToString(CultureInfo.InvariantCulture));
            int index = indexCategory;
            item.ClickButton = () =>
            {
                ClickCategoryButton(index);
            };
            listButtonCategory.Add(item);
            indexCategory++;
        }
        AddLightButton();

        subCategoryProducts.OnClickSubCategory = OnClickSubCategory;
    }
    void ShowFloorWall()
    {
        foreach (var item in listButtonCategory)
        {
            Destroy(item.gameObject);
        }
        listButtonCategory.Clear();
        iconSpriteChange.sprite = listIconImg[1];
        txtNameProducts.text = "床・壁";
        indexCategory = 0;

        List<string> list =  listCategoryUpdate.Skip(7).Take(2).ToList();;
        foreach (var nameC in list)
        {
            OnActionCategoryBtn item = Instantiate(categoryBtnPrefab, parCategoryBtn);
            float totalSub = 0;
            totalSub = AddressableDownloadManager.ResourcesData.GetTotalSubCategory(nameC);
            item.SetData(indexCategory, nameC, listSubCategoryFloorWallImg[indexCategory], totalSub.ToString(CultureInfo.InvariantCulture));
            int index = indexCategory;
            item.ClickButton = () =>
            {
                ClickCategoryButton(index + 7);
            };
            listButtonCategory.Add(item);
            indexCategory++;
        }

        subCategoryProducts.OnClickSubCategory = OnClickSubCategory;
    }
    void AddLightButton()
    {
        //Add light
        string nameLight = Sub_Category_Light;
        Sprite sprite = null;
        float totalSubLight = 0;
        indexCategory = listCategoryUpdate.Count - 5; // last category
        ItemDataCategory categoryThumbnail = LoadResourcesData.Instance.GetDataCategoryByName(listCategoryUpdate[indexCategory]);
        for (int i = 0; i < categoryThumbnail.listSubCategoryData.Count; i++)
        {
            if (nameLight.Equals(categoryThumbnail.listSubCategoryData[i].categorySubName))
            {
                sprite = categoryThumbnail.listSubCategoryData[i].thumbnai;
                totalSubLight = AddressableDownloadManager.ResourcesData.GetTotalSubCategory(listCategoryUpdate[indexCategory + 4]);
                break;
            }
        }

        OnActionCategoryBtn item = Instantiate(categoryBtnPrefab, parCategoryBtn);
        item.SetData(indexCategory, nameLight, sprite, totalSubLight.ToString(CultureInfo.InvariantCulture));
        int index = indexCategory;
        item.ClickButton = () =>
        {
            ClickCategoryButton(index + 4);
        };
        listButtonCategory.Add(item);

    }
    void ShowTree()
    {
        indexCategory = listCategoryUpdate.Count - 6;
        ClickCategoryButton(indexCategory);
        iconChangeTreeInsize.gameObject.SetActive(true);
        iconChangeTreeInsize.sprite = listIconImg[2];
        subCategoryProducts.OnClickSubCategory = OnClickSubCategory;
    }
    void ShowInSide()
    {
        BaseScreenUiControllerV2.Instance.isCheckOpenInsize = true;
        indexCategory = listCategoryUpdate.Count - 2;
        ClickCategoryButton(indexCategory);
        iconChangeTreeInsize.gameObject.SetActive(true);
        iconChangeTreeInsize.sprite = listIconImg[3];
        subCategoryProducts.OnClickSubCategory = OnClickSubCategory;
    }
    public string Sub_Category_Around => LoadResourcesData.Instance.listCategoryMenu[(int)ETypeMenuNewItem.Around].nameCategory;//= "建物";
    public string Sub_Category_Floor_Wall => LoadResourcesData.Instance.listCategoryMenu[(int)ETypeMenuNewItem.Floor_Wall].nameCategory;//= "床・壁";
    public string Sub_Category_Car => LoadResourcesData.Instance.listCategoryMenu[(int)ETypeMenuNewItem.Car].nameCategory;//= "乗り物";
    public string Sub_Category_InSide => LoadResourcesData.Instance.listCategoryMenu[(int)ETypeMenuNewItem.In_Side].nameCategory;//= "インテリア";
    /// <summary>
    /// Special add to OutSide Category
    /// </summary>
    public const string Sub_Category_Light = "照明";
    void ShowLight()
    {
        ShowSubCategoryOtherByRequire(Sub_Category_Light);
    }
    // void ShowFloorWall()
    // {
    //     ShowSubCategoryOtherByRequire(Sub_Category_Floor_Wall);
    // }
    // void ShowInSide()
    // {
    //     ShowSubCategoryOtherByRequire(Sub_Category_InSide);
    // }
    void ShowCar()
    {
        ShowSubCategoryOtherByRequire(Sub_Category_Car);
    }
    void ShowAround()
    {
        ShowSubCategoryOtherByRequire(Sub_Category_Around);
    }
    void ShowSubCategoryOtherByRequire(string subName)
    {
        indexCategory = listCategoryUpdate.Count - 5;
        CategoryDataAsset category = AddressableDownloadManager.ResourcesData.GetCategoryDataAsset(listCategory[indexCategory]);
        string categoryName = category.NameCategory;
        OnClickSubCategory(category, subName, categoryName, forceUseSubNameForTitle: true);
    }
}

public enum ETypeMenuNewItem
{
    Floor_Wall,
    Out_Side,
    In_Side,
    Tree,
    Car,
    Around
}

[Serializable]
public class MenuNewItemData
{
    /// <summary>
    /// name use for load model from data addressable
    /// </summary>
    public string nameCategory;
    public ETypeMenuNewItem type;
    /// <summary>
    /// name use for title
    /// </summary>
    public string nameTitle;
    /// <summary>
    /// Icon for category
    /// </summary>
    public Sprite iconImage;
}