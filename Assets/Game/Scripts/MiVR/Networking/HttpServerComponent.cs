using TWT.Networking.Server;
using UnityEngine;

public class HttpServerComponent : SingletonMonoBehavior<HttpServerComponent>
{
    VrSimpleHttpServer myServer;

    public void StartHttpServer(string path, int port)
    {
        DebugExtension.Log(path);
            
//Creating server with specified port
        if(myServer == null)
            myServer = new VrSimpleHttpServer(path, port);


//Now it is running:
        DebugExtension.Log("[VrSimpleHttpServer] Server is running on this port: " + myServer.Port.ToString());
    }
    
    private void OnDestroy()
    {
        myServer?.Stop();
        DebugExtension.Log("[VrSimpleHttpServer] Server stop");
    }
}