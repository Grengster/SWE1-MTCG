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
using FightQueueClass;

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

        public static Dictionary<string, SessUser> loggedUsers = new Dictionary<string, SessUser>();
        public static List <string> onlineUsers = new List<string>();
        public static List<FightQueue> fightQueue = new List<FightQueue>();
        public static void Main()
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

                //LIST/DICTIONARY HERE

                // Buffer for reading data

                // Enter the listening loop.
                while (true)
                {
                    if(server.Pending())
                    {
                        Console.WriteLine("Waiting for a connection...");
                        TcpClient client = server.AcceptTcpClient();
                        Console.WriteLine("Connected!");
                        Thread t = new Thread(UserAction);
                        t.Start(client);
                        Console.WriteLine("Started Thread: " + t.ManagedThreadId);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                loggedUsers.Clear();
                // Stop listening for new clients.
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }

        public static void UserAction(Object temp)
        {
            TcpClient client = (TcpClient)temp;
            NetworkStream stream = client.GetStream();
            RequestContext handler = new RequestContext();
            string data;
            Byte[] bytes = new Byte[2048];
            SessUser user = new SessUser();
            int i;
            bool userConnected = true;
            while (userConnected)
            {
                try
                {
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        int pFrom = 0, pTo = 0;
                        data = Encoding.ASCII.GetString(bytes, 0, i);
                        if(data.Contains("Username\":\""))
                        {
                            pFrom = data.IndexOf("Username\":\"") + "Username\":\"".Length;
                            pTo = data.LastIndexOf("\",");
                        }
                        if (data.Contains("Authorization:"))
                        {
                            pFrom = data.IndexOf("Basic ") + "Basic ".Length;
                            pTo = data.LastIndexOf("-mtcgToken");
                        }
                        String result = data[pFrom..pTo];
                        user.username = result;
                        handler.GetPostFunct(data, stream, ref user, ref userConnected);
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
                    if (MyTcpListener.loggedUsers.ContainsKey(user.username))
                        MyTcpListener.loggedUsers.Remove(user.username);
                }
            }
            stream.Close();
            client.Close();
        }
    }
}