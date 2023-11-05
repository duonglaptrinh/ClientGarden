using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuNewItem : MonoBehaviour
{
    public List<Text> listTextButton;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < listTextButton.Count; i++)
        {
            MenuNewItemData data = LoadResourcesData.Instance.listCategoryMenu[i];
            listTextButton[i].text = data.nameTitle;
        }
    }
}
