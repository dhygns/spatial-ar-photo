using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//
using protocol = CoSpatialProtocol;

public class CoSpatial
{
    //SingleTon Object
    static CoSpatial _instance = null;
    static CoSpatial instance
    {
        get
        {
            if (_instance == null) _instance = new CoSpatial();
            return _instance;
        }
    }

    //interface for singleton

    //create server
    static public void CreateServer()
    {
        instance.create(Host.server);
    }

    //create client
    static public void CreateClient(string ip)
    {
        instance.create(Host.client, ip);
    }

    //getter for network object
    static public CoSpatialHost Network
    {
        get { return instance.getNetwork(); }
    }


    private CoSpatialHost network = null;
    public enum Host { client, server };

    //create host
    public void create(Host host, string ip = "")
    {
        if (network != null) return;

        switch (host)
        {
            case Host.client: network = new CoSpatialClient(ip); break;
            case Host.server: network = new CoSpatialServer(); break;
            default: Debug.Log("Unkown host type"); break;
        }

        registHandlers();
    }

    Texture2D tex = null;
    byte[] pixels = null;
    int maxstep = 0;
    int refcount = 0;
    private void registHandlers()
    {

        //When host gets get image
        network.RegisterHandler(protocol.Type.GetImage, (netMsg) =>
        {
            refcount++;
            protocol.GetImage msg = netMsg.ReadMessage<protocol.GetImage>();
            Debug.Log("Get Image : " + refcount + "/" + msg.maxstep);
            if (maxstep == 0)
            {
                maxstep = msg.maxstep;
                pixels = new byte[msg.width * msg.height * 4];
            }

            for (int i = 0; i < msg.pixels.Length; i++)
            {
                pixels[msg.step * 1024 + i] = msg.pixels[i];
            }

            if (refcount == msg.maxstep)
            {
                Debug.Log("Download Compeleted");
                tex = new Texture2D(msg.width, msg.height, TextureFormat.RGBA32, false);
                tex.LoadRawTextureData(pixels);
                tex.Apply();
                pixels = null;
                maxstep = 0; refcount = 0;
                ImageUIObjectsManager.SetTexture(tex);

            }

        });
    }

    public CoSpatialHost getNetwork()
    {
        return network;
    }
}
