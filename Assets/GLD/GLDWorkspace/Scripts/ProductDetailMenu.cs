using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Takasho.GLD.VGS
{
    /// <summary>
    /// 商品詳細メニューのコントローラクラス
    /// </summary>
    public class ProductDetailMenu : MonoBehaviour
    {
        [SerializeField] private Text _category;
        [SerializeField] private Text _name;
        [SerializeField] private Text _price;
        [SerializeField] private Text _material;
        [SerializeField] private Image _image;
        [SerializeField] private Button _returnButton;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private Button _placeButton;
        [SerializeField] private Button _colorButton;
        [SerializeField] private Button _sizeButton;

        public Action OnReturnButtonClicked;

        public void UpdateProductDetail(ProductSO currentProduct, string category, string type)
        {
            _category.text = category + "　" + type;

            //商品名に半角スペースがある場合は改行する
            _name.text = currentProduct.DataName.Replace(" ", "\r\n");

            //priceを価格表示に変換
            _price.text = string.Format("{0:c}（税込）", currentProduct.ProductPrice);

            _material.text = "素材:" + currentProduct.ProductMaterial;
            _image.sprite = currentProduct.ProductImage;
        }

        public void ShowMenu()
        {
            gameObject.SetActive(true);
        }

        public void HideMenu()
        {
            gameObject.SetActive(false);
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

        public void SetPlaceButton(Action onPlaceButtonClicked)
        {
            _placeButton.onClick.RemoveAllListeners();
            _placeButton.onClick.AddListener(() => onPlaceButtonClicked());
        }

        public void SetColorButton(Action onColorButtonClicked)
        {
            _colorButton.onClick.RemoveAllListeners();
            _colorButton.onClick.AddListener(() => onColorButtonClicked());
        }

        public void SetSizeButton(Action onSizeButtonClicked)
        {
            _sizeButton.onClick.RemoveAllListeners();
            _sizeButton.onClick.AddListener(() => onSizeButtonClicked());
        }

    }
}