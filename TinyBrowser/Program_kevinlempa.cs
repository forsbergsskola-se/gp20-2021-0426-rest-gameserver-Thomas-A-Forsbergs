using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace TinyBrowser {
    class Program_kevinlempa {
        static List<int> GetIndexList(String str, String s) {
            var list = new List<int>();
            bool flag = false;
            for (int i = 0; i < str.Length - s.Length + 1; i++) {
                if (str.Substring(i,
                        s.Length)
                    .Equals(s)) {
                    list.Add(i);
                    flag = true;
                }
            }

            if (!flag) {
                Console.WriteLine("No occurence found!");
            }

            return list;
        }


        public static void MainMethod() {
            GrabHtml("", "");
        }

        static void GrabHtml(string current, string request) {
            string currentRequest = request;
            int key;
            List<int> indices = new List<int>();
            TcpClient client = new TcpClient();
            byte[] bytes = new byte[11000];
            string html = "";
            var pattern = @"//";
            var rgx = new Regex(pattern);

            var regResult = rgx.Replace(currentRequest, "", 1);
            if (regResult.StartsWith("/")) {
                regResult = regResult.Remove(0, 1);
            }

            while (true) {
                client.Connect("www.acme.com", 80);
                if (client.Connected) {
                    var stream = client.GetStream();
                    var buffer = Encoding.ASCII.GetBytes($"GET /{regResult} HTTP/1.1" + Environment.NewLine +
                                                         "Host: acme.com" +
                                                         Environment.NewLine + Environment.NewLine);
                    Console.WriteLine(regResult);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Read(bytes, 0, bytes.Length);
                    html = Encoding.ASCII.GetString(bytes);
                    stream.Flush();
                    stream.Close();
                    client.Close();
                }

                break;
            }

            var from = html.IndexOf("<title>") + "<title>".Length;
            var to = html.LastIndexOf("</title>");

            string title = new string("");
            if (from - to < 0)
                title = html.Substring(from, to - from);
            var list = GetIndexList(html, "<a href=\"");
            var list2 = GetIndexList(html, "</a>");
            List<string> hrefList = new List<string>();
            List<string> URLs = new List<string>();
            List<string> displayNames = new List<string>();
            Console.WriteLine(title);
            for (int i = 0;
                i < list.Count;
                i++) {
                hrefList.Add(html.Substring(list[i] + 9, list2[i] - list[i]));
            }

            foreach (var href in hrefList) {
                var index = href.IndexOf("\"");
                URLs.Add(href.Substring(0, index));
                var index2 = href.IndexOf("\">") + 2;
                var index3 = href.IndexOf("</a>");
                displayNames.Add(href.Substring(index2, index3 - index2));
            }

            for (int i = 0;
                i < URLs.Count;
                i++) {
                Console.WriteLine($"{i} {displayNames[i]} \n({URLs[i]})");
            }

            var s = Console.ReadLine();
            key = Convert.ToInt32(s);
            if (key < URLs.Count) {
                GrabHtml(request, URLs[key]);
            }
        }
    }
}