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
        // Logger.
        private readonly ILogger Log = LogManager.GetCurrentClassLogger();

        // id.
        public int idClient = 0;

        // Postion - rotation.
        Vector3DPosition position = new Vector3DPosition();
        QuaternionRotation rotation = new QuaternionRotation();

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
                    Log.Debug("another load");
                    break;
                case (byte)OperationCode.Move:
                    Move(operationRequest, sendParameters);
                    break;
                case (byte)OperationCode.ExitGame:
                    ExitGame(operationRequest, sendParameters);
                    break;
                case (byte)OperationCode.Login:
                    LoginHandler(operationRequest, sendParameters);
                    break;
                default:
                    break;
            }
        }

        #region RequestsHandlers

        void LoginHandler(OperationRequest operationRequest, SendParameters sendParameters)
        {
            string loginName = Convert.ToString(operationRequest.Parameters[(byte)PropertiesCode.login]);
            string password = Convert.ToString(operationRequest.Parameters[(byte)PropertiesCode.password]);

            if (Server.inst.database.Login(loginName, password))
            {
                OperationResponse response = new OperationResponse(operationRequest.OperationCode);
                Log.Debug("login " + idClient);
                response.Parameters = new Dictionary<byte, object> {{ (byte)ErrorCode.Login, "OK"} };

                SendOperationResponse(response, sendParameters);
            }
            else
            {
                OperationResponse response = new OperationResponse(operationRequest.OperationCode);
                response.Parameters = new Dictionary<byte, object> { { (byte)ErrorCode.Login, "NO" } };

                SendOperationResponse(response, sendParameters);
            }
        }

        void InGameEntering(OperationRequest operationRequest, SendParameters sendParameters)
        {
            // Позиция.

            float x = Convert.ToSingle(operationRequest.Parameters[(byte)PropertiesCode.posX]);
            float y = Convert.ToSingle(operationRequest.Parameters[(byte)PropertiesCode.posY]);
            float z = Convert.ToSingle(operationRequest.Parameters[(byte)PropertiesCode.posZ]);

            position = new Vector3DPosition(x, y, z);

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
            Log.Debug("another load0");
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

                Log.Debug("another load1");

                SendOperationResponse(response, sendParameters);
            }
        }

        void Move(OperationRequest operationRequest, SendParameters sendParameters)
        {
            float x = Convert.ToSingle(operationRequest.Parameters[(byte)PropertiesCode.posX]);
            float y = Convert.ToSingle(operationRequest.Parameters[(byte)PropertiesCode.posY]);
            float z = Convert.ToSingle(operationRequest.Parameters[(byte)PropertiesCode.posZ]);

            float rotX = Convert.ToSingle(operationRequest.Parameters[(byte)PropertiesCode.rotX]);
            float rotY = Convert.ToSingle(operationRequest.Parameters[(byte)PropertiesCode.rotY]);
            float rotZ = Convert.ToSingle(operationRequest.Parameters[(byte)PropertiesCode.rotZ]);
            float rotW = Convert.ToSingle(operationRequest.Parameters[(byte)PropertiesCode.rotW]);

            position = new Vector3DPosition(x, y, z);
            rotation = new QuaternionRotation(rotX, rotY, rotZ, rotW);

            var eventDataMove = new EventData((byte)EventCode.Move);

            eventDataMove.Parameters = new Dictionary<byte, object> {
                        { (byte)PropertiesCode.posX,  position.X},
                        { (byte)PropertiesCode.posY,  position.Y},
                        { (byte)PropertiesCode.posZ,  position.Z},
                        { (byte)PropertiesCode.idClient, idClient},
                        { (byte)PropertiesCode.rotX,  rotation.X},
                        { (byte)PropertiesCode.rotY,  rotation.Y},
                        { (byte)PropertiesCode.rotZ,  rotation.Z},
                        { (byte)PropertiesCode.rotW,  rotation.W}
                    };

            sendParameters.Unreliable = false;

            // Отправляем событие все, кроме вызвающего.
            eventDataMove.SendTo(Server.inst.AllBeyondId(idClient), sendParameters);
        }

        void ExitGame(OperationRequest operationRequest, SendParameters sendParameters)
        {
            OperationResponse response = new OperationResponse(operationRequest.OperationCode);

            Log.Debug("id client " + idClient + " exit");

            SendOperationResponse(response, sendParameters);


            var eventData1 = new EventData((byte)EventCode.OtherPlayerExitGame);

            eventData1.Parameters = new Dictionary<byte, object> {{ (byte)PropertiesCode.idClient, idClient }};

            eventData1.SendTo(Server.inst.AllBeyondId(idClient), sendParameters);

            Log.Debug("clients " + Server.inst.allClients.Count);

            Server.inst.allClients.Remove(this);

            Log.Debug("clients " + Server.inst.allClients.Count);

        }

        #endregion
    }
}

