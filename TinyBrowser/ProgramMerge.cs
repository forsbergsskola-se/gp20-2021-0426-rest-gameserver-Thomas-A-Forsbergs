using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace TinyBrowser {
    static class Program {
        const int port = 80;
        const string version = "1.1";
        static bool shouldRun = true;
        static string hostname = "acme.com";
        static string url = $"http://{hostname}/";
        static List<string> history = new List<string>();
        static int historyIndexer;
        static bool newPage = true;
        static bool isPrintResults = true;

        static void Main(string[] args) {
            while (shouldRun) {
                RunningURL();
            }
        }

        static void RunningURL() {
            if (newPage) {
                history.Add(url);
                historyIndexer = history.Count - 1;
            }

            var targetUrl = history[historyIndexer];
            var isExternalUrl = IsExternalLink(targetUrl, out var newHost, out var newUrl);
            if (isExternalUrl) {
                hostname = newHost;
                targetUrl = newUrl;
            }

            using var client = new TcpClient();
            client.ReceiveTimeout = 2000;
            client.Connect(hostname, port);
            var data = ConnectToClient(client, targetUrl);
            client.Close();

            var title = ExtractHeading(data, "Title");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Blue;
            if (isPrintResults) Console.WriteLine($"Webpage Title: {title}");
            if (isPrintResults) Console.WriteLine("Links");
            var urls = ExtractHyperLinks(data);

            Console.ForegroundColor = ConsoleColor.Cyan;
            for (int i = 0; i < urls.Count; i++) {
                if (isPrintResults) {
                    var padRight = (i + ":").PadRight(3);
                    Console.WriteLine($"{padRight} {PrettifyPrint(urls[i][0].TrimStart().TrimEnd())} ({urls[i][1]})");
                }

                urls[i][1] = NormalizeUrl(urls[i][1], hostname);
            }
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            var userChoiceString = "Enter a number from list or choose any of following:     " +
                                   "b (Go back)     f (Go forward)     h (Show history)     g (Goto (Link))     q (Quit browsing)"; 
            Console.WriteLine(userChoiceString);
            ReadUserInput(urls);
        }

        static string ConnectToClient(TcpClient client, string targetUrl) {
            using var networkStream = client.GetStream();
            networkStream.ReadTimeout = 2000;
            using var writer = new StreamWriter(networkStream);

            var request = RequestLine(version, hostname, targetUrl);

            var bytes = Encoding.UTF8.GetBytes(request);
            networkStream.Write(bytes, 0, bytes.Length);
            using var reader = new StreamReader(networkStream, Encoding.UTF8);
            var data = reader.ReadToEnd();
            return data;
        }

        static void ReadUserInput(List<string[]> urls) {
            var userInput = Console.ReadLine()?.ToLower();
            newPage = int.TryParse(userInput, out var linksIndex);
            isPrintResults = !Equals(userInput, "h");
            switch (userInput) {
                default:
                    if (newPage) url = urls[linksIndex][1];
                    break;
                case "h":
                    for (var i = 0; i < history.Count; i++) {
                        var pointer = historyIndexer == i ? "==>" : "   ";
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{pointer} History({i}): {history[i]}");
                    }
                    break;
                case "b":
                    if (historyIndexer > 0) historyIndexer--;
                    break;
                case "f":
                    if (history.Count - 1 > historyIndexer) historyIndexer++;
                    break;
                case "g":
                    Console.WriteLine("Please type in a link (E.g \"http://www.mycoollink.now\")");
                    url = Console.ReadLine();
                    newPage = true;
                    break;
                case "q":
                    shouldRun = false;
                    break;
            }
        }

        static List<string[]> ExtractHyperLinks(string html) {
            List<string[]> list = new List<string[]>();
            var regex = new Regex("<a href=[\"|'](?<link>.*?)[\"|'].*?>(<b>|<img.*?>)?(?<name>.*?)(</b>)?</a>",
                RegexOptions.IgnoreCase);
            if (!regex.IsMatch(html)) return list;
            foreach (Match match in regex.Matches(html))
                list.Add(new[] {match.Groups["name"].Value, match.Groups["link"].Value});
            return list;
        }

        static string ExtractHeading(string data, string info) {
            return Regex.Match(data, $"<{info}>(?<Info>.*?)</{info}>", RegexOptions.IgnoreCase).Groups["Info"].Value;
        }

        static string RequestLine(string httpVersion, string host, string url) {
            var request = $"GET {url}";
            switch (httpVersion) {
                case "0.9":
                    request += "\r\n";
                    break;
                case "1.1":
                    request += $" HTTP/{httpVersion}" +
                               "\r\nAccept: text/html, charset=utf-8" +
                               "\r\nAccept-Language: en-US" +
                               "\r\nUser-Agent: C# program" +
                               "\r\nConnection: close" +
                               $"\r\nHost: {host}" + "\r\n\r\n";
                    break;
            }

            return request;
        }

        static bool IsExternalLink(string targetLink, out string host, out string localLink) {
            host = string.Empty;
            localLink = targetLink;
            if (!targetLink.Contains("//")) return false;
            host = Regex.Match(targetLink, "//(www.|WWW.)?(?<host>.*?)/", RegexOptions.IgnoreCase).Groups["host"].Value;
            if (targetLink.StartsWith("//"))
                localLink = localLink.Remove(0, 2);
            return true;

        }

        static string NormalizeUrl(string targetLink, string hostName) {
            var tar = targetLink.StartsWith("/") ? targetLink : "/" + targetLink;
            if (!(targetLink.StartsWith("//") | targetLink.StartsWith("http")))
                return $"http://{hostName}{tar}";
            return targetLink;
        }

        static string PrettifyPrint(string s) {
            return s.Length > 15 ? string.Concat(s.Substring(0, 6), "...", s.Substring(s.Length - 7, 6)) : s.PadRight(15);
        }
    }
}