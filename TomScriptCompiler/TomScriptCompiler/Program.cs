using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.CommandLine;
using System.CommandLine.Binding;
using System.IO;

namespace TomScriptCompiler
{
    static class Program
    {
        /// <summary>
        /// TomScript Compiler compiles TomScript code into executable Python
        /// </summary>
        /// <param name="compile">Whether the compiler instantly compiles, or shows the UI</param>
        /// <param name="outputDir">The directory to output files to</param>
        /// <param name="source">The source TomScript file(s) for compilation</param>
        /// <param name="outputExe">Whether the compiler should also generate an executable exe file</param>
        [STAThread]
        static void Main (bool compile = false, string[] source = null, string outputDir = null, bool outputExe = false)
        {
            if (compile)
            {
                // The user has set the commandline argument to compile instantaneously

                for (int i = 0; i < source.Length; i++)
                {
                    new Compiler(
                        source[i],
                        Path.Combine(outputDir, Path.GetFileName(source[i]) + ".py"), 
                        outputExe ? Path.Combine(outputDir, Path.GetFileName(source[i]) + ".exe") : null
                    ).Compile();
                }

                // compiler.exe --compile --source s.tms --output-exe
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1(source, outputDir, outputExe));
            }
        }
    }
}
