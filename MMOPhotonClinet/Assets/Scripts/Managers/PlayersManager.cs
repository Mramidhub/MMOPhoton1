using Assets.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager Instance;

    public GameObject playerPrefab;
    public Player localPlayer;
    Vector3 lastPosLocalPlayer;

    UnityEvent destroyPlayers = new UnityEvent();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        lastPosLocalPlayer = Vector3.zero;
    }

    public List<Player> players = new List<Player>();

    // Инстанс локального игрока.
    public void InstLocalPlayer(Vector3 position, int idClient)
    {
        // Создаем игрока.
        localPlayer = Instantiate(playerPrefab).GetComponent<Player>();

        // Делаем его дочерним к текущему миру.
        localPlayer.transform.parent = WorldManager.Instance.AreasList[WorldManager.Instance.currentArea].transform;

        // Позиционируем.
        localPlayer.transform.position = position;

        players.Add(localPlayer.GetComponent<Player>());
        players[players.Count-1].idClient = idClient;
    }

    public void DestroyLocalPlayer()
    {
        Destroy(localPlayer.gameObject);
    }

    public void InstAnotherPlayer(Vector3 position, int idClient)
    {
        // Создаем игрока.
        var anotherPlayer = (GameObject)Instantiate(playerPrefab);

        // Делаем его дочерним к текущему миру.
        anotherPlayer.transform.parent = WorldManager.Instance.AreasList[WorldManager.Instance.currentArea].transform;

        // Позиционируем.
        anotherPlayer.transform.position = position;

        // Добавляем в список игроков и присваеваем ID;
        players.Add(anotherPlayer.GetComponent<Player>());
        players[players.Count-1].idClient = idClient;
    }

    public void DeleteAllPlayers()
    {
        for (int b = 0; b < players.Count; b++)
        {
            destroyPlayers.AddListener(players[b].DestroyEntity);
        }

        destroyPlayers.AddListener(localPlayer.DestroyEntity);

        destroyPlayers.Invoke();

        destroyPlayers.RemoveAllListeners();

        PlayersManager.Instance.players.Clear();
    }
}
