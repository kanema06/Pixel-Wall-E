using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelWallE
{

    public class Parser
    {
        public ExecutionResult Result { get; } = new ExecutionResult();
        private readonly List<Token> _tokens;
        private int _current;
        private readonly Dictionary<string, int> _labels = new Dictionary<string, int>();
        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _current = 0;
        }

        public ProgramNode Parse()
        {
            var (program, _) = ParseWithResult();
            return program;
        }
        public (ProgramNode Program, ExecutionResult Result) ParseWithResult()
        {
            var statements = new List<StatementNode>();
            var labels = new Dictionary<string, int>();

            _current = 0;

            while (!IsAtEnd())
            {
                try
                {
                    var statement = ParseStatement();
                    if (statement != null)
                    {
                        if (statement is LabelNode labelNode)
                        {
                            labels[labelNode.Label.Value] = statements.Count;
                        }
                        statements.Add(statement);
                    }
                }
                catch (ParseError)
                {
                    Synchronize();
                }
            }

            return (new ProgramNode(statements, labels), Result);
        }

        private StatementNode ParseStatement()
        {
            if (Match(TokenType.Spawn)) return ParseSpawn();
            if (Match(TokenType.Color)) return ParseColor();
            if (Match(TokenType.Size)) return ParseSize();
            if (Match(TokenType.DrawLine)) return ParseDrawLine();
            if (Match(TokenType.DrawCircle)) return ParseDrawCircle();
            if (Match(TokenType.DrawRectangle)) return ParseDrawRectangle();
            if (Match(TokenType.Fill)) return ParseFill();
            if (Match(TokenType.GoTo)) return ParseGoTo();
            if (Check(TokenType.Identifier))
            {
                TokenType? nextType = PeekAhead(1)?.Type;

                 if (nextType == null || PeekAhead(1).LineNumber > PeekAhead(0).LineNumber)
        return ParseLabel();

                if (nextType == TokenType.Assignment)
                    return ParseAssignment();
            }
            throw Error(Peek(), "Expected statement.");
        }

        private SpawnNode ParseSpawn()
        {
            var token = Previous();
            Consume(TokenType.LeftParen, "Expect '(' after 'Spawn'.");
            var x = ParseExpression();
            Consume(TokenType.Comma, "Expect ',' after x coordinate.");
            var y = ParseExpression();
            Consume(TokenType.RightParen, "Expect ')' after spawn coordinates.");
            return new SpawnNode(token, x, y);
        }

        private ColorNode ParseColor()
        {
            var token = Previous();
            Consume(TokenType.LeftParen, "Expect '(' after 'Color'.");
            var color = Consume(TokenType.ColorLiteral, "Expect color name.");
            Consume(TokenType.RightParen, "Expect ')' after color.");
            return new ColorNode(token, color);
        }

        private SizeNode ParseSize()
        {
            var token = Previous();
            Consume(TokenType.LeftParen, "Expect '(' after 'Size'.");
            var size = ParseExpression();
            Consume(TokenType.RightParen, "Expect ')' after size.");
            return new SizeNode(token, size);
        }

        private DrawLineNode ParseDrawLine()
        {
            var token = Previous();
            Consume(TokenType.LeftParen, "Expect '(' after 'DrawLine'.");
            var dirX = ParseExpression();
            Consume(TokenType.Comma, "Expect ',' after dirX.");
            var dirY = ParseExpression();
            Consume(TokenType.Comma, "Expect ',' after dirY.");
            var distance = ParseExpression();
            Consume(TokenType.RightParen, "Expect ')' after DrawLine parameters.");
            return new DrawLineNode(token, dirX, dirY, distance);
        }

        private DrawCircleNode ParseDrawCircle()
        {
            var token = Previous();
            Consume(TokenType.LeftParen, "Expect '(' after 'DrawCircle'.");
            var dirX = ParseExpression();
            Consume(TokenType.Comma, "Expect ',' after dirX.");
            var dirY = ParseExpression();
            Consume(TokenType.Comma, "Expect ',' after dirY.");
            var radius = ParseExpression();
            Consume(TokenType.RightParen, "Expect ')' after DrawCircle parameters.");
            return new DrawCircleNode(token, dirX, dirY, radius);
        }

        private DrawRectangleNode ParseDrawRectangle()
        {
            var token = Previous();
            Consume(TokenType.LeftParen, "Expect '(' after 'DrawRectangle'.");
            var dirX = ParseExpression();
            Consume(TokenType.Comma, "Expect ',' after dirX.");
            var dirY = ParseExpression();
            Consume(TokenType.Comma, "Expect ',' after dirY.");
            var distance = ParseExpression();
            Consume(TokenType.Comma, "Expect ',' after distance.");
            var width = ParseExpression();
            Consume(TokenType.Comma, "Expect ',' after width.");
            var height = ParseExpression();
            Consume(TokenType.RightParen, "Expect ')' after DrawRectangle parameters.");
            return new DrawRectangleNode(token, dirX, dirY, distance, width, height);
        }

        private FillNode ParseFill()
        {
            var token = Previous();
            Consume(TokenType.LeftParen, "Expect '(' after 'Fill'.");
            Consume(TokenType.RightParen, "Expect ')' after Fill.");
            return new FillNode(token);
        }

        private GoToNode ParseGoTo()
        {
            var token = Previous();
            Consume(TokenType.LeftBracket, "Expect '[' after 'GoTo'.");
            var label = Consume(TokenType.Identifier, "Expect label name.");
            Consume(TokenType.RightBracket, "Expect ']' after label.");
            Consume(TokenType.LeftParen, "Expect '(' after GoTo label.");
            var condition = ParseExpression();
            Consume(TokenType.RightParen, "Expect ')' after condition.");
            return new GoToNode(token, label, condition);
        }

        private AssignmentNode ParseAssignment()
        {
            var identifier = Consume(TokenType.Identifier, "Expect variable name.");
            Consume(TokenType.Assignment, "Expect '<-' after variable name.");
            var expression = ParseExpression();
            return new AssignmentNode(identifier, expression);
        }

        private LabelNode ParseLabel()
        {
            var identifier = Consume(TokenType.Identifier, "Expect label name.");
            return new LabelNode(identifier);
        }

        private ExpressionNode ParseExpression()
        {
            return ParseOr();
        }

        private ExpressionNode ParseOr()
        {
            var expr = ParseAnd();

            while (Match(TokenType.Or))
            {
                var op = Previous();
                var right = ParseAnd();
                expr = new LogicalNode(expr, op, right);
            }

            return expr;
        }

        private ExpressionNode ParseAnd()
        {
            var expr = ParseComparison();

            while (Match(TokenType.And))
            {
                var op = Previous();
                var right = ParseComparison();
                expr = new LogicalNode(expr, op, right);
            }

            return expr;
        }

        private ExpressionNode ParseComparison()
        {
            var expr = ParseTerm();

            while (Match(TokenType.Equal, TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
            {
                var op = Previous();
                var right = ParseTerm();
                expr = new BinaryNode(expr, op, right);
            }

            return expr;
        }

        private ExpressionNode ParseTerm()
        {
            var expr = ParseFactor();

            while (Match(TokenType.Plus, TokenType.Minus))
            {
                var op = Previous();
                var right = ParseFactor();
                expr = new BinaryNode(expr, op, right);
            }

            return expr;
        }

        private ExpressionNode ParseFactor()
        {
            var expr = ParseUnary();

            while (Match(TokenType.Multiply, TokenType.Divide, TokenType.Modulo, TokenType.Power))
            {
                var op = Previous();
                var right = ParseUnary();
                expr = new BinaryNode(expr, op, right);
            }

            return expr;
        }

        private ExpressionNode ParseUnary()
        {
            if (Match(TokenType.Minus))
            {
                var op = Previous();
                var right = ParsePrimary();
                return new UnaryNode(op, right);
            }
            return ParsePrimary();
        }

        private ExpressionNode ParsePrimary()
        {
            if (Match(TokenType.Number)) return new LiteralNode(Previous());
            if (Match(TokenType.Boolean)) return new LiteralNode(Previous());
            if (Match(TokenType.ColorLiteral)) return new LiteralNode(Previous());
            if (Match(TokenType.Identifier)) return new VariableNode(Previous());

            if (Match(TokenType.LeftParen))
            {
                var expr = ParseExpression();
                Consume(TokenType.RightParen, "Expect ')' after expression.");
                return new GroupingNode(expr);
            }

            // Function calls
            if (Check(TokenType.GetActualX) || Check(TokenType.GetActualY) || Check(TokenType.GetCanvasSize) ||
                Check(TokenType.GetColorCount) || Check(TokenType.IsBrushColor) || Check(TokenType.IsBrushSize) ||
                Check(TokenType.IsCanvasColor))
            {
                return ParseFunctionCall();
            }

            throw Error(Peek(), "Expect expression.");
        }

        private FunctionCallNode ParseFunctionCall()
        {
            var functionName = Advance();
            Consume(TokenType.LeftParen, "Expect '(' after function name.");

            var arguments = new List<ExpressionNode>();

            if (!Check(TokenType.RightParen))
            {
                do
                {
                    arguments.Add(ParseExpression());
                } while (Match(TokenType.Comma));
            }

            Consume(TokenType.RightParen, "Expect ')' after function arguments.");
            return new FunctionCallNode(functionName, arguments);
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            throw Error(Peek(), message);
        }

        private ParseError Error(Token token, string message)
        {
            string errorMsg = $"Error at line {token.LineNumber}: {message}";
            Result.AddError(errorMsg);
            return new ParseError();
        }

        private void Synchronize()
        {
            Advance();

            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.EndOfLine) return;

                switch (Peek().Type)
                {
                    case TokenType.Spawn:
                    case TokenType.Color:
                    case TokenType.Size:
                    case TokenType.DrawLine:
                    case TokenType.DrawCircle:
                    case TokenType.DrawRectangle:
                    case TokenType.Fill:
                    case TokenType.GoTo:
                        return;
                }

                Advance();
            }
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }

        private Token Advance()
        {
            if (!IsAtEnd()) _current++;
            return Previous();
        }

        private void Advance(int count)
        {
            for (int i = 0; i < count && !IsAtEnd(); i++)
            {
                _current++;
            }
        }
        private Token PeekAhead(int lookahead = 1)
        {
            int pos = _current + lookahead;
            return pos < _tokens.Count ? _tokens[pos] : null;
        }
        private bool IsAtEnd() => Peek().Type == TokenType.EndOfFile;

        private Token Peek() => _tokens[_current];

        private Token Previous() => _tokens[_current - 1];
    }

    public class ParseError : Exception { }
}