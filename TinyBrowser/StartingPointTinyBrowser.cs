using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace TinyBrowser {
    public static class StartingPointTinyBrowser {
        static string FindTextBetweenTags(string original, string start, string end) {
            var titleIndex = original.IndexOf(start);
            var title = string.Empty;
            if (titleIndex != -1) {
                // Offset the index by the length of the <title>-Tag, to ommit it
                titleIndex += start.Length;
                // Find the start of the </title>-End-Tag
                var titleEndIndex = original.IndexOf(end);
                if (titleEndIndex > titleIndex) {
                    // Get the string in between both
                    title = original[titleIndex..titleEndIndex];
                }
            }

            return title;
        }

        public static void MainMethod() {
            var host = "acme.com";
            var uri = "/";
            // Here, acme.com is only used for DNS-Resolving and gives us an IP
            var tcpClient = new TcpClient(host, 80);
            var stream = tcpClient.GetStream();
            var streamWriter = new StreamWriter(stream, Encoding.ASCII);
        
            // This is a valid HTTP/1.1-Request to send:
            var request = $"GET {uri} HTTP/1.1\r\nHost: {host}\r\n\r\n";
            streamWriter.Write(request); // add data to the buffer
            streamWriter.Flush(); // actually send the buffered data

            var streamReader = new StreamReader(stream);
            var response = streamReader.ReadToEnd();

            var uriBuilder = new UriBuilder(null, host);
            uriBuilder.Path = uri;
            Console.WriteLine($"Opened {uriBuilder}");

            var titleText = FindTextBetweenTags(response, "<title>", "</title>");
            Console.WriteLine("Title: "+titleText);
        }
    }
}