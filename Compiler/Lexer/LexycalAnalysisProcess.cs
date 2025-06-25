using System;
using System.Collections.Generic;
using System.Text;
using Godot;

namespace PixelWallE
{
    public class LexicalAnalysisProcess
    {
        private readonly string _input;
        private int _position;
        private int _lineNumber;
        private int _currentLinePosition;
        private readonly StringBuilder _currentToken = new StringBuilder();

        private static readonly Dictionary<string, TokenType> Keywords = new Dictionary<string, TokenType>
        {
            {"Spawn", TokenType.Spawn},
            {"Color", TokenType.Color},
            {"Size", TokenType.Size},
            {"DrawLine", TokenType.DrawLine},
            {"DrawCircle", TokenType.DrawCircle},
            {"DrawRectangle", TokenType.DrawRectangle},
            {"Fill", TokenType.Fill},
            {"GetActualX", TokenType.GetActualX},
            {"GetActualY", TokenType.GetActualY},
            {"GetCanvasSize", TokenType.GetCanvasSize},
            {"GetColorCount", TokenType.GetColorCount},
            {"IsBrushColor", TokenType.IsBrushColor},
            {"IsBrushSize", TokenType.IsBrushSize},
            {"IsCanvasColor", TokenType.IsCanvasColor},
            {"GoTo", TokenType.GoTo},
            {"true", TokenType.Boolean},
            {"false", TokenType.Boolean}
        };

        public static Dictionary<string, TokenType> Colors = new Dictionary<string, TokenType>
        {
            {"Red", TokenType.ColorLiteral},
            {"Blue", TokenType.ColorLiteral},
            {"Green", TokenType.ColorLiteral},
            {"Yellow", TokenType.ColorLiteral},
            {"Orange", TokenType.ColorLiteral},
            {"Purple", TokenType.ColorLiteral},
            {"Black", TokenType.ColorLiteral},
            {"White", TokenType.ColorLiteral},
            {"Transparent", TokenType.ColorLiteral}
        };

        public LexicalAnalysisProcess(string input)
        {
            _input = input;
            _position = 0;
            _lineNumber = 1;
            _currentLinePosition = 1;
        }

        public Token ReadNextToken()
        {
            if (_position >= _input.Length)
                return new Token(TokenType.EndOfFile, "", _lineNumber, _currentLinePosition);

            var current = Peek();

            // Skip white-space
            while (char.IsWhiteSpace(current))
            {
                if (current == '\n')
                {
                    _lineNumber++;
                    _currentLinePosition = 1;
                }
                else
                {
                    _currentLinePosition++;
                }
                Consume();
                if (_position >= _input.Length)
                    return new Token(TokenType.EndOfFile, "", _lineNumber, _currentLinePosition);
                current = Peek();
            }

            int tokenStartPosition = _currentLinePosition;

            if (char.IsDigit(current))
                return ReadNumber(tokenStartPosition);

            if (char.IsLetter(current))
                return ReadIdentifier(tokenStartPosition);

            switch (current)
            {
                case '<':
                    if (Peek(1) == '-')
                    {
                        Consume(2);
                        _currentLinePosition += 2;
                        return new Token(TokenType.Assignment, "<-", _lineNumber, tokenStartPosition);
                    }
                    if (Peek(1) == '=')
                    {
                        Consume(2);
                        _currentLinePosition += 2;
                        return new Token(TokenType.LessEqual, "<=", _lineNumber, tokenStartPosition);
                    }
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.Less, "<", _lineNumber, tokenStartPosition);

                case '>':
                    if (Peek(1) == '=')
                    {
                        Consume(2);
                        _currentLinePosition += 2;
                        return new Token(TokenType.GreaterEqual, ">=", _lineNumber, tokenStartPosition);
                    }
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.Greater, ">", _lineNumber, tokenStartPosition);

