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

        public Text login;
        public Text password;


        void Awake()
        {
            login.text = "";
            password.text = "";
        }

        public void Login()
        {
            GameManager.Instance.EnterInGame(login.text, password.text);
        }

        public void Register()
        {
            GameManager.Instance.Register(login.text, password.text);
        }
    }
}
