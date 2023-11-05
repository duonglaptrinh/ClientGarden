using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HideDeviceKeyboard : InputField
{
    protected override void Start()
    {
        keyboardType = (TouchScreenKeyboardType)(-1);
        base.Start();
    }
    protected override void LateUpdate()
    {
#if (UNITY_EDITOR ||  UNITY_STANDALONE)
        base.LateUpdate();
#endif
    }
}
