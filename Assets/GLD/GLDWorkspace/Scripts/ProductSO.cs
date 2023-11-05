using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Takasho.GLD.VGS
{
    /// <summary>
    /// Scriptable Object class of product
    /// </summary>
    [CreateAssetMenu(fileName = "Product", menuName = "ScriptableObjects/Product", order = 3)]
    public class ProductSO : BaseDataSO
    {

        [SerializeField] private int _productPrice;
        [SerializeField] private string _ProductMaterial;

        [SerializeField] private Sprite _productImage;

        [SerializeField] private ProductController _productPrefab;

        [SerializeField] private string[] _colorList;
        [SerializeField] private string[] _sizeList;

        public int ProductPrice { get => _productPrice; set => _productPrice = value; }
        public string ProductMaterial { get => _ProductMaterial; set => _ProductMaterial = value; }
        public Sprite ProductImage { get => _productImage; set => _productImage = value; }
        public ProductController ProductPrefab { get => _productPrefab; set => _productPrefab = value; }
        public string[] ColorList { get => _colorList; set => _colorList = value; }
        public string[] SizeList { get => _sizeList; set => _sizeList = value; }
    }
}