using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace TinyBrowser
{
    public struct HostAndPath
    {
        public HostAndPath(string host, string path)
        {
            HostName = host;
            PathName = path;
        }
        public string HostName {get; set; }
        public string PathName {get; set; }
    }
    
    class Program_jor_el_swe {
        static TcpClient tcpClient;
        static NetworkStream netStream;
        static readonly List<string> linkNames = new List<string>();
        static readonly List<string> hyperLinks = new List<string>();
        const int MAX_LINKNAME_LENGHT = 50;


        private const string InitialHostName = "www.acme.com";
        private const string InitialPathName = "/";
        
        private static int currentPathIndex=0;
        static List<HostAndPath> pathHistory = new List<HostAndPath>();
        private static HostAndPath currentHostandPath;
        
        const int tcpPort = 80;

        public static void MainMethod() {
            pathHistory.Add(new HostAndPath(InitialHostName, InitialPathName));
            currentHostandPath = pathHistory[currentPathIndex];
            RunMainLoop();
        }

        static void RunMainLoop() {
            while (true) {
                InitClient();
                
                SendGetRequest();
                
                var stringToParse = GetResponseFromWebSite();

                FindAllLinks(stringToParse);

                PrintAllLinks();

                AskUserForLink();
                
                CloseStreamAndTCPClient();
            }
        }

        static void AskUserForLink() {
            /*- Ask the user for Input
                - it should be a Number between 0 and the number of options
                - Follow the link that the user wants to follow and start at the beginning of the application again
                - (Send a TCP Request to acme.com...)
                
                Also allow for 
                b – back
                f – forward
                r – refresh
                h – history
                g – goto [user enter URL]
            */
           
            var isUserInputValid = false;
            while (!isUserInputValid)
            {
                Console.Write("Which link do you want to follow: ");
                var userInput = IsUserInputCorrectNumber(out isUserInputValid, out var linkNumber);
                
                //we received a correct link number
                if (isUserInputValid)
                {
                    currentHostandPath = TrimPathName(hyperLinks[linkNumber]);
                   
                }
                //we did not receive a valid number.
                //let's try with one of the letter options
                else
                {
                    isUserInputValid = true;
                    switch (userInput)
                    {
                        case "r":
                        case "R":
                            //on refresh, we don't need to to anything. 
                            //just keep the current link path for next iteration
                            break;
                        case "f":
                        case "F":
                            //move forward
                            break;
                        case "b":
                        case "B":
                            //move backward
                            break;
                    
                        case "h":
                        case "H":
                            //show browser history
                        
                            break;
                            //goto user defined link
                        case "g":
                        case "G":
                        
                            break;

                        default: 
                            isUserInputValid = false;
                            break;
                    } 
                }
            }//while !valid input
            
            Console.WriteLine("you want :" + currentHostandPath.PathName);
            Console.WriteLine("press any key to follow that link");
            Console.ReadLine();
            
            pathHistory.Add(currentHostandPath);
            currentPathIndex++;
        }

        private static string IsUserInputCorrectNumber(out bool test, out int num)
        {
            var userInput = Console.ReadLine();
            test = int.TryParse(userInput, out num);
            if (num > hyperLinks.Count || num < 0) {
                Console.WriteLine("wrong number. press any key to continue");
                Console.ReadLine();
                PrintAllLinks();
                test = false;
            }

            return userInput;
        }

        private static HostAndPath TrimPathName(string hyperLink)
        {
            var pathName = hyperLink;
            pathName = pathName.TrimStart('/');
            pathName = "/" + pathName;
            return new HostAndPath(InitialHostName, pathName);
        }

        static void PrintAllLinks() {
            for (var i = 0; i < linkNames.Count; i++) {
                var iteratorString = i + ": ";
                Console.Write(iteratorString);
                Console.Write(linkNames[i]);
  
                var spaces = "";
                for (var j = 0; j < (MAX_LINKNAME_LENGHT-linkNames[i].Length-iteratorString.Length); j++) {
                    spaces += " ";
                }
                Console.Write(spaces);
                Console.WriteLine("(" +hyperLinks[i] + ")");
            }
        }

        static void SendGetRequest() {
            //- Write a valid HTTP-Request to the Stream.
            var request = "GET " + currentHostandPath.PathName +" HTTP/1.1\r\n";
            request += "Host:"+ currentHostandPath.HostName+"\r\n";
            request += "\r\n";
            netStream.Write(Encoding.ASCII.GetBytes(request));
        }

        static void FindAllLinks(string stringToParse) {
            hyperLinks.Clear();
            linkNames.Clear();

            Console.WriteLine("Opened: " + currentHostandPath.HostName + currentHostandPath.PathName);
            
            /*- Print that string (between `<title>` and `</title>`) to the console.*/
            var foundString = FindStringBetweenTwoStrings(stringToParse, "<title>", "</title>", 0,out var foundPosition);
            Console.WriteLine("Title: " + foundString);
            
            //Find all the hyperlinks
            var currentPosition = 0;
            while (true) {
                //find the hyperlink
                var hyperlink = FindStringBetweenTwoStrings(stringToParse, "<a href=\"", "\">", currentPosition, out currentPosition);
                if (currentPosition == -1)
                    break;
                
                //find the link display name
                var linkName = FindStringBetweenTwoStrings(stringToParse, "\">", "</a>", currentPosition, out currentPosition);
                if (linkName.Length > 50) continue;
                
                //store in lists here
                linkNames.Add(linkName);
                hyperLinks.Add(hyperlink);
            }
        }

        static string FindStringBetweenTwoStrings(string sourceString, string startString, string endString, int startAtPosition, out int foundPosition)
        {
            foundPosition = sourceString.IndexOf(startString, startAtPosition, StringComparison.OrdinalIgnoreCase);
            if (foundPosition == -1)
            {
                return "";
            }
            
            foundPosition += startString.Length;
            var pTo = sourceString.IndexOf(endString, foundPosition, StringComparison.OrdinalIgnoreCase);
            var result = sourceString[foundPosition..pTo];
            return result;
        }

        static string GetResponseFromWebSite() {
            if(netStream.CanRead){
                var streamReader = new StreamReader(netStream);
                var completeMessage = streamReader.ReadToEnd();
                
                return completeMessage;
            }
            Console.WriteLine("Sorry.  You cannot read from this NetworkStream.");
            return "";
        }

        static void CloseStreamAndTCPClient() {
            Console.WriteLine("closing stream and TPC client...");
            netStream.Close();
            tcpClient.Close();
            
        }

        static void InitClient() {
            tcpClient = new TcpClient(currentHostandPath.HostName, tcpPort);
            netStream = tcpClient.GetStream();
            Console.WriteLine("connected to: " + netStream.Socket.RemoteEndPoint);
        }
    }
}