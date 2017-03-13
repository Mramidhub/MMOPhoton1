using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using PhotonMMO.Common;
using ExitGames.Logging;
using PhotonMMOLib.UniverseStructure;

namespace PhotonMMOLib
{
    public class DBManager
    {
        #region DBSettings
        string userID = "root";
        string database = "MMOPhoton";
        string port = "3307";
        string password = "";
        string host = "localhost";
        string Connect = "";
        #endregion

        #region DBRequests
        string getLogins = "SELECT login FROM accou nts";
        string getPasswords = "SELECT password FROM accounts WHERE login=";
        string regAccount = "INSERT INTO accounts(login, password) VALUES ";

        string getCharacterData = "SELECT * FROM characters WHERE login=";

        string getAllGalaxies = "SELECT * FROM galaxies";
        string getAllSectors = "SELECT * FROM sectors";
        string getAllSystems = "SELECT * FROM systems";
        string getAllOrbits = "SELECT * FROM orbits";
        string getAllPlanets= "SELECT * FROM planets";
        string getAllPlanetsAreas = "SELECT * FROM planetareas";

        string getGalaxyData = "SELECT * FROM galaxies WHERE id=";
        string getSectorData = "SELECT * FROM sectors WHERE id=";
        string getSystemData = "SELECT * FROM systems WHERE id=";
        string getPlanetData = "SELECT * FROM planets WHERE id=";
        string getPlanetAreaData = "SELECT * FROM planetareas WHERE id=";
        #endregion

        public static DBManager inst;

        private readonly ILogger Log = LogManager.GetCurrentClassLogger();

        public void DBInit()
        {
            if (inst == null)
                inst = this;

            Log.Debug("Init DB");

            //Connect = "Database=" + database + ";Data Source=" + host + ";User Id=" + userID + ";Password=" + password;
            Connect = "server=" + host +
               ";user=" + userID +
               ";database=" + database +
               ";port=" + port +
               ";password=" + password + ";";
        }

        public void DBSet()
        {
            string CommandText = "Наш SQL скрипт";
            MySqlConnection myConnection = new MySqlConnection(Connect);
            MySqlCommand myCommand = new MySqlCommand(CommandText, myConnection);
            myConnection.Open(); //Устанавливаем соединение с базой данных.
                                 //Что то делаем...
                                 //MyCommand.ExecuteNonQuery(); - внесение изменнений.
                                 //MyCommand.ExecuteScalar(); - выборка одного значения.
                                 //Множество значений:
                                 //MySqlDataReader MyDataReader;
                                 //MyDataReader = myCommand.ExecuteReader();

            //while (MyDataReader.Read())
            //{
            //    string result = MyDataReader.GetString(0); //Получаем строку
            //    int id = MyDataReader.GetInt32(1); //Получаем целое число
            //}

            myConnection.Close(); //Обязательно закрываем соединение!
        }

