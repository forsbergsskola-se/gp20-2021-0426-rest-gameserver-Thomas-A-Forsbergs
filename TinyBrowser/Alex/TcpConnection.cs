using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TinyBrowser.Alex{
    public class TcpConnection{
        Dictionary<int, string> sitesDictionary = new();

        const string MainSite = "www.Acme.com";
        const int Port = 80;

        string linkToOpen = "";
        readonly UserInput userInput;

        public TcpConnection(){
            this.userInput = new UserInput();
        }

        public async void ConnectToSite(){
            this.sitesDictionary[0] = "";
            while (true){
                var valueFromWeb = await SendRequest();

                if (Utilities.IsBadRequest(valueFromWeb)){
                    Utilities.PrintOutSites(this.sitesDictionary);
                    CustomOutputs.ConsoleWriteLine("Bad request, try again...", ConsoleColor.Red);
                }
                else{
                    SitesToDictionary(valueFromWeb);
                    Utilities.PrintOutSites(this.sitesDictionary);
                }

                if (this.userInput.GetUserChoice(out var value, this.sitesDictionary)){
                    this.linkToOpen = this.sitesDictionary[(int) value];
                }
                else{
                    this.linkToOpen = (string) value;
                    Console.WriteLine($"Out value is: {this.linkToOpen}");
                }
            }
        }

        async Task<string> SendRequest(){
            var tcpClient = new TcpClient(MainSite, Port);
            var stream = tcpClient.GetStream();
            await WriteToSite(stream);
            var bytes = new byte[tcpClient.ReceiveBufferSize];
            var receivedBytesLength = await stream.ReadAsync(bytes);
            var valueFromWeb = Encoding.Default.GetString(bytes).Remove(receivedBytesLength);
            stream.Close();
            return valueFromWeb;
        }

        async Task WriteToSite(Stream stream){
            await stream.WriteAsync(Encoding.Default.GetBytes(Utilities
                .Builder(MainSite, this.linkToOpen).ToString()));
        }

        void SitesToDictionary(string valueFromWeb){
            this.sitesDictionary = new Dictionary<int, string>{[0] = ""};
            var textStartIndex = 0;
            var txtStartIndex = 0;
            var siteIndex = 1;
            var title = Utilities.FindTextBetween(valueFromWeb, "<title>", "</title>", ref textStartIndex);

            this.sitesDictionary[siteIndex] = title;

            while (true){
                var foundLink = Utilities.FindTextBetween(valueFromWeb, "<a href=\"", "\"", ref txtStartIndex);

                if (foundLink == "") break;
                if (foundLink.Contains("mailto") || foundLink.Contains("https")) continue;

                if (foundLink.StartsWith('/')){
                    foundLink = foundLink.Substring(1);
                }

                siteIndex++;
                if (foundLink.StartsWith("http")){
                    this.sitesDictionary[siteIndex] = $"{foundLink}";
                    continue;
                }

                this.sitesDictionary[siteIndex] = $"{foundLink}/";
            }
        }
    }
}