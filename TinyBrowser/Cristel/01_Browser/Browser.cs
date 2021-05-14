using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using TinyBrowser.Cristel._02_LinksAndTitles;

namespace TinyBrowser.Cristel._01_Browser {
    public static class Browser {
        static TcpClient tcpClient;
        static StreamReader streamReader;
        static StreamWriter streamWriter;
        static NetworkStream networkStream;
        static string host = "www.acme.com";
        static int port = 80;
        static AllLinksAndTitles[] links;
        static string path = "/";

        public static void BrowserRun() {
            while (true) {
                try {
                    ConnectViaTcp();
                    RequestWebsite();

                    var getResponse = WebsiteResponse();
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("You are now at the page with the title: ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(DisplayWebsitesTitle(getResponse));

                    links = FilterAllLinksWithTitles(getResponse).ToArray();
                    DisplayWebsitesLinks();
                    ReadUserInput();
                }
                catch (Exception exception) {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(exception.Message);
                }
            }
        }

        static void ConnectViaTcp() {
            tcpClient = new TcpClient(host, port);
            networkStream = tcpClient.GetStream();
            streamReader = new StreamReader(networkStream);
            streamWriter = new StreamWriter(networkStream);
            
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("You are now connected to: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(networkStream.Socket.RemoteEndPoint);
        }
        
        static void RequestWebsite() {
            var request =  $"GET {path} HTTP/1.1\r\n";
            request += $"Host: {host}\r\n\r\n";

            streamWriter.AutoFlush = true;
            streamWriter.Write(request);
        }
        
        static string WebsiteResponse() {
            if (networkStream.CanRead) {
                var response = streamReader.ReadToEnd();
                return response;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: Cannot read this data...");
            return string.Empty;
        }
        
        static string DisplayWebsitesTitle(string response) {
            const string startsWith = "<title>";
            const string endsWith = "</title>";

            var startsAtIndex = response.IndexOf(startsWith, StringComparison.OrdinalIgnoreCase);
            startsAtIndex += startsWith.Length;
            var endsAtIndex = response.IndexOf(endsWith, StringComparison.OrdinalIgnoreCase);

            return response[startsAtIndex..endsAtIndex];
        }

        static void DisplayWebsitesLinks(){
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Displaying all links found on the page: ");
            if (links.Length > 0){
                for (var i = 0; i < links.Length; i++){
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{i}: {links[i].displayLinksText} ({links[i].hyperlinks})");
                }   
            }
            else{
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No links found");
            }
        }

        static void ReadUserInput() {
            var userInputIsValid = false;
            var userNumberChoice = 0;
            
            while(!userInputIsValid && links.Length > 0) {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Type a number from (0-{links.Length-1}) to go to that link");
                
                userInputIsValid = int.TryParse(Console.ReadLine(), out userNumberChoice);
                
                if (userNumberChoice >= 0 && userNumberChoice <= links.Length -1) {
                    userInputIsValid = true;
                    continue;
                }

                userInputIsValid = false;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("The number you typed is not on the list\nPlease type another one");
                Console.ReadLine();

                DisplayWebsitesLinks();
            }

            if (links.Length < 1) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("This page has no links");
            }
            
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("You have typed: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{userNumberChoice}\n{links[userNumberChoice].displayLinksText}");
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Please confirm you want to go to that number / link by pressing any key");
            Console.ReadKey();
            
            
            Console.Read();
            path = links[userNumberChoice].hyperlinks;
            path = path.TrimStart('/');
            path = "/" + path;
        }
        
        
        static IEnumerable<AllLinksAndTitles> FilterAllLinksWithTitles(string response) {
            var linkTag = "<a href=\"";
            var quotationMark = '"';
            var linkIndexStarts = '>';
            var linkIndexEnds = "</a>";
            var allLinksList = new List<AllLinksAndTitles>();
            
            var arrayFilter = response.Split(linkTag);
            arrayFilter = arrayFilter.Skip(1).ToArray();
            
            foreach (var dataFiltered in arrayFilter){
                var hyperlink = dataFiltered.TakeWhile(symbol => symbol != quotationMark).ToArray();
                var filterAfterHyperlink = dataFiltered[hyperlink.Length..];
                var filteredDataStartsAt = filterAfterHyperlink.IndexOf(linkIndexStarts) + 1;
                var filteredDataEndsAt = filterAfterHyperlink.IndexOf(linkIndexEnds, StringComparison.Ordinal);
                var dataToDisplay = 
                    filterAfterHyperlink.Substring(filteredDataStartsAt,(filteredDataEndsAt - filteredDataStartsAt))
                    .Replace("<b>", string.Empty).Replace("</b>", string.Empty);
                if (dataToDisplay.StartsWith("<img")){
                    dataToDisplay = string.Empty;
                }
                allLinksList.Add(new AllLinksAndTitles{
                    hyperlinks = new string(hyperlink),
                    displayLinksText = new string(dataToDisplay)
                });
            }
            return allLinksList;
        }
    }
}
