using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using PhotonMMO.Common;

namespace TestPhotonLib.Operations
{
    public class Move 
    {
        public Move(IRpcProtocol protocol, OperationRequest request)
        {

        }

        [DataMember(Code = (byte)PropertiesCode.posX)]
        public float X { get; set; }

        [DataMember(Code = (byte)PropertiesCode.posY)]
        public float Y { get; set; }

        [DataMember(Code = (byte)PropertiesCode.posZ)]
        public float Z { get; set; }
    }
}