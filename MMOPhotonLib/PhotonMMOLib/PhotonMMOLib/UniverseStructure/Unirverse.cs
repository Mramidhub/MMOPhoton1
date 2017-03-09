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

        public void TestGalaxyInit()
        {
            idArea = 1;

            allGalaxies.Add(new Galaxy());
            allGalaxies[0].idArea = 1;
            allGalaxies[0].allSectors.Add(new Sector());
            allGalaxies[0].allSectors[0].idArea = 1;
            allGalaxies[0].allSectors[0].allSystems.Add(new StarSystem());
            allGalaxies[0].allSectors[0].allSystems[0].idArea = 1;
            allGalaxies[0].allSectors[0].allSystems[0].allPlanets.Add(new Planet());
            allGalaxies[0].allSectors[0].allSystems[0].allPlanets[0].idArea = 1;
            allGalaxies[0].allSectors[0].allSystems[0].allPlanets[0].allPlanetAreas.Add(new PlanetArea());
            allGalaxies[0].allSectors[0].allSystems[0].allPlanets[0].allPlanetAreas[0].idArea = 1;
            allGalaxies[0].allSectors[0].allSystems[0].allPlanets[0].allPlanetAreas.Add(new PlanetArea());
            allGalaxies[0].allSectors[0].allSystems[0].allPlanets[0].allPlanetAreas[1].idArea = 2;
        }
    }
}
