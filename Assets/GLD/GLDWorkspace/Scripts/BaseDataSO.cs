using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Takasho.GLD.VGS
{
    /// <summary>
    /// Base class for Scriptable Object of database 
    /// </summary>
    public class BaseDataSO : ScriptableObject
    {
        [SerializeField] private int _dataId;
        [SerializeField] private string _dataName;
        [SerializeField] private Sprite _dataSprite;

        public int DataId { get => _dataId; set => _dataId = value; }
        public string DataName { get => _dataName; set => _dataName = value; }
        public Sprite DataSprite { get => _dataSprite; set => _dataSprite = value; }
    }
}