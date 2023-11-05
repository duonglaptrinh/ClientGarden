using static VrGardenApi.RoomApi.GetRoomsResponse;
using System.Collections.Generic;
using VrGardenApi;
using Newtonsoft.Json;

public static class RuntimeData
{
    public static string RoomID { get; set; }
#if UNITY_EDITOR
        = "20";
#else
        = "1";
#endif
    public static int RoomIDNumber => int.Parse(RoomID);
    public static string AssetToken { get; set; }
    public static List<RoomInfo> Rooms { get; set; }
    public static RoomInfo CurrentRooms { get; set; }
    public static UserDataJWT JWTUser { get; set; } //Data User convert from AccessToken
    public static GetUserResponse User { get; set; } //Data User Get By API GetUserId
    public static UserVisitorData CurrentUser;
}

public class UserDataJWT
{
    [JsonProperty("id")]
    public long Id { get; set; }
    [JsonProperty("username")]
    public string Username { get; set; }
    [JsonProperty("type")]
    public int Type { get; set; }
    [JsonProperty("iat")]
    public int Iat { get; set; }
    [JsonProperty("exp")]
    public int Exp { get; set; }
}
