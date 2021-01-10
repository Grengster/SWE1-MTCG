using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            string response;
            int option;
            response = Console.ReadLine();
            //string response = "Hallo";
            string serverResponse = "HTTP/1.1 200 OK \nServer: myserver \nContent - Length:" + response.Length + " \nContent - Language: de \nConnection: close \nContent - Type: text / plain\n\n" + response;
            Console.WriteLine(serverResponse);
            byte[] sendBytes = Encoding.ASCII.GetBytes(serverResponse);
            stream.Write(sendBytes, 0, sendBytes.Length);
            stream.Flush();
        }
    }
}
