using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public bool InGameEntering;

        public UnityEvent StartGame = new UnityEvent();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            StartServerSetup();

            PhotonServer.Instance.InGameEnter.AddListener(InGameEnteringTrue);
            PhotonServer.Instance.GameExit.AddListener(InGameEnteringFalse);

            StartGame.Invoke();
        }

        void StartServerSetup()
        {
        }

        public void EnterInGame()
        {
            PhotonServer.Instance.Connect();
        }

        void InGameEnteringTrue()
        {
            InGameEntering = true;
        }

        void InGameEnteringFalse()
        {
            InGameEntering = false;
        }
    }
}
