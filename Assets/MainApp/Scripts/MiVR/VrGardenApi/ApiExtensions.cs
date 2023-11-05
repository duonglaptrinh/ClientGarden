using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace VrGardenApi
{
    public static class ApiExtensions
    {
        public static VrgApiException ToVrgApiException(this UnityWebRequestException ex)
        {
            ErrorResponse errorResponse;
            try
            {
                errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(ex.Text);
            }
            catch
            {
                errorResponse = new ErrorResponse() { StatusCode = ex.ResponseCode, Message = ex.Message, Error = ex.Error };
            }

            return new VrgApiException(ex.Message, errorResponse);
        }

        public static async Task<Response> Post<Request, Response>(string uri, Request request, string accessToken = null, CancellationToken cancellationToken = default, Action<VrgApiException> OnError = null)
        {
            using var wr = CreatePostWebRequest(uri, accessToken);
            var data = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
            wr.uploadHandler = new UploadHandlerRaw(data);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                VrgApiException exception = ex.ToVrgApiException();
                OnError?.Invoke(exception);
                //DebugExtension.Log(JsonConvert.SerializeObject(exception));
                throw exception;
            }
            return JsonConvert.DeserializeObject<Response>(wr.downloadHandler.text);
        }

        public static UnityWebRequest CreatePostWebRequest(string uri, string accessToken = null)
        {
            var wr = UnityWebRequest.Post(uri, string.Empty);
            wr.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                wr.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            }

            return wr;
        }
        public static UnityWebRequest CreatePostWebRequest(string uri, WWWForm form, string accessToken = null)
        {
            var wr = UnityWebRequest.Post(uri, form);
            if (!string.IsNullOrEmpty(accessToken))
            {
                wr.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            }

            return wr;
        }

        public static UnityWebRequest CreateGetWebRequest(string uri, string accessToken = null)
        {
            var wr = UnityWebRequest.Get(uri);
            DebugExtension.Log(uri);
            wr.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                wr.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            }

            return wr;
        }

        public static UnityWebRequest CreatePutWebRequest(string uri, string accessToken = null)
        {
            var wr = UnityWebRequest.Put(uri, string.Empty);
            wr.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                wr.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            }

            return wr;
        }
        public static UnityWebRequest CreatePutWebRequest(string uri, WWWForm form, string accessToken = null)
        {
            UnityWebRequest wr = UnityWebRequest.Post(uri,form);
            wr.method = "PUT";

            if (!string.IsNullOrEmpty(accessToken))
            {
                wr.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            }

            return wr;
        }
        public static UnityWebRequest CreateDeleteWebRequest(string uri, string accessToken = null)
        {
            var wr = UnityWebRequest.Delete(uri);
            wr.downloadHandler = new DownloadHandlerBuffer();

            wr.SetRequestHeader("Content-Type", "application/json");
            if (!string.IsNullOrEmpty(accessToken))
            {
                wr.SetRequestHeader("Authorization", $"Bearer {accessToken}");
            }

            return wr;
        }

        public static async UniTask<Texture2D> LoadImageByUrl(string uri, CancellationToken cancellationToken = default)
        {
            DebugExtension.Log(uri);
#if UNITY_EDITOR
            using (HttpClient httpClient = new HttpClient())
            {
                // Set headers (optional)
                httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(uri, cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        byte[] content = await response.Content.ReadAsByteArrayAsync();
                        DebugExtension.Log($"File downloaded successfully. {content.Length}");
                        Texture2D texture = new Texture2D(1, 1);
                        texture.LoadImage(content);

                        return texture;
                    }
                    else
                    {
                        DebugExtension.LogError("Failed to download file. Status code: " + response.StatusCode);
                    }
                }
                catch (Exception e)
                {
                    DebugExtension.LogError("Error: " + e.Message);
                }
            }
#else
            var wr = UnityWebRequestTexture.GetTexture(uri);
            //wr.SetRequestHeader("Accept-Encoding", "gzip");
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
                Texture2D texture = DownloadHandlerTexture.GetContent(wr);
                if (texture != null)
                {
                    return texture;
                }
            }
            catch (UnityWebRequestException ex)
            {
                throw ex.ToVrgApiException();
            }
#endif
            return null;
        }
    }
}
