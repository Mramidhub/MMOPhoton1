using Assets.Scripts.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersManager : MonoBehaviour
{
    public static PlayersManager Instance;

    public GameObject playerPrefab;
    public Player localPlayer;
    Vector3 lastPosLocalPlayer;
    

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

}
