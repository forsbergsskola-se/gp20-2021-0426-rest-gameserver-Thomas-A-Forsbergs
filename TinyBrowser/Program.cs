using System;
using System.Net.Sockets;
using System.Text;

namespace TinyBrowser {
    public static class Program {
        static void Main(string[] arguments) {
            // Connect to acme.com Server via TCP
            const string hostName = "example.com";
            const int port = 80;
            var tcpClient = new TcpClient(hostName, port);
            var stream = tcpClient.GetStream();
            // Get the bytes for our HTTP Request
            var bytes = Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\nHost: "+hostName+"\r\n\r\n");
            // Send them over TCP
            stream.Write(bytes);
            var buffer = new byte[1024*2+10];
            var streamRead = stream.Read(buffer, 0, buffer.Length);
            var getStringBuffer = Encoding.ASCII.GetString(buffer);
            Console.Write("Server said: "+getStringBuffer);
            const string indexOfString = "<title>";
            // ReSharper disable once StringIndexOfIsCultureSpecific.1
            Console.WriteLine("Index Of: "+indexOfString+getStringBuffer.IndexOf(indexOfString));
        }
    }
}