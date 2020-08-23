using System;
using System.IO;

namespace TDSStudios.TomScript.Core
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputFile">The input file</param>
        static void Main(string inputFile = "sample.tms")
        {
            var input = File.ReadAllText(inputFile);

            var compiler = new TomScriptCompiler(input, true);
            string output = compiler.Compile();

            File.WriteAllText("output.py", output);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nCompiled code:");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{output}\n");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Thanks for using TomScript!");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
