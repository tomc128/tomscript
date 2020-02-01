using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TomScriptCompiler
{
    class Program
    {
        private const string identation = "    ";

        private static string sourceFile = @"E:\Repos\tomscript\tests\script-three.tms";
        private static string outputFile = @"E:\Repos\tomscript\tests\script-three.py";

        private static string languagePath = @"E:\Repos\tomscript\lang\";
        private static string language = "friendly";
        private static string source;
        private static Queue<string> tokenQueue = new Queue<string>();
        private static Dictionary<string, object> variables = new Dictionary<string, object>();

        private static List<string> identifiers = new List<string>()
        {
            "START_PROGRAM",
            "END_PROGRAM",
            "CREATE",
            "VARIABLE",
            "WRITE",
            "READ",
            "SET",
            "TO",
            "CALCULATE",
            "REPEAT",
            "FOREVER",
            "TIMES",
            "REPEAT_END",
            "|",
            "RESULT",
            "IF",
            "IF_ELSE",
            "IF_ELSE_IF",
            "IF_END",
            "IS_EQUAL_TO",
            "IS_GREATER_THAN",
            "IS_LESS_THAN"
        };
        private static Dictionary<string, List<string>> translations = new Dictionary<string, List<string>>();
        /*
         * {
         *      "standard": {
         *          "start",
         *          "end"
         *      }    
         * }
         * 
         */
        private static string generatedCode = "";


        static void Main(string[] args)
        {
            var startTime = DateTime.Now;
            LoadLanguages();
            ReadFile();
            IndentifierIdentification();
            TokeniseSource();
            PrintTokens();
            ParseTokens();
            GenerateCode();
            var endTime = DateTime.Now;
            var timeTaken = endTime - startTime;

            Console.WriteLine($"Compilation finished in {timeTaken.TotalMilliseconds}ms");
            Console.ReadLine();
        }

        static void LoadLanguages()
        {
            foreach (var file in new DirectoryInfo(languagePath).EnumerateFiles())
            {
                string[] lines = File.ReadAllLines(file.FullName);
                var language = new List<string>();

                for (int i = 0; i < lines.Length; i++)
                {
                    language.Add(lines[i].Split('=')[1]);
                }

                translations.Add(Path.GetFileNameWithoutExtension(file.FullName), language);
            }
        }

        static void ReadFile()
        {
            source = File.ReadAllText(sourceFile);

            Regex languageRegex = new Regex(@"language [a-zA-Z]+");
            foreach (var line in source.Split('\n'))
            {
                if (languageRegex.IsMatch(line)) language = languageRegex.Match(line).Value.Split(' ')[1];
            }

            print("Detected language: " + language);

            //source = source.Substring(source.IndexOf(translations[language][identifiers.IndexOf("START_PROGRAM")])) + "\n";
        }


        private static void IndentifierIdentification()
        {
            string newSource = source;
            foreach (var identifier in translations[language])
            {
                newSource = Regex.Replace(newSource, $@"(\s*){identifier}(\s+)", $"$1{identifiers[translations[language].IndexOf(identifier)]}$2");
            }
            source = newSource;
        }


        private static void TokeniseSource()
        {
            foreach (var line in source.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (var word in Regex.Split(line, @"(?<=[ ])"))
                {
                    if (string.IsNullOrEmpty(word.Trim())) continue;

                    // The line below was needed without the regex-powered identifier identification
                    // int index = translations[language].IndexOf(word.ToLower().Trim());

                    int index = identifiers.IndexOf(word.Trim());
                    tokenQueue.Enqueue(index != -1 ? identifiers[index] : word.Trim(new char[] { '\r' }));
                }
                tokenQueue.Enqueue("|");
            }
        }

        private static string ParseTokens(string prefix = "")
        {
            if (tokenQueue.Count == 0) return "";

            string token = tokenQueue.Dequeue();
            string code = prefix;

            print($"-> PARSING '{token}'");

            switch (token)
            {
                case "|":
                    break;

                case "WRITE":
                    token = tokenQueue.Dequeue();
                    code += "print('";

                    while (token != "|")
                    {
                        if (variables.ContainsKey(token))
                            code += $"' + {EscapeValue(token)} + ' ";
                        else
                            code += EscapeValue(token);

                        token = tokenQueue.Dequeue();
                    }

                    code += "')\n";
                    break;

                case "CREATE":
                    token = tokenQueue.Dequeue();

                    switch (token)
                    {
                        case "VARIABLE":
                            token = tokenQueue.Dequeue();
                            variables[token] = null;

                            code += $"{token} = None\n";
                            break;
                    }
                    break;

                case "SET":
                    string variableName = tokenQueue.Dequeue();
                    tokenQueue.Dequeue();
                    string variableValue = "";

                    token = tokenQueue.Dequeue();
                    if (token == "READ")
                    {
                        variables[variableName] = "/userinput/";
                        code += $"{variableName} = input()\n";
                    }
                    else
                    {
                        while (token != "|")
                        {
                            variableValue += token;
                            token = tokenQueue.Dequeue();
                        }

                        variables[variableName] = variableValue;
                        code += $"{variableName} = '{EscapeValue(variableValue)}'\n";
                    }

                    break;

                case "REPEAT":
                    token = tokenQueue.Dequeue();

                    if (token == "FOREVER")
                    {
                        code += "while True:\n";

                        token = tokenQueue.Dequeue();
                        while (token != "REPEAT_END")
                        {
                            code += ParseTokens(prefix = identation);
                            token = tokenQueue.Dequeue();
                        }
                        break;
                    }
                    else
                    {
                        code += $"for i in range({token}):\n";

                        tokenQueue.Dequeue();
                        token = tokenQueue.Dequeue();
                        while (token != "REPEAT_END")
                        {
                            code += ParseTokens(prefix = identation);
                            token = tokenQueue.Dequeue();
                        }
                        break;
                    }

                case "READ":
                    code += "input()\n";
                    break;
            }

            print($"-> PARSED '{code.Replace('\n', '/')}'");

            return code;
        }

        private static string EscapeValue(string value)
        {
            return value.Replace("'", "\\'");
        }

        private static void GenerateCode()
        {
            int i = 0;
            while (tokenQueue.Count > 0)
            {
                generatedCode += ParseTokens();
                Console.WriteLine("Compiling" + new string('.', i++));
            }

            generatedCode += "input()";

            File.WriteAllText(outputFile, generatedCode);
            Process.Start(Path.GetDirectoryName(outputFile));
        }

        static void PrintTokens()
        {
            Console.WriteLine("[");
            foreach (var item in tokenQueue)
            {
                Console.WriteLine($"'{item}', ");
            }
            Console.WriteLine("]");
        }

        static void print(object o)
        {
            Console.WriteLine(o);
        }
    }
}
