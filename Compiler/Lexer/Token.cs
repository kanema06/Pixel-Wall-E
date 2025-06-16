namespace PixelWallE
{
    public enum TokenType
    {
        Spawn, Color, Size, DrawLine, DrawCircle, DrawRectangle, Fill,
        GetActualX, GetActualY, GetCanvasSize, GetColorCount, IsBrushColor, IsBrushSize, IsCanvasColor,
        GoTo,
        Number, ColorLiteral, Boolean, Identifier, String,
        Assignment, Plus, Minus, Multiply, Divide, Modulo, Power,
        Equal, Greater, GreaterEqual, Less, LessEqual,
        And, Or,
        LeftParen, RightParen, LeftBracket, RightBracket, Comma, Colon,
        EndOfLine, Unknown, EndOfFile
    }

    public class Token
    {
        public TokenType Type { get; }
        public string Value { get; }
        public int LineNumber { get; }
        public int Position { get; }

        public Token(TokenType type, string value, int lineNumber, int position)
        {
            Type = type;
            Value = value;
            LineNumber = lineNumber;
            Position = position;
        }

        public override string ToString() => $"{Type}: '{Value}' (Line {LineNumber}, Pos {Position})";
    }
}