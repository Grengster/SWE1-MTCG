using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Request;

class MyTcpListener
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
    public static void Main()
    {

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

            RequestContext handler = new RequestContext();
            List<string> messageList = new List<string>();
            int msgNum = 1;
            // Buffer for reading data

            // Enter the listening loop.
            while (true)
            {
                // Loop to receive all the data sent by the client.
                handler.GetPostFunct(server,ref messageList,ref msgNum);
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