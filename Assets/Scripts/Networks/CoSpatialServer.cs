using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CoSpatialServer
{
    public CoSpatialServer()
    {
    }

    ~CoSpatialServer()
    {

    }

    public void create() {
        NetworkServer.Listen(8080);
        NetworkServer.RegisterHandler(MsgType.Connect, (netMsg) => {
            Debug.Log("Connected");
        });
    }


    //singleton interface 
    static private CoSpatialServer _instance = null;
    static private CoSpatialServer instance
    {
        get
        {
            if (_instance == null) _instance = new CoSpatialServer();
            return _instance;
        }
    }

    static public void Create()
    {
        instance.create();
    }

    static public string GetIP() 
    {
        return Network.player.ipAddress;
    }
}
