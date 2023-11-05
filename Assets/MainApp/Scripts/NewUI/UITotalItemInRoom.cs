using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITotalItemInRoom : MonoBehaviour
{
    [SerializeField] Text textName;
    [SerializeField] private Text textQuanlity;
    [SerializeField] private Text textPrice;
    bool isHouse = false;

    public void Setup(string name, int quanlyti, string price)
    {
        textName.text = name;
        textQuanlity.text = quanlyti.ToString();
        textPrice.text = price.ToString();
    }
    public void SetupHouse(string nameT, int quanlytiHouse, string priceHouse)
    {
        isHouse = true;
        textName.text = nameT;
        textQuanlity.text = quanlytiHouse.ToString();
        textPrice.text = priceHouse.ToString();
    }
    
}
