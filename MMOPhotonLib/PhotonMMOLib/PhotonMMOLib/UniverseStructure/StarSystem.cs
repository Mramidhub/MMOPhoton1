using PhotonMMOLib.UniverseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotonMMOLib.UniverseStructure
{
    public class StarSystem : BaseArea
    {
        public List<Planet> allPlanets = new List<Planet>();
        public List<Orbit> allOrbits = new List<Orbit>();
        public List<Space> allSpaceZone = new List<Space>();
    }
}
