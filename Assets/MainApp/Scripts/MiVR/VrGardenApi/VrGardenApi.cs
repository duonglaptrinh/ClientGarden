using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace VrGardenApi
{
    public class VrGardenApi : IApiTokenManager
    {
        private string _baseUrl;
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

        public TurnServerCredential TurnServerCredential { get; set; }

        public UserApi UserApi { get; private set; }
        public VisitorApi VisitorApi { get; private set; }
        public RoomApi RoomApi { get; private set; }
        public SaveImageApi SaveImageApi { get; private set; }

        public VrGardenApi(string baseUrl)
        {
            _baseUrl = baseUrl;
            DebugExtension.Log(baseUrl);
            DebugExtension.Log($"{_baseUrl}/{USERS_API}");

            UserApi = new UserApi($"{_baseUrl}/{USERS_API}", this);
            VisitorApi = new VisitorApi($"{_baseUrl}/{VISITOR_API}", this);
            RoomApi = new RoomApi($"{_baseUrl}/{ROOM_API}", $"{_baseUrl}/{ROOM_JSON_TEMPLATE_API}", this);
            SaveImageApi = new SaveImageApi($"{_baseUrl}/{IMAGE_POST_UPLOAD}",
                $"{_baseUrl}/{IMAGE_GET_LIST}",
                $"{_baseUrl}/{IMAGE_GET_BY_ID}",
                $"{_baseUrl}/{IMAGE_DELETE_BY_ID}",
                $"{_baseUrl}/{IMAGE_POST_VIEW_UPLOAD}",
                $"{_baseUrl}/{IMAGE_GET_VIEW_LIST}",
                $"{_baseUrl}/{IMAGE_GET_VIEW_BY_ID}",
                $"{_baseUrl}/{IMAGE_DELETE_VIEW_BY_ID}",
                this);
        }

        public async UniTask LoginAsync(string username, string password, Action<VrgApiException> OnError)
        {
            var response = await UserApi.LoginAsync(new LoginRequest { Username = username, Password = password }, OnError);
            RefreshToken = response.RefreshToken;
            AccessToken = response.AccessToken;
            TurnServerCredential = response.TurnServerCredential;

            DebugExtension.Log($"Login response : {JsonConvert.SerializeObject(response)}");
        }

        const string USERS_API = "api/users";
        const string VISITOR_API = "api/visitors";
        const string ROOM_API = "api/rooms?withGameData=0";
        const string ROOM_JSON_TEMPLATE_API = "api/room-data/{0}";
        const string IMAGE_POST_UPLOAD = "api/file";
        const string IMAGE_GET_LIST = "api/file";
        const string IMAGE_GET_BY_ID = "api/file/{0}";
        const string IMAGE_DELETE_BY_ID = "api/file/{0}";
        const string IMAGE_POST_VIEW_UPLOAD = "api/view";
        const string IMAGE_GET_VIEW_LIST = "api/view/room";
        const string IMAGE_GET_VIEW_BY_ID = "api/view/{0}";
        const string IMAGE_DELETE_VIEW_BY_ID = "api/view/{0}";

    }

    public interface IApiTokenManager
    {
        string AccessToken { get; }
        string RefreshToken { get; }
    }

    public class ErrorResponse
    {
        [JsonProperty("statusCode")]
        public long StatusCode;
        [JsonProperty("message")]
        public string Message;
        [JsonProperty("error")]
        public string Error;
    }

    public class VrgApiException : Exception
    {
        public ErrorResponse ErrorResponse { get; private set; }

        public VrgApiException(string message, ErrorResponse errorResponse) : base(message)
        {
            ErrorResponse = errorResponse;
        }
    }
}
