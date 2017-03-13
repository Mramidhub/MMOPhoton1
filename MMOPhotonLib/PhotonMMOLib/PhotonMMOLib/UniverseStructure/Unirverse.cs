using PhotonMMOLib.UniverseStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotonMMOLib
{
    public class Universe : BaseArea
    {
        public List<Galaxy> allGalaxies = new List<Galaxy>();

        public List<Sector> allSectors = new List<Sector>();

        public List<StarSystem> allSystems = new List<StarSystem>();

        public List<Orbit> allOrbits = new List<Orbit>();

        public List<Planet> allPlanet = new List<Planet>();

        public List<Space> allSpaces = new List<Space>();

        public List<PlanetArea> allPlanetAres = new List<PlanetArea>();
    }
}
