using System.Collections;
using System.Collections.Generic;
using TWT.Model;
using UnityEngine;

public class ItemDataNewView : ItemDataBase
{
    public VRStartPointData VRStartPointData;
    public bool IsCreateNew;
    
    public ItemDataNewView(VRStartPointData vrStartPointData, bool isCreateNew)
    {
        this.VRStartPointData = vrStartPointData;
        this.IsCreateNew = isCreateNew;
    }
}
