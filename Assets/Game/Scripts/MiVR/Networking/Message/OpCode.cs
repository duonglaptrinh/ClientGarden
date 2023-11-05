
namespace Game.Networking.Message
{
    public enum MyMes : short
    {
        SendListCamera = 1,
        AddCameraInfo,
        RemoveCameraInfo,
        UpdateCameraInfo,
        GetListCamera,

        // realtime mode message
        GetCamerasAvailable,
        ChangeCameraInfo,
        ShootAImage,
        CancelShootImage,

        // server status
        SendServerStatus,

        // Content
        ServerChangeContent,
        ChangeContent,
        CheckCurrentContent,

        PingPong
    }
}