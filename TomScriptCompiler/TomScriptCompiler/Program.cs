using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TomScriptCompiler
{
    class Program
    {
        private static string sourceFile = @"D:\Repos\tomscript\tests\new.tms";
        private static string languagePath = @"D:\Repos\tomscript\lang\";
        private static string language = "standard";
        private static List<string> source;
        private static List<string> tokens;

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



        static void Main(string[] args)
        {
            LoadLanguages();
            ReadFile();
            TokeniseSource();

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
            source = File.ReadAllLines(sourceFile).ToList();

            Regex languageRegex = new Regex(@"language [a-zA-Z]+");
            foreach (var line in source)
            {
                if (languageRegex.IsMatch(line)) language = languageRegex.Match(line).Value.Split(' ')[1];
            }

            source = source.Skip(source.IndexOf(translations[language][identifiers.IndexOf("START_PROGRAM")])).ToList();
        }

        private static void TokeniseSource()
        {
            tokens = new List<string>();

            foreach (var line in source)
            {
                foreach (var word in line.Split(' '))
                {
                    if (string.IsNullOrEmpty(word.Trim())) continue;

                    int index = translations[language].IndexOf(word.ToLower());
                    tokens.Add(index != -1 ? identifiers[index] : word);
                }
                tokens.Add("|");
            }

            PrintList(tokens);
        }

        static void PrintList(List<string> list)
        {
            Console.Write("[");
            foreach (var item in list)
            {
                Console.Write(item + ", ");
            }
            Console.WriteLine("]");
        }

        static void print(object o)
        {
            Console.WriteLine(o);
        }
    }
}
