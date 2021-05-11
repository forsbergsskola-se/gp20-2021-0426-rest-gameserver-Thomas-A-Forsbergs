using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace TinyBrowser
{
    class Program_Sopuffer
    {
        public static void MainMethod()
        {
            Start();
            Console.ReadKey();
        }
        static void Start()
        {
            var tcpClient = new TcpClient("acme.com", 80);
            var stream = tcpClient.GetStream();

            var bytes = Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\nHost: acme.com\r\n\r\n");
            stream.Write(bytes);
           
            string httpRequestTitle = Encoding.ASCII.GetString(bytes);
            Console.WriteLine(httpRequestTitle);

            byte[] resultBytes = new byte[124 * 124];
            var totalBytesReceived = 0;
            var bytesReceived = 1;
            while (bytesReceived != 0)
            {
                bytesReceived = stream.Read(resultBytes, totalBytesReceived, resultBytes.Length - totalBytesReceived);
                totalBytesReceived += bytesReceived;
            }
            string website = Encoding.ASCII.GetString(resultBytes, 0, totalBytesReceived);

            GetTitleOfWebsite(website, "<title>", "</title>");
            GetHRefConnections(website);


            tcpClient.Close();
            stream.Close();
        }

        public static void GetTitleOfWebsite(string text, string firstString, string lastString)
        {
            string content = text;
            string STRFirst = firstString;
            string STRLast = lastString;

            int Pos1 = content.IndexOf(STRFirst) + STRFirst.Length;
            int Pos2 = content.IndexOf(STRLast);
            string FinalString = content.Substring(Pos1, Pos2 - Pos1);
            Console.WriteLine("Title: "  + FinalString+  "\r\n\r\n");
        }
        
        
        
        public static void GetHRefConnections(string website)
        {
            Regex regex = new Regex("href\\s*=\\s*(?:\"(?<1>[^\"]*)\"|(?<1>\\S+))", RegexOptions.IgnoreCase);
            Match match;
            List<Group> options = new List<Group>();
            for (match = regex.Match(website); match.Success; match = match.NextMatch())
            {
                options.Add(match);
                Console.WriteLine(options.IndexOf(match));
                Console.WriteLine("Found a href connection. The groups in the href are: ");
                foreach (Group group in match.Groups)
                { 
                    Console.WriteLine("Group value: {0}", group);
                }
                Console.WriteLine("\r\n\r\n");
            }


            ConnectToHref(options);
        }

        public static void ConnectToHref(List<Group> hrefs)
        {
            Console.WriteLine("There are " + hrefs.Count + " website connections, with the first connection being at stored at index 0.");
    
            while (true)
            {
                Console.WriteLine("Please write a number to pick one of these indexes: ");

                string c = Console.ReadLine();
                int value;
                
                if (int.TryParse(c, out value)) {
                    if (value < hrefs.Count && value >= 0)
                    {
                        ExtractHref(hrefs, value);
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Error! Number is out of range. Please try again.");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("Error! This is not a number! Please try again.");
                    continue;
                }
            }
        }

        public static void ExtractHref(List<Group> hrefs, int chosenNumber)
        {
            Console.WriteLine("Chosen number: " + chosenNumber);
            for (int i = 0; i <= hrefs.Count; i++)
            {
                Group chosenConnection = hrefs[chosenNumber];
                EnterHrefTcpClient(chosenConnection);
                break;
            }
        }

        public static void EnterHrefTcpClient(Group hrefConnection)
        {
            var connection = hrefConnection.ToString();
            int i;
            string tcpName = connection.Substring(connection.IndexOf('"') +1);
            tcpName = tcpName.Remove(tcpName.Length - 1);
            Console.WriteLine(tcpName);
            string[] websitenames = { "google", "paypal", "mapper.acme", "99dogs", "validator" };  
                for (i = 0; i <= websitenames.Length; i++)
                {
                    if (tcpName.Contains(websitenames[i]))
                    {
                        switch (i)
                        {
                            case 0:
                                    var tcpClient = new TcpClient("google.com", 80);
                                    var stream = tcpClient.GetStream();

                                    var bytes = Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\nHost: www.google.com\r\n\r\n");
                                    stream.Write(bytes);
                                    string httpRequestTitle = Encoding.ASCII.GetString(bytes);
                                    Console.WriteLine(httpRequestTitle);
                                    byte[] resultBytes = new byte[224 * 224];
                                    var totalBytesReceived = 0;
                                    var bytesReceived = 1;
                                    while (bytesReceived != 0)
                                    {
                                        bytesReceived = stream.Read(resultBytes, totalBytesReceived, resultBytes.Length - totalBytesReceived);
                                        totalBytesReceived += bytesReceived;
                                        string website = Encoding.ASCII.GetString(resultBytes, 0, totalBytesReceived);
                                    }

                                    tcpClient.Close();
                                    stream.Close();
                            break;

                            case 1:
                                    var secondtcpClient = new TcpClient("paypal.com", 80);
                                    var secondstream = secondtcpClient.GetStream();

                                    var secondbytes = Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\nHost: www.paypal.com\r\n\r\n");
                                    secondstream.Write(secondbytes);
                                    string secondhttpRequestTitle = Encoding.ASCII.GetString(secondbytes);
                                    Console.WriteLine(secondhttpRequestTitle);
                                    byte[] secondresultBytes = new byte[124 * 124];
                                    var secondtotalBytesReceived = 0;
                                    var secondbytesReceived = 1;
                                    while (secondbytesReceived != 0)
                                    {
                                        bytesReceived = secondstream.Read(secondresultBytes, secondtotalBytesReceived, secondresultBytes.Length - secondtotalBytesReceived);
                                        secondtotalBytesReceived += bytesReceived;
                                        string website = Encoding.ASCII.GetString(secondresultBytes, 0, secondtotalBytesReceived);
                                    }       

                                    secondtcpClient.Close();
                                    secondstream.Close();
                                break;

                            case 2:
                                    var thirdtcpClient = new TcpClient("mapper.acme.com", 80);
                                    var thirdstream = thirdtcpClient.GetStream();

                                    var thirdbytes = Encoding.ASCII.GetBytes("GET / HTTP/1.1\r\nHost: mapper.acme.com\r\n\r\n");
                                    thirdstream.Write(thirdbytes);
                                    string thirdhttpRequestTitle = Encoding.ASCII.GetString(thirdbytes);
                                    Console.WriteLine(thirdhttpRequestTitle);
                                    byte[] thirdresultBytes = new byte[224 * 224];
                                    var thirdtotalBytesReceived = 0;
                                    var thirdbytesReceived = 1;
                                    while (thirdbytesReceived != 0)
                                    {
                                        bytesReceived = thirdstream.Read(thirdresultBytes, thirdtotalBytesReceived, thirdresultBytes.Length - thirdtotalBytesReceived);
                                        thirdtotalBytesReceived += bytesReceived;
                                        string website = Encoding.ASCII.GetString(thirdresultBytes, 0, thirdtotalBytesReceived);
                                
                                    }

                                    thirdtcpClient.Close();
                                    thirdstream.Close();
                             break;


                        }
                    }

                }
              {
             
                
            }

        }
    }
}