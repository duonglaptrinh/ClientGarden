using jp.co.mirabo.Application.RoomManagement;
using Newtonsoft.Json;
using SyncRoom.Schemas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VrGardenApi
{
    public class ApiTests : MonoBehaviour
    {
        public Button btnLogin;
        public InputField inputPlayerName;
        public InputField inputPassword;
        public Text userName;

        string user => inputPlayerName.text;
        string pass => inputPassword.text;
        public void Login()
        {
            ConnectServer.Instance.StartLogin(user, "123456", null);
        }
        void OnLoginDone()
        {
            userName.text = "Login Success:  UserName = " + user + "\n";
            userName.gameObject.SetActive(true);
        }
        private void AddPlayer(Visitor player, bool isLocalPlayer)
        {
            userName.text += "Add Player: " + player.name + " ==> Coun Player = " + RoomManager.Instance.GameRoom.GetUsers().Count + "\n";
        }
        private async void Start()
        {
            ConnectServer.OnLoginDone += OnLoginDone;
            RoomManager.Instance.GameRoom.OnPublisherAddPlayer += AddPlayer;
            btnLogin.onClick.AddListener(Login);
            //var api = new VrGardenApi(@"https://webrtc.gld-lab.link:6789");

            /*
            var response = await api.UserApi.CreateUserAsync(new CreateUserRequest()
            {
                Username = "google1",
                Password = "12345678",
                Visitor = new UserInfo()
                {
                    Email = "google1@gmail.com",
                    BirthDate = System.DateTime.Now,
                    Gender = Gender.Male,
                    Name = "Google"
                }
            });

            DebugExtension.Log(JsonConvert.SerializeObject(response));
            */

            //await api.LoginAsync("test", "123456");
            //DebugExtension.Log(api.AccessToken);
            //VrGDataCommon.AssetToken = api.AccessToken; 

            //var getUser = await api.UserApi.GetUserAsync(1);
            //DebugExtension.Log(JsonConvert.SerializeObject(getUser));

            //var createRoom = await api.RoomApi.CreateRoomAsync(new RoomApi.CreateRoomRequest { Name = "test_room", Password = "12345678", VisitorNumber = 10 });

            //DebugExtension.LogError($"Create room : {createRoom.Id}");

            //var getRooms = await api.RoomApi.GetRoomsAsync(0, 100, null);
            //foreach (var item in getRooms.Rooms)
            //{
            //    DebugExtension.Log(JsonConvert.SerializeObject(item));
            //}

            //DebugExtension.LogError("Update room");
            //var updateRoom = await api.RoomApi.UpdateRoomAsync(createRoom.Id, new RoomApi.UpdateRoomRequest { Name = "updated_test_room", Password = "12341234", VisitorNumber = 8 });

            //DebugExtension.LogError("After update room");
            //getRooms = await api.RoomApi.GetRoomsAsync(0, 100, null);
            //foreach (var item in getRooms.Rooms)
            //{
            //    DebugExtension.Log(JsonConvert.SerializeObject(item));
            //}

            //DebugExtension.LogError($"Delete room {createRoom.Id}");
            //var deleteRoom = await api.RoomApi.DeleteRoomAsync(createRoom.Id);

            //DebugExtension.LogError("After delete room");
            //getRooms = await api.RoomApi.GetRoomsAsync(0, 100, null);
            //foreach (var item in getRooms.Rooms)
            //{
            //    DebugExtension.Log(JsonConvert.SerializeObject(item));
            //}

        }
    }
}
