using UnityEngine.Networking;
using System.Threading;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Http;
using System.Security.Policy;

namespace VrGardenApi
{
    public enum TypeAPISaveImage
    {
        View = 1,
        ThumbDome = 2
    }
    public class SaveImageApi
    {
        //private string _baseUrl;
        private string _postImageUrl;
        private string _getListImageUrl;
        private string _getImageByIdUrl;
        private string _deleteImageByIdUrl;

        private string _postImageViewUrl;
        private string _updateImageByIdViewUrl;
        private string _getListImageViewUrl;
        private string _getImageByIdViewUrl;
        private string _deleteImageByIdViewUrl;
        private IApiTokenManager _tokenManager;

        public SaveImageApi(string postImageUrl, string getListImageUrl, string getImageByIdUrl, string deleteImageByIdUrl, string postImageNewUrl, string getListImageNewUrl, string getImageByIdNewUrl, string deleteImageByIdNewUrl, IApiTokenManager tokenManager)
        {
            _postImageUrl = postImageUrl;
            _getListImageUrl = getListImageUrl;
            _getImageByIdUrl = getImageByIdUrl;
            _deleteImageByIdUrl = deleteImageByIdUrl;

            _postImageViewUrl = postImageNewUrl;
            _updateImageByIdViewUrl = getImageByIdNewUrl;
            _getListImageViewUrl = getListImageNewUrl;
            _getImageByIdViewUrl = getImageByIdNewUrl;
            _deleteImageByIdViewUrl = deleteImageByIdNewUrl;
            //_baseUrl = baseUrl;
            _tokenManager = tokenManager;
        }
        #region End Save ScreenShot menu
        public async UniTask<GetJsonListImageResponse> GetListImageScreenShotAsync(int skip, int take, string orderBy, CancellationToken cancellationToken = default)
        {
            string url = string.Format(_getListImageUrl);
            var uriBuilder = new UriBuilder(url);
            var query = UtilityWeb.ParseQueryStringEmpty();// System.Web.ParseQueryString(string.Empty);

            query["skip"] = skip.ToString();
            query["take"] = take.ToString();

            if (!string.IsNullOrEmpty(orderBy))
                query["orderBy"] = orderBy;

            uriBuilder.Query = query.ToString();

            using var wr = ApiExtensions.CreateGetWebRequest(url, _tokenManager.AccessToken);
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                throw ex.ToVrgApiException();
            }

