using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScrollItemBase : MonoBehaviour
{
    [SerializeField] protected Button btnClick;

    public Action<int> OnClickItem = null;

    public ItemDataBase CurrentData;
    protected int myIndex;
    public int MyIndex => myIndex;

    protected virtual void Start()
    {
        if (btnClick)
            btnClick.onClick.AddListener(() =>
            {
                OnClickItem?.Invoke(myIndex);
            });
    }
    /// <summary>
    /// Alway call when create scroll -> called
    /// </summary>
    /// <param name="data"></param>
    /// <param name="index"></param>
    public virtual void SetData(ItemDataBase data, int index)
    {
        myIndex = index;
        CurrentData = data;
    }
    public virtual void UnSelect()
    {
    }

    public virtual void Select()
    {
    }
    public virtual void ClickFirst()
    {
        btnClick.onClick.Invoke();
    }
}
