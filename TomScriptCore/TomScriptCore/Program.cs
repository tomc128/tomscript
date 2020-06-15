using System;
using System.IO;

namespace TDSStudios.TomScript.Core
{
    class Program
    {
        static string tomscriptCode = @"
               
            start

            set number to user input

            if number is even
            write number is even.
            else
            write number is odd.
            done

            end

            ";

        static void Main()
        {
            var compiler = new TomScriptCompiler(tomscriptCode, true) ;
            string output = compiler.Compile();

            File.WriteAllText("output.py", output);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\nCompiled code:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{output}\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Thanks for using TomScript!");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
