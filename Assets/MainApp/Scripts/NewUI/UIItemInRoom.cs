using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemInRoom : MonoBehaviour
{
    public Action<VRModelV2> OnClick = null;
    public Action OnClickHouse = null;
    [SerializeField] Text textName;
    [SerializeField] Button btn;
    VRModelV2 Data { get; set; }
    bool isHouse = false;
    void Start()
    {
        btn.onClick.AddListener(() =>
        {
            if (isHouse)
                OnClickHouse?.Invoke();
            else
                OnClick?.Invoke(Data);
        });
    }
    public void Setup(VRModelV2 data)
    {
        Data = data;
        textName.text = data.DataAsset.NameOnApp.Replace("$", " ");
    }
    public void SetupHouse(string nameT)
    {
        isHouse = true;
        textName.text = nameT;
    }
}
