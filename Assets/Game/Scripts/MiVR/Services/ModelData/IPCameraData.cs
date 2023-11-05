using System;

[Serializable]
public class IPCameraData
{
    public string name;
    public string ip;
    public string password;
}

[Serializable]
public class ListIPCamera
{
    public IPCameraData[] listCameras;
}
