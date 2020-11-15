using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Request;
using System.Threading.Tasks;

public interface ITCPClient : IDisposable
{
    Stream GetStream();
    EndPoint RemoteEndPoint { get; }
    Task ConnectAsync(string ip, int port);
}


namespace RestServer
{
    public class TcpClientMock : ITCPClient
    {
        private readonly List<string> _sentMessages;

        public TcpClientMock(List<string> sentMessages)
        {
            _sentMessages = sentMessages;
        }

        public void SendMessage(string Message)
        {
            _sentMessages.Add(Message);
        }

        //rest of non-implemented methods
    }
}
