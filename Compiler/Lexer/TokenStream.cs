using System.Collections.Generic;

namespace PixelWallE
{
    public class TokenStream
    {
        private readonly List<Token> _tokens;
        private int _position;

        public TokenStream(List<Token> tokens)
        {
            _tokens = tokens;
            _position = 0;
        }

        public Token Peek()
        {
            return _position < _tokens.Count ? _tokens[_position] : null;
        }

        public Token PeekAhead(int lookahead = 1)
        {
            return (_position + lookahead) < _tokens.Count ? _tokens[_position + lookahead] : null;
        }

        public Token Consume()
        {
            return _position < _tokens.Count ? _tokens[_position++] : null;
        }

        public bool Match(TokenType type)
        {
            if (Peek()?.Type == type)
            {
                Consume();
                return true;
            }
            return false;
        }

        public bool IsAtEnd => _position >= _tokens.Count;

        public Token Previous => _position > 0 ? _tokens[_position - 1] : null;
    }
}