            return JsonConvert.DeserializeObject<GetJsonListImageResponse>(wr.downloadHandler.text);
            //return wr.downloadHandler.text;
        }
        public async UniTask<GetJsonImageResponse> GetImageByIdAsync(int id, int skip, int take, string orderBy, CancellationToken cancellationToken = default)
        {
            string url = string.Format(_getImageByIdUrl, id);
            var uriBuilder = new UriBuilder(url);
            var query = UtilityWeb.ParseQueryStringEmpty();// System.Web.ParseQueryString(string.Empty);

            query["skip"] = skip.ToString();
            query["take"] = take.ToString();

            if (!string.IsNullOrEmpty(orderBy))
                query["orderBy"] = orderBy;

            uriBuilder.Query = query.ToString();

            using var wr = ApiExtensions.CreateGetWebRequest(url, _tokenManager.AccessToken);
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                throw ex.ToVrgApiException();
            }
            //DebugExtension.LogError(wr.downloadHandler.text);
            return JsonConvert.DeserializeObject<GetJsonImageResponse>(wr.downloadHandler.text);
            //return wr.downloadHandler.text;
        }
        public async UniTask<Texture2D> LoadImageByUrlAsync(string url, int skip, int take, string orderBy, CancellationToken cancellationToken = default)
        {
            return await ApiExtensions.LoadImageByUrl(url);
        }
        //public async UniTask<CreateRoomResponse> CreateRoomAsync(CreateRoomRequest request, CancellationToken cancellationToken = default)
        //{
        //    return await ApiExtensions.Post<CreateRoomRequest, CreateRoomResponse>(_baseUrl, request, _tokenManager.AccessToken);
        //}

        public async UniTask<UploadImageResponse> UploadImageAsync(Texture2D request, CancellationToken cancellationToken = default)
        {
            var data = request.EncodeToPNG();

            WWWForm form = new WWWForm();
            form.AddField("accept", "*/*");
            form.AddField("Content-Type", "multipart/form-data");
            form.AddBinaryData("file", data, "image.png", "image/png");

            using var wr = ApiExtensions.CreatePostWebRequest($"{_postImageUrl}", form, _tokenManager.AccessToken);

            //wr.uploadHandler = new UploadHandlerRaw(data);
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                throw ex.ToVrgApiException();
            }

            return JsonConvert.DeserializeObject<UploadImageResponse>(wr.downloadHandler.text);
        }

        public async UniTask<DeleteImageByIdResponse> DeleteImageByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            string url = string.Format(_deleteImageByIdUrl, id);
            using var wr = ApiExtensions.CreateDeleteWebRequest($"{url}", _tokenManager.AccessToken);
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                throw ex.ToVrgApiException();
            }
            //DebugExtension.LogError(wr.downloadHandler.text);
            return JsonConvert.DeserializeObject<DeleteImageByIdResponse>(wr.downloadHandler.text);
        }
        #endregion End Save ScreenShot menu

        #region New View

        public async UniTask<GetJsonListImageViewResponse> GetListImageViewAsync(int roomId, int plan_id, TypeAPISaveImage type, int skip, int take, string orderBy, CancellationToken cancellationToken = default)
        {
            string url = string.Format(_getListImageViewUrl) + "?roomId=" + roomId + "&" + "type=" + ((int)type) + "&" + "domeId=" + plan_id;
            var uriBuilder = new UriBuilder(url);
            var query = UtilityWeb.ParseQueryStringEmpty();// System.Web.ParseQueryString(string.Empty);

            query["skip"] = skip.ToString();
            query["take"] = take.ToString();

            if (!string.IsNullOrEmpty(orderBy))
                query["orderBy"] = orderBy;

            uriBuilder.Query = query.ToString();

            using var wr = ApiExtensions.CreateGetWebRequest(url, _tokenManager.AccessToken);
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                throw ex.ToVrgApiException();
            }

            //DebugExtension.LogError(wr.downloadHandler.text);
            return JsonConvert.DeserializeObject<GetJsonListImageViewResponse>(wr.downloadHandler.text);
        }
        public async UniTask<GetJsonImageViewResponse> GetImageByIdViewAsync(int id, int skip, int take, string orderBy, CancellationToken cancellationToken = default)
        {
            string url = string.Format(_getImageByIdViewUrl, id);
            var uriBuilder = new UriBuilder(url);
            var query = UtilityWeb.ParseQueryStringEmpty();// System.Web.ParseQueryString(string.Empty);

            query["skip"] = skip.ToString();
            query["take"] = take.ToString();

            if (!string.IsNullOrEmpty(orderBy))
                query["orderBy"] = orderBy;

            uriBuilder.Query = query.ToString();

            using var wr = ApiExtensions.CreateGetWebRequest(url, _tokenManager.AccessToken);
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                Debug.LogError(ex.ToVrgApiException());
                return null;
            }
            //DebugExtension.LogError(wr.downloadHandler.text);
            return JsonConvert.DeserializeObject<GetJsonImageViewResponse>(wr.downloadHandler.text);
            //return wr.downloadHandler.text;
        }

        public async UniTask<UploadImageViewResponse> UploadImageViewAsync(int roomId, int plan_id, TypeAPISaveImage type, string location, Texture2D request, CancellationToken cancellationToken = default)
        {
            string uri = string.Format(_postImageViewUrl);
            var data = request.EncodeToPNG();

            WWWForm form = new WWWForm();
            form.AddField("roomId", roomId);
            form.AddField("domeId", plan_id);
            form.AddField("type", (int)type);
            form.AddField("location", location);
            form.AddField("accept", "*/*");
            form.AddField("Content-Type", "multipart/form-data");
            form.AddBinaryData("file", data, "image.png", "image/png");

            using var wr = ApiExtensions.CreatePostWebRequest(uri, form, _tokenManager.AccessToken);

            //wr.uploadHandler = new UploadHandlerRaw(data);
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                Debug.LogError(ex.ToVrgApiException());
                return null;
            }
            //DebugExtension.LogError(uri);
            //DebugExtension.LogError(wr.downloadHandler.text);
            return JsonConvert.DeserializeObject<UploadImageViewResponse>(wr.downloadHandler.text);
        }
        public async UniTask<UploadImageViewResponse> UpdatePutImageViewAsync(int id_image, int roomId, int plan_id, TypeAPISaveImage type, string location, Texture2D request, CancellationToken cancellationToken = default)
        {
            string uri = string.Format(_updateImageByIdViewUrl, id_image);
            var data = request.EncodeToPNG();

            WWWForm form = new WWWForm();
            form.AddField("roomId", roomId);
            form.AddField("domeId", plan_id);
            form.AddField("type", (int)type);
            form.AddField("location", location);
            form.AddField("accept", "*/*");
            form.AddField("Content-Type", "multipart/form-data");
            form.AddBinaryData("file", data, "image.png", "image/png");

            using var wr = ApiExtensions.CreatePutWebRequest(uri, form, _tokenManager.AccessToken);

            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                Debug.LogError(ex.ToVrgApiException());
                return null;
            }
            //DebugExtension.LogError(uri);
            //DebugExtension.LogError(wr.downloadHandler.text);
            if (string.IsNullOrEmpty(wr.downloadHandler.text))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<UploadImageViewResponse>(wr.downloadHandler.text);
        }

        public async UniTask<DeleteImageByIdViewResponse> DeleteImageByIdViewAsync(string url_id_image, CancellationToken cancellationToken = default)
        {
            string url = string.Format(_deleteImageByIdViewUrl, url_id_image);
            using var wr = ApiExtensions.CreateDeleteWebRequest($"{url}", _tokenManager.AccessToken);
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                Debug.LogError(ex.ToVrgApiException());
                return null;
            }
            return JsonConvert.DeserializeObject<DeleteImageByIdViewResponse>(wr.downloadHandler.text);
        }

        #endregion

        public class UploadImageResponse
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("userId")]
            public long UserId { get; set; }
            [JsonProperty("blob")]
            public string Blob { get; set; }

            [JsonProperty("mimeType")]
            public string MimeType { get; set; }

            [JsonProperty("status")]
            public int Status { get; set; }
        }

        public class DeleteImageByIdResponse
        {
            [JsonProperty("success")]
            public bool Status { get; set; }
        }

        public class GetJsonListImageResponse
        {
            [JsonProperty("listImage")]
            public List<GetJsonImageResponse> ListImage { get; set; }
        }
        public class GetJsonImageResponse
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("mimeType")]
            public string MimeType { get; set; }
            [JsonProperty("blob")]
            public string Blob { get; set; }
            [JsonProperty("url")]
            /// <summary>
            /// Url random and Encoding on server, exprice after 1 minute
            /// </summary>
            public string Url { get; set; }
        }

        #region New View

        public class GetJsonListImageViewResponse
        {
            [JsonProperty("listView")]
            public List<GetJsonImageViewResponse> listView { get; set; }
        }
        public class GetJsonImageViewResponse
        {
            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("mimeType")]
            public string MimeType { get; set; }
            [JsonProperty("location")]
            public string location { get; set; }
            [JsonProperty("blob")]
            public string Blob { get; set; }
            [JsonProperty("url")]
            /// <summary>
            /// Url random and Encoding on server, exprice after 1 minute
            /// </summary>
            public string Url { get; set; }
        }
        public class UploadImageViewResponse
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("userId")]
            public long UserId { get; set; }
            [JsonProperty("blob")]
            public string Blob { get; set; }

            [JsonProperty("mimeType")]
            public string MimeType { get; set; }

            [JsonProperty("status")]
            public int Status { get; set; }
        }
        public class DeleteImageByIdViewResponse
        {
            [JsonProperty("success")]
            public bool Status { get; set; }
        }
        #endregion
    }
}
