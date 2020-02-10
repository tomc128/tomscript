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
        /// TomScriptCompiler compiles TomScript into Python
        /// </summary>
        /// <param name="compile">Whether to instantaneously compile using arguments, or open up the UI and populate the controls</param>
        /// <param name="source">The source tms file(s)</param>
        /// <param name="outputDir">The directory to output all compiled files to</param>
        /// <param name="outputExe">Whether to build an exe in addition to the py file</param>
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
