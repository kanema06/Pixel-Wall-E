using System.Collections.Generic;

namespace PixelWallE
{
    public class LexicalAnalyzer
    {
        public List<Token> Tokenize(string input)
        {
            var process = new LexicalAnalysisProcess(input);
            var tokens = new List<Token>();

            Token token;
            do
            {
                token = process.ReadNextToken();
                if (token.Type != TokenType.Unknown) 
                {
                    tokens.Add(token);
                }
            } while (token.Type != TokenType.EndOfFile);

            return tokens;
        }

        public TokenStream CreateTokenStream(string input)
        {
            var tokens = Tokenize(input);
            return new TokenStream(tokens);
        }
    }
}