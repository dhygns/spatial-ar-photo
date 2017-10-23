using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class CoSpatialClient
{

    private NetworkClient client;

    private CoSpatialClient()
    {
        client = new NetworkClient();
    }

    ~CoSpatialClient()
    {
        client = null;
    }

    public void connect(string IPAddress)
    {
        //Connect to server
        client.Connect(IPAddress, 8080);
        //client.GetConnectionStats().
        //client.RegisterHandler();
    }


    //singleton interface 
    static private CoSpatialClient _instance = null;
    static private CoSpatialClient instance
    {
        get
        {
            if (_instance == null) _instance = new CoSpatialClient();
            return _instance;
        }
    }

    static public void Connect(string IPAddress)
    {
        instance.connect(IPAddress);
    }

}
