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
using MockServer;
using System.Collections.Specialized;

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
                using var cmd = new NpgsqlCommand($"INSERT INTO \"user\" (username, user_pwd, lastlogin, curr_balance) VALUES (@u,@p, '" + time.ToString(format) + "', @c)", conn);
                cmd.Parameters.AddWithValue("u", username);
                cmd.Parameters.AddWithValue("p", MD5Hash(pass));
                cmd.Parameters.AddWithValue("t", time.ToString(format));
                cmd.Parameters.AddWithValue("c", 100);
                cmd.ExecuteNonQuery();

                //INSERT INTO public.stats(points, id, wins, losses, username) VALUES(0, DEFAULT, 0, 0, 'kienboeck');
                conn.Close();
                conn.Open();

                using var cmd2 = new NpgsqlCommand($"INSERT INTO stats (points, wins, losses, username) VALUES (0, 0, 0, @u)", conn);
                cmd2.Parameters.AddWithValue("u", username);
                cmd2.ExecuteNonQuery();
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
            if (MyTcpListener.loggedUsers.ContainsKey(user.username))
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
                //Console.WriteLine("\nWrong password or username!");
                return -1;
            }
            else
            {
                DateTime time = DateTime.Now;              // Use current time
                string format = "yyyy-MM-dd HH:mm:ss";    // modify the format depending upon input required in the column in 
                //Console.WriteLine("\nSuccessfully logged in!");
                MyTcpListener.loggedUsers.Add(user.username, user);
                MyTcpListener.loggedUsers[user.username].SetUser(user.username, user.password);
                //Console.WriteLine(MyTcpListener.loggedUsers[user.username].GetInfo());
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

                CheckStats(user, "get", 0, 0, 0);
                CheckUserInfo(user, "get", "");
                //RetrieveCards(MyTcpListener.loggedUsers[user.username]);
                MyTcpListener.onlineUsers.Add(user.username);

                return 1;
            }
        }


        public int CheckStats(SessUser user, string option, int points, int wins, int losses)
        {
            if (option == "get")
            {
                //Retrieve all rows
                using var cmd = new NpgsqlCommand("SELECT points, wins, losses FROM stats WHERE username = @u", conn);
                cmd.Parameters.AddWithValue("u", user.username);
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        //Console.WriteLine(reader.GetInt32(0)); GET THE ID OR FOR STRING ToString(0);
                        MyTcpListener.loggedUsers[user.username].points = (Convert.ToInt32(reader["points"]));
                        MyTcpListener.loggedUsers[user.username].wins = (Convert.ToInt32(reader["wins"]));
                        MyTcpListener.loggedUsers[user.username].losses = (Convert.ToInt32(reader["losses"]));
                        break;
                    }
                return 1;
            }
            else if (option == "set")
            {
                
                using var cmd = new NpgsqlCommand($"UPDATE stats SET points = @n, wins = @b, losses = @i WHERE username = @u", conn);
                cmd.Parameters.AddWithValue("n", points);
                cmd.Parameters.AddWithValue("b", wins);
                cmd.Parameters.AddWithValue("i", losses);
                cmd.Parameters.AddWithValue("u", user.username);
                cmd.ExecuteNonQuery();
                MyTcpListener.loggedUsers[user.username].points = points;
                MyTcpListener.loggedUsers[user.username].wins = wins;
                MyTcpListener.loggedUsers[user.username].losses = losses;
                return 1;
            }
            else
                return -1;

        }



        public int CheckUserInfo(SessUser user, string option, string jsonData)
        {
            if (option == "get")
            {
                //Retrieve all rows
                using var cmd = new NpgsqlCommand("SELECT name, bio, image FROM \"user\" WHERE username = @u", conn);
                cmd.Parameters.AddWithValue("u", user.username);
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                        //Console.WriteLine(reader.GetInt32(0)); GET THE ID OR FOR STRING ToString(0);
                        MyTcpListener.loggedUsers[user.username].name = (Convert.ToString(reader["name"]));
                        MyTcpListener.loggedUsers[user.username].bio = (Convert.ToString(reader["bio"]));
                        MyTcpListener.loggedUsers[user.username].image = (Convert.ToString(reader["image"]));
                        break;
                    }
                return 1;
            }
            else if (option == "set")
            {
                if (!jsonData.Contains("Name") || !jsonData.Contains("Bio") || !jsonData.Contains("Image"))
                    return -1;
                List<string> userInfo = new List<string>();
                string tempJson = jsonData;
                for (int j = 0; j < 3; j++)
                {
                    int posFrom = tempJson.IndexOf(": \"");
                    if (posFrom != -1) //if found char
                    {
                        int posTo;
                        if (j == 2)
                            posTo = tempJson.IndexOf("\"}", posFrom + 2);
                        else
                            posTo = tempJson.IndexOf("\",", posFrom + 2);
                        if (posTo != -1) //if found char
                        {
                            userInfo.Add(tempJson.Substring(posFrom + 2, posTo - posFrom - 1));
                            tempJson = tempJson.Remove(posFrom, posTo - posFrom + 2);
                            //Console.WriteLine("\n" + userInfo[j] + "\n");
                        }
                    }
                }


                using var cmd = new NpgsqlCommand($"UPDATE \"user\" SET name = @n, bio = @b, image = @i WHERE username = @u", conn);
                cmd.Parameters.AddWithValue("n", userInfo[0]);
                cmd.Parameters.AddWithValue("b", userInfo[1]);
                cmd.Parameters.AddWithValue("i", userInfo[2]);
                cmd.Parameters.AddWithValue("u", user.username);
                cmd.ExecuteNonQuery();
                MyTcpListener.loggedUsers[user.username].name = userInfo[0];
                MyTcpListener.loggedUsers[user.username].bio = userInfo[1];
                MyTcpListener.loggedUsers[user.username].image = userInfo[2];
                return 1;
            }
            else
                return -1;
            
        }





        public string CheckScore(SessUser user)
        {
            string allPoints = "Scoreboard:\n";
                //Retrieve all rows
                using var cmd = new NpgsqlCommand("SELECT points, username FROM stats ORDER BY points DESC", conn);
                cmd.ExecuteNonQuery();
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                    {
                    //Console.WriteLine(reader.GetInt32(0)); GET THE ID OR FOR STRING ToString(0);
                        if(Convert.ToString(reader["username"]) == user.username)
                            allPoints += "---" + Convert.ToString(reader["username"]) + " : " + Convert.ToInt32(reader["points"]) + "---" + "\n";
                        else
                            allPoints += Convert.ToString(reader["username"]) +" : "+  Convert.ToInt32(reader["points"]) + "\n";
                    }
                return allPoints;
        }




        public int GetDecks(SessUser user)
        {
            if (!MyTcpListener.loggedUsers.ContainsKey(user.username))
                return -2;

            using var cmd1 = new NpgsqlCommand("SELECT curr_balance FROM \"user\" WHERE username = @a", connTemp);
            cmd1.Parameters.AddWithValue("a", user.username);
            cmd1.ExecuteNonQuery();
            using var readerTmp = cmd1.ExecuteReader();
            int balance = 0;
            while (readerTmp.Read())
            {
                balance = readerTmp.GetInt32(0);
                //Console.WriteLine("----" + balance + "----");
                break;
            }
            connTemp.Close();
            connTemp.Open();
            if (balance - 5 < 0)
                return -3;

            var rand = new Random(); //instantiate random generator
            using var cmd2 = new NpgsqlCommand("SELECT deckid FROM decks WHERE deckowner IS NULL", connTemp);
            cmd2.ExecuteNonQuery();
            int decknumbers = 0;
            using var readerTmp1 = cmd2.ExecuteReader();
            while (readerTmp1.Read())
            {
                decknumbers = readerTmp1.GetInt32(0);
                break;
            }

            string deckJson = "";
            //int selectDeck = rand.Next(1, decknumbers);
            // Retrieve all rows
            using var cmd = new NpgsqlCommand("SELECT * FROM decks WHERE deckId = @n AND deckowner IS NULL", conn);
            //Console.WriteLine(decknumbers);
            cmd.Parameters.AddWithValue("n", decknumbers);
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
                        //Console.WriteLine("\n ----------- \n" + deckJson + "\n ----------- \n");
                        break;
                    }
                        
                    deckJson = deckJson + tempJson + ",";
                    isFound = true;
                    count++;
                }
            if (!isFound)
            {
                //Console.WriteLine("WEEEE " + decknumbers + " REEEEEE");
                //Console.WriteLine("\nNo deck was found with that number!");
                return -1;
            }
            else
            {

                RootObject test = JsonConvert.DeserializeObject<RootObject>(deckJson);
                count = 0;
                if(MyTcpListener.loggedUsers[user.username].userDeck != null)
                {
                    foreach (var userDeckData in MyTcpListener.loggedUsers[user.username].userDeck)
                    {
                        if (MyTcpListener.loggedUsers[user.username].userDeck[count].GetId() == test.card[count].GetId())
                            return -2;
                        count++;
                    }
                }

                connTemp.Close();
                connTemp.Open();

                using var cmd3 = new NpgsqlCommand("UPDATE decks SET deckowner = @b WHERE deckId = @n AND deckowner IS NULL", connTemp);
                cmd3.Parameters.AddWithValue("b", user.username);
                cmd3.Parameters.AddWithValue("n", decknumbers);
                cmd3.ExecuteNonQuery();


                //MyTcpListener.loggedUsers[user.username].userDeck = test.card;
                //Console.WriteLine(user.SeeDeck(MyTcpListener.loggedUsers[user.username]));

                if (balance - 5 >= 0)
                {
                    connTemp.Close();
                    connTemp.Open();
                    using var cmd4 = new NpgsqlCommand("UPDATE \"user\" SET curr_balance = @b WHERE username = @a", connTemp);
                    cmd4.Parameters.AddWithValue("b", balance - 5);
                    cmd4.Parameters.AddWithValue("a", user.username);
                    cmd4.ExecuteNonQuery();
                    //Console.WriteLine("BALANCE SET TO " + (balance - 5));
                    connTemp.Close();
                    connTemp.Open();
                }
                    return balance;
            }
        }





        public string SeeBoughtCards(SessUser user)
        {
            string tempJson = "{ \"card\": [";
            connTemp.Close();
            connTemp.Open();

            bool CardIsFound = false;
            using var cmd4 = new NpgsqlCommand($"SELECT cards FROM decks WHERE deckowner = @u", connTemp);
            cmd4.Parameters.AddWithValue("u", user.username);
            cmd4.ExecuteNonQuery();
            using var reader2 = cmd4.ExecuteReader();
            while (reader2.Read())
            {
                //Console.WriteLine(reader.GetInt32(0)); GET THE ID OR FOR STRING ToString(0
                tempJson = tempJson + reader2.GetString(0) + ",";
                CardIsFound = true;
            }
            tempJson += "]}";
            Console.WriteLine(tempJson);

            string returnCards = "Your currently owned cards:\n";

            if (!CardIsFound)
                return "zero"; //no cards
            else if (CardIsFound)
            {
                RootObject test = JsonConvert.DeserializeObject<RootObject>(tempJson);

                int count = 0;
                foreach (var userDeckData in test.card)
                {
                    returnCards += test.card[count].UserDeckInfo();
                    count++;
                }

                return returnCards;
            }
            return "zero";
        }


        public int InsertCards(SessUser user, string cardJson)
        {

            if (cardJson == "")
                return -1;

            List <string> cardList = new List<string>();
            string tempJson = "{ \"card\": [";
            string cardTemp = "";
            cardTemp = cardJson;

            connTemp.Close();
            connTemp.Open();
            Console.WriteLine("\n");
            var result = string.Join("", cardJson.Split('"').Where((x, i) => i % 2 != 0));
            for(int j = 0; j < 4; j++)
            {
                int posFrom = cardTemp.IndexOf('"');
                if (posFrom != -1) //if found char
                {
                    int posTo = cardTemp.IndexOf('"', posFrom + 1);
                    if (posTo != -1) //if found char
                    {
                        cardList.Add(cardTemp.Substring(posFrom + 1, posTo - posFrom - 1));
                        cardTemp = cardTemp.Remove(posFrom, posTo - posFrom + 1);
                        Console.WriteLine("\n" + cardList[j] + "\n");
                    }
                }
            }
            if (cardList.Count() < 4)
                return -1;

            bool CardIsFound = false;
            using var cmd4 = new NpgsqlCommand($"SELECT cards FROM decks WHERE deckowner = @u", connTemp);
            cmd4.Parameters.AddWithValue("u", user.username);
            cmd4.ExecuteNonQuery();
            using var reader2 = cmd4.ExecuteReader();
            while (reader2.Read())
            {
                //Console.WriteLine(reader.GetInt32(0)); GET THE ID OR FOR STRING ToString(0
                tempJson = tempJson + reader2.GetString(0) + ",";
                CardIsFound = true;
            }
            tempJson += "]}";
            Console.WriteLine(tempJson);


            if (!CardIsFound)
                return 1; //no cards
            else if (CardIsFound)
            {
                RootObject test = JsonConvert.DeserializeObject<RootObject>(tempJson);

                int count = 0;
                foreach (var userDeckData in test.card)
                {
                    if (cardList.Count() == 0)
                        break;
                    if (MyTcpListener.loggedUsers[user.username].userDeck.Any(p => p.Id == test.card[count].Id))
                        count++;
                    else
                    {
                        //int cardCount = 0;
                        //foreach(var cardItem in cardList)
                        //{
                            if(cardList.Contains(test.card[count].Id))
                            {
                                MyTcpListener.loggedUsers[user.username].userDeck.Add(test.card[count]);
                                Console.WriteLine(MyTcpListener.loggedUsers[user.username].userDeck);
                            }
                        count++;
                            //cardCount++;
                        //}
                        
                        
                    }
                }
                if (MyTcpListener.loggedUsers[user.username].userDeck.Count() != 4)
                    return -1;
                //Console.WriteLine("------- TEST HERE\n");
                //MyTcpListener.loggedUsers[user.username].SeeDeck(MyTcpListener.loggedUsers[user.username]);
                //Console.WriteLine("\n------- TEST HERE\n");
                return 2;
            }
            return 1;
        }



        public int TakeOverDeck(SessUser player1, SessUser player2, char options)
        {
            List<DeckData> jsonCards = new List<DeckData>();
            if (options == '2')
            {
                SessUser tempClass = player1;
                player1 = player2;
                player2 = tempClass;
            }
            string tempJson = "{ \"card\": [";
            connTemp.Close();
            connTemp.Open();

            bool CardIsFound = false;
            using var cmd4 = new NpgsqlCommand($"SELECT cards FROM decks WHERE deckowner = @u", connTemp);
            cmd4.Parameters.AddWithValue("u", player2.username);
            cmd4.ExecuteNonQuery();
            using var reader2 = cmd4.ExecuteReader();
            while (reader2.Read())
            {
                //Console.WriteLine(reader.GetInt32(0)); GET THE ID OR FOR STRING ToString(0
                tempJson = tempJson + reader2.GetString(0) + ",";
                CardIsFound = true;
            }
            tempJson += "]}";
            //Console.WriteLine(tempJson);


            if (!CardIsFound)
                return 1; //no cards
            else if (CardIsFound)
            {
                

                RootObject test = JsonConvert.DeserializeObject<RootObject>(tempJson);


                int count = 0;
                foreach (var userDeckData in test.card)
                {
                    if (MyTcpListener.loggedUsers[player2.username].userDeck.Any(p => p.Id == test.card[count].Id))
                    {
                        
                        jsonCards.Add(test.card[count]);
                        count++;

                    }
                    else
                        count++;
                }
                foreach(var card in jsonCards)
                {
                //Console.WriteLine("\n" + card.GetDeckInfo() + "\n");
                    connTemp.Close();
                    connTemp.Open();
                    using var cmd5 = new NpgsqlCommand($"UPDATE decks SET deckowner = @s WHERE deckowner = @u AND cards = @c", connTemp);
                    cmd4.Parameters.AddWithValue("s", player1.username);
                    cmd4.Parameters.AddWithValue("u", player2.username);
                    cmd4.Parameters.Add(new NpgsqlParameter("c", NpgsqlDbType.Jsonb) { Value = card.GetDeckInfo() });
                    cmd4.ExecuteNonQuery();
                }
                return 2;
            }
            return 1;
        }





        public int RetrieveCards(SessUser user)
        {
            string tempJson = "{ \"card\": [";
            connTemp.Close();
            connTemp.Open();

            bool CardIsFound = false;
            using var cmd4 = new NpgsqlCommand($"SELECT cards FROM decks WHERE deckowner = @u", connTemp);
            cmd4.Parameters.AddWithValue("u", user.username);
            cmd4.ExecuteNonQuery();
            using var reader2 = cmd4.ExecuteReader();
            while (reader2.Read())
            {
                //Console.WriteLine(reader.GetInt32(0)); GET THE ID OR FOR STRING ToString(0
                tempJson = tempJson + reader2.GetString(0) + ",";
                CardIsFound = true;
            }
            tempJson += "]}";
            //Console.WriteLine(tempJson);


            if (!CardIsFound)
                return 1; //no cards
            else if (CardIsFound)
            {
                RootObject test = JsonConvert.DeserializeObject<RootObject>(tempJson);

                int count = 0;
                foreach (var userDeckData in test.card)
                {
                    if (MyTcpListener.loggedUsers[user.username].userDeck.Any(p => p.Id == test.card[count].Id))
                        count++;
                    else
                    {
                        MyTcpListener.loggedUsers[user.username].userDeck.Add(test.card[count]);
                        count++;
                    }
                }

                return 2;
            }
            return 1;
        }



        public bool SetDecks(List<DeckData> results, ref SessUser user)
        {
            if (!MyTcpListener.loggedUsers.ContainsKey(user.username))
                return false;
            if(MyTcpListener.loggedUsers[user.username].username != "admin")
                return false;
            using var cmd1 = new NpgsqlCommand("SELECT COUNT(*) FROM decks", connTemp);
            cmd1.ExecuteNonQuery();
            int decknumbers = 0;
            using var reader = cmd1.ExecuteReader();
            while (reader.Read())
            {
                decknumbers = reader.GetInt32(0);
                break;  
            }
            //Console.WriteLine(user.GetUser()); //still doesnt work, dont know why, dont care why
            bool working = true;
            foreach (var DeckData in results)
            {
                using var cmd2 = new NpgsqlCommand($"INSERT INTO decks (cards, deckId) VALUES (@i,@d)", conn);
                cmd2.Parameters.Add(new NpgsqlParameter("i", NpgsqlDbType.Jsonb) { Value = DeckData.GetDeckInfo() }); //we need to map the parameters to json
                cmd2.Parameters.AddWithValue("d", decknumbers + 1);
                if (cmd2.ExecuteNonQuery() == 0)
                    working = false;
                //Console.WriteLine(DeckData.GetDeckInfo());

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
