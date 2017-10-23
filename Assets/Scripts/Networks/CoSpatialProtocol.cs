using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CoSpatialProtocol {
    
    public class Type : MsgType {
        static public short Connected = MsgType.Connect;
    }

    public class DataConnected : MessageBase {
        
    }
}
