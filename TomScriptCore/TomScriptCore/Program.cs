using System;
using System.IO;

namespace TDSStudios.TomScript.Core
{
    class Program
    {
        static void Main()
        {
            var compiler = new TomScriptCompiler(@"
               
            start

            create variable number and set number to 0
        
            set number to user input

            if number is even
            write Your input is even.
            else
            write Your input is odd.
            done

            end

            ", true);
            string output = compiler.Compile();

            File.WriteAllText("output.py", output);

            Console.WriteLine($"\n\nCode: ---\n\n{output}\n\n---");
        }
    }
}
