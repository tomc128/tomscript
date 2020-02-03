using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace TomScriptCompiler
{
    class Compiler
    {
        private const string indentation = "    ";

        private string sourceFile = @"E:\Repos\tomscript\tests\while-conditional.tms";
        private string outputFile = @"E:\Repos\tomscript\tests\while-conditional.tms.py";

        private string languagePath = @"E:\Repos\tomscript\lang\";
        private string language = "standard";
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
            "CALCULATE",
            "REPEAT",
            "FOREVER",
            "TIMES",
            "WHILE",
            "REPEAT_END",
            "|",
            "RESULT",
            "IF",
            "IF_ELSE_IF",
            "IF_ELSE",
            "IF_END",
            "IS_EQUAL_TO",
            "IS_GREATER_THAN",
            "IS_LESS_THAN",
            "TO"
        };
        private Dictionary<string, List<string>> translations = new Dictionary<string, List<string>>();

        private string generatedCode = "";


        public Compiler() { }

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
            PrintTokens();
            GenerateCode();
            var endTime = DateTime.Now;
            var timeTaken = endTime - startTime;

            Console.WriteLine($"Compilation finished in {timeTaken.TotalMilliseconds}ms");
            Console.ReadLine();
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

            var match = Regex.Match(source, @"^language ([a-zA-Z])+", RegexOptions.IgnoreCase);
            if (match.Success) language = match.Value.Split(' ')[1];

            print("Detected language subset: " + language);
        }


        private void IndentifierIdentification()
        {
            print("Identifying identifiers...");
            string newSource = source;
            foreach (var identifier in translations[language])
            {
                newSource = Regex.Replace(newSource, $@"(\s+){identifier}(\s+)", $"$1{identifiers[translations[language].IndexOf(identifier)]}$2", RegexOptions.IgnoreCase);
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
                    code = "";
                    break;

                case "WRITE":
                    token = tokenQueue.Peek();
                    code += "print('";

                    while (token != "|" && token.Trim() != "//")
                    {
                        token = tokenQueue.Dequeue();
                        if (variables.ContainsKey(token.Trim()))
                        {
                            if (token.EndsWith(" "))
                                code += $"' + {EscapeValue(token.Trim())} + ' ";
                            else
                                code += $"' + {EscapeValue(token.Trim())} + '";
                        }
                        else
                            code += EscapeValue(token);

                        token = tokenQueue.Peek();
                    }

                    code += "')\n";
                    break;



                case "CREATE":
                    token = tokenQueue.Dequeue();

                    switch (token)
                    {
                        case "VARIABLE":
                            token = tokenQueue.Dequeue();
                            variables[token.Trim()] = null;

                            code += $"{token.Trim()} = None\n";
                            break;
                    }
                    break;

                case "SET":
                    string variableName = tokenQueue.Dequeue().Trim();

                    if (variables.ContainsKey(variableName))
                    {
                        tokenQueue.Dequeue();
                        string variableValue = "";

                        token = tokenQueue.Peek();
                        if (token == "READ")
                        {
                            token = tokenQueue.Dequeue();
                            if (variables[variableName] == typeof(float)) code += $"{variableName} = float(input())\n";
                            else if (variables[variableName] == typeof(int)) code += $"{variableName} = int(input())\n";
                            else code += $"{variableName} = input()\n";
                        }
                        else
                        {
                            while (token != "|" && token.Trim() != "//")
                            {
                                token = tokenQueue.Dequeue();
                                variableValue += token;
                                token = tokenQueue.Peek();
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
                                code += $"{variableName} = '{EscapeValue(variableValue).Trim()}'\n";
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
                            code += ParseTokens(prefix = indentation);
                            token = tokenQueue.Dequeue();
                        }
                    }
                    else if (token == "WHILE")
                    {
                        // While loop!
                        token = tokenQueue.Peek();
                        var leftParams = new List<string>();
                        var rightParams = new List<string>();
                        string operation = "";

                        while (token != "IS_EQUAL_TO" && token != "IS_GREATER_THAN" && token != "IS_LESS_THAN")
                        {
                            token = tokenQueue.Dequeue();
                            leftParams.Add(token.Trim());
                            token = tokenQueue.Peek();
                        }

                        switch (token)
                        {
                            case "IS_EQUAL_TO":
                                operation = "==";
                                break;
                            case "IS_GREATER_THAN":
                                operation = ">";
                                break;
                            case "IS_LESS_THAN":
                                operation = "<";
                                break;
                        }

                        token = tokenQueue.Dequeue();

                        while (token != "|" && token.Trim() != "//")
                        {
                            token = tokenQueue.Dequeue();
                            rightParams.Add(token);
                            token = tokenQueue.Peek();
                        }


                        string left = "";
                        string right = "";

                        for (int i = 0; i < leftParams.Count; i++)
                        {
                            if (variables.ContainsKey(leftParams[i].Trim()))
                            {
                                left += leftParams[i];
                            }
                            else
                            {
                                left += $"'{EscapeValue(leftParams[i])}'";
                                if (i - 1 >= 0)
                                {
                                    if (variables.ContainsKey(leftParams[i - 1].Trim())) left = "+" + left.Trim();
                                }
                                if (i + 1 < leftParams.Count)
                                {
                                    if (variables.ContainsKey(leftParams[i + 1].Trim())) left = left.Trim() + "+";
                                }
                            }
                        }

                        for (int i = 0; i < rightParams.Count; i++)
                        {
                            if (variables.ContainsKey(rightParams[i].Trim()))
                            {
                                right += rightParams[i];
                            }
                            else
                            {
                                right += $"'{EscapeValue(rightParams[i]).Trim()}'";
                                if (i - 1 >= 0)
                                {
                                    if (variables.ContainsKey(rightParams[i - 1].Trim())) right = "+" + right.Trim();
                                }
                                if (i + 1 < rightParams.Count)
                                {
                                    if (variables.ContainsKey(rightParams[i + 1].Trim())) right = right.Trim() + "+";
                                }
                            }
                        }


                        code += $"while ({left} {operation} {right}):\n";
                                               
                        while (token != "REPEAT_END")
                        {
                            code += ParseTokens(prefix = indentation);
                            token = tokenQueue.Dequeue();
                        }
                    }
                    else
                    {
                        code += $"for i in range({token}):\n";

                        tokenQueue.Dequeue();
                        token = tokenQueue.Dequeue();
                        while (token != "REPEAT_END")
                        {
                            code += ParseTokens(prefix = indentation);
                            token = tokenQueue.Peek();
                        }
                    }
                    break;

                case "READ":
                    code += "input()\n";
                    break;

                case "IF":
                    {
                        token = tokenQueue.Peek();
                        var leftParams = new List<string>();
                        var rightParams = new List<string>();
                        string ifOperation = "";

                        while (token != "IS_EQUAL_TO" && token != "IS_GREATER_THAN" && token != "IS_LESS_THAN")
                        {
                            token = tokenQueue.Dequeue();
                            leftParams.Add(token.Trim());
                            token = tokenQueue.Peek();
                        }

                        switch (token)
                        {
                            case "IS_EQUAL_TO":
                                ifOperation = "==";
                                break;
                            case "IS_GREATER_THAN":
                                ifOperation = ">";
                                break;
                            case "IS_LESS_THAN":
                                ifOperation = "<";
                                break;
                        }

                        token = tokenQueue.Dequeue();

                        while (token != "|" && token.Trim() != "//")
                        {
                            token = tokenQueue.Dequeue();
                            rightParams.Add(token);
                            token = tokenQueue.Peek();
                        }


                        string left = "";
                        string right = "";

                        for (int i = 0; i < leftParams.Count; i++)
                        {
                            if (variables.ContainsKey(leftParams[i].Trim()))
                            {
                                left += leftParams[i];
                            }
                            else
                            {
                                left += $"'{EscapeValue(leftParams[i])}'";
                                if (i - 1 >= 0)
                                {
                                    if (variables.ContainsKey(leftParams[i - 1].Trim())) left = "+" + left.Trim();
                                }
                                if (i + 1 < leftParams.Count)
                                {
                                    if (variables.ContainsKey(leftParams[i + 1].Trim())) left = left.Trim() + "+";
                                }
                            }
                        }

                        for (int i = 0; i < rightParams.Count; i++)
                        {
                            if (variables.ContainsKey(rightParams[i].Trim()))
                            {
                                right += rightParams[i];
                            }
                            else
                            {
                                right += $"'{EscapeValue(rightParams[i]).Trim()}'";
                                if (i - 1 >= 0)
                                {
                                    if (variables.ContainsKey(rightParams[i - 1].Trim())) right = "+" + right.Trim();
                                }
                                if (i + 1 < rightParams.Count)
                                {
                                    if (variables.ContainsKey(rightParams[i + 1].Trim())) right = right.Trim() + "+";
                                }
                            }
                        }
                        code += $"if({left.Trim()} {ifOperation.Trim()} {right.Trim()}):\n";

                        while (token != "IF_END" && token != "IF_ELSE" && token != "IF_ELSE_IF")
                        {
                            code += ParseTokens(prefix = indentation);
                            token = tokenQueue.Peek();
                        }
                        break;
                    }

                case "IF_ELSE_IF":
                    {
                        token = tokenQueue.Peek();
                        var leftParams = new List<string>();
                        var rightParams = new List<string>();
                        string ifOperation = "";

                        while (token != "IS_EQUAL_TO" && token != "IS_GREATER_THAN" && token != "IS_LESS_THAN")
                        {
                            token = tokenQueue.Dequeue();
                            leftParams.Add(token.Trim());
                            token = tokenQueue.Peek();
                        }

                        switch (token)
                        {
                            case "IS_EQUAL_TO":
                                ifOperation = "==";
                                break;
                            case "IS_GREATER_THAN":
                                ifOperation = ">";
                                break;
                            case "IS_LESS_THAN":
                                ifOperation = "<";
                                break;
                        }

                        token = tokenQueue.Dequeue();

                        while (token != "|" && token != "//")
                        {
                            token = tokenQueue.Dequeue();
                            rightParams.Add(token);
                            token = tokenQueue.Peek();
                        }


                        string left = "";
                        string right = "";

                        for (int i = 0; i < leftParams.Count; i++)
                        {
                            if (variables.ContainsKey(leftParams[i].Trim()))
                            {
                                left += leftParams[i];
                            }
                            else
                            {
                                left += $"'{EscapeValue(leftParams[i])}'";
                                if (i - 1 >= 0)
                                {
                                    if (variables.ContainsKey(leftParams[i - 1].Trim())) left = "+" + left;
                                }
                                if (i + 1 < leftParams.Count)
                                {
                                    if (variables.ContainsKey(leftParams[i + 1].Trim())) left += "+";
                                }
                            }
                        }

                        for (int i = 0; i < rightParams.Count; i++)
                        {
                            if (variables.ContainsKey(rightParams[i].Trim()))
                            {
                                right += rightParams[i];
                            }
                            else
                            {
                                right += $"'{EscapeValue(rightParams[i])}'";
                                if (i - 1 >= 0)
                                {
                                    if (variables.ContainsKey(rightParams[i - 1].Trim())) right = "+" + right;
                                }
                                if (i + 1 < rightParams.Count)
                                {
                                    if (variables.ContainsKey(rightParams[i + 1].Trim())) right = right + "+";
                                }
                            }
                        }
                        code += $"elif({left} {ifOperation} {right}):\n";

                        while (token != "IF_END" && token != "IF_ELSE" && token != "IF_ELSE_IF")
                        {
                            code += ParseTokens(prefix = indentation);
                            token = tokenQueue.Peek();
                        }
                        break;
                    }

                case "IF_ELSE":
                    token = tokenQueue.Dequeue();

                    code += "else:\n";

                    while (token != "IF_END" && token != "IF_ELSE" && token != "IF_ELSE_IF")
                    {
                        code += ParseTokens(prefix = indentation);
                        token = tokenQueue.Peek();
                    }
                    break;

                case "//":
                    code = "";
                    token = tokenQueue.Dequeue();
                    while (token != "|")
                    {
                        token = tokenQueue.Dequeue();
                    }
                    break;
            }

            // If the code is just whitespace, it consists of just the indent prefix; so remove it
            if (string.IsNullOrWhiteSpace(code)) code = "";

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
