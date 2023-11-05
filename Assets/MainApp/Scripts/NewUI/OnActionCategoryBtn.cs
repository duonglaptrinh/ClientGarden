using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnActionCategoryBtn : MonoBehaviour
{
    public Action<int, string> ShowCategory = null;
    public Action ClickButton = null;
    [SerializeField] Text text;
    [SerializeField] private TMP_Text textTotalSubCategory;
    [SerializeField] Button button;
    public Image Image;

    public string Category { get; set; }
    public int Index { get; set; }
    public Button getBtn { get { return button; } }
    // Start is called before the first frame update
    private void Awake()
    {
        button = GetComponent<Button>();
    }
    void Start()
    {
        button.onClick.AddListener(() => 
        { 
            ClickButton.Invoke();
        });
    }
    public void SetData(int index, string categoryName, Sprite image,string totalSub)
    {
        Index = index;
        Category = categoryName;
        text.text = categoryName;
        Image.sprite = image;
        textTotalSubCategory.text = totalSub;
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
