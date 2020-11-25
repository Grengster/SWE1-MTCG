using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Functions
{
    class RequestContext
    {
        public void MessageHandler(ref NetworkStream stream, ref List<string> userMessages, ref string data, string type)
        {
            //++wenn / mit Zahl ist, dann Zahl herausfinden (wie man sie bekommt)
            //++wie man nur user msg in Liste einfügt

            // Differentiate between POST, GET, PUT, DEL
            switch (type)
            {
                case "POST":
                    {
                        if (data.Contains("/messages "))
                        {
                            // Every character before the 113th gets cut away from data
                            string message = data;
                            message = data.Substring(113);
                            

                            if (message.Length >= 10)
                            {
                                // You only cut away from the message
                                message = message.Substring(1);
                                Console.WriteLine("\n[Message:{0}] \n", message);
                                
                            }
                            
                            
                            userMessages.Add(message);
                            Console.WriteLine("Message:{0} ", userMessages[0]);
                            ServerResponse(stream, message, 200);
                            break;
                        } else
                        {
                            ServerResponse(stream, "Error!", 403);
                            break;
                        }
                    }

                case "GET":
                    {
                        if (data.Contains("/messages/"))
                        {
                            //++??
                            var stringNum = data.Substring(data.LastIndexOf("es/") + 3, ' ');
                            string modifiedString = stringNum.Split(" ")[0];
                            int result = Int32.Parse(modifiedString);
                            if (userMessages.Count() >= result && result > 0)
                                ServerResponse(stream, userMessages[result-1], 200);
                            else
                                ServerResponse(stream, "Error!", 404);
                        }
                        else if(data.Contains("/messages"))
                        {
                            if (userMessages.Count() > 0)
                            {
                                string Msg = "";
                                foreach (object msgObj in userMessages)
                                {
                                    Msg = Msg + msgObj + "\n";
                                }
                                ServerResponse(stream, Msg, 200);
                            }
                            else
                                ServerResponse(stream, "Error!", 404);
                                break;
                        }
                        break;
                    }

                case "PUT":
                    {
                        if (data.Contains("/messages/"))
                        {
                            //++??
                            var stringNum = data.Substring(data.LastIndexOf("es/") + 3, ' ');
                            string modifiedString = stringNum.Split(" ")[0];
                            int result = Int32.Parse(modifiedString);
                            if (userMessages.Count() < result && result > 0)
                            {
                                ServerResponse(stream, "Error!", 403);
                                break;
                            }
                                
                            string message = data;
                            message = data.Substring(114);


                            if (message.Length >= 10)
                            {
                                // You only cut away from the message
                                message = message.Substring(1);
                                Console.WriteLine("\n[Message:{0}] \n", message);

                            }
                            int count = 1;
                            bool found = false;
                            foreach (string msgObj in userMessages)
                            {
                                if (result == count)
                                {
                                    //Console.WriteLine("[{0}]", msgObj);
                                    //Console.WriteLine("[{0}]", message);
                                    userMessages[count - 1] = message;
                                    ServerResponse(stream, userMessages[count - 1], 200);
                                    found = true;
                                    break;
                                }
                                count++;    
                            }
                            if(!found)
                                ServerResponse(stream, "Error!", 403);
                                break;
                        }
                        else
                        {
                            ServerResponse(stream, "Error!", 404);
                            break;
                        }
                    }

                case "DELETE":
                    {
                        if (data.Contains("/messages/"))
                        {
                            //++??
                            var stringNum = data.Substring(data.LastIndexOf("es/") + 3, ' ');
                            string modifiedString = stringNum.Split(" ")[0];
                            int result = Int32.Parse(modifiedString);
                            if (userMessages.Count() < result && result > 0)
                            {
                                ServerResponse(stream, "Error!", 404);
                                break;
                            }
                            userMessages.RemoveAt(result - 1);
                            ServerResponse(stream, "Removed at " + result, 200);
                            break;
                        }
                        else
                            ServerResponse(stream, "Error!", 403);
                            break;
                    }
                default:
                    ServerResponse(stream, "Error!", 403);
                    break;
            }
        }

        public void ServerResponse(NetworkStream stream, string response, int rspCode)
        {
            // Process the data sent by the client.
            // Reponse Codes -> OK or ERROR
            string serverMsg = "";
            if(rspCode == 200)
            {
               serverMsg = "HTTP/1.1 200 OK \nServer: myserver \nContent - Length:" + response.Length + " \nContent - Language: de \nConnection: close \nContent - Type: text / plain\n\n" + response;
            }
            if (rspCode == 403)
            {
                serverMsg = "HTTP/1.1 403 FORBIDDEN \nServer: myserver \nContent - Length:" + response.Length + " \nContent - Language: de \nConnection: close \nContent - Type: text / plain\n\n" + response;
            }
            if (rspCode == 404)
            {
                serverMsg = "HTTP/1.1 404 NOT FOUND \nServer: myserver \nContent - Length:" + response.Length + " \nContent - Language: de \nConnection: close \nContent - Type: text / plain\n\n" + response;
            }

            // Capital letters
            byte[] sendBytes = Encoding.ASCII.GetBytes(serverMsg);
            stream.Write(sendBytes, 0, sendBytes.Length);
            stream.Flush();
        }

        public void ClientConnect(TcpListener server, ref List<string> userMessages)
        {
            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;
            

            Console.Write("Waiting for a connection... ");
                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                data = null;

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;

                // Message is read in
                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    
                    // Which Type is found
                    if(data.Contains("GET"))
                    {
                        MessageHandler(ref stream, ref userMessages, ref data, "GET");
                    }
                    if (data.Contains("POST"))
                    {
                        MessageHandler(ref stream, ref userMessages, ref data, "POST");
                    }
                    if (data.Contains("PUT"))
                    {
                        MessageHandler(ref stream, ref userMessages, ref data, "PUT");
                    }
                    if (data.Contains("DELETE"))
                    {
                        MessageHandler(ref stream, ref userMessages, ref data, "DELETE");
                    }
                    break;
                }

                // Shutdown and end connection
                client.Close();
        }
    }
}