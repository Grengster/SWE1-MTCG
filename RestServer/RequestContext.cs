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

        public void CheckMessage(  NetworkStream stream,   string data, string RqstType,   List<string> messageList)
        {
            char space = (char)32;
            if (data.Contains(RqstType))
            {
                
                    if (!CheckError(RqstType))
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


                                    Userdata user = JsonConvert.DeserializeObject<Userdata>(result);
                                    if(data.Contains("/users HTTP/1.1"))
                                    {
                                        if (database.RegisterUser(user.Username, user.Password)) //go into database and use name & pwd from json decoded class
                                            SendStatus(stream, user.Username + " " + user.Password, 200);
                                        else
                                            SendStatus(stream, "USERNAME TAKEN", 403);
                                    }
                                    if (data.Contains("/sessions HTTP/1.1"))
                                    {
                                        if (database.LoginUser(user.Username, user.Password) != null) //go into database and use name & pwd from json decoded class
                                            SendStatus(stream, "Logged in as: " + user.Username, 200);
                                        else
                                            SendStatus(stream, "WRONG PASSWORD/USERNAME", 403);
                                    }
                            }
                            else
                                SendStatus(stream, "ERROR", 404);
                            }
                        }
                        else
                        {
                            string userMsg = data.Substring(113);
                            if (userMsg.Contains(space))
                            {
                                userMsg = userMsg.Substring(0);
                                if (userMsg.Length > 10)
                                    userMsg = userMsg.Substring(1);
                            }
                            messageList.Add(userMsg);
                            Console.WriteLine("Added message at number {0}", messageList.Count());
                            SendStatus(stream, userMsg, 200);
                        }
                    }
                    else
                        SendStatus(stream, "FORBIDDEN", 403);
            }
            else
            {
                SendStatus(stream, "FORBIDDEN", 403);
            }
        }


        public void GetPostFunct( string data, List<string> messageList, NetworkStream stream, TcpClient client)
        {

                if (data.Contains("GET"))
                    CheckMessage(stream, data, "GET",messageList);
                else
                if (data.Contains("POST"))
                    CheckMessage(stream, data, "POST",  messageList);
                else
                if (data.Contains("PUT"))
                    CheckMessage(stream, data, "PUT",   messageList);
                else
                if (data.Contains("DELETE"))
                    CheckMessage(stream, data, "DELETE",messageList);
                if (data.Contains("QUIT"))
                    client.Close();
            //string response = "Hallo";
            //string serverResponse = "HTTP/1.1 200 OK \nServer: myserver \nContent - Length:" + response.Length + " \nContent - Language: de \nConnection: close \nContent - Type: text / plain\n\n" + response;
            //Console.WriteLine(serverResponse);
            //byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
            //stream.Write(sendBytes, 0, sendBytes.Length);
            //stream.Flush();

        }


    }
}