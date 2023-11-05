using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Takasho.GLD.VGS
{
    /// <summary>
    /// Database access object of product
    /// </summary>
    public class ProductDatabaseManager : MonoBehaviour
    {

        [SerializeField] private CategorySO[] _categoryList;
        [SerializeField] private HouseSO[] _houseList;

        private CategorySO _currentCategory;
        private TypeSO _currentType;
        private ProductSO _currentProduct;

        public CategorySO[] CategoryList { get => _categoryList; set => _categoryList = value; }
        public CategorySO CurrentCategory { get => _currentCategory; set => _currentCategory = value; }
        public TypeSO CurrentType { get => _currentType; set => _currentType = value; }
        public ProductSO CurrentProduct { get => _currentProduct; set => _currentProduct = value; }
        public HouseSO[] HouseList { get => _houseList; set => _houseList = value; }

        private void Start()
        {
            _currentCategory = _categoryList[0];
            _currentType = _currentCategory.TypeList[0];
            _currentProduct = _currentType.ProductList[0];
        }

        public void UpdateCurrentCategory(int id)
        {
            _currentCategory = _categoryList[id];
        }

        public void UpdateCurrentType(int id)
        {
            _currentType = _currentCategory.TypeList[id];
        }

        public void UpdateCurrentProduct(int id)
        {
            _currentProduct = _currentType.ProductList[id];
        }

        public ProductController GetProduct(ProductSO product)
        {
            return product.ProductPrefab; 
        }
    }
}
