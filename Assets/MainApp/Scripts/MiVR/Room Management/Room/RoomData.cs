using System;

namespace jp.co.mirabo.Application.RoomManagement
{
    [Serializable]
    public class RoomData
    {
        public int id;
        public string roomId;
        public string processId;
        public int clients;
        public int maxClients;
        public string name;
        public string createdAt;

        public RoomData() {}

        public RoomData(string roomId, string processId, int clients, int maxClients, string name, string createdAt)
        {
            this.roomId = roomId;
            this.processId = processId;
            this.clients = clients;
            this.maxClients = maxClients;
            this.name = name;
            this.createdAt = createdAt;
        }
    }
}