using UnityEngine.Networking;
using System.Threading;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;

namespace VrGardenApi
{
    public class UserApi
    {
        private string _baseUrl;
        private IApiTokenManager _tokenManager;

        public UserApi(string baseUrl, IApiTokenManager tokenManager)
        {
            _baseUrl = baseUrl;
            _tokenManager = tokenManager;
        }

        public async UniTask<CreateUserResponse> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            return await ApiExtensions.Post<CreateUserRequest, CreateUserResponse>(_baseUrl, request);
        }

        internal async UniTask<LoginResponse> LoginAsync(LoginRequest request, Action<VrgApiException> OnError, CancellationToken cancellationToken = default)
        {
            return await ApiExtensions.Post<LoginRequest, LoginResponse>($"{_baseUrl}/login", request, null, cancellationToken, OnError);
        }

        public async UniTask LogoutAsync(CancellationToken cancellationToken = default)
        {
            using var wr = ApiExtensions.CreatePostWebRequest($"{_baseUrl}/logout", _tokenManager.AccessToken);
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                throw ex.ToVrgApiException();
            }
        }

        public async UniTask<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            return await ApiExtensions.Post<RefreshTokenRequest, RefreshTokenResponse>($"{_baseUrl}/refreshToken", request, null, cancellationToken);
        }

        public async UniTask<GetUsersResponse> GetUsersAsync(int skip, int take, string orderBy, string userName, CancellationToken cancellationToken = default)
        {
            var uriBuilder = new UriBuilder(_baseUrl);
            var query = UtilityWeb.ParseQueryStringEmpty();// System.Web.HttpUtility.ParseQueryString(string.Empty);

            query["skip"] = skip.ToString();
            query["take"] = take.ToString();
            if (!string.IsNullOrEmpty(orderBy))
                query["orderBy"] = orderBy;
            if (!string.IsNullOrEmpty(userName))
                query["userName"] = userName;

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

            return JsonConvert.DeserializeObject<GetUsersResponse>(wr.downloadHandler.text);
        }

        public async UniTask<GetUserResponse> GetUserAsync(long userId, CancellationToken cancellationToken = default)
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

            return JsonConvert.DeserializeObject<GetUserResponse>(wr.downloadHandler.text);
        }

        public async UniTask<UpdateUserResponse> UpdateUserAsync(long userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            using var wr = ApiExtensions.CreatePutWebRequest($"{_baseUrl}/{userId}", _tokenManager.AccessToken);
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

            return JsonConvert.DeserializeObject<UpdateUserResponse>(wr.downloadHandler.text);
        }

        public async UniTask<DeleteUserResponse> DeleteUserAsync(long userId, CancellationToken cancellationToken = default)
        {
            using var wr = ApiExtensions.CreateDeleteWebRequest($"{_baseUrl}/{userId}", _tokenManager.AccessToken);
            try
            {
                await wr.SendWebRequest().WithCancellation(cancellationToken);
            }
            catch (UnityWebRequestException ex)
            {
                throw ex.ToVrgApiException();
            }

            return JsonConvert.DeserializeObject<DeleteUserResponse>(wr.downloadHandler.text);
        }

    }

    public class CreateUserRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("visitor")]
        public UserInfo Visitor { get; set; }
    }

    public enum Gender
    {
        Male = 0,
        Female
    }

    public class UserInfo
    {
        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("birthDate")]
        public DateTime BirthDate { get; set; }

        [JsonProperty("gender")]
        public Gender Gender { get; set; }

    }

    public class CreateUserResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("visitor")]
        public VisitorInfo[] Visitor { get; set; }
    }

    public class VisitorInfo
    {
        [JsonProperty("userId")]
        public long UserId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("birthday")]
        public DateTime BirthDay { get; set; }

        [JsonProperty("gender")]
        public Gender Gender { get; set; }
    }
    public class LoginRequest
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonProperty("credential")]
        public TurnServerCredential TurnServerCredential { get; set; }
    }

    public class TurnServerCredential
    {
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
        [JsonProperty("uris")]
        public string[] URIs;
    }

    public class RefreshTokenRequest
    {
        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
    }

    public class RefreshTokenResponse
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; }
    }

    public class GetUsersResponse
    {

    }

    public class GetUserResponse
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("visitor")]
        public VisitorInfo Visitor { get; set; }
    }

    public class UpdateUserRequest
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("type")]
        public int Type { get; set; }
        [JsonProperty("visitor")]
        public VisitorInfo[] Visitor { get; set; }
    }

    public class UpdateUserResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public class DeleteUserResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}

