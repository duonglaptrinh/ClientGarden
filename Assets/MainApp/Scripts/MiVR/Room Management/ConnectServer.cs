using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using jp.co.mirabo.Application.RoomManagement;
using static jp.co.mirabo.Application.RoomManagement.RoomConfig;
using VrGardenApi;
using static VrGardenApi.SaveImageApi;

public class ConnectServer : SingletonMonoBehaviour<ConnectServer>
{
    //public jp.co.mirabo.Application.RoomManagement.SingletonMonoBehaviour
    public static Action OnStartLogin;
    public static Action OnLoginDone;
    public static Action<bool> OnCheckPassRoom;
    public static Action OnJoinRoomDone;

    private VrGardenApi.VrGardenApi api;
    public TurnServerCredential TurnServerCredential => api != null ? api.TurnServerCredential : null;
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public async void StartLogin(string userName, string password, Action<VrgApiException> OnError)
    {
        OnStartLogin?.Invoke();
        api = new VrGardenApi.VrGardenApi(GameContext.LinkServerAPI);

        //Login
        await api.LoginAsync(userName, password, OnError);

        string json = Base64Encode(api.AccessToken);
        DebugExtension.Log(json);
        RuntimeData.JWTUser = JsonConvert.DeserializeObject<UserDataJWT>(json);

        DebugExtension.Log(api.AccessToken);
        RuntimeData.AssetToken = api.AccessToken;

        //Get User theo UserId
        var getUser = await api.UserApi.GetUserAsync(RuntimeData.JWTUser.Id);
        RuntimeData.User = getUser;
        DebugExtension.Log(JsonConvert.SerializeObject(getUser));
        OnLoginDone?.Invoke();
    }
    public async void CheckRoom(string roomId, string passJoinRoom)
    {
        DebugExtension.Log("CHECK ROOM ID = " + roomId);// + "   PASSS = " + passJoinRoom);
        //Get list Room
        var getRooms = await api.RoomApi.GetRoomsAsync(0, 100, null);
        RuntimeData.Rooms = getRooms.Rooms;
        bool isCheckPassword = false;
        foreach (var item in getRooms.Rooms)
        {
            //DebugExtension.Log(JsonConvert.SerializeObject(item));
            //{"id":1,"name":"defaultRoom1","password":"2209","visitorNumber":10}
            if (roomId == item.Id.ToString())
            {
                isCheckPassword = passJoinRoom.Equals(item.Password);
                break;
            }
        }
        OnCheckPassRoom?.Invoke(isCheckPassword);
    }
    public async void GetJsonTemplate(string roomId, Action<string> OnloadDone)
    {
        DebugExtension.Log("GetJsonTemplate ROOM ID = " + roomId);
        //Get list Room
        var jsonResponse = await api.RoomApi.GetJsonTemplateRoomsAsync(roomId, 0, 100, null);
        OnloadDone?.Invoke(jsonResponse.GameData);
    }
    public async void GetListImage(Action<GetJsonListImageResponse> OnloadDone)
    {
        //Get list Room
        var jsonResponse = await api.SaveImageApi.GetListImageScreenShotAsync(0, 100, null);
        OnloadDone?.Invoke(jsonResponse);
    }
    public async void GetImageById(int id, Action<GetJsonImageResponse> OnloadDone)
    {
        //Get list Room
        var texture = await api.SaveImageApi.GetImageByIdAsync(id, 0, 100, null);
        OnloadDone?.Invoke(texture);
    }
    public async void LoadImageByUrl(string url, Action<Texture2D> OnloadDone)
    {
        //Get list Room
        var texture = await api.SaveImageApi.LoadImageByUrlAsync(url, 0, 100, null);
        OnloadDone?.Invoke(texture);
    }
    public async void UploadImage(Texture2D texture, Action<UploadImageResponse> OnloadDone)
    {
        //Get list Room
        var response = await api.SaveImageApi.UploadImageAsync(texture);
        OnloadDone?.Invoke(response);
    }
    public async void DeleteImageById(int id, Action<DeleteImageByIdResponse> OnloadDone)
    {
        //Get list Room
        var response = await api.SaveImageApi.DeleteImageByIdAsync(id);
        OnloadDone?.Invoke(response);
    }

    #region New View

    public async void GetListImageView(int roomId, int plan_id, TypeAPISaveImage type, Action<GetJsonListImageViewResponse> OnloadDone)
    {
        var jsonResponse = await api.SaveImageApi.GetListImageViewAsync(roomId,plan_id, type, 0, 100, null);
        OnloadDone?.Invoke(jsonResponse);
    }
    public async void GetImageByIdView(int id, Action<GetJsonImageViewResponse> OnloadDone)
    {
        var texture = await api.SaveImageApi.GetImageByIdViewAsync(id, 0, 100, null);
        OnloadDone?.Invoke(texture);
    }
    public async void UploadImageView(int roomId, int plan_id, TypeAPISaveImage type, string location, Texture2D texture, Action<UploadImageViewResponse> OnloadDone)
    {
        var response = await api.SaveImageApi.UploadImageViewAsync(roomId, plan_id, type, location, texture);
        OnloadDone?.Invoke(response);
    }
    public async void UpdateImageView(int id_Image, int roomId, int plan_id, TypeAPISaveImage type, string location, Texture2D texture, Action<UploadImageViewResponse> OnloadDone)
    {
        var response = await api.SaveImageApi.UpdatePutImageViewAsync(id_Image, roomId, plan_id, type, location, texture);
        OnloadDone?.Invoke(response);
    }
    public async void DeleteImageByIdView(string url_id, Action<DeleteImageByIdViewResponse> OnloadDone)
    {
        var response = await api.SaveImageApi.DeleteImageByIdViewAsync(url_id);
        OnloadDone?.Invoke(response);
    }

    #endregion
    public void JoinRoom(string roomID, string pass)
    {
        DebugExtension.Log("JOIN ROOOM -------  ROOM ID = " + roomID);// + "   PASSS = " + pass);
        RoomManager.OnGameRoomReady = OnGameRoomReady;
        Dictionary<string, object> options = new Dictionary<string, object>();

        //Add Visistor
        options.Add("userId", RuntimeData.User.Visitor.UserId);
        options.Add("name", RuntimeData.User.Visitor.Name);
        options.Add("email", RuntimeData.User.Visitor.Email);
        options.Add("birthday", RuntimeData.User.Visitor.BirthDay);
        options.Add("gender", RuntimeData.User.Visitor.Gender);

        //Add room info
        options.Add("roomName", "Room-" + roomID);
        options.Add("roomPassword", pass);
        options.Add("userToken", RuntimeData.AssetToken);
        //join sync room

        ////join chat room
        //RoomManager.QuickConnect("chat", null);
        //RoomManager.QuickConnect("test-room", options);
        RoomManager.Instance.QuickConnect("Room-" + roomID, options);
    }

    private void OnGameRoomReady()
    {
        DebugExtension.Log("OnGameRoomReady");
        OnJoinRoomDone?.Invoke();
    }
    public static string Base64Encode(string token)
    {
        var parts = token.Split('.');
        if (parts.Length > 2)
        {
            var decode = parts[1];
            var padLength = 4 - decode.Length % 4;
            if (padLength < 4)
            {
                decode += new string('=', padLength);
            }
            var bytes = System.Convert.FromBase64String(decode);
            var userInfo = System.Text.ASCIIEncoding.ASCII.GetString(bytes);
            return userInfo;
        }
        return "";
    }
}