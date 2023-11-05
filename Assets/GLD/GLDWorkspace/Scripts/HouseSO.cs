using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Takasho.GLD.VGS
{
    /// <summary>
    /// Scriptable Object class of house
    /// </summary>
    [CreateAssetMenu(fileName = "House", menuName = "ScriptableObjects/House", order = 3)]
    public class HouseSO : BaseDataSO
    {
        [SerializeField] private GameObject _housePrefab;

        public GameObject HousePrefab { get => _housePrefab; set => _housePrefab = value; }
    }
}