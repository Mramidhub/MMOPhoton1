using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhotonHostRuntimeInterfaces;
using ExitGames.Logging;
using PhotonMMO.Common;
using TestPhotonLib.Operations;

namespace PhotonMMOLib
{
    // Логика клиента на стороне сервера.
    public class UnityClient : PeerBase
    {
        // Логгер.
        private readonly ILogger Log = LogManager.GetCurrentClassLogger();

        // id
        int idClient = 0;
        Vector3DPosition position = new Vector3DPosition();

        public UnityClient(IRpcProtocol protocol, IPhotonPeer unmanagedPeer, int id) : base(protocol, unmanagedPeer)
        {
            Log.Debug("Client ip:" + unmanagedPeer.GetRemoteIP());
            idClient = id;
        }

        // Когда клиент отключается.
        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail)
        {
            Server.inst.allClients.Remove(this);
            Log.Debug("Client disconnected");
        }

        // Когда клиент что-либо присылает.
        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters)
        {
            switch (operationRequest.OperationCode)
            {
                // Если клиент прислал код операции 1, то отслаем ему ответ .
                case (byte)OperationCode.EnterInGame:
                    InGameEntering(operationRequest, sendParameters);
                    break;
                //case 2:
                // Если клиент прислал код операции 2, то отслаем ему ивент.
                    //if (operationRequest.Parameters.ContainsKey(1))
                    //{
                    //    Log.Debug("recv:" + operationRequest.Parameters[1]);
                    //    EventData eventData = new EventData(1);
                    //    eventData.Parameters = new Dictionary<byte, object> { { 1, "response message" } };
                    //    SendEvent(eventData, sendParameters);
                    //}
                    //break;
                default:
                    break;
            }
        }

        #region RequestsHandlers
        void InGameEntering(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // Позиция.
            Move newMove = new Move(Protocol, operationRequest);
            position = new Vector3DPosition(newMove.X, newMove.Y, newMove.Z);

            // Ответ вызывавшему.
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);

            Log.Debug("id client " + idClient);

            response.Parameters = new Dictionary<byte, object> {
                        { (byte)PropertiesCode.posX,  position.X},
                        { (byte)PropertiesCode.posY,  position.Y},
                        { (byte)PropertiesCode.posZ,  position.Z},
                        { (byte)PropertiesCode.idClient, idClient }
                        };

            SendOperationResponse(response, sendParameters);
            // Событие остальным.
            var clients = Server.inst.allClients;

            Log.Debug("clients count " + Server.inst.allClients.Count);

            var eventData1 = new EventData((byte)EventCode.OtherPlayerEnterInGame);

            eventData1.Parameters = new Dictionary<byte, object> {
                        { (byte)PropertiesCode.posX,  position.X},
                        { (byte)PropertiesCode.posY,  position.Y},
                        { (byte)PropertiesCode.posZ,  position.Z},
                        { (byte)PropertiesCode.idClient, idClient }
                    };

            eventData1.SendTo(Server.inst.allClients, sendParameters);
        }
        #endregion
    }
}

