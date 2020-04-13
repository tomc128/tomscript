using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace TDSStudios.TomScript.Core
{
    public class TomScriptCompiler
    {
        private const string Indentation = "    "; // The indentation to use when compiling statements - eg. if, while
        private readonly List<string> identifiers = new List<string>() // A list of all identifiers
        {
            "START_PROGRAM",
            "END_PROGRAM",
            "CREATE",
            "VARIABLE",
            "WRITE",
            "USER_INPUT",
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
            "IS_NOT_EQUAL_TO",
            "IS_GREATER_THAN",
            "IS_GREATER_THAN_OR_EQUAL_TO",
            "IS_LESS_THAN",
            "IS_LESS_THAN_OR_EQUAL_TO",
            "IS_EVEN",
            "IS_ODD",
            "BLANK_LINE",
            "TO"
        };
        private readonly List<string> operations = new List<string>  // A list of possible operations
        {
            "IS_EQUAL_TO",
            "IS_NOT_EQUAL_TO",
            "IS_GREATER_THAN",
            "IS_GREATER_THAN_OR_EQUAL_TO",
            "IS_LESS_THAN",
            "IS_LESS_THAN_OR_EQUAL_TO",
            "IS_EVEN",
            "IS_ODD"
        };
        private readonly List<string> rawTextInitiators = new List<string> // A list of indentifiers which, if at the beginning of a line, will cause subsequent variables and commands to not be translated
        {
            "WRITE"
        };

        private string language = "standard"; // The language to use, defaulting to standard
        private string source; // The source TomScript code
        private Queue<string> tokenQueue = new Queue<string>(); // A queue of tokens to parse
        private Dictionary<string, Type> variables = new Dictionary<string, Type>(); // A table containing the name and type of each variable

        private Dictionary<string, List<string>> translations = new Dictionary<string, List<string>>(); // A dictionary containing each language and its identifiers

        private bool verbose = false; // Whether or not to print verbose messages during compilation

        private string generatedCode = ""; // The ouput code


        /// <summary>
        /// Creates a new TomScript compiler object, with predefined values.
        /// </summary>
        /// <param name="sourceCode">A string containing the source code destined to be compiled</param>
        /// <param name="verbose">Should verbose log messages be outputted?</param>
        public TomScriptCompiler(string sourceCode, bool verbose = false)
        {
            this.source = sourceCode;
            this.verbose = verbose;
        }

        /// <summary>
        /// Starts compilation, calling every function required.
        /// </summary>
        /// <returns></returns>
        public string Compile()
        {
            print($"\n-- Starting compilation --");

            var startTime = DateTime.Now;
            LoadLanguages();
            IndentifierIdentification();
            TokeniseSource();
            GenerateCode();
            var endTime = DateTime.Now;
            var timeTaken = endTime - startTime;

            Console.WriteLine($"Compilation finished in {timeTaken.TotalMilliseconds}ms");

            return generatedCode;
        }

        /// <summary>
        /// Loads the languages from the project's resources.
        /// </summary>
        void LoadLanguages()
        {
            print("Reading installed language files...");

            var languageFiles = new Dictionary<string, string>()
            {
                ["standard"] = Properties.Resources.standard,
                ["friendly"] = Properties.Resources.friendly,
                ["fancy"] = Properties.Resources.fancy,
                ["francais"] = Properties.Resources.francais
            };

            foreach (var file in languageFiles)
            {
                string[] lines = file.Value.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                var language = new List<string>();

                for (int i = 0; i < lines.Length; i++)
                {
                    language.Add(lines[i].Split('=')[1]);
                }

                translations.Add(file.Key, language);
                print("Found language: " + file.Key.Split('.')[0]);
            }

        }

        /// <summary>
        /// Identifies the script features, before the start instruction.
        /// </summary>
        private void IndentifierIdentification()
        {
            print("Identifying identifiers...");

            string newSource = source;

            string rawTextInitiatorSearch = "";
            rawTextInitiators.ForEach(t => rawTextInitiatorSearch += t + "|");
            rawTextInitiatorSearch = rawTextInitiatorSearch.Trim('|');

            foreach (var identifier in translations[language])
            {
                newSource = Regex.Replace(newSource, $@"(\s+){identifier}(\s+)", $"$1{identifiers[translations[language].IndexOf(identifier)]}$2", RegexOptions.IgnoreCase);
            }
            source = newSource;
        }

        /// <summary>
        /// Converts the source code into tokens defined by the current language, using regex.
        /// </summary>
        private void TokeniseSource()
        {
            print("Tokenising the source file...");
            foreach (var line in source.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                foreach (var word in Regex.Split(line, @"(?<=[ ]+)|([!,.:?(){}\[\]][ ]*)"))
                {
                    if (string.IsNullOrEmpty(word.Trim())) continue;

                    int index = identifiers.IndexOf(word.Trim());
                    tokenQueue.Enqueue(index != -1 ? identifiers[index] : word.Trim(new char[] { '\r' }));
                }
                tokenQueue.Enqueue("|");
            }
        }

        /// <summary>
        /// Parses the tokens in the queue, starting at the beginning and continuting until the end of the instruction.
        /// </summary>
        /// <param name="prefix">What to write before each line, in case the instruction is within a loop</param>
        /// <returns></returns>
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
                            {
                                if (variables[token.Trim()] == typeof(float) || variables[token.Trim()] == typeof(int) || variables[token.Trim()] == null) code += $"' + str({EscapeValue(token.Trim())}) + ' ";
                                else code += $"' + {EscapeValue(token.Trim())} + ' ";
                            }
                            else
                            {
                                if (variables[token.Trim()] == typeof(float) || variables[token.Trim()] == typeof(int) || variables[token.Trim()] == null) code += $"' + str({EscapeValue(token.Trim())}) + '";
                                else code += $"' + {EscapeValue(token.Trim())} + '";
                            }
                        }
                        else
                        {
                            if (token == "BLANK_LINE") code += "";
                            else code += EscapeValue(token);
                        }

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
                        if (token == "USER_INPUT")
                        {
                            tokenQueue.Dequeue();
                            if (variables[variableName] == typeof(float)) code += $"{variableName} = float(input())\n";
                            else if (variables[variableName] == typeof(int)) code += $"{variableName} = int(input())\n";
                            else code += $"{variableName} = input()\n";
                        }
                        else if (token == "RESULT")
                        {
                            tokenQueue.Dequeue();
                            code += $"{variableName} = eval('";
                            while (token != "|" && token.Trim() != "//")
                            {
                                token = tokenQueue.Dequeue();
                                code += EscapeValue(token);
                                token = tokenQueue.Peek();
                            }
                            code += "')\n";
                        }
                        else
                        {
                            while (token != "|" && token.Trim() != "//")
                            {
                                token = tokenQueue.Dequeue();
                                variableValue += token;
                                token = tokenQueue.Peek();
                            }

                            variableValue = variableValue.Trim();

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
                            code += ParseTokens(prefix = Indentation);
                            token = tokenQueue.Dequeue();
                        }
                    }
                    else if (token == "WHILE")
                    {
                        token = tokenQueue.Peek();
                        var leftParams = new List<string>();
                        var rightParams = new List<string>();
                        string operation = "";

                        while (!operations.Contains(token))
                        {
                            token = tokenQueue.Dequeue();
                            leftParams.Add(token.Trim());
                            token = tokenQueue.Peek();
                        }

                        bool isOneSidedOperator = false;

                        switch (token)
                        {
                            case "IS_EQUAL_TO":
                                operation = "==";
                                break;
                            case "IS_NOT_EQUAL_TO":
                                operation = "!=";
                                break;
                            case "IS_GREATER_THAN":
                                operation = ">";
                                break;
                            case "IS_GREATER_THAN_OR_EQUAL_TO":
                                operation = ">=";
                                break;
                            case "IS_LESS_THAN_OR_EQUAL_TO":
                                operation = "<=";
                                break;
                            case "IS_EVEN":
                                isOneSidedOperator = true;
                                operation = "%2==0";
                                break;
                            default:
                                throw new InvalidOperationException($"The operation '{token}' is invalid.");
                        }

                        if (isOneSidedOperator) token = tokenQueue.Dequeue();
                        token = tokenQueue.Dequeue();

                        while (token != "|" && token.Trim() != "//")
                        {
                            token = tokenQueue.Dequeue();
                            rightParams.Add(token);
                            token = tokenQueue.Peek();
                        }


                        string left = "";
                        for (int i = 0; i < leftParams.Count; i++)
                        {
                            if (variables.ContainsKey(leftParams[i].Trim()))
                            {
                                left += leftParams[i];
                            }
                            else
                            {
                                if (GetValueType(leftParams[i]) == typeof(float) || GetValueType(leftParams[i]) == typeof(int)) left += EscapeValue(leftParams[i]).Trim();
                                else left += $"'{EscapeValue(leftParams[i]).Trim()}'";

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

                        string right = "";
                        for (int i = 0; i < rightParams.Count; i++)
                        {
                            if (variables.ContainsKey(rightParams[i].Trim()))
                            {
                                right += rightParams[i];
                            }
                            else
                            {
                                if (GetValueType(rightParams[i]) == typeof(float) || GetValueType(rightParams[i]) == typeof(int)) right += EscapeValue(rightParams[i]).Trim();
                                else right += $"'{EscapeValue(rightParams[i]).Trim()}'";

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

                        token = tokenQueue.Peek();

                        while (token != "REPEAT_END")
                        {
                            code += ParseTokens(prefix = Indentation);
                            token = tokenQueue.Peek();
                        }
                    }
                    else
                    {
                        code += $"for i in range({token}):\n";

                        tokenQueue.Dequeue();
                        token = tokenQueue.Dequeue();
                        while (token != "REPEAT_END")
                        {
                            code += ParseTokens(prefix = Indentation);
                            token = tokenQueue.Peek();
                        }
                    }
                    break;

                case "USER_INPUT":
                    code += "input()\n";
                    break;

                case "IF":
                    {
                        token = tokenQueue.Peek();
                        var leftParams = new List<string>();
                        var rightParams = new List<string>();
                        string operation = "";

                        while (!operations.Contains(token))
                        {
                            token = tokenQueue.Dequeue();
                            leftParams.Add(token.Trim());
                            token = tokenQueue.Peek();
                        }


                        bool isOneSidedOperator = false;

                        switch (token)
                        {
                            case "IS_EQUAL_TO":
                                operation = "==";
                                break;
                            case "IS_NOT_EQUAL_TO":
                                operation = "!=";
                                break;
                            case "IS_GREATER_THAN":
                                operation = ">";
                                break;
                            case "IS_GREATER_THAN_OR_EQUAL_TO":
                                operation = ">=";
                                break;
                            case "IS_LESS_THAN_OR_EQUAL_TO":
                                operation = "<=";
                                break;
                            case "IS_EVEN":
                                isOneSidedOperator = true;
                                operation = "%2==0";
                                break;
                            default:
                                throw new InvalidOperationException($"The operation '{token}' is invalid.");
                        }

                        if (isOneSidedOperator) token = tokenQueue.Dequeue();
                        token = tokenQueue.Dequeue();

                        while (token.Trim() != "|" && token.Trim() != "//")
                        {
                            token = tokenQueue.Dequeue();
                            rightParams.Add(token);
                            token = tokenQueue.Peek();
                        }


                        string left = "";

                        for (int i = 0; i < leftParams.Count; i++)
                        {
                            if (variables.ContainsKey(leftParams[i].Trim()))
                            {
                                left += leftParams[i];
                            }
                            else
                            {
                                if (GetValueType(leftParams[i]) == typeof(float) || GetValueType(leftParams[i]) == typeof(int)) left += EscapeValue(leftParams[i]).Trim();
                                else left += $"'{EscapeValue(leftParams[i]).Trim()}'";

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

                        string right = "";
                        for (int i = 0; i < rightParams.Count; i++)
                        {
                            if (rightParams[i].Trim() == "|")
                            {
                                right = " and 1 == 1";
                                break;
                            }
                            if (variables.ContainsKey(rightParams[i].Trim()))
                            {
                                right += rightParams[i];
                            }
                            else
                            {
                                if (GetValueType(rightParams[i]) == typeof(float) || GetValueType(rightParams[i]) == typeof(int)) right += EscapeValue(rightParams[i]).Trim();
                                else right += $"'{EscapeValue(rightParams[i]).Trim()}'";

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
                        code += $"if({left.Trim()} {operation.Trim()} {right.Trim()}):\n";



                        while (token != "IF_END" && token != "IF_ELSE" && token != "IF_ELSE_IF")
                        {
                            code += ParseTokens(prefix = Indentation);
                            token = tokenQueue.Peek();
                        }
                        break;
                    }

                case "IF_ELSE_IF":
                    {
                        token = tokenQueue.Peek();
                        var leftParams = new List<string>();
                        var rightParams = new List<string>();
                        string operation = "";

                        while (!operations.Contains(token))
                        {
                            token = tokenQueue.Dequeue();
                            leftParams.Add(token.Trim());
                            token = tokenQueue.Peek();
                        }

                        bool isOneSidedOperator = false;

                        switch (token)
                        {
                            case "IS_EQUAL_TO":
                                operation = "==";
                                break;
                            case "IS_NOT_EQUAL_TO":
                                operation = "!=";
                                break;
                            case "IS_GREATER_THAN":
                                operation = ">";
                                break;
                            case "IS_GREATER_THAN_OR_EQUAL_TO":
                                operation = ">=";
                                break;
                            case "IS_LESS_THAN_OR_EQUAL_TO":
                                operation = "<=";
                                break;
                            case "IS_EVEN":
                                isOneSidedOperator = true;
                                operation = "%2==0";
                                break;
                            default:
                                throw new InvalidOperationException($"The operation '{token}' is invalid.");
                        }

                        if (isOneSidedOperator) token = tokenQueue.Dequeue();
                        token = tokenQueue.Dequeue();

                        while (token != "|" && token != "//")
                        {
                            token = tokenQueue.Dequeue();
                            rightParams.Add(token);
                            token = tokenQueue.Peek();
                        }


                        string left = "";
                        for (int i = 0; i < leftParams.Count; i++)
                        {
                            if (variables.ContainsKey(leftParams[i].Trim()))
                            {
                                left += leftParams[i];
                            }
                            else
                            {
                                if (GetValueType(leftParams[i]) == typeof(float) || GetValueType(leftParams[i]) == typeof(int)) left += EscapeValue(leftParams[i]).Trim();
                                else left += $"'{EscapeValue(leftParams[i]).Trim()}'";

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

                        string right = "";
                        for (int i = 0; i < rightParams.Count; i++)
                        {
                            if (variables.ContainsKey(rightParams[i].Trim()))
                            {
                                right += rightParams[i];
                            }
                            else
                            {
                                if (GetValueType(rightParams[i]) == typeof(float) || GetValueType(rightParams[i]) == typeof(int)) right += EscapeValue(rightParams[i]).Trim();
                                else right += $"'{EscapeValue(rightParams[i]).Trim()}'";

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
                        code += $"elif({left} {operation} {right}):\n";

                        while (token != "IF_END" && token != "IF_ELSE" && token != "IF_ELSE_IF")
                        {
                            code += ParseTokens(prefix = Indentation);
                            token = tokenQueue.Peek();
                        }
                        break;
                    }

                case "IF_ELSE":
                    token = tokenQueue.Dequeue();

                    code += "else:\n";

                    while (token != "IF_END" && token != "IF_ELSE" && token != "IF_ELSE_IF")
                    {
                        code += ParseTokens(prefix = Indentation);
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

        /// <summary>
        /// Gets the type of the variable given from the dictionary of variables.
        /// </summary>
        /// <param name="value">The name of the variable to get the type of</param>
        /// <returns></returns>
        private Type GetValueType(string value)
        {
            value = value.Trim();

            if (Regex.IsMatch(value, @"^-*\d+\.+\d+$")) return typeof(float);
            else if (Regex.IsMatch(value, @"^-*\d+$")) return typeof(int);
            else return typeof(string);
        }


        /// <summary>
        /// Escapes any illegal characters in the given string.
        /// </summary>
        /// <param name="value">The string to escape</param>
        /// <returns></returns>
        private string EscapeValue(string value) => value.Replace("'", "\\'");

        /// <summary>
        /// Generates the code by looping through the remaining tokens in the queue.
        /// </summary>
        private void GenerateCode()
        {
            print("Starting code generation...");
            int i = 0;
            while (tokenQueue.Count > 0)
            {
                generatedCode += ParseTokens();
                Console.Write($"Compiling {new string('.', Math.Min(Console.WindowWidth - 20, i))}\r");
                i++;
            }
            print("");

            generatedCode = $"# Generated with TomScript Compiler - https://github.com/tomc128/tomscript \n{generatedCode}\nprint('Press enter to quit...')\ninput()";
        }

        /// <summary>
        /// A function which writes each token to the console if verbose logging is enabled.
        /// </summary>
        void PrintTokens()
        {
            if (verbose)
            {
                Console.WriteLine("[");
                foreach (var item in tokenQueue)
                {
                    Console.WriteLine($"'{item}', ");
                }
                Console.WriteLine("]");
            }
        }

        /// <summary>
        /// A function which writes the object to the console if verbose logging is enabled.
        /// </summary>
        /// <param name="o">The object to print to the console</param>
        void print(object o)
        {
            if (verbose)
            {
                Console.WriteLine(o);
            }
        }
    }

    public class VariableNotDefinedException : Exception
    {
        public VariableNotDefinedException() { }
        public VariableNotDefinedException(string message) : base(message) { }
    }

    public class InvalidOperationException : Exception
    {
        public InvalidOperationException() { }
        public InvalidOperationException(string message) : base(message) { }
    }
}
