using UnityEngine;
using UnityEngine.Networking;

public class CoSpatialServer : CoSpatialHost
{
    public CoSpatialServer()
    {
        Debug.Log("Co-Spatial Server Created");
        NetworkServer.Listen(8080);
    }

    ~CoSpatialServer()
    {

    }

    public override void RegisterHandler(short msgType, NetworkMessageDelegate handler)
    {
        NetworkServer.RegisterHandler(msgType, handler);
    }

    public override void Send(short msgType, MessageBase msg)
    {
        NetworkServer.SendToAll(msgType, msg);
    }
}
