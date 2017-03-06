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
            //X = (float)request.Parameters[(byte)PropertiesCode.posX];
            //Y = (float)request.Parameters[(byte)PropertiesCode.posY];
            //Z = (float)request.Parameters[(byte)PropertiesCode.posZ];
        }

        [DataMember(Code = (byte)PropertiesCode.posX)]
        public float X { get; set; }

        [DataMember(Code = (byte)PropertiesCode.posY)]
        public float Y { get; set; }

        [DataMember(Code = (byte)PropertiesCode.posZ)]
        public float Z { get; set; }
    }
}