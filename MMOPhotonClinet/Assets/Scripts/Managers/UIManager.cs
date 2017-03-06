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
        #endregion


        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
    }
}
