using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CoSpatialProtocol {
    
    public class Type : MsgType {
        //only client to server
        static public short Connected = MsgType.Connect;

        static public short GetImage = MsgType.Highest + 1;
    }



    public class GetImage : MessageBase {
        public int width, height;
        public int step, maxstep;
        public byte[] pixels;   //max 1024
    }
}
