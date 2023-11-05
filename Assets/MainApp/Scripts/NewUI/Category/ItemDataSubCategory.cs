using UnityEngine;
[System.Serializable]
public class ItemDataSubCategory : ItemDataBase
{
    public string categorySubName;
    public string categoryNameOnApp;
    public Sprite thumbnai;

    public ItemDataSubCategory(string name, Sprite icon)
    {
        categorySubName = name;
        this.thumbnai = icon;
    }
    //public void SetData(string name)
    //{
    //    categorySubName = name;
    //}
}
