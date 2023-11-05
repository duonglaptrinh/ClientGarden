using System;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class ImageUtility
{
    /// <summary>
    /// Not support bmg
    /// </summary>
    /// <param name="texture2dOrigin"></param>
    /// <param name="url"></param>
    /// <param name="onError"></param>
    /// <param name="objectTracking"></param>
    /// <returns></returns>
    public static async UniTask<Texture2D> LoadImageAsync(this Texture2D texture2dOrigin, 
        string url, 
        Action<Exception> onError = null,
        GameObject objectTracking = null)
    {
        try
        {
            if (objectTracking)
            {
                var bytes = await ObservableUnityWebRequest.GetBytesAsObservable(url).TakeUntilDestroy(objectTracking);
                texture2dOrigin.LoadImage(bytes as byte[]);
            }
            else
            {
                var bytes = await ObservableUnityWebRequest.GetBytesAsObservable(url);
                texture2dOrigin.LoadImage(bytes as byte[]);
            }
        }
        catch (Exception e)
        {
            onError?.Invoke(e);
        }

        return texture2dOrigin;
    }
}