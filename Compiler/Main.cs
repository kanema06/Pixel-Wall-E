using System;
using System.IO;

namespace PixelWallE
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Pixel Wall-E Interpreter (Structured Version)");
            Console.WriteLine("Enter commands (type 'RUN' to execute or 'EXIT' to quit):");

            string input;
            var code = new System.Text.StringBuilder();

            while (true)
            {
                input = Console.ReadLine();
                if (input?.ToUpper() == "EXIT") break;
                if (input?.ToUpper() == "RUN")
                {
                    try
                    {
                        // Lexing
                        var lexer = new LexicalAnalyzer();
                        var tokens = lexer.Tokenize(code.ToString());

                        // Parsing
                        var parser = new Parser(tokens);
                        var program = parser.Parse();

                        // Interpretation
                        var interpreter = new Interpreter(program);
                        interpreter.Interpret();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }

                    code.Clear();
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(input))
                {
                    code.AppendLine(input);
                }
            }
        }
    }
}