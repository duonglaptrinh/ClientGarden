using Shim.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIScrollBase : MonoBehaviour
{
    public Action<UIScrollItemBase> OnCreateOneItem = null;
    public Action<int> OnClickOneItem = null;

    [SerializeField] protected UIScrollItemBase prefabItemScroll;
    [SerializeField] protected ScrollRect scroll;
    [SerializeField] protected List<UIScrollItemBase> listItems = new List<UIScrollItemBase>();
    public List<UIScrollItemBase> ListItems => listItems;

    public ScrollRect Scroll => scroll;
    protected List<ItemDataBase> listDatas = new List<ItemDataBase>();
    public List<ItemDataBase> ListDatas => listDatas;
    public float MoreContent { get; set; }
    public int CountAllItem => listItems.Count(x => x.gameObject.activeInHierarchy);

    protected virtual void Start()
    {
        //Initialize(new List<ItemDataBase>() {
        //new ItemDataBase(), new ItemDataBase(), new ItemDataBase(), new ItemDataBase(), new ItemDataBase(),
        // new ItemDataBase(), new ItemDataBase(), new ItemDataBase(), new ItemDataBase(), new ItemDataBase(),
        //  new ItemDataBase(), new ItemDataBase(), new ItemDataBase(), new ItemDataBase(), new ItemDataBase()
        //}); // Test Only
    }
    public void ClearAll()
    {
        foreach (var item in listItems)
        {
            item.gameObject.SetActive(false);
        }
    }
    public void removeItem(UIScrollItemBase item)
    {
        item.gameObject?.SetActive(false);
    }
    public virtual void Initialize(List<ItemDataBase> list, Transform otherParent = null, float moreContent = 0)
    {
        MoreContent = moreContent;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 100;
        ClearAll();
        listDatas = list;
        for (int i = 0; i < listDatas.Count; i++)
        {
            ItemDataBase data = listDatas[i];
            //add a button to the list of buttons
            UIScrollItemBase ui;
            if (i < listItems.Count)
                ui = listItems[i];
            else
            {
                ui = Instantiate(prefabItemScroll, scroll.content);
                listItems.Add(ui);
            }
            ui.gameObject.SetActive(true);
            ui.SetData(data, i);
            ui.OnClickItem = OnClickItem;
            CreateOneItem(ui);
        }

        CalculateFitContent(moreContent, listDatas.Count);
    }

    protected virtual void CreateOneItem(UIScrollItemBase item)
    {
        OnCreateOneItem?.Invoke(item);
        DebugExtension.Log($"Create One Item {item}");
    }

    protected virtual void OnClickItem(int index)
    {
        OnClickOneItem?.Invoke(index);
        DebugExtension.Log($"Click Item {index}");
    }

    public void CalculateFitContent()
    {
        Debug.LogError(CountAllItem);
        CalculateFitContent(MoreContent, CountAllItem);
    }
    protected virtual void CalculateFitContent(float moreContent = 0, int count = 0)
    {
        float spacing = 0;
        RectOffset pading = new RectOffset(0, 0, 0, 0);
        HorizontalOrVerticalLayoutGroup layout = scroll.content.GetComponent<HorizontalOrVerticalLayoutGroup>();
        if (layout)
        {
            pading = layout.padding;
            spacing = layout.spacing;
        }
        RectTransform rect = (RectTransform)prefabItemScroll.transform;
        float width = rect.rect.width;
        float height = rect.rect.height;

        RectTransform rectContent = (RectTransform)scroll.content.transform;

        if (layout is VerticalLayoutGroup)
        {
            rectContent.sizeDelta = new Vector2(rectContent.rect.width, pading.top + pading.bottom + (height + spacing) * count + moreContent);

            RectTransformExtensions.SetLeft(rectContent, 0);
            RectTransformExtensions.SetRight(rectContent, 0);
        }
        else if (layout is HorizontalLayoutGroup)
        {
            rectContent.sizeDelta = new Vector2(pading.left + pading.right + (width + spacing) * count + moreContent, rectContent.rect.height);
            RectTransformExtensions.SetTop(rectContent, 0);
            RectTransformExtensions.SetBottom(rectContent, 0);
        }
    }
    public virtual void FocusItem(RectTransform rect)
    {
        HorizontalOrVerticalLayoutGroup layout = scroll.content.GetComponent<HorizontalOrVerticalLayoutGroup>();
        if (layout is VerticalLayoutGroup)
        {
            RectTransformExtensions.SetTop(scroll.content, -rect.offsetMax.y);
        }
        else if (layout is HorizontalLayoutGroup)
        {
            RectTransformExtensions.SetLeft(scroll.content, -rect.offsetMax.x);
        }
    }
}
