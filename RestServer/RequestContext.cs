using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Request
{
    public class RequestContext
    {

        public RequestContext() { }

        public void SendStatus(ref NetworkStream stream, string message, int statuscode)
        {
            
            string statusString = statuscode.ToString();
            string serverResponse ="", serverResponse1 = "", serverResponse2 = "", statusMsg = "";
                if (statuscode == 200)
                    statusMsg = " OK";
                else if (statuscode == 403)
                    statusMsg = " FORBIDDEN";
                else if (statuscode == 404)
                    statusMsg = " NOT FOUND";
                if (statuscode >= 100)
                {
                    serverResponse1 = "HTTP/1.1 " + statusString;
                    serverResponse2 = "\nServer: myserver \nContent - Length:" + message.Length + " \nContent - Language: de \nConnection: close \nContent - Type: text / plain\n\n" + message;
                    serverResponse = serverResponse1 + statusMsg + serverResponse2;
                }

            byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
            stream.Write(sendBytes, 0, sendBytes.Length);
            stream.Flush();
        }

        public bool CheckError(ref NetworkStream stream, string RqstType)
        {
            switch(RqstType)
            {
                case "PUT":
                    return true;
                case "DELETE":
                    return true;
                default:
                    return false;
            }
        }



        public void CheckMessage(ref NetworkStream stream, ref string data, string RqstType, ref List<string> messageList)
        {
            char space = (char)32;
            if (data.Contains(RqstType))
            {
                if (!data.Contains("/messages/"))
                {
                    if (data.Contains("/messages"))
                    {
                        if (!CheckError(ref stream, RqstType))
                        {
                            if (RqstType == "GET")
                            {
                                string fullMsg = "";
                                bool noMsg = true;
                                foreach (object o in messageList)
                                {
                                    Console.WriteLine(o);
                                    fullMsg = fullMsg + o.ToString() + "\n";
                                    noMsg = false;
                                }
                                if (noMsg)
                                {
                                    SendStatus(ref stream, "ERROR", 404);
                                }
                                else
                                    SendStatus(ref stream, fullMsg, 200);
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
                                    SendStatus(ref stream, userMsg, 200);
                            }
                        }
                        else
                            SendStatus(ref stream, "FORBIDDEN", 403);
                    }
                }
                else
                if(RqstType == "GET" || RqstType == "PUT" || RqstType == "DELETE")
                {
                    var stringNum = data.Substring(data.LastIndexOf("es/") + 3, space);
                    string modifiedString = stringNum.Split(" ")[0];
                    int result = Int32.Parse(modifiedString);
                    bool noMsg = true;
                    int counter = 0;
                    string fullMsg = "";
                    foreach (object o in messageList)
                    {
                        if (result - 1 == counter)
                        {
                            fullMsg = fullMsg + o.ToString();
                            if (RqstType == "GET")
                            { 
                                Console.WriteLine(o);
                                SendStatus(ref stream, fullMsg, 200);
                            }
                            else if (RqstType == "PUT")
                            {
                                string userMsg = data.Substring(114);
                                if(userMsg.Length >= 10)
                                    userMsg = userMsg.Substring(0);
                                messageList[counter] = messageList[counter].Replace(fullMsg, userMsg);
                                SendStatus(ref stream, userMsg, 200);
                            }
                            else if (RqstType == "DELETE")
                            {
                                messageList.RemoveAt(counter);
                                SendStatus(ref stream, "Removed: " + fullMsg, 200);
                            }
                            noMsg = false;
                            break;
                        }
                        counter++;
                    }
                    if (noMsg)
                        SendStatus(ref stream, "NOT FOUND", 404);                        
                }
                else
                {
                    SendStatus(ref stream, "FORBIDDEN", 403);
                }
            }
            else
            {
                SendStatus(ref stream, "FORBIDDEN", 403);
            }
        }


        public void GetPostFunct(ref TcpListener server, ref List<string> messageList)
        {
            Byte[] bytes = new Byte[256];
            String userData = null;
            Console.Write("Waiting for a connection... ");

            // Perform a blocking call to accept requests.
            // You could also use server.AcceptSocket() here.
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Connected!");

            userData = null;

            // Get a stream object for reading and writing
            NetworkStream stream = client.GetStream();
            
            int i;
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                userData = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                if (userData.Contains("GET"))
                    CheckMessage(ref stream, ref userData, "GET", ref messageList);
                else 
                if (userData.Contains("POST"))
                    CheckMessage(ref stream, ref userData, "POST", ref messageList);
                else
                if (userData.Contains("PUT"))
                    CheckMessage(ref stream, ref userData, "PUT", ref messageList);
                else
                if (userData.Contains("DELETE"))
                    CheckMessage(ref stream, ref userData, "DELETE", ref messageList);
                break;
            }
            //string response = "Hallo";
            //string serverResponse = "HTTP/1.1 200 OK \nServer: myserver \nContent - Length:" + response.Length + " \nContent - Language: de \nConnection: close \nContent - Type: text / plain\n\n" + response;
            //Console.WriteLine(serverResponse);
            //byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
            //stream.Write(sendBytes, 0, sendBytes.Length);
            //stream.Flush();
            client.Close();
        }


    }
}