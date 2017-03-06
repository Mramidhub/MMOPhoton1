using Assets.Scripts.Lsocation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class WorldManager : MonoBehaviour
    {
        public static WorldManager Instance;

        public List<Area> AreasList = new List<Area>();

        public int currentArea = 0;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
    }
}
