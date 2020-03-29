using System;

namespace TDSStudios.TomScript.Core
{
    class Program
    {
        static void Main(string[] args)
        {
            var compiler = new TomScriptCompiler("start\nwrite Hello, World!\nend");
            string output = compiler.Compile();

            Console.WriteLine("\n\nCode: ---\n" + output);
        }
    }
}
