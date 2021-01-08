﻿using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using DatabaseHandler;
using System.Xml.XPath;
using RestServer;
using MockServer;

namespace Request
{
    public class RequestContext
    {
        readonly DatabaseHandlerClass database = new DatabaseHandlerClass();
        public RequestContext(){ database.DBConnect(); }

        public string currUser = "";

        public void SendStatus(  NetworkStream stream, string message, int statuscode)
        {

            string statusString = statuscode.ToString();
            string serverResponse = "", serverResponse1, serverResponse2, statusMsg = "";
            if (statuscode == 200)
                statusMsg = " OK";
            else if (statuscode == 403)
                statusMsg = " FORBIDDEN";
            else if (statuscode == 404)
                statusMsg = " NOT FOUND";
            else if (statuscode == 499)
                statusMsg = " CLIENT CLOSED REQUEST";
            if (statuscode >= 100)
            {
                serverResponse1 =   "HTTP/1.1 " + statusString;
                serverResponse2 =   "\nServer: myserver " +
                                    "\nContent-Length: " + message.Length +
                                    "\nContent-Language: de" +
                                    "\nConnection: close" +
                                    "\nContent-Type: text/plain\n\n" + message;
                serverResponse = serverResponse1 + statusMsg + serverResponse2;
            }

            byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
            stream.Write(sendBytes, 0, sendBytes.Length);
            stream.Flush();
            return;
        }

        public bool CheckError(string RqstType)
        {
            return RqstType switch
            {
                "GET" => false,
                "PUT" => false,
                "POST" => false,
                _ => true,
            };
        }

        public void CheckMessage(NetworkStream stream, string data, string RqstType, ref SessUser user)
        {
            Console.WriteLine(data);
            if (data.Contains(RqstType))
            {
                if (!CheckError(RqstType))
                {
                    StreamPost(stream, data, RqstType,ref user);
                }
                else
                    SendStatus(stream, "FORBIDDEN", 403);
            }
            else
            {
                SendStatus(stream, "FORBIDDEN", 403);
            }
        }



