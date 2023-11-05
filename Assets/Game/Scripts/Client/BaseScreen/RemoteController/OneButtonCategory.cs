using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OneButtonCategory : MonoBehaviour
{
    public Action<int, string> OnClickButton = null;
    [SerializeField] Text text;
    [SerializeField] Button button;

    public string Category { get; set; }
    public int Index { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(() => { OnClickButton?.Invoke(Index, Category); });
    }
    public void SetData(int index, string categoryName)
    {
        Index = index;
        Category = categoryName;
        text.text = categoryName;
    }
    public void OnActived()
    {
        button.interactable = false;
    }
    public void Inactive()
    {
        button.interactable = true;
    }
}
