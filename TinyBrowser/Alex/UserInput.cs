using System;
using System.Collections.Generic;
using System.Globalization;

namespace TinyBrowser.Alex{
    public class UserInput{
        Stack<string> back = new();
        Stack<string> forward = new();
        string currentSite = "";

        public bool GetUserChoice(out object value, Dictionary<int, string> sitesDictionary){
            while (true){
                CustomOutputs.ConsoleWriteLine(
                    $"Type in a number between 0-{sitesDictionary.Count - 1}, or type \"b\" to go back or \"f\" to go forward",
                    ConsoleColor.Green);
                var userInput = CustomOutputs.ConsoleReadLine(ConsoleColor.Yellow);

                if (IsNumeric(userInput, out var numericInput)){
                    if (IsValidNumber(ref numericInput, sitesDictionary)){
                        this.forward.Clear();
                        value = numericInput;
                        this.back.Push(this.currentSite);
                        Console.WriteLine($"Current site is: {this.currentSite}");
                        this.currentSite = sitesDictionary[numericInput];
                        this.forward.Push(this.currentSite);
                        Console.WriteLine($"Forward site: {this.forward.Peek()}  back site: {this.back.Peek()}");
                        return true;
                    }

                    Console.WriteLine("Wrong input, try again ");
                    continue;
                }

                if (IsValidCharacter(ref userInput)){
                    value = GetSite(userInput);
                    this.currentSite = (string) value;
                    return false;
                }

                break;
            }

            value = "";
            return default;
        }

        bool IsNumeric(string s, out int userChoice){
            return int.TryParse(s, NumberStyles.Integer, new NumberFormatInfo(), out userChoice);
        }

        bool IsValidNumber(ref int value, Dictionary<int, string> sitesDictionary){
            return value < 0 || value < sitesDictionary.Count - 1;
        }

        bool IsValidCharacter(ref string value){
            return value is "b" or "f";
        }

        string GetSite(string value){
            //Console.WriteLine($"Forward site: {this.forward.Peek()}  back site: {this.back.Peek()}");
            if (value == "b" && this.back.Count > 0){
                this.forward.Push(this.currentSite);
                return this.back.Pop();
            }

            if (this.forward.Peek() == this.currentSite){
                return this.forward.Peek();
            }

            if (value == "f" && this.forward.Count > 0){
                Console.WriteLine("should not be here");
                this.back.Push(this.currentSite);
                return this.forward.Pop();
            }

            return "";
        }
    }
}