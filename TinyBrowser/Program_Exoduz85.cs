using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace TinyBrowser {
    class Program_Exoduz85 {
        const int port = 80;
        const string hostUrl = "acme.com";
        static readonly string request = $"GET / HTTP/1.1\r\nHost: {hostUrl}\r\n\r\n";
        static StreamReader sr;
        static StreamWriter sw;
        static NetworkStream stream;
        static TcpClient client;
        static bool shouldRun = false;

        public static void MainMethod() {

            shouldRun = true;

            while (shouldRun) {
                client = new TcpClient(hostUrl, port);
                stream = client.GetStream();
                sr = new StreamReader(stream);
                var encoding = Encoding.ASCII.GetBytes(request);
                stream.Write(encoding, 0, encoding.Length);
                var response = sr.ReadToEnd();
                var header = ExtractHeader(response);
                var links = ExtractLinks(response);
                Console.WriteLine(header);
                for (var index = 0; index < links.Count; index++) {
                    var str = links[index];
                    Console.WriteLine($"{index}: {str[0]} ({PrittyfyString(str[1])})");
                }
                client.Close();
                Console.WriteLine("Which link do you want to follow? (#)");
                if (int.TryParse(Console.ReadLine(), out var userInput)) {
                    
                }
            }
        }
        public static List<string[]> ExtractLinks(string pageResponse) {
            List<string[]> hyperLinks = new List<string[]>();
            var regex = new Regex("<a href=[\"|'](?<link>.*?)[\"|'].*?>(<b>|<img.*?>)?(?<name>.*?)(</b>)?</a>", 
                RegexOptions.IgnoreCase);
            if (!regex.IsMatch(pageResponse)) return hyperLinks;
            foreach(Match match in regex.Matches(pageResponse))
                hyperLinks.Add(new []{match.Groups["name"].Value, match.Groups["link"].Value});
            return hyperLinks;
        }
        public static string ExtractHeader(string str) {
            var first = str.IndexOf("<title>", StringComparison.OrdinalIgnoreCase) + 7;
            var last = str.LastIndexOf("</title>", StringComparison.OrdinalIgnoreCase);
            return str[first..last];
        }
        public static string PrittyfyString(string makePritty) {
            return makePritty.Length > 15 ? String.Concat(makePritty.Substring(0, 6),
                "...",
                makePritty.Substring(makePritty.Length - 7, 6)) : makePritty;
        }
    }
}