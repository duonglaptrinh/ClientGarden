using Shim.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UIScroll;
using UnityEngine;
using UnityEngine.UI;

public class MenuTutorialUI : MonoBehaviour
{
    public Action<TutorialData> OnClickViewContent = null;
    public Action OnCloseMenu = null;
    public Action OnBackMenu = null;
    [SerializeField] UIScrollGrid scrollObject;
    [SerializeField] TextAsset jsonTutorial;
    [SerializeField] Button btnClose;
    [SerializeField] Button btnBack;
    int currentIndex;
    private void Awake()
    {
        scrollObject.OnCreateOneItem = CreateOneItemObject;
        scrollObject.OnClickOneItem = OnClickItemObject;
        btnClose.onClick.AddListener(() =>
        {
            OnCloseMenu?.Invoke();  
        });
        btnBack.onClick.AddListener(() =>
        {
            OnBackMenu?.Invoke();
        });
    }
    void Start()
    {
        LoadData();
    }
    public void LoadData()
    {
        RectTransformExtensions.SetLeft(scrollObject.Scroll.content, 0);
        RectTransformExtensions.SetRight(scrollObject.Scroll.content, 0);
        Vector3 pos = scrollObject.Scroll.content.localPosition;
        pos.y = 0;
        scrollObject.Scroll.content.localPosition = pos;

        JsonTutorialData data = JsonUtility.FromJson<JsonTutorialData>(jsonTutorial.text);

        List<ItemDataBase> list = new List<ItemDataBase>();

        for (int i = 0; i < data.tutorial_list.Count; i++)
        {
            list.Add(new ItemDataTutorial(data.tutorial_list[i]));
        }
        if (list.Count > 0)
        {
            scrollObject.ClearAll();
            scrollObject.Initialize(list, 2);
        }
    }
    protected virtual void CreateOneItemObject(UIScrollItemBase item)
    {
        UIItemTutorial itemgrid = (UIItemTutorial)item;
        ItemDataTutorial data = (ItemDataTutorial)itemgrid.CurrentData;
        itemgrid.SetDataStart(data.Data);
    }

    protected virtual void OnClickItemObject(int index)
    {
        UIItemTutorial itemgrid = (UIItemTutorial)scrollObject.ListItems[index];
        ItemDataTutorial data = (ItemDataTutorial)itemgrid.CurrentData;
        currentIndex = index;
        OnClickViewContent?.Invoke(data.Data);
        OnCloseMenu?.Invoke();
    }
}
