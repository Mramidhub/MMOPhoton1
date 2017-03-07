using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.UIScripts;
namespace Assets.Scripts.Managers
{
    class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        #region Panels
        public UIMainMenuPanel UIMainMenuPanel;
        public UIInGameMenuPanel UIInGamePanel;
        #endregion


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            GameManager.Instance.StartGame.AddListener(UIMainMenuPanel.Show);
            PhotonServer.Instance.InGameEnter.AddListener(UIInGamePanel.Show);
            PhotonServer.Instance.GameExit.AddListener(UIInGamePanel.Hide);
            PhotonServer.Instance.InGameEnter.AddListener(UIMainMenuPanel.Hide);
            PhotonServer.Instance.GameExit.AddListener(UIMainMenuPanel.Show);
        }
    }
}
