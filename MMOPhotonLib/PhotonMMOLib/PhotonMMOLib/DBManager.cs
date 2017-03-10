using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;

namespace PhotonMMOLib
{
    public class DBManager
    {
        //Переменная Connect - это строка подключения в которой:
        //БАЗА - Имя базы в MySQL
        //ХОСТ - Имя или IP-адрес сервера (если локально то можно и localhost)
        //ПОЛЬЗОВАТЕЛЬ - Имя пользователя MySQL
        //ПАРОЛЬ - говорит само за себя - пароль пользователя БД MySQL
        string userID = "adminPhoton";
        string database = "MMOPhoton";
        string port = "3307";
        string password = "";
        string host = "localhost";
        string Connect = "";

        public static DBManager inst;

        public void DBInit()
        {
            if (inst == null)
                inst = this;

            Connect = "Database=" + database + ";Data Source=" + host + ";User Id=" + userID + ";Password=" + password + "";
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

        public bool Login(string name, string pass)
        {
            string CommandText = "Наш SQL скрипт";

            MySqlConnection myConnection = new MySqlConnection(Connect);
            MySqlCommand myCommand = new MySqlCommand(CommandText, myConnection);

            string result = myCommand.ExecuteScalar().ToString();

            if (result == "true")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
