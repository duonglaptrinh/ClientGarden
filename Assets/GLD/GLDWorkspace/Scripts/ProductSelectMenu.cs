using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Takasho.GLD.VGS
{
    /// <summary>
    /// 商品選択メニューのコントローラクラス
    /// </summary>
    public class ProductSelectMenu : MonoBehaviour
    {
        #region フィールド

        [SerializeField] private Text _menuTitle;
        [SerializeField] private Text _description;
        [SerializeField] private Transform _cardPanel;
        [SerializeField] private Button _returnButton;
        [SerializeField] private Button _previousButton;
        [SerializeField] private Button _nextButton;
        [SerializeField] private MenuCard _menuCard;
        [SerializeField] private Button _deleteButton;

        private List<MenuCard> _cardList = new List<MenuCard>();

        public Action<CategorySO> OnCategorySet;

        public Action<TypeSO> OnTypeSet;

        public Action<ProductSO> OnProductSet;

        public Action OnReturnButtonClicked;

        #endregion

        private void Start()
        {

        }


        private void Update()
        {

        }

        /// <summary>
        /// Initialize Card Panel Pooling
        /// </summary>
        public void InitializePanel()
        {
            for (int i = 0; i < 10; i++)
            {
                MenuCard card = Instantiate(_menuCard) as MenuCard;
                _cardList.Add(card);
                card.transform.SetParent(_cardPanel);
                card.transform.localScale = new Vector3(1f, 1f, 1f);
                card.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Show main menu
        /// </summary>
        public void ShowMenu()
        {
            this.gameObject.SetActive(true);
        }

        /// <summary>
        /// Hide main menu
        /// </summary>
        public void HideMenu()
        {
            this.gameObject.SetActive(false);
        }


        /// <summary>
        /// Update category menu with category list in database
        /// </summary>
        /// <param name="database"></param>
        public void UpdateCategoryMenu(CategorySO[] categoryList, Action<CategorySO> onCategorySet)
        {
            _menuTitle.text = "商品選択メニュー";
            _description.text = "カテゴリーを選択してください";
            _returnButton.gameObject.SetActive(false);

            for (int i = 0; i < _cardList.Count; i++)
            {
                if (i < categoryList.Length)
                {
                    _cardList[i].UpdateContent(categoryList[i]);
                    _cardList[i].gameObject.SetActive(true);

                    Button button = _cardList[i].GetComponent<Button>();


                    //各ボタンのクリックイベントにカテゴリー選択メソッドを設定
                    int ii = i;
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => onCategorySet(categoryList[ii]));
                }
                else
                {
                    _cardList[i].gameObject.SetActive(false);
                }
            }
        }

        public CategorySO UpdateTypeMenu(CategorySO currentCategory, Action<TypeSO> onTypeSet)
        {
            _description.text = currentCategory.DataName;
            _returnButton.gameObject.SetActive(true);


            for (int i = 0; i < _cardList.Count; i++)
            {
                if (i < currentCategory.TypeList.Length)
                {
                    _cardList[i].UpdateContent(currentCategory.TypeList[i]);
                    _cardList[i].gameObject.SetActive(true);

                    int ii = i;
                    Button button = _cardList[i].GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => onTypeSet(currentCategory.TypeList[ii])); 
                }
                else
                {
                    _cardList[i].gameObject.SetActive(false);
                }
            }

            return currentCategory;

        }

        /// <summary>
        /// 商品メニューを更新
        /// </summary>
        public void UpdateProductMenu(TypeSO currentType, Action<ProductSO> onProductSet)
        {
            _description.text = currentType.DataName;
            _returnButton.gameObject.SetActive(true);

            for (int i = 0; i < _cardList.Count; i++)
            {
                if (i < currentType.ProductList.Length)
                {
                    _cardList[i].UpdateContent(currentType.ProductList[i]);
                    _cardList[i].gameObject.SetActive(true);

                    int ii = i;
                    Button button = _cardList[i].GetComponent<Button>();
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => onProductSet(currentType.ProductList[ii]));
                }
                else
                {
                    _cardList[i].gameObject.SetActive(false);
                }
            }

        }

        public void SetReturnButton(Action onReturnButtonClicked)
        {
            _returnButton.onClick.RemoveAllListeners();
            _returnButton.onClick.AddListener(() => onReturnButtonClicked());
        }

        public void SetDeleteButton(Action onDeleteButtonClicked)
        {
            _deleteButton.onClick.RemoveAllListeners();
            _deleteButton.onClick.AddListener(() => onDeleteButtonClicked());
        }
    }
}


