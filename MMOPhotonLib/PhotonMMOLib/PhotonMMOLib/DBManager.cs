using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using PhotonMMO.Common;
using ExitGames.Logging;

namespace PhotonMMOLib
{
    public class DBManager
    {
        // Переменная Connect - это строка подключения в которой:
        // БАЗА - Имя базы в MySQL
        // ХОСТ - Имя или IP-адрес сервера (если локально то можно и localhost)
        // ПОЛЬЗОВАТЕЛЬ - Имя пользователя MySQL
        // ПАРОЛЬ - говорит само за себя - пароль пользователя БД MySQL
        string userID = "root";
        string database = "MMOPhoton";
        string port = "3307";
        string password = "";
        string host = "localhost";
        string Connect = "";

        // Запросы.
        string getLogins = "SELECT login FROM accounts";
        string getPasswords = "SELECT password FROM accounts WHERE login=";
        string regAccount = "INSERT INTO accounts(login, password) VALUES ";

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

        public ErrorCode Register(string loginName, string pass)
        {
            List<string> loginList = new List<string>();

            bool loginExisting = false;
            Log.Debug("1");
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
            Log.Debug("2");
            if (loginExisting)
            {
                return ErrorCode.UserExisting;
            }

            Log.Debug("3");
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
            Log.Debug("4");
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
                                Log.Debug("login " +loginName);
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
                using (var cmd = new MySqlCommand(getPasswords +"'"+loginName+"'", connection))
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
    }
}
