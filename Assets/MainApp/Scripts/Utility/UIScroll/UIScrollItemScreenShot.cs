using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class UIScrollItemScreenShot : UIScrollItemBase
{
    public bool IsNeedReload { get; set; } = true;
    public override void SetData(ItemDataBase data, int index)
    {
        ItemDataBaseScreenShot Cdata = (ItemDataBaseScreenShot)data;
        ItemDataBaseScreenShot olddata = (ItemDataBaseScreenShot)CurrentData;
        if (olddata != null)
        {
            if (olddata.imageResponse.Blob == Cdata.imageResponse.Blob)
            {
                IsNeedReload = false;
                //DebugExtension.LogError("1 " + IsNeedReload);
            }
            else
            {
                IsNeedReload = true;
                //DebugExtension.LogError("2 " + IsNeedReload);
            }
        }
        else
        {
            IsNeedReload = true;
            //DebugExtension.LogError("3 " + IsNeedReload);
        }
        base.SetData(data, index);
    }

}
