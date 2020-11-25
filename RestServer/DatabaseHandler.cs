using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace DatabaseHandler
{
    class DatabaseHandlerClass
    {
        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
        public MySqlConnection databaseConnection;

        public DatabaseHandlerClass()
        {
            setConnect();
        }

        public void setConnect()
        {
            string mySQLConnectionString = "datasource=127.0.0.1;port=3306; username=root;password=;database=swe_project;";
            databaseConnection = new MySqlConnection(mySQLConnectionString);
        }

        public void runQuery(string queryToRun)
        {
            MySqlCommand commandDatabase = new MySqlCommand(queryToRun, databaseConnection);
            commandDatabase.CommandTimeout = 60;
            try
            {
                databaseConnection.Open();
                MySqlDataReader myReader = commandDatabase.ExecuteReader();

                if (myReader.HasRows)
                {
                    Console.WriteLine("Query Generated result:");

                    while (myReader.Read())
                    {

                        Console.WriteLine(myReader.GetValue(0).GetType() + " - " + myReader.GetString(1).GetType() + " - " + myReader.GetString(2).GetType());
                                            //id                        //loginname                                          
                        Console.WriteLine(myReader.GetValue(0) + " - " + myReader.GetString(1) + " - " + myReader.GetString(2));
                    }
                }
                else
                {
                    Console.WriteLine("Query Successfully executed!");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Query Error: " + e.Message);
            }
        }
    }
}
