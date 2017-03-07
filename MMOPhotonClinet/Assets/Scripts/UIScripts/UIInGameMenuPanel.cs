using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UIScripts
{
    public class UIInGameMenuPanel : UIBasePanels
    {
        public Button ExitGameBut;


        public void ExitGame()
        {
            PhotonServer.Instance.ExitGame();
        }
    }
}