        public void InitUniverse()
        {
            // Получаем список галактик и создаем их.
            #region CreateGlalxys
            using (var connection = new MySqlConnection(Connect))
            {
                connection.Open();
                using (var cmd = new MySqlCommand(getAllGalaxies, connection))
                {
                    var galaxies = cmd.ExecuteReader();
                    while (galaxies.Read())
                    {
                        var newGalaxy = new Galaxy();

                        newGalaxy.idArea = galaxies.GetInt32(0).ToString();

                        newGalaxy.nameArea = galaxies.GetString(1);

                        Server.inst.MainUniverse.allGalaxies.Add(newGalaxy);
                    }
                }
            }
            #endregion

            // Получаем список секторов и создаем их.
            #region CreateSectors
            using (var connection = new MySqlConnection(Connect))
            {
                connection.Open();
                using (var cmd = new MySqlCommand(getAllSectors, connection))
                {
                    var sectors = cmd.ExecuteReader();
                    while (sectors.Read())
                    {
                        var newSector = new Sector();

                        newSector.idArea = sectors.GetInt32(0).ToString();

                        newSector.nameArea = sectors.GetString(1);

                        newSector.idParent = sectors.GetString(2);

                        Log.Debug("galaxies " + Server.inst.MainUniverse.allGalaxies.Count);

                        List<BaseArea> tempList = new List<BaseArea>();

                        foreach (var area in Server.inst.MainUniverse.allGalaxies)
                        {
                            tempList.Add(area);
                        }

                        Galaxy galaxy = getAreaForID(tempList, newSector.idParent) as Galaxy;

                        galaxy.allSectors.Add(newSector);

                        Server.inst.MainUniverse.allSectors.Add(newSector);
                    }
                }
            }
            #endregion

            // Получаем список систем и создаем их.
            #region CreateSystems
            using (var connection = new MySqlConnection(Connect))
            {
                connection.Open();
                using (var cmd = new MySqlCommand(getAllSystems, connection))
                {
                    var sectors = cmd.ExecuteReader();
                    while (sectors.Read())
                    {
                        var newSystem = new StarSystem();

                        newSystem.idArea = sectors.GetInt32(0).ToString();

                        newSystem.nameArea = sectors.GetString(1);

                        newSystem.idParent = sectors.GetString(2);

                        List<BaseArea> tempList = new List<BaseArea>();

                        foreach (var area in Server.inst.MainUniverse.allSectors)
                        {
                            tempList.Add(area);
                        }

                        Sector sector = getAreaForID(tempList, newSystem.idParent) as Sector;

                        sector.allSystems.Add(newSystem);

                        Server.inst.MainUniverse.allSystems.Add(newSystem);
                    }
                }
            }
            #endregion

            // Получаем список планет и создаем их.
            #region CreatePlanets
            using (var connection = new MySqlConnection(Connect))
            {
                connection.Open();
                using (var cmd = new MySqlCommand(getAllPlanets, connection))
                {
                    var sectors = cmd.ExecuteReader();
                    while (sectors.Read())
                    {
                        var newPlanet = new Planet();

                        newPlanet.idArea = sectors.GetInt32(0).ToString();

                        newPlanet.nameArea = sectors.GetString(1);

                        newPlanet.idParent = sectors.GetString(2);

                        List<BaseArea> tempList = new List<BaseArea>();

                        foreach (var area in Server.inst.MainUniverse.allSystems)
                        {
                            tempList.Add(area);
                        }

                        StarSystem planet = getAreaForID(tempList, newPlanet.idParent) as StarSystem;

                        planet.allPlanets.Add(newPlanet);

                        Server.inst.MainUniverse.allPlanet.Add(newPlanet);
                    }
                }
            }
            #endregion

            // Получаем список орбит и создаем их.
            #region CreateOrbits
            using (var connection = new MySqlConnection(Connect))
            {
                connection.Open();
                using (var cmd = new MySqlCommand(getAllOrbits, connection))
                {
                    var sectors = cmd.ExecuteReader();
                    while (sectors.Read())
                    {
                        var newOrbit = new Orbit();

                        newOrbit.idArea = sectors.GetInt32(0).ToString();

                        newOrbit.nameArea = sectors.GetString(1);

                        newOrbit.idParent = sectors.GetString(2);

                        List<BaseArea> tempList = new List<BaseArea>();

                        foreach (var area in Server.inst.MainUniverse.allSystems)
                        {
                            tempList.Add(area);
                        }

                        StarSystem starsystem = getAreaForID(tempList, newOrbit.idParent) as StarSystem;

                        starsystem.allOrbits.Add(newOrbit);

                        Server.inst.MainUniverse.allOrbits.Add(newOrbit);
                    }
                }
            }
            #endregion

            // Получаем список планетарныхучастков и создаем их.
            #region PlanetAreas
            using (var connection = new MySqlConnection(Connect))
            {
                connection.Open();
                using (var cmd = new MySqlCommand(getAllPlanetsAreas, connection))
                {
                    var planetArea = cmd.ExecuteReader();
                    while (planetArea.Read())
                    {
                        var newArea = new PlanetArea();

                        newArea.idArea = planetArea.GetInt32(0).ToString();

                        newArea.nameArea = planetArea.GetString(1);

                        newArea.idParent = planetArea.GetString(2);

                        List<BaseArea> tempList = new List<BaseArea>();

                        foreach (var area in Server.inst.MainUniverse.allPlanet)
                        {
                            tempList.Add(area);
                        }

                        Planet planet = getAreaForID(tempList, newArea.idParent) as Planet;

                        planet.allPlanetAreas.Add(newArea);

                        Server.inst.MainUniverse.allPlanetAres.Add(newArea);
                    }
                }
            }
            #endregion

            Log.Debug(
                " Galaxies " + Server.inst.MainUniverse.allGalaxies.Count
               + " Sectors " + Server.inst.MainUniverse.allSectors.Count
               + " Systems " + Server.inst.MainUniverse.allSystems.Count
               + " Orbits " + Server.inst.MainUniverse.allOrbits.Count
               + " Planets " + Server.inst.MainUniverse.allPlanet.Count
               + " PlanetAreas " + Server.inst.MainUniverse.allPlanetAres.Count
                );
        }

