using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotonMMOLib.UniverseStructure
{
    public class Sector : BaseArea
    {
        public List<StarSystem> allSystems = new List<StarSystem>();
        public List<Space> allDeepSpace = new List<Space>();
    }
}
