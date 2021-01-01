using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Npgsql;
using RestServer;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using DatabaseHandler;
using System.Xml.XPath;

namespace DatabaseHandler
{
    public class DatabaseHandlerClass
    {

        string connString = "Host=localhost;Username=admin;Password=1234;Database=TCG";
        private NpgsqlConnection conn;
        public DatabaseHandlerClass() { DBConnect(); }
        public void DBConnect()
        {
            try
            {
                conn = new NpgsqlConnection(connString);
                conn.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool RegisterUser(string username, string pass)
        {
            if (CheckUser(username))
            {
                DateTime time = DateTime.Now;              // Use current time
                string format = "yyyy-MM-dd HH:mm:ss";    // modify the format depending upon input required in the column in 
                using var cmd = new NpgsqlCommand($"INSERT INTO \"user\" (username, user_pwd, lastlogin) VALUES (@u,@p, '" + time.ToString(format) + "')", conn);
                cmd.Parameters.AddWithValue("u", username);
                cmd.Parameters.AddWithValue("p", MD5Hash(pass));
                cmd.Parameters.AddWithValue("t", time.ToString(format));
                cmd.ExecuteNonQuery();
                return true;
            }
            else
                return false;
        }
        

        public bool CheckUser(string username)
        {

            //Retrieve all rows
            using var cmd = new NpgsqlCommand("SELECT username FROM \"user\" WHERE username = @u", conn);
            cmd.Parameters.AddWithValue("u", username);
            cmd.ExecuteNonQuery();
            bool isFound = false;
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    isFound = true;
                    break;
                }
            if (isFound)
                return false;
            else
                return true;
        }



        public int LoginUser(string username, string pass, ref SessUser user)
        {

            // Retrieve all rows
            using var cmd = new NpgsqlCommand("SELECT * FROM \"user\" WHERE username = @u AND user_pwd = @p", conn);
            cmd.Parameters.AddWithValue("u", username);
            cmd.Parameters.AddWithValue("p", MD5Hash(pass));
            cmd.ExecuteNonQuery();
            bool isFound = false;
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    //Console.WriteLine(reader.GetInt32(0)); GET THE ID OR FOR STRING ToString(0);
                    isFound = true;
                    break;
                }
            if (!isFound)
            {
                Console.WriteLine("\nWrong password or username!");
                return -1;
            }
            else
            {
                DateTime time = DateTime.Now;              // Use current time
                string format = "yyyy-MM-dd HH:mm:ss";    // modify the format depending upon input required in the column in 
                Console.WriteLine("\nSuccessfully logged in!");
                user.setUser(username, pass);
                using (var cmd2 = new NpgsqlCommand($"SELECT lastlogin FROM \"user\" WHERE username = @u", conn))
                {
                    cmd2.Parameters.AddWithValue("u", username);
                    using var reader = cmd2.ExecuteReader();
                    while (reader.Read())
                    {
                        //Console.WriteLine(reader.GetInt32(0)); GET THE ID OR FOR STRING ToString(0);
                        user.SetLastLogin(Convert.ToDateTime(reader["lastlogin"]));
                        break;
                    }
                }
                using (var cmd3 = new NpgsqlCommand($"UPDATE \"user\" SET lastlogin = '" + time.ToString(format) + "' WHERE username = @u", conn))
                {
                    cmd3.Parameters.AddWithValue("u", username);
                    cmd3.Parameters.AddWithValue("t", time.ToString(format));
                    cmd3.ExecuteNonQuery();
                }
                return 1;
            }
        }


        public deckData GetDecks(int decknumber)
        {
            string deckJson = "";
            // Retrieve all rows
            using var cmd = new NpgsqlCommand("SELECT * FROM \"decks\" WHERE 'deckId' = @n", conn);
            cmd.Parameters.AddWithValue("n", decknumber.ToString());
            cmd.ExecuteNonQuery();
            bool isFound = false;
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    //Console.WriteLine(reader.GetInt32(0)); GET THE ID OR FOR STRING ToString(0);
                    deckJson = reader.GetString(0);
                    Console.WriteLine(deckJson);
                    isFound = true;
                    break;
                }
            if (!isFound)
            {
                Console.WriteLine("\nNo deck was found with that number!");
                return null;
            }
            else
            {

                deckData deck = JsonConvert.DeserializeObject<deckData>(deckJson);
                Console.WriteLine(deck.GetDeckInfo());
                return deck;
            }
        }



        public bool SetDecks(List<deckData> results)
        {
            // Retrieve all rows
            using var cmd = new NpgsqlCommand("INSERT INTO public.decks(cards) VALUES ('{ \"Id\": \"@i\", \"Name\":\"@n\", \"Damage\": @d}'); ", conn);
            cmd.ExecuteNonQuery();
            cmd.Parameters.AddWithValue("i", );
            cmd.Parameters.AddWithValue("n", MD5Hash(pass));
            cmd.Parameters.AddWithValue("d", time.ToString(format));
            if (cmd.ExecuteNonQuery() == 1)
                return true;
            else
                return false;
        }






        public static void CreateUser(string name, string pwd)
        {
            //sessUser name = new sessUser();
        }

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

        /*
            catch (Exception e)
            {
                Console.WriteLine("Query Error: " + e.Message);
            }
        */
    }
}
