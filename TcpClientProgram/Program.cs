using System;
using System.Text;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Net.Http;

namespace Client
{
    public class TcpClientProgram
    {
        public static async Task Main(string[] args)
        {
            Int32 port = 13000;
            string address = "127.0.0.1";
            string uri = "http://" + address + ":" + port;
            //string altUri = "https://httpbin.org/";
            HttpClient client = new HttpClient();


            TcpClient t_client = new TcpClient(address, port);
            NetworkStream stream = t_client.GetStream();
            //t_client.Client.

            string input;

            do
            {
                PrintMenu();
                input = Console.ReadLine();

                switch (input)
                {
                    case "EXIT":
                        Console.WriteLine("Ending Program!");
                        //HttpRequest("EXIT", uri, stream);
                        break;
                    case "1":
                        Console.WriteLine("Get ALL Messages!");
                        //await GetAllMessages(client, uri);
                        HttpRequest("GET1", uri, stream);

                        break;
                    case "2":
                        Console.WriteLine("Get Specific Message!");
                        //await GetMessage(client, uri);
                        HttpRequest("GET2", uri, stream);

                        break;
                    case "3":
                        Console.WriteLine("Post Message!");
                        //await PostMessage(client, uri);
                        HttpRequest("POST", uri, stream);

                        break;
                    case "4":
                        Console.WriteLine("Put Message!");
                        //await PutMessage(client, uri);
                        HttpRequest("PUT", uri, stream);

                        break;
                    case "5":
                        Console.WriteLine("Delete message!");
                        //await DelMessage(client, uri);
                        HttpRequest("DELETE", uri, stream);

                        break;
                    default:
                        break;
                }

                // Bytes Array to receive Server Response.
                Byte[] data = new Byte[256];
                String response = String.Empty;
                // Read the Tcp Server Response Bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                response = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", response);
                //Thread.Sleep(2);


            } while (input != "EXIT");
            Console.ReadKey();
        }


        public static void PrintMenu()
        {
            Console.WriteLine("\n\n");
            Console.WriteLine("***************************************************************************");
            Console.WriteLine("1) Get all messages");
            Console.WriteLine("2) Get one messages");
            Console.WriteLine("3) Send and save message on the Server");
            Console.WriteLine("4) Update message (overwrite)");
            Console.WriteLine("5) Delete message");
            Console.WriteLine("EXIT) Close The Program");
            Console.WriteLine("***************************************************************************");
            Console.WriteLine("\n");
        }

        static void HttpRequest(string reqType, string uri, NetworkStream stream)
        {
            string message = "";
            string path = "/messages";

            if (reqType == "GET1") { reqType = "GET"; }
            else if (reqType == "GET2")
            {
                reqType = "GET";
                Console.WriteLine("Please Enter Message ID!");
                path += "/" + Console.ReadLine();
            }
            else if (reqType == "POST")
            {
                Console.WriteLine("Please Enter new Message!");
                message = Console.ReadLine();
            }
            else if (reqType == "PUT")
            {
                Console.WriteLine("Please Enter Message ID!");
                path += "/" + Console.ReadLine();
                Console.WriteLine("Please Enter new Message");
                message = Console.ReadLine();
            }
            else if (reqType == "DELETE")
            {
                Console.WriteLine("Please Enter Message ID!");
                path += "/" + Console.ReadLine();
            }

            string answerString = "";
            if (message.Length == 0)
            {
                answerString =
                        reqType + " " + path + " " + "HTTP/1.1\n" +
                        "Host: " + uri + "\n" +
                        "Connection: keep-alive \n" +
                        "Keep-Alive: timeout=50, max=0 \n" +
                        "Access-Control-Allow-Origin: *\n" +
                        "Access-Control-Allow-Credentials: true\n" +
                        "Content-Type: text/plain; charset=utf-8\n";
            }
            else
            {
                answerString =
                        reqType + " " + path + " " + "HTTP/1.1\n" +
                        "Host: " + uri + "\n" +
                        "Content-Length:" + message.Length + " \n" +
                        "Content-Language: de \n" +
                        "Connection: keep-alive \n" +
                        "Keep-Alive: timeout=50, max=0 \n" +
                        "Access-Control-Allow-Origin: *\n" +
                        "Access-Control-Allow-Credentials: true\n" +
                        "Content-Type: text/plain; charset=utf-8\n" +
                        "\n" + message;
            }
            Byte[] reply = Encoding.ASCII.GetBytes(answerString);
            stream.Write(reply, 0, reply.Length);
            stream.Flush();
            Console.WriteLine("Sent: \n {0}", answerString);
        }
    }
}