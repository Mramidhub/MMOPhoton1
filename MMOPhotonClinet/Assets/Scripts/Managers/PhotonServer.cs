﻿using ExitGames.Client.Photon;
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
        Connect();
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
            case (byte)OperationCode.Login:
                LoginHandler(operationResponse);
                break;
            case (byte)OperationCode.Register:
                RegisterHandler(operationResponse);
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
    public void Register(string login, string password)
    {
        Debug.Log("register");
        PhotonPeer.OpCustom((byte)OperationCode.Register, new Dictionary<byte, object> {
            { (byte)PropertiesCode.login, login},
            { (byte)PropertiesCode.password, password}
        }, true);
    }
    public void Login(string login, string password)
    {
        Debug.Log("login");
        PhotonPeer.OpCustom((byte)OperationCode.Login, new Dictionary<byte, object> {
            { (byte)PropertiesCode.login, login},
            { (byte)PropertiesCode.password, password}
        }, true);
    }

    public void EnterInGame()
    {
        PhotonPeer.OpCustom((byte)OperationCode.EnterInGame, new Dictionary<byte, object> {
            { (byte)PropertiesCode.posX, 0},
            { (byte)PropertiesCode.posY, 0},
            { (byte)PropertiesCode.posZ, 0},
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
        Dictionary<byte, object> properies = new Dictionary<byte, object> {

            { (byte)PropertiesCode.posX, PlayersManager.Instance.localPlayer.transform.position.x},
            { (byte)PropertiesCode.posY, PlayersManager.Instance.localPlayer.transform.position.y},
            { (byte)PropertiesCode.posZ, PlayersManager.Instance.localPlayer.transform.position.z},
            { (byte)PropertiesCode.rotX, PlayersManager.Instance.localPlayer.transform.rotation.x},
            { (byte)PropertiesCode.rotY, PlayersManager.Instance.localPlayer.transform.rotation.y},
            { (byte)PropertiesCode.rotZ, PlayersManager.Instance.localPlayer.transform.rotation.z},
            { (byte)PropertiesCode.rotW, PlayersManager.Instance.localPlayer.transform.rotation.w}
        };

        PhotonPeer.OpCustom((byte)OperationCode.Move, properies, false);
    }
    #endregion

    // Обработчики ответов от сервера и событий.
    #region RequestsAndEventsHadlers

    void RegisterHandler(OperationResponse operationResponse)
    {
        Debug.Log("1");

        if ((byte)operationResponse.Parameters[(byte)OperationCode.Login] == (byte)ErrorCode.NoError)
        {
            Debug.Log("Register ok");
        }
        else if ((byte)operationResponse.Parameters[(byte)OperationCode.Login] == (byte)ErrorCode.UserExisting)
        {
            Debug.Log("User Exiting");
        }
    }

    void LoginHandler(OperationResponse operationResponse)
    {
        Debug.Log("1");

        if ((byte)operationResponse.Parameters[(byte)OperationCode.Login] == (byte)ErrorCode.NoError)
        {
            EnterInGame();
        }
        else if ((byte)operationResponse.Parameters[(byte)OperationCode.Login] == (byte)ErrorCode.WrongLogin)
        {
            Debug.Log("Wrong Login");
        }
        else if ((byte)operationResponse.Parameters[(byte)OperationCode.Login] == (byte)ErrorCode.WrongPassword)
        {
            Debug.Log("Wrong Password");
        }
    }

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
                //Debug.Log((float)eventData.Parameters[(byte)PropertiesCode.posX] + " " + (float)eventData.Parameters[(byte)PropertiesCode.posY] + " " + (float)eventData.Parameters[(byte)PropertiesCode.posZ]);
                Vector3 newPosition = new Vector3(
                    (float)eventData.Parameters[(byte)PropertiesCode.posX],
                    (float)eventData.Parameters[(byte)PropertiesCode.posY],
                    (float)eventData.Parameters[(byte)PropertiesCode.posZ]);

                Quaternion newRotation = new Quaternion(
                    (float)eventData.Parameters[(byte)PropertiesCode.rotX],
                    (float)eventData.Parameters[(byte)PropertiesCode.rotY],
                    (float)eventData.Parameters[(byte)PropertiesCode.rotZ],
                    (float)eventData.Parameters[(byte)PropertiesCode.rotW]);

                player.MoveEtentity(newPosition);
                player.RotateEtentuty(newRotation);

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
