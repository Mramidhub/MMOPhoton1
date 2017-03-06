using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public bool InGameEntering;

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
        }

        void StartServerSetup()
        {
        }

        public void EnterInGame()
        {
            PhotonServer.Instance.ConnectedToServer();
        }

        void InGameEnteringTrue()
        {
            InGameEntering = true;
        }
    }
}
