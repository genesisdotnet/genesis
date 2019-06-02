using System;
using System.Collections.Generic;
using System.Text;

namespace Genesis
{
    /// <summary>
    /// Console text color helpers, just reduces the amount of code needed to interpolate colors
    /// </summary>
    public static class Text
    {
        public static void White(string message = "", params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.White;

            if (parameters is null) { Console.Write(message); }
            else { Console.Write(message, parameters); }

            Console.ResetColor();
        }
        public static void WhiteLine(string message = "", params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.White;

            if (parameters is null) { Console.WriteLine(message); }
            else { Console.WriteLine(message, parameters); }

            Console.ResetColor();
        }

        public static void Cyan(string message = "", params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            if (parameters is null) { Console.Write(message); }
            else { Console.Write(message, parameters); }

            Console.ResetColor();
        }
        public static void CyanLine(string message = "", params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            if (parameters is null) { Console.WriteLine(message); }
            else { Console.WriteLine(message, parameters); }

            Console.ResetColor();
        }

        public static void Red(string message = "", params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            if (parameters is null) { Console.Write(message); }
            else { Console.Write(message, parameters); }

            Console.ResetColor();
        }
        public static void RedLine(string message = "", params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            if (parameters is null) { Console.WriteLine(message); }
            else { Console.WriteLine(message, parameters); }

            Console.ResetColor();
        }

        public static void Green(string message = "", params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            if (parameters is null) { Console.Write(message); }
            else { Console.Write(message, parameters); }

            Console.ResetColor();
        }
        public static void GreenLine(string message = "", params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            if (parameters is null) { Console.WriteLine(message); }
            else { Console.WriteLine(message, parameters); }

            Console.ResetColor();
        }

        public static void Blue(string message = "", params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.Blue;

            if (parameters is null) { Console.Write(message); }
            else { Console.Write(message, parameters); }

            Console.ResetColor();
        }
        public static void BlueLine(string message = "", params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.Blue;

            if (parameters is null) { Console.WriteLine(message); }
            else { Console.WriteLine(message, parameters); }

            Console.ResetColor();
        }

        public static void Yellow(string message = "", params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            if (parameters is null) { Console.Write(message); }
            else { Console.Write(message, parameters); }

            Console.ResetColor();
        }
        public static void YellowLine(string message = "", params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            if (parameters is null) { Console.WriteLine(message); } //NOTE: Console probably handles this
            else { Console.WriteLine(message, parameters); }

            Console.ResetColor();
        }
        public static void DarkYellow(string message = "", params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            if (parameters is null) { Console.Write(message); }
            else { Console.Write(message, parameters); }

            Console.ResetColor();
        }
        public static void DarkYellowLine(string message = "", params string[] parameters)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;

            if (parameters is null) { Console.WriteLine(message); } //NOTE: Console probably handles this
            else { Console.WriteLine(message, parameters); }

            Console.ResetColor();
        }
    }
}
