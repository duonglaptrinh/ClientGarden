using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Takasho.GLD.VGS
{
    /// <summary>
    /// Scriptable Object class of product category
    /// </summary>
    [CreateAssetMenu(fileName = "Category", menuName = "ScriptableObjects/Cateogry", order = 1)]
    public class CategorySO : BaseDataSO
    {
        [SerializeField] private TypeSO[] _typeList;

        public TypeSO[] TypeList { get => _typeList; set => _typeList = value; }
    }
}
