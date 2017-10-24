using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public abstract class CoSpatialHost
{

    public CoSpatialHost()
    {

    }

    ~CoSpatialHost()
    {

    }


    //interface
    public virtual void Create()
    {
        Debug.Log("Created CoSpatial");
    }

    public virtual void RegisterHandler(short msgType, NetworkMessageDelegate handler)
    {

    }

    public virtual void Send(short msgType, MessageBase msg)
    {
        
    }

    //IP Getter
    public string IP
    {
        get { return Network.player.ipAddress; }
    }

}
