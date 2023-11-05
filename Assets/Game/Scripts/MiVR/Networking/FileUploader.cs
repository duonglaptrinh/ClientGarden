using System;
using System.Collections.Generic;
using System.IO;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace TWT.Networking
{
    public static class FileUploader
    {
        public static async UniTask<bool> UploadFileAsync(string uploadUrl, string filePath, string saveWithName, GameObject objectExecute = null)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            var data = File.ReadAllBytes(filePath);
            return await UploadFileAsync(uploadUrl, data, saveWithName, objectExecute);
        }
        
        public static async UniTask<bool> UploadFileAsync(string uploadUrl, byte[] data, string saveWithName, GameObject objectExecute = null)
        {
            var header = new Dictionary<string, string>()
            {
                {"filename", saveWithName}
            };
            try
            {
                if (objectExecute != null)
                {
                    await ObservableUnityWebRequest.PostAsObservable(uploadUrl, data, requestHeaders: header).TakeUntilDestroy(objectExecute);
                }
                else
                {
                    await ObservableUnityWebRequest.PostAsObservable(uploadUrl, data, requestHeaders: header);
                }

                return true;
            }
            catch (Exception e)
            {
                DebugExtension.LogError(e);
                return false;
            }
        }
    }
}