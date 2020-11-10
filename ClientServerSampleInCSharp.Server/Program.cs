using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;




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

            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;

            string findPOST = "POST";
            string findGET = "GET";
            List<string> messageList = new List<string>();
            int msgNum = 1;
            // Enter the listening loop.
            while (true)
            {

                Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also use server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                data = null;

                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();

                int i;
                
                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    //Console.WriteLine("Received: {0} ", data);

                    char space = (char)32;
                    if (data.Contains(findGET))
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
                                if(noMsg)
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
                    else if (data.Contains(findPOST))
                    {
                        if (!data.Contains("/messages/"))
                        {
                            if (data.Contains("/messages"))
                            {
                                string userMsg = data.Substring(113);
                                if(userMsg.Contains(space))
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
                    //List<string> serverNames = data.Split("\n").ToList();
                    //serverNames.ForEach(i => Console.Write("{0}\t", i));
                }
                

                // Shutdown and end connection
                client.Close();
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