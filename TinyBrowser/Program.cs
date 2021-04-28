using System;
using System.Net.Sockets;
using System.Text;

namespace TinyBrowser {
    public static class Program {
        static void Main(string[] arguments) {
            // Connect to acme.com Server via TCP
            var hostName = "example.com";
            var port = 80;
            var tcpClient = new TcpClient(hostName, port);
            var stream = tcpClient.GetStream();
            // Get the bytes for our HTTP Request
            var bytes = Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\nHost: example.com\r\n\r\n");
            // Send them over TCP
            stream.Write(bytes);
            var buffer = new byte[1024*2];
            var response = stream.Read(buffer, 0, buffer.Length);
            Console.Write("Server said: "+Encoding.ASCII.GetString(buffer));
        }
    }
}