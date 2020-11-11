using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;

namespace Request
{
    class RequestContext
    {

        public RequestContext(){}

        public void GetPostFunct(TcpListener server,ref List<string> messageList,ref int msgNum)
        {
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
            char space = (char)32;
            int i;
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                Console.WriteLine("Received: {0} ", data);
                if (data.Contains("GET"))
                    {
                        if (!data.Contains("/messages/"))
                        {
                            if (data.Contains("/messages"))
                            {
                                bool noMsg = true;
                                foreach (object o in messageList)
                                {
                                    Console.WriteLine(o);
                                    noMsg = false;
                                }
                                if (noMsg)
                                {
                                    Console.WriteLine("No messages available!");
                                }
                                break;
                            }
                        }
                        else
                        {
                            var stringNum = data.Substring(data.LastIndexOf("es/") + 3, space);
                            string modifiedString = stringNum.Split(" ")[0];
                            int result = Int32.Parse(modifiedString);
                            bool noMsg = true;
                            int counter = 0;
                            foreach (object o in messageList)
                            {
                                if (result - 1 == counter)
                                {
                                    Console.WriteLine(o);
                                    noMsg = false;
                                    break;
                                }
                                counter++;
                            }
                            if (noMsg)
                            {
                                Console.WriteLine("No messages found at this spot.");
                                break;
                            }
                            break;
                        } //Hier int nach "messages/" finden
                    }
                    else if (data.Contains("POST"))
                    {
                        if (!data.Contains("/messages/"))
                        {
                            if (data.Contains("/messages"))
                            {
                                string userMsg = data.Substring(113);
                                if (userMsg.Contains(space))
                                {
                                    userMsg = data.Substring(0);
                                    messageList.Add(userMsg);
                                    Console.WriteLine("Added message at number {0}", msgNum);
                                    msgNum++;
                                    break;
                                }
                                else
                                {
                                    messageList.Add(userMsg);
                                    Console.WriteLine("Added message at number {0}", msgNum);
                                    msgNum++;
                                    break;
                                }

                            }
                        }
                        else
                        {
                            Console.WriteLine("Error encountered, wrong usage.");
                            break;
                        }
                        //Substring 139
                    }
        }
            string response = "Hallo";
            string serverResponse = "HTTP/1.1 200 OK \nServer: myserver \nContent - Length:" + response.Length + " \nContent - Language: de \nConnection: close \nContent - Type: text / plain\n\n" + response;
            //Console.WriteLine(serverResponse);
            byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
            stream.Write(sendBytes, 0, sendBytes.Length);
            stream.Flush();
            client.Close();
        }
        

    }
}