        public ErrorCode Register(string loginName, string pass)
        {
            List<string> loginList = new List<string>();

            bool loginExisting = false;
            using (var connection = new MySqlConnection(Connect))
            {
                connection.Open();
                using (var cmd = new MySqlCommand(getLogins, connection))
                {
                    var login = cmd.ExecuteReader();
                    while (login.Read())
                    {
                        for (int a = 0; a < login.FieldCount; a++)
                        {
                            if (login.GetString(a) == loginName)
                            {
                                Log.Debug("login " + loginName);
                                loginExisting = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (loginExisting)
            {
                return ErrorCode.UserExisting;
            }

            using (var connection = new MySqlConnection(Connect))
            {
                connection.Open();
                using (var cmd = new MySqlCommand(regAccount + "('" + loginName + "','" + pass + "')", connection))
                {
                    var password = cmd.ExecuteNonQuery();

                    if (password.ToString() == pass)
                    {
                        Log.Debug("pass row" + password);
                    }
                }
            }
            return ErrorCode.NoError;
        }

        public ErrorCode CheckLogin(string loginName, string pass)
        {
            List<string> loginList = new List<string>();

            bool loginExisting = false;
            bool passwordCorrect = false;

            using (var connection = new MySqlConnection(Connect))
            {
                connection.Open();
                using (var cmd = new MySqlCommand(getLogins, connection))
                {
                    var login = cmd.ExecuteReader();
                    while (login.Read())
                    {
                        for (int a = 0; a < login.FieldCount; a++)
                        {
                            if (login.GetString(a) == loginName)
                            {
                                Log.Debug("login " + loginName);
                                loginExisting = true;
                                break;
                            }
                        }
                    }
                }
            }

            using (var connection = new MySqlConnection(Connect))
            {
                connection.Open();
                using (var cmd = new MySqlCommand(getPasswords + "'" + loginName + "'", connection))
                {
                    var password = cmd.ExecuteScalar();

                    if (password.ToString() == pass)
                    {
                        Log.Debug("pass " + pass);
                        passwordCorrect = true;
                    }
                }
            }

            if (loginExisting && passwordCorrect)
            {
                return ErrorCode.NoError;
            }
            else if (loginExisting && !passwordCorrect)
            {
                return ErrorCode.WrongPassword;
            }
            else if (!loginExisting)
            {
                return ErrorCode.WrongLogin;
            }
            else
            {
                return ErrorCode.WrongLogin;
            }
        }

        public Dictionary<string, string> GetDataCharacter(string loginName)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            using (var connection = new MySqlConnection(Connect))
            {
                connection.Open();
                using (var cmd = new MySqlCommand(getCharacterData + "'" + loginName + "'", connection))
                {
                    var dataCharacter = cmd.ExecuteReader();
                    while (dataCharacter.Read())
                    {
                        data.Add("id", dataCharacter.GetString(0));
                        data.Add("name", dataCharacter.GetString(1));
                        data.Add("currentarea", dataCharacter.GetString(2));
                    }
                }
            }

            return data;

        }


        BaseArea getAreaForID(List<BaseArea> areas, string id)
        {
            var baseAreas = areas as List<BaseArea>;

            foreach (BaseArea area in areas)
            {
                if (area.idArea == id)
                {
                    return area;
                }
            }

            return baseAreas[0];
        }

        
    }
}
