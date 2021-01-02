using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Request;
using DatabaseHandler;
using RestServer;
using System.Diagnostics;

namespace MockServer
{
    public class MyTcpListener
    {
        public static int EndIndexOf(string source, string value)
        {
            int index = source.IndexOf(value);
            if (index >= 0)
            {
                index += value.Length;
            }

            return index;
        }
        public static void Main(string[] args)
        {
            DatabaseHandlerClass databaseServer = new DatabaseHandlerClass();
            databaseServer.DBConnect();
            TcpListener server = null;
            try
            {
                // Set the TcpListener on port 13000.
                Int32 port = 13000;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");

                // TcpListener server = new TcpListener(port);
                server = new TcpListener(localAddr, port);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data

                // Enter the listening loop.
                while (true)
                {
                    Console.WriteLine("Waiting for a connection...");
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");
                    Thread t = new Thread(UserAction);
                    t.Start(client);
                    
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        public static void UserAction(Object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();
            RequestContext handler = new RequestContext();
            List<string> messageList = new List<string>();
            string data = null;
            Byte[] bytes = new Byte[2048];
            int i;
            SessUser user = new SessUser();
            bool userConnected = true;
            while(userConnected)
            {
                try
                {
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        handler.GetPostFunct(data, messageList, stream, client, user, ref userConnected);
                        if (!userConnected)
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e.ToString());
                    stream.Close();
                    client.Close();
                    userConnected = false;
                }
            }
            data = null;
            stream.Close();
            client.Close();
        }


    }
}