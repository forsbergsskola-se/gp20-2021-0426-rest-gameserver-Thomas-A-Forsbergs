using System;

namespace TinyBrowser.Alex{
    public class CustomOutputs{
        public static void ConsoleWriteLine(string text, ConsoleColor color){
            Console.ForegroundColor = color;
            Console.WriteLine(text);
        }

        public static string ConsoleReadLine(ConsoleColor color){
            Console.ForegroundColor = color;
            return Console.ReadLine();
        }
        
        public static void ConsoleWriteLine(string text){
            Console.ResetColor();
            Console.WriteLine(text);
        }

        public static string ConsoleReadLine(){
            Console.ResetColor();
            return Console.ReadLine();
        }
    }
}