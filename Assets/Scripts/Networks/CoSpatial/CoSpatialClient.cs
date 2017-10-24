using UnityEngine.Networking;
using UnityEngine;

public class CoSpatialClient : CoSpatialHost
{

    private NetworkClient client;

    public CoSpatialClient(string IPAddress)
    {
        Debug.Log("Co-Spatial Client Created");
        client = new NetworkClient();
        client.Connect(IPAddress, 8080);
    }

    ~CoSpatialClient()
    {
        client = null;
    }

    public override void RegisterHandler(short msgType, NetworkMessageDelegate handler)
    {
        this.client.RegisterHandler(msgType, handler);
    }

    public override void Send(short msgType, MessageBase msg)
    {
        this.client.Send(msgType, msg);
    }
}
