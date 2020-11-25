using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Functions;

// TCP Listener https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=netcore-3.1

class MyTcpListener
{
    public static void Main()
    {
        TcpListener server = null;
        try
        {
            // Set the TcpListener on port 13000.
            Int32 port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            List<string> userMessages = new List<string>();
            RequestContext client = new RequestContext();

            // TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            server.Start();

            // Enter the listening loop.
            while (true)
            {
                client.ClientConnect(server, ref userMessages);
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
}