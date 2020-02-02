using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace TomScriptCompiler
{
    class Compiler
    {
        private const string identation = "    ";

        private string sourceFile = @"E:\Repos\tomscript\tests\script-one.tms";
        private string outputFile = @"E:\Repos\tomscript\tests\script-one.py";

        private string languagePath = @"E:\Repos\tomscript\lang\";
        private string language = "friendly";
        private string source;
        private Queue<string> tokenQueue = new Queue<string>();
        private Dictionary<string, Type> variables = new Dictionary<string, Type>();

        private List<string> identifiers = new List<string>()
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
        private Dictionary<string, List<string>> translations = new Dictionary<string, List<string>>();

        private string generatedCode = "";


        public Compiler(string sourceFile, string outputFile)
        {
            this.sourceFile = sourceFile;
            this.outputFile = outputFile;
        }


        public void Compile()
        {
            var startTime = DateTime.Now;
            LoadLanguages();
            ReadFile();
            IndentifierIdentification();
            TokeniseSource();
            GenerateCode();
            var endTime = DateTime.Now;
            var timeTaken = endTime - startTime;

            Console.WriteLine($"Compilation finished in {timeTaken.TotalMilliseconds}ms");
        }

        void LoadLanguages()
        {
            print("Reading installed language files...");
            foreach (var file in new DirectoryInfo(languagePath).EnumerateFiles())
            {
                string[] lines = File.ReadAllLines(file.FullName);
                var language = new List<string>();

                for (int i = 0; i < lines.Length; i++)
                {
                    language.Add(lines[i].Split('=')[1]);
                }

                translations.Add(Path.GetFileNameWithoutExtension(file.FullName), language);
                print("Found language: " + file.Name.Split('.')[0]);
            }
        }

        void ReadFile()
        {
            print("Reading source file: " + sourceFile);
            source = File.ReadAllText(sourceFile);

            var match = Regex.Match(source, @"^language ([a-zA-Z])+$", RegexOptions.IgnoreCase);
            if (match.Success) language = match.Value.Split(' ')[1];

            print("Detected language subset: " + language);
        }


        private void IndentifierIdentification()
        {
            print("Identifying identifiers...");
            string newSource = source;
            foreach (var identifier in translations[language])
            {
                newSource = Regex.Replace(newSource, $@"([\s]*){identifier}([\s]+)", $"$1{identifiers[translations[language].IndexOf(identifier)]}$2");
            }
            source = newSource;
        }


        private void TokeniseSource()
        {
            print("Tokenising the source file...");
            foreach (var line in source.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (var word in Regex.Split(line, @"(?<=[ ]+)|([!,][ ]*)"))
                {
                    if (string.IsNullOrEmpty(word.Trim())) continue;

                    int index = identifiers.IndexOf(word.Trim());
                    tokenQueue.Enqueue(index != -1 ? identifiers[index] : word.Trim(new char[] { '\r' }));
                }
                tokenQueue.Enqueue("|");
            }
        }

        private string ParseTokens(string prefix = "")
        {
            if (tokenQueue.Count == 0) return "";

            string token = tokenQueue.Dequeue();
            string code = prefix;

            switch (token.Trim())
            {
                case "|":
                    break;

                case "WRITE":
                    token = tokenQueue.Dequeue();
                    code += "print('";

                    while (token != "|")
                    {
                        if (variables.ContainsKey(token))
                        {
                            if (token.EndsWith(" "))
                                code += $"' + {EscapeValue(token)} + ' ";
                            else
                                code += $"' + {EscapeValue(token)} + '";
                        }
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
                    string variableName = tokenQueue.Dequeue().Trim();

                    if (variables.ContainsKey(variableName))
                    {
                        tokenQueue.Dequeue();
                        string variableValue = "";

                        token = tokenQueue.Dequeue();
                        if (token == "READ")
                        {
                            if (variables[variableName] == typeof(float)) code += $"{variableName} = float(input())\n";
                            else if (variables[variableName] == typeof(int)) code += $"{variableName} = int(input())\n";
                            else code += $"{variableName} = input()\n";
                        }
                        else
                        {
                            while (token != "|")
                            {
                                variableValue += token;
                                token = tokenQueue.Dequeue();
                            }

                            if (Regex.IsMatch(variableValue, @"^\d+\.+\d+$"))
                            {
                                // Variable is a float
                                variables[variableName] = typeof(float);
                                code += $"{variableName} = {EscapeValue(variableValue)}\n";
                            }
                            else if (Regex.IsMatch(variableValue, @"^\d+$"))
                            {
                                // Variable is an integer
                                variables[variableName] = typeof(int);
                                code += $"{variableName} = {EscapeValue(variableValue)}\n";
                            }
                            else
                            {
                                // Variable is a string
                                variables[variableName] = typeof(string);
                                code += $"{variableName} = '{EscapeValue(variableValue)}\n'";
                            }
                        }
                    }
                    else
                    {
                        throw new VariableNotDefinedException($"Variable '{variableName}' not defined.");
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

            return code;
        }

        private string EscapeValue(string value)
        {
            return value.Replace("'", "\\'");
        }

        private void GenerateCode()
        {
            print("Starting code generation...");
            while (tokenQueue.Count > 0)
            {
                generatedCode += ParseTokens();
                Console.Write("Compiling...\r");
            }

            generatedCode += "input()";

            // Remove 2 or 3 spaces, but not 4 spaces in a row
            //generatedCode = Regex.Replace(generatedCode, "  +(?! {4})", " ");

            File.WriteAllText(outputFile, generatedCode);
        }

        void PrintTokens()
        {
            Console.WriteLine("[");
            foreach (var item in tokenQueue)
            {
                Console.WriteLine($"'{item}', ");
            }
            Console.WriteLine("]");
        }
        void print(object o)
        {
            Console.WriteLine(o);
        }
    }

    public class VariableNotDefinedException : Exception
    {
        public VariableNotDefinedException() { }
        public VariableNotDefinedException(string message) : base(message) { }
    }
}
