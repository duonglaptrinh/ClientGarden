using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class EditValueComponent : MonoBehaviour
{
    public Action<ETypeSizeHWD, float> OnSendData = null;

    [SerializeField] protected private Image imageIcon;
    [SerializeField] protected private Text txtSize;
    [SerializeField] protected private Button resetBtn;
    [SerializeField] protected Button btnIncrease;
    [SerializeField] protected Button btnDecrease;

    int[] currentlist;
    ETypeSizeHWD type;
    int currentIndex = 0;
    int valueResetIndex = 0;
    string unit = "";
    public int CurrentValueData => currentlist[currentIndex];

    // Start is called before the first frame update
    void Start()
    {
        btnIncrease.onClick.AddListener(Increase);
        btnDecrease.onClick.AddListener(Decrease);
        resetBtn.onClick.AddListener(ResetData);
    }

    public void Setup(ETypeSizeHWD type, int[] list, float value, string unit)
    {
        this.unit = unit;
        currentlist = list;
        this.type = type;
        if (currentlist.Length == 1)
        {
            btnIncrease.interactable = false;
            btnDecrease.interactable = false;
            resetBtn.enabled = false;
            imageIcon.enabled = false;
        }
        else
        {
            btnIncrease.interactable = true;
            btnDecrease.interactable = true;
            resetBtn.enabled = true;
            imageIcon.enabled = true;
        }
        currentIndex = GetCurrentIndex(currentlist, value);
        valueResetIndex = currentIndex;
        ShowText(txtSize, currentlist[currentIndex]);
    }
    int GetCurrentIndex(int[] list, float value)
    {
        if (list.Length > 0)
        {
            for (int i = 0; i < list.Length; i++)
            {
                if (value == list[i]) return i;
            }
        }
        return 0;
    }
    void ShowText(Text txt, int value)
    {
        float v = value;
        if (unit == MenuEditScaleMode2.Unit_CM) v = value / 10f;
        else if (unit == MenuEditScaleMode2.Unit_MM) v = value;
        else if (unit == MenuEditScaleMode2.Unit_M) v = value / 1000f;
        txt.text = v.ToString() + " " + unit;
    }
    public void UpdateUnit(string unit)
    {
        this.unit = unit;
        ShowText(txtSize, currentlist[currentIndex]);
    }
    void Increase()
    {
        currentIndex++;
        if (currentIndex >= currentlist.Length) currentIndex = 0;
        ShowText(txtSize, currentlist[currentIndex]);
        SendData();
    }
    void Decrease()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = currentlist.Length - 1;
        ShowText(txtSize, currentlist[currentIndex]);
        SendData();
    }
    public void ResetData()
    {
        currentIndex = valueResetIndex;
        ShowText(txtSize, currentlist[currentIndex]);
        SendData();
    }
    void SendData()
    {
        OnSendData?.Invoke(type, currentlist[currentIndex]);
    }
}