        public void StreamPost(NetworkStream stream, string data, string RqstType, ref SessUser user)
        {
            if (RqstType == "POST")
            {
                if (data.Contains("/users HTTP/1.1") || data.Contains("/sessions HTTP/1.1"))
                {
                    if (data.Contains("Content-Type: application/json"))
                    {
                        int pFrom = data.IndexOf("{") + "{".Length;
                        int pTo = data.LastIndexOf("}");

                        String result = data[pFrom..pTo];
                        result = "{" + result + "}";
                        Userdata userJson = JsonConvert.DeserializeObject<Userdata>(result);
                        //user.setUser(userJson.Username, userJson.Password);
                        if (data.Contains("/users HTTP/1.1"))
                        {
                            if (database.RegisterUser(userJson.Username, userJson.Password)) //go into database and use name & pwd from json decoded class
                                SendStatus(stream, userJson.Username + " " + userJson.Password, 200);
                            else
                                SendStatus(stream, "USERNAME TAKEN", 403);
                        }
                        if (data.Contains("/sessions HTTP/1.1"))
                        {
                            int resultLogin = database.LoginUser(userJson.Username, userJson.Password, ref user);
                            if (resultLogin == 1) //go into database and use name & pwd from json decoded class
                            {
                                SendStatus(stream, "Logged in as: " + MyTcpListener.loggedUsers[user.username].username, 200);
                            }
                            else if (resultLogin == -1)
                                SendStatus(stream, "WRONG PASSWORD OR USERNAME", 403);
                            else if (resultLogin == -2)
                                SendStatus(stream, "ALREADY LOGGED IN AS USER: " + MyTcpListener.loggedUsers[userJson.Username].username, 403);
                            else if (resultLogin == 2)
                                SendStatus(stream, "Welcome, " + MyTcpListener.loggedUsers[userJson.Username].username + ".\n Your cards are: " + MyTcpListener.loggedUsers[userJson.Username].SeeDeck(MyTcpListener.loggedUsers[userJson.Username]), 403);
                            else
                                SendStatus(stream, "UNKOWN ERROR", 403);
                        }

                    }
                    //else if()

                    else
                        SendStatus(stream, "ERROR", 404);
                }
                else if (!MyTcpListener.loggedUsers.ContainsKey(user.username))
                    SendStatus(stream, "LOGIN FIRST AAAAAAAAAA", 403);
                else if (data.Contains("transactions/packages HTTP/1.1"))
                {
                    if (data.Contains("Content-Type: application/json"))
                    {
                        int result = database.GetDecks(MyTcpListener.loggedUsers[user.username]);
                        if (result == 1)
                        {
                            if(user.SeeDeck(MyTcpListener.loggedUsers[user.username]) != null)
                                SendStatus(stream, MyTcpListener.loggedUsers[user.username].SeeDeck(MyTcpListener.loggedUsers[user.username]), 200);
                            else
                                SendStatus(stream, "DECK EMPTY", 403);
                        }
                        else if(result == -1)
                            SendStatus(stream, "WRONG DECKCODE", 403);
                        else if (result == -2)
                            SendStatus(stream, "CARD ALREADY IN DECK", 403);
                        else if (result == -3)
                            SendStatus(stream, "NO BALANCE", 403);
                    }
                }
                else if (data.Contains("/packages HTTP/1.1"))
                {
                    int pFrom = data.IndexOf("{") + "{".Length;
                    int pTo = data.LastIndexOf("}");

                    String result = data[pFrom..pTo];
                    result = "{ \"card\": [{" + result + "}]}";
                    RootObject test = JsonConvert.DeserializeObject<RootObject>(result);
                    Console.WriteLine(test.card[0]);
                    if(database.SetDecks(test.card,ref user))
                        SendStatus(stream, "INSERT SUCCESS", 200);
                    else
                        SendStatus(stream, "ERROR EXECUTING COMMAND", 403);
                }
                else
                    SendStatus(stream, "WRONG COMMAND", 403);
            }
            if (RqstType == "GET")
            {
                if (data.Contains("/cards HTTP/1.1"))
                {
                    int result = database.RetrieveCards(MyTcpListener.loggedUsers[user.username]);
                    if (result == 1)
                        SendStatus(stream, "NO DECKS IN CARDS", 200);
                    if (result == 2)
                        SendStatus(stream, "Your deck: \n" + MyTcpListener.loggedUsers[user.username].SeeDeck(MyTcpListener.loggedUsers[user.username]), 200);
                }  
                else
                    SendStatus(stream, "WRONG COMMAND", 403);
            }
        }

        public void GetPostFunct( string data, NetworkStream stream, ref SessUser user, ref bool userConnected)
        {
            if(user == null)
            {
                SendStatus(stream, "Please login before trying to access any other functions.", 403);
                return;
            }
                
            if (data.Contains("QUIT") || data.Contains("quit") && MyTcpListener.loggedUsers.ContainsKey(user.username))
            {
                MyTcpListener.loggedUsers.Remove(user.username);
                SendStatus(stream, "Successfully logged out", 499);
                userConnected = false;
                return;
            }
            if (data.Contains("GET"))
                CheckMessage(stream, data, "GET",ref user);
            else
            if (data.Contains("POST"))
                CheckMessage(stream, data, "POST",ref user);
            else
            if (data.Contains("PUT"))
                CheckMessage(stream, data, "PUT",ref user);
            else
            if (data.Contains("DELETE"))
                CheckMessage(stream, data, "DELETE",ref user);
            
                    
            //string response = "Hallo";
            //string serverResponse = "HTTP/1.1 200 OK \nServer: myserver \nContent - Length:" + response.Length + " \nContent - Language: de \nConnection: close \nContent - Type: text / plain\n\n" + response;
            //Console.WriteLine(serverResponse);
            //byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
            //stream.Write(sendBytes, 0, sendBytes.Length);
            //stream.Flush();

        }


    }
}