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
using NpgsqlTypes;
using System.ComponentModel;

namespace DatabaseHandler
{
    public class DatabaseHandlerClass
    {
        readonly string connString = "Host=localhost;Username=admin;Password=1234;Database=TCG";
        private NpgsqlConnection conn;
        private NpgsqlConnection connTemp;
        public DatabaseHandlerClass() { DBConnect(); }
        public void DBConnect()
        {
            try
            {
                conn = new NpgsqlConnection(connString);
                connTemp = new NpgsqlConnection(connString);
                conn.Open();
                connTemp.Open();
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



        public int LoginUser(string username, string pass, SessUser user)
        {
            if(user.username != "")
                return -2;
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


        public int GetDecks(SessUser user)
        {
            var rand = new Random(); //instantiate random generator
            using var cmd1 = new NpgsqlCommand("SELECT COUNT(*)/5 FROM decks", connTemp);
            cmd1.ExecuteNonQuery();
            int decknumbers = 0;
            using var readerTmp = cmd1.ExecuteReader();
            while (readerTmp.Read())
            {
                decknumbers = readerTmp.GetInt32(0);
                break;
            }

            string deckJson = "";
            int selectDeck = rand.Next(1, decknumbers);
            // Retrieve all rows
            using var cmd = new NpgsqlCommand("SELECT * FROM decks WHERE deckId = @n AND deckowner IS NULL", conn);
            Console.WriteLine(selectDeck);
            cmd.Parameters.AddWithValue("n", selectDeck);
            cmd.ExecuteNonQuery();
            bool isFound = false;
            int count = 0;
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                {
                    //Console.WriteLine(reader.GetInt32(0)); GET THE ID OR FOR STRING ToString(0);
                    string tempJson = reader.GetString(0);
                    if(count == 4)
                    {
                        deckJson += tempJson;
                        deckJson = "{ \"card\": [" + deckJson + "]}";
                        Console.WriteLine("\n ----------- \n" + deckJson + "\n ----------- \n");
                        break;
                    }
                        
                    deckJson = deckJson + tempJson + ",";
                    isFound = true;
                    count++;
                }
            if (!isFound)
            {
                Console.WriteLine("\nNo deck was found with that number!");
                return -1;
            }
            else
            {

                RootObject test = JsonConvert.DeserializeObject<RootObject>(deckJson);
                count = 0;

                foreach (var userDeckData in user.userDeck)
                {
                    if (user.userDeck[count].GetId() == test.card[count].GetId())
                        return -2;
                    count++;
                }
                user.userDeck = test.card;
                Console.WriteLine(user.SeeDeck());
                    return 1;
            }
        }



        public bool SetDecks(List<deckData> results, SessUser user)
        {
            using var cmd1 = new NpgsqlCommand("SELECT COUNT(*) FROM decks", connTemp);
            cmd1.ExecuteNonQuery();
            int decknumbers = 0;
            using var reader = cmd1.ExecuteReader();
            while (reader.Read())
            {
                decknumbers = reader.GetInt32(0);
                break;
            }
            Console.WriteLine(user.GetUser()); //still doesnt work, dont know why, dont care why
            //if(user.username != "admin")
            //    return false;
            bool working = true;
            foreach (var deckData in results)
            {
                using var cmd2 = new NpgsqlCommand($"INSERT INTO decks (cards, deckId) VALUES (@i,@d)", conn);
                cmd2.Parameters.Add(new NpgsqlParameter("i", NpgsqlDbType.Jsonb) { Value = deckData.GetDeckInfo() }); //we need to map the parameters to json
                cmd2.Parameters.AddWithValue("d", decknumbers + 1);
                if (cmd2.ExecuteNonQuery() == 0)
                    working = false;
                Console.WriteLine(deckData.GetDeckInfo());

            }
            if (working)
                return true;
            else
                return false;
            // Retrieve all rows
            
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
