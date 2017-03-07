using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

namespace Assets.Scripts.UIScripts
{
    public class UIMainMenuPanel : UIBasePanels
    {
        public Button EnterTheGame;


        void Awake()
        {
        }

        public void EnterInGame()
        {
            GameManager.Instance.EnterInGame();
        }
    }
}