                case '=':
                    if (Peek(1) == '=')
                    {
                        Consume(2);
                        _currentLinePosition += 2;
                        return new Token(TokenType.Equal, "==", _lineNumber, tokenStartPosition);
                    }
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.Unknown, "=", _lineNumber, tokenStartPosition);

                case '+':
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.Plus, "+", _lineNumber, tokenStartPosition);

                case '-':
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.Minus, "-", _lineNumber, tokenStartPosition);

                case '*':
                    if (Peek(1) == '*')
                    {
                        Consume(2);
                        _currentLinePosition += 2;
                        return new Token(TokenType.Power, "**", _lineNumber, tokenStartPosition);
                    }
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.Multiply, "*", _lineNumber, tokenStartPosition);

                case '/':
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.Divide, "/", _lineNumber, tokenStartPosition);

                case '%':
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.Modulo, "%", _lineNumber, tokenStartPosition);

                case '&':
                    if (Peek(1) == '&')
                    {
                        Consume(2);
                        _currentLinePosition += 2;
                        return new Token(TokenType.And, "&&", _lineNumber, tokenStartPosition);
                    }
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.Unknown, "&", _lineNumber, tokenStartPosition);

                case '|':
                    if (Peek(1) == '|')
                    {
                        Consume(2);
                        _currentLinePosition += 2;
                        return new Token(TokenType.Or, "||", _lineNumber, tokenStartPosition);
                    }
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.Unknown, "|", _lineNumber, tokenStartPosition);

                case '(':
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.LeftParen, "(", _lineNumber, tokenStartPosition);

                case ')':
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.RightParen, ")", _lineNumber, tokenStartPosition);

                case '[':
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.LeftBracket, "[", _lineNumber, tokenStartPosition);

                case ']':
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.RightBracket, "]", _lineNumber, tokenStartPosition);

                case ',':
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.Comma, ",", _lineNumber, tokenStartPosition);

                case ':':
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.Colon, ":", _lineNumber, tokenStartPosition);

                default:
                    Consume();
                    _currentLinePosition++;
                    return new Token(TokenType.Unknown, current.ToString(), _lineNumber, tokenStartPosition);
            }
        }

        private Token ReadNumber(int startPosition)
        {
            bool isNegative = false;
            char current = Peek();

            if (current == '-' && char.IsDigit(Peek(1)))
            {
                isNegative = true;
                Consume();
                _currentLinePosition++;
            }
            else if (current == '+') 
            {
                Consume();
                _currentLinePosition++;
            }

            var start = _position;
            while (_position < _input.Length && char.IsDigit(Peek()))
            {
                Consume();
                _currentLinePosition++;
            }

            if (_position == start && isNegative)
            {
                return new Token(TokenType.Minus, "-", _lineNumber, startPosition);
            }

            string numberValue = _input.Substring(start, _position - start);
            if (isNegative)
            {
                numberValue = "-" + numberValue;
            }

            return new Token(TokenType.Number, numberValue, _lineNumber, startPosition);
        }



private Token ReadIdentifier(int startPosition)
{
    var start = _position;
     if (!char.IsLetter(Peek()))
    {
        Consume();
        _currentLinePosition++;
        return new Token(TokenType.Unknown, _input.Substring(start, 1), _lineNumber, startPosition);
    }

    while (_position < _input.Length && (char.IsLetterOrDigit(Peek()) || Peek() == '_')) 
    {
        Consume();
        _currentLinePosition++;
    }
    
    var value = _input.Substring(start, _position - start);

    if (Keywords.TryGetValue(value, out var keywordType))
    {
        return new Token(keywordType, value, _lineNumber, startPosition);
    }

    if (Colors.TryGetValue(value, out var colorType))
    {
        return new Token(colorType, value, _lineNumber, startPosition);
    }

    return new Token(TokenType.Identifier, value, _lineNumber, startPosition);
}


        private char Peek(int ahead = 0)
        {
            var pos = _position + ahead;
            return pos < _input.Length ? _input[pos] : '\0';
        }

        private void Consume(int count = 1)
        {
            _position += count;
        }
    }
}