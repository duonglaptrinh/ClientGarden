using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Takasho.GLD.VGS
{
    /// <summary>
    /// Scriptable Object class of product type
    /// </summary>
    [CreateAssetMenu(fileName = "Type", menuName = "ScriptableObjects/Type", order = 2)]
    public class TypeSO : BaseDataSO
    {
        [SerializeField] private ProductSO[] _productList;

        public ProductSO[] ProductList { get => _productList; set => _productList = value; }
    }
}