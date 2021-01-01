using System;
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

namespace Request
{
    public class RequestContext
    {
        DatabaseHandlerClass database = new DatabaseHandlerClass();
        public RequestContext(){ database.DBConnect(); }

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

        public void CheckMessage(NetworkStream stream, string data, string RqstType, List<string> messageList, ref SessUser user)
        {
            Console.WriteLine(data);
            if (data.Contains(RqstType))
            {
                if (!CheckError(RqstType))
                {
                    StreamPost(stream, data, RqstType, ref user);
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
                            if (database.LoginUser(userJson.Username, userJson.Password, ref user) == 1) //go into database and use name & pwd from json decoded class
                            {
                                SendStatus(stream, "Logged in as: " + user.GetUser(), 200);
                            } 
                            else
                                SendStatus(stream, "WRONG PASSWORD/USERNAME", 403);
                        }
                        
                    }
                    //else if()

                    else 
                        SendStatus(stream, "ERROR", 404);
                }
                if (data.Contains("transactions/packages HTTP/1.1"))
                {
                    if (data.Contains("Content-Type: application/json"))
                    {
                        deckData deck;
                        if (database.GetDecks(5) != null)
                        {
                            deck = database.GetDecks(2);
                            SendStatus(stream, deck.GetDeckInfo(), 200);
                        }
                        else
                            SendStatus(stream, "WRONG DECKCODE", 403);
                    }
                }
            }
        }

        public void GetPostFunct( string data, List<string> messageList, NetworkStream stream, TcpClient client, ref SessUser user, ref bool userConnected)
        {
            if (data.Contains("QUIT"))
            {
                SendStatus(stream, "Successfully logged out", 499);
                userConnected = false;
                return;
            }
            if (data.Contains("GET"))
                CheckMessage(stream, data, "GET",messageList, ref user);
            else
            if (data.Contains("POST"))
                CheckMessage(stream, data, "POST",  messageList, ref user);
            else
            if (data.Contains("PUT"))
                CheckMessage(stream, data, "PUT",   messageList, ref user);
            else
            if (data.Contains("DELETE"))
                CheckMessage(stream, data, "DELETE",messageList, ref user);
            
                    
            //string response = "Hallo";
            //string serverResponse = "HTTP/1.1 200 OK \nServer: myserver \nContent - Length:" + response.Length + " \nContent - Language: de \nConnection: close \nContent - Type: text / plain\n\n" + response;
            //Console.WriteLine(serverResponse);
            //byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
            //stream.Write(sendBytes, 0, sendBytes.Length);
            //stream.Flush();

        }


    }
}