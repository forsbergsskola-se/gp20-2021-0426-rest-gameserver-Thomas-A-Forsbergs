using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace TinyBrowser {
    public class Program_Pazoozoo {
        public static void MainMethod() {
            var host = "acme.com";
            var uri = "/";
            // Here, acme.com is only used for DNS-Resolving and gives us an IP
            var tcpClient = new TcpClient(host, 80);
            var stream = tcpClient.GetStream();
            var streamWriter = new StreamWriter(stream, Encoding.ASCII);
            // Here, acme.com is passed to the Webserver at the IP, so it knows, which website to give us
            // In case, that one computer hosts multiple websites (think of Web-Hosts like wix.com)
        
            /*
         * GET / HTTP.1.1
         * Host: acme.com
         * Content-Length: 7
         *
         * abcdefg
         */
        
            // This is a valid HTTP/1.1-Request to send:
            var request = $"GET {uri} HTTP/1.1\r\nHost: {host}\r\n\r\n";
            streamWriter.Write(request); // add data to the buffer
            streamWriter.Flush(); // actually send the buffered data

            var streamReader = new StreamReader(stream);
            var response = streamReader.ReadToEnd();

            var uriBuilder = new UriBuilder(null, host);
            uriBuilder.Path = uri;
            Console.WriteLine($"Opened {uriBuilder}");

            var titleText = FindTextBetweenTags(response, "title");
            Console.WriteLine("Title: " + titleText);
            // Console.WriteLine("response: " + response);

            var hrefLinks = GetTextBetweenStringsFromString(response, "<a href=\"", "\">");
            var hrefLinkList = hrefLinks.ToList();
            var hrefDescriptionList = new List<string>();

            for (var i = 0; i < hrefLinkList.Count; i++) {
                var description = GetTextBetweenStringsFromString(response, hrefLinkList[i] + "\">", "</a>");
                hrefDescriptionList.AddRange(description);
                
                Console.WriteLine($"{i}: {hrefDescriptionList[i]} ({hrefLinkList[i]})");
            }
        }
        
        static string FindTextBetweenTags(string original, string tag) {
            var startTag = $"<{tag}>";
            // Find the start of the <tag>
            var tagIndex = original.IndexOf(startTag);
            string result = string.Empty;
            if (tagIndex != -1) {
                // Offset the index by the length of the <tag>, to omit it
                tagIndex += startTag.Length;
                // Find the start of the </tag>-End
                var tagEndIndex = original.IndexOf($"</{tag}>");
                if (tagEndIndex > tagIndex) {
                    // Get the string in between both
                    result = original[tagIndex..tagEndIndex];
                }
            }
            return result;
        }
    
        static IEnumerable<string> GetTextBetweenStringsFromString(string text, string start, string end) {
            int currentIndex = 0;
            while (true) {
                var startIndex = text.IndexOf(start, currentIndex);
                if (startIndex == -1)
                    yield break;
                var endIndex = text.IndexOf(end, startIndex);
                if (endIndex == -1)
                    yield break;

                yield return text[(startIndex + start.Length)..endIndex];
                currentIndex = endIndex;
            }
        }
    }

// Extension Method
    public static class StringExtensions {
        public static string FindTextBetweenTags(this string original, string start, string end) {
            throw new NotImplementedException();
        }
    }
}