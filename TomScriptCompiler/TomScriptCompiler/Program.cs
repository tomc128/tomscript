using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace TomScriptCompiler
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string source = null;
            string outputDir = null;
            bool outputExe = false;
            bool compile = false;
            if (args.Length >= 2)
            {
                source = args[0];
                outputDir = args[1];
                outputExe = args.Contains("--output-exe");
                compile = args.Contains("--compile");
            }

            if (compile)
            {

                new Compiler(
                    source,
                    Path.Combine(outputDir, Path.GetFileName(source) + ".py"),
                    outputExe ? Path.Combine(outputDir, Path.GetFileName(source) + ".exe") : null
                ).Compile();

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
