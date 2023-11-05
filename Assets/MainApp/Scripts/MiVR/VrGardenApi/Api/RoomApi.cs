using UnityEngine.Networking;
using System.Threading;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace VrGardenApi
{
    public class RoomApi
    {
        private string _baseUrl;
        private string _jsonTemplateUrl;
        private IApiTokenManager _tokenManager;

        public RoomApi(string baseUrl, string jsonTemplateUrl, IApiTokenManager tokenManager)
        {
            _jsonTemplateUrl = jsonTemplateUrl;
            _baseUrl = baseUrl;
            _tokenManager = tokenManager;
        }

        public async UniTask<GetRoomsResponse> GetRoomsAsync(int skip, int take, string orderBy, CancellationToken cancellationToken = default)
        {
            var uriBuilder = new UriBuilder(_baseUrl);
            var query = UtilityWeb.ParseQueryStringEmpty();// System.Web.ParseQueryString(string.Empty);

            query["skip"] = skip.ToString();
            query["take"] = take.ToString();

            if (!string.IsNullOrEmpty(orderBy))
                query["orderBy"] = orderBy;

            uriBuilder.Query = query.ToString();

            using var wr = ApiExtensions.CreateGetWebRequest(_baseUrl, _tokenManager.AccessToken);
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                throw ex.ToVrgApiException();
            }

            return JsonConvert.DeserializeObject<GetRoomsResponse>(wr.downloadHandler.text);
        }
        public async UniTask<GetJsonTemplateRoomsResponse> GetJsonTemplateRoomsAsync(string roomId, int skip, int take, string orderBy, CancellationToken cancellationToken = default)
        {
            string url = string.Format(_jsonTemplateUrl, roomId);
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

            return JsonConvert.DeserializeObject<GetJsonTemplateRoomsResponse>(wr.downloadHandler.text);
            //return wr.downloadHandler.text;
        }
        public async UniTask<CreateRoomResponse> CreateRoomAsync(CreateRoomRequest request, CancellationToken cancellationToken = default)
        {
            return await ApiExtensions.Post<CreateRoomRequest, CreateRoomResponse>(_baseUrl, request, _tokenManager.AccessToken);
        }

        public async UniTask<UpdateRoomResponse> UpdateRoomAsync(long roomId, UpdateRoomRequest request, CancellationToken cancellationToken = default)
        {
            using var wr = ApiExtensions.CreatePutWebRequest($"{_baseUrl}/{roomId}", _tokenManager.AccessToken);
            var data = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request));
            wr.uploadHandler = new UploadHandlerRaw(data);
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                throw ex.ToVrgApiException();
            }

            return JsonConvert.DeserializeObject<UpdateRoomResponse>(wr.downloadHandler.text);
        }

        public async UniTask<DeleteRoomResponse> DeleteRoomAsync(long roomId, CancellationToken cancellationToken = default)
        {
            using var wr = ApiExtensions.CreateDeleteWebRequest($"{_baseUrl}/{roomId}", _tokenManager.AccessToken);
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                throw ex.ToVrgApiException();
            }

            return JsonConvert.DeserializeObject<DeleteRoomResponse>(wr.downloadHandler.text);
        }
        public class CreateRoomRequest
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }

            [JsonProperty("visitorNumber")]
            public int VisitorNumber { get; set; }
        }

        public class CreateRoomResponse
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }

            [JsonProperty("visitorNumber")]
            public int VisitorNumber { get; set; }
        }

        public class UpdateRoomRequest
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }

            [JsonProperty("visitorNumber")]
            public int VisitorNumber { get; set; }
        }

        public class UpdateRoomResponse
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }

            [JsonProperty("visitorNumber")]
            public int VisitorNumber { get; set; }
        }

        public class DeleteRoomResponse
        {
            [JsonProperty("id")]
            public long Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("password")]
            public string Password { get; set; }

            [JsonProperty("visitorNumber")]
            public int VisitorNumber { get; set; }
        }

        public class GetRoomsResponse
        {
            public class RoomInfo
            {
                [JsonProperty("id")]
                public long Id { get; set; }

                [JsonProperty("name")]
                public string Name { get; set; }

                [JsonProperty("password")]
                public string Password { get; set; }

                [JsonProperty("visitorNumber")]
                public int VisitorNumber { get; set; }
            }

            [JsonProperty("data")]
            public List<RoomInfo> Rooms { get; set; }

            [JsonProperty("total")]
            public int TotalRooms { get; set; }
        }
        public class GetJsonTemplateRoomsResponse
        {
            [JsonProperty("roomId")]
            public int RoomID { get; set; }

            [JsonProperty("gameData")]
            public string GameData { get; set; }
        }
    }
}
