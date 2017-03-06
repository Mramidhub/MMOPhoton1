﻿using Photon.SocketServer;
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
        public int idClient = 0;
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
                case (byte)OperationCode.EnterInGame:
                    InGameEntering(operationRequest, sendParameters);
                    break;
                case (byte)OperationCode.LoadAnotherPlayers:
                    AnotherPlayersLoading(operationRequest, sendParameters);
                    break;
                case (byte)OperationCode.Move:
                    Move(operationRequest, sendParameters);
                    break;
                default:
                    break;
            }
        }

        #region RequestsHandlers
        void InGameEntering(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // Позиция.
            position = new Vector3DPosition((float)operationRequest.Parameters[(byte)PropertiesCode.posX],
                                            (float)operationRequest.Parameters[(byte)PropertiesCode.posY],
                                            (float)operationRequest.Parameters[(byte)PropertiesCode.posZ]);

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

        void AnotherPlayersLoading(OperationRequest operationRequest, SendParameters sendParameters)
        {
            for (int a = 0; a < Server.inst.allClients.Count; a++)
            {
                var client = Server.inst.allClients[a];

                OperationResponse response = new OperationResponse(operationRequest.OperationCode);

                response.Parameters = new Dictionary<byte, object> {
                        { (byte)PropertiesCode.posX,  client.position.X},
                        { (byte)PropertiesCode.posY,  client.position.Y},
                        { (byte)PropertiesCode.posZ,  client.position.Z},
                        { (byte)PropertiesCode.idClient, client.idClient }
                        };

                SendOperationResponse(response, sendParameters);
            }
        }

        void Move(OperationRequest operationRequest, SendParameters sendParameters)
        {
            position = new Vector3DPosition((float)operationRequest.Parameters[(byte)PropertiesCode.posX],
                                            (float)operationRequest.Parameters[(byte)PropertiesCode.posY],
                                            (float)operationRequest.Parameters[(byte)PropertiesCode.posZ]);

            // Log.Debug(operationRequest.Parameters[(byte)PropertiesCode.posX] + " " + operationRequest.Parameters[(byte)PropertiesCode.posY] + " " + operationRequest.Parameters[(byte)PropertiesCode.posZ]);
            // Log.Debug(newMove.X + " " + newMove.Y + " " + newMove.Z);

            var eventDataMove = new EventData((byte)EventCode.Move);

            eventDataMove.Parameters = new Dictionary<byte, object> {
                        { (byte)PropertiesCode.posX,  position.X},
                        { (byte)PropertiesCode.posY,  position.Y},
                        { (byte)PropertiesCode.posZ,  position.Z},
                        { (byte)PropertiesCode.idClient, idClient }
                    };

            // Отправляем событие все, кроме вызвающего.
            eventDataMove.SendTo(Server.inst.AllBeyondId(idClient), sendParameters);
        }

        #endregion
    }
}

