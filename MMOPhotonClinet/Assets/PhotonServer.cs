using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    private void Connect()
    {
        if (PhotonPeer != null)
            PhotonPeer.Connect(CONNECTION_STRING, APP_NAME);
    }

    public void DebugReturn(DebugLevel level, string message)
    {

    }

    // Вызвается когда от сервера приходит операция. Ответ от сервера на запрос.
    public void OnOperationResponse(OperationResponse operationResponse)
    {
        switch (operationResponse.OperationCode)
        {
            case 1:
                if (operationResponse.Parameters.ContainsKey(1))
                {
                    Debug.Log("recv:" + operationResponse.Parameters[1]);
                    SendOperation2();
                }
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
            case 1:
                if (eventData.Parameters.ContainsKey(1))
                {
                    Debug.Log("recv:" + eventData.Parameters[1]);
                }
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
                SendOperation();
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
    public void SendOperation()
    {
        PhotonPeer.OpCustom(1,new Dictionary<byte, object> { { 1, "send message" } }, true);
    }

    public void SendOperation2()
    {
        PhotonPeer.OpCustom(2,new Dictionary<byte, object> { { 1, "send message for event" } }, true);
    }
}
