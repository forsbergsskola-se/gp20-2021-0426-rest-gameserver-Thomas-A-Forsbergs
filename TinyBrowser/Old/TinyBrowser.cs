using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

// ReSharper disable StringIndexOfIsCultureSpecific.1

namespace TinyBrowser.Old {
    public static class TinyBrowser {
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
            const string host = "example.com";
            const int port = 80;
            const string uri = "/";
            var tcpClient = new TcpClient(host, port);
            var stream = tcpClient.GetStream();
            var streamWriter = new StreamWriter(stream, Encoding.ASCII);
            
            var request = $"GET {uri} HTTP/1.1\r\nHost: {host}\r\n\r\n";
            streamWriter.Write(request);
            streamWriter.Flush();
            
            var streamReader = new StreamReader(stream);
            var response = streamReader.ReadToEnd();
            
            var uriBuilder = new UriBuilder(null, host);
            uriBuilder.Path = uri;
            Console.WriteLine($"Opened {uriBuilder}");
            
            const string titleTag = "<title>";
            var titleIndex = response.IndexOf(titleTag);
            string title = string.Empty;
            if (titleIndex != -1) {
                titleIndex += titleTag.Length;
                const string titleEndTag = "</title>";
                var titleEndIndex = response.IndexOf(titleEndTag);
                if (titleEndIndex > titleIndex) {
                    title = response[titleIndex..titleEndIndex];
                }
            }

            // Get the bytes for our HTTP Request
            Console.WriteLine("Title: " + title);
        }
    }
}