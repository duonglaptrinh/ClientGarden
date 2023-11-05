using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TypeLandDirection
{
    frontOf, behide, left, right
}

public class UpDownButtonComponent : MonoBehaviour
{
    public Action<TypeLandDirection> onUp;
    public Action<TypeLandDirection> onDown;
    [SerializeField] TypeLandDirection type;
    [SerializeField] Button buttonUp;
    [SerializeField] Button buttonDown;
    [SerializeField] Text textStr;
    public string text
    {
        get => textStr.text;
        set => textStr.text = value;
    }
    void Start()
    {
        buttonDown.onClick.AddListener(() => { onDown?.Invoke(type); });
        buttonUp.onClick.AddListener(() => { onUp?.Invoke(type); });
    }

}
