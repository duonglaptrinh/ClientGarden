using TWT.Model;
using UnityEngine;
using static VrGardenApi.SaveImageApi;

public class ItemDataBaseScreenShot : ItemDataBase
{
    public GetJsonImageResponse imageResponse;

    public ItemDataBaseScreenShot(GetJsonImageResponse imageResponse)
    {
        this.imageResponse = imageResponse;
    }
}
