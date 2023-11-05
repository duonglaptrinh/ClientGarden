using UnityEngine.Networking;
using System.Threading;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace VrGardenApi
{
    public class VisitorApi
    {
        private string _baseUrl;
        private IApiTokenManager _tokenManager;

        public VisitorApi(string baseUrl, IApiTokenManager tokenManager)
        {
            _baseUrl = baseUrl;
            _tokenManager = tokenManager;
        }

        public async UniTask<GetVisistorResponse> GetUserAsync(long userId, CancellationToken cancellationToken = default)
        {
            using var wr = ApiExtensions.CreateGetWebRequest($"{_baseUrl}/{userId}", _tokenManager.AccessToken);
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                throw ex.ToVrgApiException();
            }

            return JsonConvert.DeserializeObject<GetVisistorResponse>(wr.downloadHandler.text);
        }

        public async UniTask<GetVisitorsResponse> GetVisitorsAsync(int skip, int take, string orderBy, string email, CancellationToken cancellationToken = default)
        {
            var uriBuilder = new UriBuilder(_baseUrl);
            var query = UtilityWeb.ParseQueryStringEmpty();// System.Web.HttpUtility.ParseQueryString(string.Empty);

            query["skip"] = skip.ToString();
            query["take"] = take.ToString();
            if (!string.IsNullOrEmpty(orderBy))
                query["orderBy"] = orderBy;
            if (!string.IsNullOrEmpty(email))
                query["email"] = email;

            uriBuilder.Query = query.ToString();

            using var wr = ApiExtensions.CreateGetWebRequest(uriBuilder.Uri.AbsoluteUri, _tokenManager.AccessToken);
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                throw ex.ToVrgApiException();
            }

            return JsonConvert.DeserializeObject<GetVisitorsResponse>(wr.downloadHandler.text);
        }

        public class GetVisistorResponse
        {
            [JsonProperty("userId")]
            public long UserId { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("birthDate")]
            public DateTime BirthDate { get; set; }

            [JsonProperty("gender")]
            public Gender Gender { get; set; }

            [JsonProperty("user")]
            public UserInfo User { get; set; }

            public class UserInfo
            {
                [JsonProperty("userId")]
                public long Id { get; set; }

                [JsonProperty("username")]
                public string UserName { get; set; }
            }
        }

        public class GetVisitorsResponse
        {
            [JsonProperty("data")]
            public List<GetVisistorResponse> Visitors { get; set; }

            [JsonProperty("total")]
            public int Total { get; set; }
        }
    }
}
