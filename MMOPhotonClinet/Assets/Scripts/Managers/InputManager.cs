using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class InputManager : MonoBehaviour
    {

        public static InputManager Instance;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void FixedUpdate()
        {
            if (!GameManager.Instance.InGameEntering)
                return;

            GetKeyInput();
        }

        private void GetKeyInput()
        {
            if (Input.GetKey(KeyCode.W))
            {
                var player = PlayersManager.Instance.localPlayer;
                Vector3 newPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z + player.stepSize);
                PlayersManager.Instance.localPlayer.RotateEtentuty();
                PlayersManager.Instance.localPlayer.MoveEtentity(newPos);
                SendMoveToServer();
            }
            if (Input.GetKey(KeyCode.S))
            {
                var player = PlayersManager.Instance.localPlayer;
                Vector3 newPos = new Vector3(player.transform.position.x, player.transform.position.y, player.transform.position.z - player.stepSize);
                PlayersManager.Instance.localPlayer.RotateEtentuty();
                PlayersManager.Instance.localPlayer.MoveEtentity(newPos);
                SendMoveToServer();
            }
            if (Input.GetKey(KeyCode.D))
            {
                var player = PlayersManager.Instance.localPlayer;
                Vector3 newPos = new Vector3(player.transform.position.x + player.stepSize, player.transform.position.y, player.transform.position.z);
                PlayersManager.Instance.localPlayer.RotateEtentuty();
                PlayersManager.Instance.localPlayer.MoveEtentity(newPos);
                SendMoveToServer();
            }
            if (Input.GetKey(KeyCode.A))
            {
                var player = PlayersManager.Instance.localPlayer;
                Vector3 newPos = new Vector3(player.transform.position.x - player.stepSize, player.transform.position.y, player.transform.position.z);
                PlayersManager.Instance.localPlayer.RotateEtentuty();
                PlayersManager.Instance.localPlayer.MoveEtentity(newPos);
                SendMoveToServer();
            }
        }

        void SendMoveToServer()
        {
            PhotonServer.Instance.SendLocalPlayerMove();
        }
    }
}
