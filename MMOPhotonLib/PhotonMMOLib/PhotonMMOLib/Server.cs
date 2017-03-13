using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net.Config;
using Photon.SocketServer;
using PhotonMMOLib.UniverseStructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PhotonMMOLib
{
    public class Server : ApplicationBase
    {
        public static Server inst;
        public DBManager database;

        // Получаем обьект логера. Через него будем писать лог.
        private readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public List<UnityClient> allClients = new List<UnityClient>();

        int lastClientsId = 0;


        // World.
        public Universe MainUniverse = new Universe();
        

        // Действия при старте сервера.
        protected override void Setup()
        {
            // Находим файл конфигурации логирования.
            var file = new FileInfo(Path.Combine(BinaryPath, "log4net.config"));
            // Если нашли, то применяем конфигурацию.
            if (file.Exists)
            {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(file);
            }

            if (inst == null)
                inst = this;

            database = new DBManager();
            database.DBInit();

            database.InitUniverse();

            Log.Debug("Server is setup!");

        }

        // Создает пир всякий раз когда к серверу подключаеться клиент. Каждому клиенту свой пир.
        protected override PeerBase CreatePeer(InitRequest initRequest)
        {
            // Создаем обьект unityclient для этого пира.
            lastClientsId++;
            var client = new UnityClient(initRequest.Protocol, initRequest.PhotonPeer, lastClientsId);
            allClients.Add(client);
            return client;
        }

        // Действия при отключении сервера.
        protected override void TearDown()
        {
            Log.Debug("Server was soped!");
        }

        // Выборка клиентов всех кроме какого то id.
        public List<UnityClient> AllBeyondId(int id)
        {
            var clients = new List<UnityClient>();

            foreach (UnityClient client in allClients)
            {
                if (client.idClient != id)
                {
                    clients.Add(client);
                }
            }

            return clients;
        }
    }
}
