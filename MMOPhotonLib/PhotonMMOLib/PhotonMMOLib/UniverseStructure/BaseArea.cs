using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotonMMOLib.UniverseStructure
{
    public class BaseArea
    {
        public string idArea;

        public string nameArea;

        public string idParent;

        public List<UnityClient> players = new List<UnityClient>();
    }
}
