using Newtonsoft.Json;
using SyncRoom.Schemas;

public class UserVisitorData : Visitor
{
    public UserData Data;
    public static UserVisitorData Convert(Visitor visitor)
    {
        UserVisitorData data = new UserVisitorData();
        data.userData = visitor.userData;
        data.sessionId = visitor.sessionId;
        data.userId = visitor.userId;
        data.state = visitor.state;
        data.name = visitor.name;
        data.Data = JsonConvert.DeserializeObject<UserData>(visitor.userData);
        return data;
    }

    public class UserData
    {
        public string themeColor;
    }
}
