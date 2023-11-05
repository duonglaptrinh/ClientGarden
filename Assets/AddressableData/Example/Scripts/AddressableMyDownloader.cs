using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
public class AddressableMyDownloader : MonoBehaviour
{
    Action<float> ProgressEvent;
    Action<bool> CompletionEvent;
    string labelDownload;
    private AsyncOperationHandle downloadHandle;

    public void Download(string label, Action<float> ProgressEvent = null, Action<bool> CompletionEvent = null)
    {
        this.ProgressEvent = ProgressEvent;
        this.CompletionEvent = CompletionEvent;
        this.labelDownload = label;
        StartCoroutine(DownloadColorTexture());
    }
    IEnumerator DownloadColorTexture()
    {
        downloadHandle = Addressables.DownloadDependenciesAsync(labelDownload, false);
        float progress = 0;

        while (downloadHandle.Status == AsyncOperationStatus.None)
        {
            float percentageComplete = downloadHandle.GetDownloadStatus().Percent;
            if (percentageComplete > progress * 1.1) // Report at most every 10% or so
            {
                progress = percentageComplete; // More accurate %
                ProgressEvent?.Invoke(progress);
            }
            yield return null;
        }

        CompletionEvent?.Invoke(downloadHandle.Status == AsyncOperationStatus.Succeeded);
        Addressables.Release(downloadHandle); //Release the operation handle
        Destroy(gameObject);
    }
}
