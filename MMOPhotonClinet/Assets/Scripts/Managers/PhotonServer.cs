using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using PhotonMMO.Common;
using System.Linq;

public class PhotonServer : MonoBehaviour, IPhotonPeerListener {

    // Адрес сервера и порт.
    private const string CONNECTION_STRING = "localhost:5055";
    // Имя приложения на сервере.
    private const string APP_NAME = "PhotonMMO";

    private static PhotonServer _instance;
    public static PhotonServer Instance
    {
        get { return _instance; }
    }

    private PhotonPeer PhotonPeer { get; set; }

    // Events.
    public UnityEvent InGameEnter = new UnityEvent();
    public UnityEvent GameExit = new UnityEvent();

    public float sendMoveDelay = 0.05f;


    void Awake()
    {
        if (Instance != null)
        {
            DestroyObject(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        // Запускаем приложение в фоновом потоке.
        Application.runInBackground = true;

        _instance = this;
    }

    void Start()
    {
        // При старте создаем нового пира. Указываем экземпляр IPhoonPeerListener и вид протокола.
        PhotonPeer = new PhotonPeer(this, ConnectionProtocol.Udp);
        // Коннектимся к севраку.
    }

    void Update()
    {
        if (PhotonPeer != null)
            PhotonPeer.Service();
    }

    void OnApplicationQuit()
    {
        // Когда приложение закрываеться - отключаемся от сервера.
        Disconnect();
    }

    private void Disconnect()
    {
        PhotonPeer.Disconnect();
    }

    public void Connect()
    {
        if (PhotonPeer != null)
            PhotonPeer.Connect(CONNECTION_STRING, APP_NAME);
    }

    public void DisconnectPeer()
    {
        if (PhotonPeer != null)
            PhotonPeer.Disconnect();
    }

    public void DebugReturn(DebugLevel level, string message)
    {

    }

    // Вызвается когда от сервера приходит операция. Ответ от сервера на запрос.
    public void OnOperationResponse(OperationResponse operationResponse)
    {
        switch (operationResponse.OperationCode)
        {
            case (byte)OperationCode.EnterInGame:
                InGameEnteringHandler(operationResponse);
                break;
            case (byte)OperationCode.LoadAnotherPlayers:
                OtherPlayerEntering(operationResponse);
                break;
            case (byte)OperationCode.ExitGame:
                ExitGameHandler(operationResponse);
                break;
            default:
                Debug.Log("Unknown OperationResponse:" + operationResponse.OperationCode);
                break;
        }
    }

    // Вызвается когда от сервера приходит ивент.
    public void OnEvent(EventData eventData)
    {
        switch (eventData.Code)
        {
            case (byte)EventCode.OtherPlayerEnterInGame:
                if (eventData.Parameters.ContainsKey(1))
                {
                    OtherPlayerEntering(eventData);
                }
                break;
            case (byte)EventCode.Move:
                MoveHandler(eventData);
                break;
            case (byte)EventCode.OtherPlayerExitGame:
                OtherPlayerExitGameHandler(eventData);
                break;
            default:
                Debug.Log("Unknown OperationResponse:" + eventData.Code);
                break;
        }
    }


    // Вызывется когда меняется статус подключения к серверу.
    public void OnStatusChanged(StatusCode statusCode)
    {
        switch (statusCode)
        {
            case StatusCode.Connect:
                Debug.Log("Connected to server!");
                EnterInGame();
                break;
            case StatusCode.Disconnect:
                Debug.Log("Disconnected from server!");
                break;
            case StatusCode.TimeoutDisconnect:
                Debug.Log("TimeoutDisconnected from server!");
                break;
            case StatusCode.DisconnectByServer:
                Debug.Log("DisconnectedByServer from server!");
                break;
            case StatusCode.DisconnectByServerUserLimit:
                Debug.Log("DisconnectedByLimit from server!");
                break;
            case StatusCode.DisconnectByServerLogic:
                Debug.Log("DisconnectedByLogic from server!");
                break;
            case StatusCode.EncryptionEstablished:
                break;
            case StatusCode.EncryptionFailedToEstablish:
                break;
            default:
                Debug.Log("Unknown status:" + statusCode.ToString());
                break;
        }
    }

    // Запросы к серваку.
    #region Operations
    public void EnterInGame()
    {
        PhotonPeer.OpCustom((byte)OperationCode.EnterInGame, new Dictionary<byte, object> {
            { (byte)PropertiesCode.posX, 0},
            { (byte)PropertiesCode.posY, 0},
            { (byte)PropertiesCode.posZ, 0}
        }, true);
    }

    public void ExitGame()
    {
        PhotonPeer.OpCustom((byte)OperationCode.ExitGame, new Dictionary<byte, object> { }, true);
    }

    public void LoadOtherPlayers()
    {
        PhotonPeer.OpCustom((byte)OperationCode.LoadAnotherPlayers, new Dictionary<byte, object> { { 1, "OtherPlayersLoaded" } }, true);
    }

    public void SendLocalPlayerMove()
    {
        sendMoveDelay -= Time.deltaTime;

        if (sendMoveDelay > 0)
            return;

        sendMoveDelay = 0.05f;

        Dictionary<byte, object> properies = new Dictionary<byte, object> {

            { (byte)PropertiesCode.posX, PlayersManager.Instance.localPlayer.transform.position.x},
            { (byte)PropertiesCode.posY, PlayersManager.Instance.localPlayer.transform.position.y},
            { (byte)PropertiesCode.posZ, PlayersManager.Instance.localPlayer.transform.position.z}
        };

        PhotonPeer.OpCustom((byte)OperationCode.Move, properies, false);
    }
    #endregion

    // Обработчики ответов от сервера и событий.
    #region RequestsAndEventsHadlers

    void InGameEnteringHandler(OperationResponse operationResponse)
    {
        Vector3 position = new Vector3((float)operationResponse.Parameters[(byte)PropertiesCode.posX],
                                       (float)operationResponse.Parameters[(byte)PropertiesCode.posY],
                                       (float)operationResponse.Parameters[(byte)PropertiesCode.posZ]);

        PlayersManager.Instance.InstLocalPlayer(position, (int)operationResponse.Parameters[(byte)PropertiesCode.idClient]);

        InGameEnter.Invoke();

        LoadOtherPlayers();
    }

    void OtherPlayerEntering(EventData eventData)
    {

        if (PlayersManager.Instance.localPlayer.idClient == (int)eventData.Parameters[(byte)PropertiesCode.idClient])
            return;

        Vector3 position = new Vector3((float)eventData.Parameters[(byte)PropertiesCode.posX],
                                       (float)eventData.Parameters[(byte)PropertiesCode.posY],
                                       (float)eventData.Parameters[(byte)PropertiesCode.posZ]);

        PlayersManager.Instance.InstAnotherPlayer(position, (int)eventData.Parameters[(byte)PropertiesCode.idClient]);
    }

    void OtherPlayerEntering(OperationResponse operationResponse)
    {
        if (PlayersManager.Instance.localPlayer.idClient == Convert.ToInt32(operationResponse.Parameters[(byte)PropertiesCode.idClient]))
            return;

        Vector3 position = new Vector3((float)operationResponse.Parameters[(byte)PropertiesCode.posX],
                                       (float)operationResponse.Parameters[(byte)PropertiesCode.posY],
                                       (float)operationResponse.Parameters[(byte)PropertiesCode.posZ]);

        PlayersManager.Instance.InstAnotherPlayer(position, (int)operationResponse.Parameters[(byte)PropertiesCode.idClient]);
    }

    void MoveHandler(EventData eventData)
    {
        foreach (Player player in PlayersManager.Instance.players)
        {
            if (player.idClient == (int)eventData.Parameters[(byte)PropertiesCode.idClient])
            {
                // Debug.Log((float)eventData.Parameters[(byte)PropertiesCode.posX] + " " + (float)eventData.Parameters[(byte)PropertiesCode.posY] + " " + (float)eventData.Parameters[(byte)PropertiesCode.posZ]);
                Vector3 newPosition = new Vector3(
                    (float)eventData.Parameters[(byte)PropertiesCode.posX],
                    (float)eventData.Parameters[(byte)PropertiesCode.posY],
                    (float)eventData.Parameters[(byte)PropertiesCode.posZ]);

                player.MoveEtentity(newPosition);

                break;
            }

        }
    }

    void ExitGameHandler(OperationResponse operationResponse)
    {
        PlayersManager.Instance.DeleteAllPlayers();

        GameExit.Invoke();

        DisconnectPeer();
    }

    void OtherPlayerExitGameHandler(EventData eventData)
    {
        Destroy(PlayersManager.Instance.players.First(p=> p.idClient == Convert.ToInt32(eventData.Parameters[(byte)PropertiesCode.idClient])).gameObject);
    }

    #endregion
}
