using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Takasho.GLD.VGS
{

    /// <summary>
    /// UI Manager class for object select menu
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        #region Field

        //[SerializeField] private MainMenu _mainmenu;
        [SerializeField] private ProductSelectMenu _productSelectMenu;
        [SerializeField] private ProductDetailMenu _productDetailMenu;
        [SerializeField] private ProductSideMenu _productSideMenu;
        [SerializeField] private ProductDatabaseManager _database;
        [SerializeField] private ProductEditMenu _productEditMenu;

        [SerializeField] private HouseSelectMenu _houseSelectMenu;

        [SerializeField] private Transform _canvas;
        [SerializeField] private Transform _player;
        [SerializeField] private Transform _menuLayout;

        [SerializeField] private LayerMask _gardenLayer;
        private float _rayDistance = 10f;

        private bool _isUIActive;
        private ProductController _spawnProduct;
        private ProductController _selectedProduct;

        private CategorySO _currentCategory;
        private ProductSO _currentProduct;
        private TypeSO _currentType;
        private HouseSO _currentHouse;

        private HorizontalLayoutGroup _menuLayoutGroup;
        private CursorManager _cursorManager;

        public bool IsUIActive { get => _isUIActive; set => _isUIActive = value; }
        public MenuState CurrentMenuState { get => _currentMenuState; set => _currentMenuState = value; }

        public enum MenuState
        {
            MAIN,
            CATEGORY,
            TYPE,
            PRODUCT,
            DETAIL,
            SETTING,
            EDIT,
            INFO,
            COLOR,
            SCALE,
            ROTATION,
            GRAB,
            NONE
        }
        public enum CursorType
        {
            DEFAULT,
            ROTATE,
            HAND,
            PRODUCT
        }

        private MenuState _currentMenuState = MenuState.NONE;

        #endregion

        private void Awake()
        {
            _menuLayoutGroup = _menuLayout.GetComponent<HorizontalLayoutGroup>();
            _cursorManager = GetComponent<CursorManager>();
        }

        // Start is called before the first frame update
        private void Start()
        {
            _productSelectMenu.InitializePanel();
            _houseSelectMenu.InitializePanel();
            SetButton();
        }

        private void SetButton()
        {
            //_mainmenu.SetProductButton(OnProductButtonClicked);
            //_mainmenu.SetHouseButton(OnHouseButtonClicked);
            //_mainmenu.SetDeleteButton(OnDeleteButtonClicked);
            //_mainmenu.SetExitButton(OnExitButtonClicked);

            _productSelectMenu.SetDeleteButton(OnDeleteButtonClicked);
            _productDetailMenu.SetDeleteButton(OnDeleteButtonClicked);
            _productSideMenu.SetDeleteButton(OnSideMenuDeleteButtonClicked);
            _productDetailMenu.SetPlaceButton(OnPlaceButtonClicked);
            _productDetailMenu.SetColorButton(OnColorButtonClicked);
            _productDetailMenu.SetSizeButton(OnSizeButtonClicked);
            _productDetailMenu.SetSizeButton(OnSizeButtonClicked);

            _productEditMenu.SetColorButton(OnEditMenuColorButtonClicked);
            _productEditMenu.SetInfoButton(OnEditMenuInfoButtonClicked);
            _productEditMenu.SetScaleButton(OnEditMenuScaleButtonClicked);
            _productEditMenu.SetRotationButton(OnEditMenuRotationButtonClicked);
            _productEditMenu.SetDeleteButton(OnDeleteButtonClicked);

            _houseSelectMenu.SetDeleteButton(OnDeleteButtonClicked);
        }

        // Update is called once per frame
        private void Update()
        {

            //press menu button or right click
            if (Input.GetButtonDown("Menu") || Input.GetMouseButtonDown(1))
            {
                switch (_currentMenuState)
                {
                    case MenuState.NONE:
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        bool isHit = Physics.Raycast(ray, out hit, _rayDistance);

                        if (isHit && hit.collider.CompareTag("Product"))
                        {
                            //When a product is clicked 
                            //Show product edit menu
                            _productEditMenu.ShowMenu();
                            _currentMenuState = MenuState.EDIT;
                            _selectedProduct = hit.collider.GetComponent<ProductController>();
                            UpdateCanvas(_selectedProduct.transform);
                            UpdateMenuLayout();
                        }
                        else
                        {
                            //Show main menu
                            //_mainmenu.ShowMenu();

                            _currentMenuState = MenuState.MAIN;
                            _isUIActive = true;

                            //show Canvas in front of the player
                            UpdateCanvas();

                        }
                        break;
                    default:
                        Debug.Log("Close Menu");
                        //_mainmenu.HideMenu();
                        _productDetailMenu.HideMenu();
                        _productSelectMenu.HideMenu();
                        _productSideMenu.HideMenu();
                        _productEditMenu.HideMenu();
                        _currentMenuState = MenuState.NONE;
                        break;
                }
            }

            //pressed left click
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                switch (_currentMenuState)
                {
                    case MenuState.NONE:

                        //Cursor.lockState = CursorLockMode.None;
                        bool isHit = Physics.Raycast(ray, out hit, _rayDistance);

                        if (isHit && hit.collider.CompareTag("Product"))
                        {
                            //When a product is clicked 
                            Debug.Log("clicked product");
                            _selectedProduct = hit.collider.GetComponent<ProductController>();
                            _selectedProduct.OnStartMoving();
                            _currentMenuState = MenuState.GRAB;
                            SetCursor(CursorType.HAND);
                        }
                        break;

                    case MenuState.SETTING:

                        if (Physics.Raycast(ray, out hit, _rayDistance, _gardenLayer))
                        {
                            if (_spawnProduct == null) return;
                            //Vector3 rotation = _spawnProduct.transform.rotation;
                            //Instantiate(_spawnProduct, hit.point, Quaternion.Euler(-90f, 0f, 0f));
                            _spawnProduct.transform.position = hit.point;
                            //Instantiate(_spawnProduct, hit.point, _spawnProduct.transform.rotation);
                            _currentMenuState = MenuState.NONE;
                            _spawnProduct.OnPlaced();
                            _spawnProduct = null;
                            //Cursor.lockState = CursorLockMode.Locked;
                        }
                        break;

                    default:
                        break;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("mouse up");
                if (_currentMenuState == MenuState.GRAB)
                {

                    _selectedProduct.OnPlaced();
                    _currentMenuState = MenuState.NONE;
                    SetCursor(CursorType.DEFAULT);

                }
            }

            if (_currentMenuState == MenuState.SETTING)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, _rayDistance, _gardenLayer))
                {
                    //instantiateが完了するまではnullになる可能性
                    if (_spawnProduct != null)
                    {
                        _spawnProduct.transform.position = hit.point;
                    }

                }
            }

            if (_currentMenuState == MenuState.GRAB)
            {
                _selectedProduct.transform.position = _player.transform.position + _player.forward * 2f;
                //_selectedProduct.transform.rotation = _player.transform.rotation;
            }
        }

        private void UpdateCategoryMenu()
        {
            Debug.Log("update category menu");
            CategorySO[] categoryList = _database.CategoryList;
            _productSelectMenu.UpdateCategoryMenu(categoryList, SetCategory);
            _productSelectMenu.ShowMenu();
            _productSelectMenu.SetReturnButton(ReturnToCategory);
            UpdateMenuLayout();
        }

        private void UpdateHouseMenu()
        {
            Debug.Log("update house menu");
            HouseSO[] houseList = _database.HouseList;
            _houseSelectMenu.UpdateHouseMenu(houseList, SetHouse);
            _houseSelectMenu.ShowMenu();
            _houseSelectMenu.SetReturnButton(ReturnToCategory);
            UpdateMenuLayout();
        }


        private void UpdateTypeMenu()
        {
            Debug.Log("update type menu");
            _productSelectMenu.UpdateTypeMenu(_currentCategory, SetType);
            _productSelectMenu.SetReturnButton(ReturnToCategory);
            UpdateMenuLayout();
        }

        /// <summary>
        /// 商品メニューを更新
        /// </summary>
        private void UpdateProductMenu()
        {
            Debug.Log("update product menu");
            _productSelectMenu.UpdateProductMenu(_currentType, SetProduct);
            _productSelectMenu.SetReturnButton(ReturnToType);
            UpdateMenuLayout();

        }
        private void UpdateProductDetail()
        {
            Debug.Log("update product detail");
            string category = _currentCategory.DataName;
            string type = _currentType.DataName;
            _productDetailMenu.UpdateProductDetail(_currentProduct, category, type);
            _productDetailMenu.SetReturnButton(ReturnToProduct);
            UpdateMenuLayout();
        }

        /// <summary>
        /// Called from a category button to set current category
        /// </summary>
        /// <param name="categoryId">pushed category Id</param>
        public void SetCategory(CategorySO category)
        {
            Debug.Log("set category");
            _currentCategory = category;
            UpdateTypeMenu();
            _currentMenuState = MenuState.TYPE;
        }

        public void SetHouse(HouseSO house)
        {
            Debug.Log("set category");
            _currentHouse = house;
            UpdateTypeMenu();
            _currentMenuState = MenuState.TYPE;
        }

        public void SetType(TypeSO currentType)
        {
            Debug.Log("set type");
            _currentType = currentType;
            UpdateProductMenu();
            _currentMenuState = MenuState.PRODUCT;

        }

        public void SetProduct(ProductSO currentProduct)
        {
            Debug.Log("set product");
            _productDetailMenu.ShowMenu();
            _productSelectMenu.HideMenu();
            _currentProduct = currentProduct;
            UpdateProductDetail();
            _currentMenuState = MenuState.SETTING;
        }

        private void DebugDirectory()
        {
            Debug.Log("category" + _database.CurrentCategory.DataName);
            Debug.Log("type" + _database.CurrentType.DataName);
            Debug.Log("product" + _database.CurrentProduct.DataName);
        }

        private void ReturnToCategory()
        {
            Debug.Log("return to category");
            UpdateCategoryMenu();
        }

        public void ReturnToType()
        {
            Debug.Log("return to type");
            UpdateTypeMenu();
        }

        public void ReturnToProduct()
        {
            Debug.Log("return to product");
            _productDetailMenu.HideMenu();
            _productSelectMenu.ShowMenu();
            UpdateProductMenu();
        }

        public void UpdateCanvas()
        {
            Debug.Log("Update canvas");
            _canvas.position = _player.position + _player.forward * 3f + new Vector3(0, 1f, 0);
            _canvas.rotation = _player.rotation;
        }

        private void UpdateMenuLayout()
        {
            if (_currentMenuState == MenuState.EDIT)
            {

                if (GetActiveChildren(_menuLayout) == 1)
                {
                    _menuLayoutGroup.spacing = 0;
                    _menuLayoutGroup.padding.left = -450;
                }
                else
                {
                    _menuLayoutGroup.padding.left = 90;
                    _menuLayoutGroup.spacing = -350;
                }
            }
            else
            {
                _menuLayoutGroup.padding.left = 0;
                if (GetActiveChildren(_menuLayout) == 2)
                {
                    _menuLayoutGroup.spacing = -300;
                }
                else
                {
                    _menuLayoutGroup.spacing = 0;
                }
            }

        }


        private int GetActiveChildren(Transform parent)
        {
            int count = 0;

            foreach (Transform child in parent)
            {
                if (child.gameObject.activeSelf)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 商品クリック時の編集メニュー
        /// </summary>
        /// <param name="target"></param>
        public void UpdateCanvas(Transform target)
        {
            _canvas.position = target.position + new Vector3(0, 1f, 0);
            _canvas.rotation = _player.rotation;
            _menuLayoutGroup.padding.left = 500;
        }

        public void OnProductButtonClicked()
        {
            UpdateCategoryMenu();
        }

        public void OnHouseButtonClicked()
        {
            UpdateHouseMenu();
        }

        public void OnDeleteButtonClicked()
        {
            //_mainmenu.HideMenu();
            _productDetailMenu.HideMenu();
            _productSelectMenu.HideMenu();
            _houseSelectMenu.HideMenu();
            _productSideMenu.HideMenu();
            _productEditMenu.HideMenu();
            _currentMenuState = MenuState.NONE;
        }

        public void OnColorButtonClicked()
        {
            _productSideMenu.ShowMenu();
            UpdateMenuLayout();
        }

        public void OnPlaceButtonClicked()
        {
            Debug.Log("place product");
            _isUIActive = false;
            _productDetailMenu.HideMenu();
            //_mainmenu.HideMenu();
            ProductController product = _database.GetProduct(_currentProduct);
            _spawnProduct = Instantiate(product, new Vector3(0f, 10f, 0f), product.transform.rotation);
            _spawnProduct.InitialezeProduct(this);
            _currentMenuState = MenuState.SETTING;
        }

        public void OnSizeButtonClicked()
        {
            _productSideMenu.ShowMenu();
        }

        public void OnSideMenuDeleteButtonClicked()
        {
            _productSideMenu.HideMenu();
            UpdateMenuLayout();
        }

        public void OnEditMenuColorButtonClicked()
        {
            _productSideMenu.ShowMenu();
            _productSideMenu.UpdateProductSideMenu(ProductSideMenu.SideMenuState.COLOR);
            UpdateMenuLayout();
        }
        public void OnEditMenuInfoButtonClicked()
        {
            _productSideMenu.ShowMenu();
            _productSideMenu.UpdateProductSideMenu(ProductSideMenu.SideMenuState.INFO);
            UpdateMenuLayout();
        }
        public void OnEditMenuScaleButtonClicked()
        {
            _productSideMenu.ShowMenu();
            _productSideMenu.UpdateProductSideMenu(ProductSideMenu.SideMenuState.SCALE);
            UpdateMenuLayout();
        }
        public void OnEditMenuRotationButtonClicked()
        {
            _productSideMenu.ShowMenu();
            _productSideMenu.UpdateProductSideMenu(ProductSideMenu.SideMenuState.ROTATION);
            UpdateMenuLayout();
        }
        public void OnExitButtonClicked()
        {
            //_mainmenu.HideMenu();
            _productDetailMenu.HideMenu();
            _productSelectMenu.HideMenu();
            _houseSelectMenu.HideMenu();
            _productSideMenu.HideMenu();
            _productEditMenu.HideMenu();
            _currentMenuState = MenuState.NONE;
            SceneManager.LoadScene("LobbyScene");
        }


        /// <summary>
        /// Change cursor according to current situation
        /// </summary>
        /// <param name="cursorType"></param>
        public void SetCursor(CursorType cursorType)
        {
            switch (cursorType)
            {
                case CursorType.DEFAULT:
                    _cursorManager.SetDefaultCursor();
                    break;
                case CursorType.ROTATE:
                    _cursorManager.SetRotateCursor();
                    break;
                case CursorType.HAND:
                    _cursorManager.SetHandCursor();
                    break;
                case CursorType.PRODUCT:
                    _cursorManager.SetProductCursor();
                    break;
                default:
                    break;
            }
        }

    }
}

