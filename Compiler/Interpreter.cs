using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
namespace PixelWallE
{
    public class Interpreter
    {
        public ExecutionResult Result { get; } = new ExecutionResult();
        private readonly ProgramNode _program;
        private int _currentStatement;
        private readonly Dictionary<string, dynamic> _variables = new Dictionary<string, dynamic>();
        private readonly Dictionary<string, int> _labels;
        private readonly List<string> _errors = new List<string>();

        public int WallE_X { get; private set; } = -1;
        public int WallE_Y { get; private set; } = -1;
        public string BrushColor { get; private set; } = "Transparent";
        public int BrushSize { get; private set; } = 1;
        public bool HasSpawned { get; private set; } = false;
        private int _jumpCount = 0;
        private const int MaxJumpCount = 1000;


        public Interpreter(ProgramNode program)
        {
            _program = program;
            _labels = program.Labels;
            _currentStatement = 0;
        }

        public void Interpret()
        {
            _currentStatement = 0;
            _jumpCount = 0;
            _currentStatement = 0;

            while (_currentStatement < _program.Statements.Count)
            {
                try
                {
                    int currentIndex = _currentStatement;
                    ExecuteStatement(_program.Statements[_currentStatement]);

                    if (_currentStatement == currentIndex)
                    {
                        _currentStatement++;
                    }
                }
                catch (RuntimeError ex)
                {
                    Result.AddError(ex.Message);
                    break;
                }
                if (_jumpCount > MaxJumpCount)
                {
                    Result.AddError("Execution halted: Possible infinite loop detected");
                    break;
                }
            }

            if (Result.Errors.Count == 0)
            {
                Result.AddOutput("Execution completed successfully.");
            }
            else
            {
                Result.AddOutput("Execution completed with errors:");
            }
        }
        private void ExecuteStatement(StatementNode statement)
        {
            if (!HasSpawned && (statement is not SpawnNode))
            {
                throw new RuntimeError($"Wall-E must be spawned first");
            }
            switch (statement)
            {
                case SpawnNode spawn:
                    ExecuteSpawn(spawn);
                    break;
                case ColorNode color:
                    ExecuteColor(color);
                    break;
                case SizeNode size:
                    ExecuteSize(size);
                    break;
                case DrawLineNode drawLine:
                    ExecuteDrawLine(drawLine);
                    break;
                case DrawCircleNode drawCircle:
                    ExecuteDrawCircle(drawCircle);
                    break;
                case DrawRectangleNode drawRectangle:
                    ExecuteDrawRectangle(drawRectangle);
                    break;
                case FillNode fill:
                    ExecuteFill(fill);
                    break;
                case GoToNode goTo:
                    ExecuteGoTo(goTo);
                    break;
                case AssignmentNode assignment:
                    ExecuteAssignment(assignment);
                    break;
                case LabelNode label:
                    break;
                default:
                    throw new RuntimeError($"Unknown statement type: {statement.GetType().Name}");
            }
        }

        private void ExecuteSpawn(SpawnNode spawn)
        {
            if (HasSpawned)
            {
                throw new RuntimeError($"Line {spawn.Token.LineNumber}: Spawn can only be used once");
            }

            int x = EvaluateExpression(spawn.X);
            int y = EvaluateExpression(spawn.Y);

            if (x < 0 || x >= CanvasManager.CanvasSize || y < 0 || y >= CanvasManager.CanvasSize)
            {
                throw new RuntimeError($"Line {spawn.Token.LineNumber}: Spawn coordinates out of bounds");
            }

            WallE_X = x;
            WallE_Y = y;
            HasSpawned = true;
            Result.AddOutput($"Visual: Wall-E spawned at position ({x}, {y})");
            GodotCommands.ExecuteGodotSpawn(WallE_X, WallE_Y);
        }

        private void ExecuteColor(ColorNode color)
        {
            BrushColor = color.Color.Value;
            GodotCommands.ExecuteGodotColor(BrushColor);
            Result.AddOutput($"Visual: Brush color changed to {BrushColor}");
        }

        private void ExecuteSize(SizeNode size)
        {
            int sizeValue = EvaluateExpression(size.Size);
            if (sizeValue <= 0)
            {
                throw new RuntimeError($"Line {size.Token.LineNumber}: Brush size must be positive");
            }

            BrushSize = sizeValue % 2 == 0 ? sizeValue - 1 : sizeValue;
            Result.AddOutput($"Visual: Brush size changed to {BrushSize}");
            GodotCommands.ExecuteGodotSize(BrushSize);
        }

        private void ExecuteDrawLine(DrawLineNode drawLine)
        {
            if (!HasSpawned)
            {
                throw new RuntimeError($"Line {drawLine.Token.LineNumber}: Wall-E must be spawned first");
            }

            int dirX = EvaluateExpression(drawLine.DirX);
            int dirY = EvaluateExpression(drawLine.DirY);
            int distance = EvaluateExpression(drawLine.Distance);

            if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1)
            {
                throw new RuntimeError($"Line {drawLine.Token.LineNumber}: Direction values must be -1, 0, or 1");
            }

            if (distance <= 0)
            {
                throw new RuntimeError($"Line {drawLine.Token.LineNumber}: Distance must be positive");
            }

            int endX = WallE_X + dirX * distance;
            int endY = WallE_Y + dirY * distance;

            if (endX < 0 || endX >= CanvasManager.CanvasSize || endY < 0 || endY >= CanvasManager.CanvasSize)
            {
                throw new RuntimeError($"Line {drawLine.Token.LineNumber}: Line would go out of canvas bounds");
            }

            Result.AddOutput($"Visual: Drawing line from ({WallE_X}, {WallE_Y}) to ({endX}, {endY}) with color {BrushColor} and size {BrushSize}");

            WallE_X = endX;
            WallE_Y = endY;
            GodotCommands.ExecuteGodotDrawLine(dirX, dirY, distance);
        }

        private void ExecuteDrawCircle(DrawCircleNode drawCircle)
        {
            if (!HasSpawned)
            {
                throw new RuntimeError($"Line {drawCircle.Token.LineNumber}: Wall-E must be spawned first");
            }

            int dirX = EvaluateExpression(drawCircle.DirX);
            int dirY = EvaluateExpression(drawCircle.DirY);
            int radius = EvaluateExpression(drawCircle.Radius);

            if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1)
            {
                throw new RuntimeError($"Line {drawCircle.Token.LineNumber}: Direction values must be -1, 0, or 1");
            }

            if (radius <= 0)
            {
                throw new RuntimeError($"Line {drawCircle.Token.LineNumber}: Radius must be positive");
            }

            int centerX = WallE_X + dirX * radius;
            int centerY = WallE_Y + dirY * radius;

            if (centerX < 0 || centerX >= CanvasManager.CanvasSize || centerY < 0 || centerY >= CanvasManager.CanvasSize)
            {
                throw new RuntimeError($"Line {drawCircle.Token.LineNumber}: Circle center would be out of canvas bounds");
            }

            Result.AddOutput($"Visual: Drawing circle at center ({centerX}, {centerY}) with radius {radius} and color {BrushColor}");

            WallE_X = centerX;
            WallE_Y = centerY;
            GodotCommands.ExecuteGodotDrawCircle(dirX, dirY, radius);
        }

        private void ExecuteDrawRectangle(DrawRectangleNode drawRectangle)
        {
            if (!HasSpawned)
            {
                throw new RuntimeError($"Line {drawRectangle.Token.LineNumber}: Wall-E must be spawned first");
            }

            int dirX = EvaluateExpression(drawRectangle.DirX);
            int dirY = EvaluateExpression(drawRectangle.DirY);
            int distance = EvaluateExpression(drawRectangle.Distance);
            int width = EvaluateExpression(drawRectangle.Width);
            int height = EvaluateExpression(drawRectangle.Height);

            if (Math.Abs(dirX) > 1 || Math.Abs(dirY) > 1)
            {
                throw new RuntimeError($"Line {drawRectangle.Token.LineNumber}: Direction values must be -1, 0, or 1");
            }

            if (distance <= 0 || width <= 0 || height <= 0)
            {
                throw new RuntimeError($"Line {drawRectangle.Token.LineNumber}: Distance, width, and height must be positive");
            }

            int centerX = WallE_X + dirX * distance;
            int centerY = WallE_Y + dirY * distance;

            Result.AddOutput($"Visual: Drawing rectangle centered at ({centerX}, {centerY}) with width {width}, height {height}, and color {BrushColor}");

            WallE_X = centerX;
            WallE_Y = centerY;
            GodotCommands.ExecuteGodotDrawRectangle(dirX, dirY, distance, width, height);
        }

        private void ExecuteFill(FillNode fill)
        {
            if (!HasSpawned)
            {
                throw new RuntimeError($"Line {fill.Token.LineNumber}: Wall-E must be spawned first");
            }

            Result.AddOutput($"Visual: Filling area around ({WallE_X}, {WallE_Y}) with color {BrushColor}");
            GodotCommands.ExecuteGodotFill();
        }

        private void ExecuteGoTo(GoToNode goTo)
        {
            bool condition = EvaluateExpression(goTo.Condition);
            if (condition)
            {
                _jumpCount++;
                if (!_labels.TryGetValue(goTo.Label.Value, out int targetLine))
                {
                    throw new RuntimeError($"Line {goTo.Token.LineNumber}: Label '{goTo.Label.Value}' not found");
                }

                _currentStatement = targetLine;
                Result.AddOutput($"Visual: Jumping to label '{goTo.Label.Value}' at line {targetLine + 1}");
            }
            else
            {
                Result.AddOutput($"Visual: GoTo condition not met, continuing to next line");
            }
        }

        private void ExecuteAssignment(AssignmentNode assignment)
        {
            dynamic value = EvaluateExpression(assignment.Expression);
            _variables[assignment.Variable.Value] = value;
            Result.AddOutput($"Visual: Variable '{assignment.Variable.Value}' set to {value}");
        }

        private dynamic EvaluateExpression(ExpressionNode expression)
        {
            switch (expression)
            {
                case LiteralNode literal:
                    if (literal.Value.Type == TokenType.Number)
                    {
                        if (int.TryParse(literal.Value.Value, out int val))
                        {
                            return val;
                        }
                        throw new RuntimeError($"Invalid number format: '{literal.Value.Value}'");
                    }
                    else if (literal.Value.Type == TokenType.Boolean)
                    {
                        return literal.Value.Value == "true";
                    }
                    else if (literal.Value.Type == TokenType.ColorLiteral)
                    {
                        return literal.Value.Value;
                    }
                    break;

                case VariableNode variable:
                    if (!_variables.TryGetValue(variable.Name.Value, out dynamic value))
                    {
                        throw new RuntimeError($"Line {variable.Name.LineNumber}: Undefined variable '{variable.Name.Value}'");
                    }
                    return value;

                case BinaryNode binary:
                    var left = EvaluateExpression(binary.Left);
                    var right = EvaluateExpression(binary.Right);

                    switch (binary.Operator.Type)
                    {
                        case TokenType.Plus: return left + right;
                        case TokenType.Minus: return left - right;
                        case TokenType.Multiply: return left * right;
                        case TokenType.Divide: return left / right;
                        case TokenType.Modulo: return left % right;
                        case TokenType.Power: return Math.Pow((double)left, (double)right);
                        case TokenType.Equal: return left == right;
                        case TokenType.Greater: return left > right;
                        case TokenType.GreaterEqual: return left >= right;
                        case TokenType.Less: return left < right;
                        case TokenType.LessEqual: return left <= right;
                    }
                    break;

                case UnaryNode unary:
                    var expr = EvaluateExpression(unary.Right);
                    if (unary.Operator.Type == TokenType.Minus)
                    {
                        return -expr;
                        throw new RuntimeError($"Line {unary.Operator.LineNumber}: Unary minus requires an integer operand");
                    }
                    break;

                case LogicalNode logical:
                    var l = EvaluateExpression(logical.Left);
                    var r = EvaluateExpression(logical.Right);

                    if (logical.Operator.Type == TokenType.And)
                    {
                        return l && r;
                    }
                    else if (logical.Operator.Type == TokenType.Or)
                    {
                        return l || r;
                    }
                    break;

                case GroupingNode grouping:
                    return EvaluateExpression(grouping.Expression);

                case FunctionCallNode functionCall:
                    return ExecuteFunction(functionCall);

                default:
                    throw new RuntimeError($"Unknown expression type: {expression.GetType().Name}");
            }

            throw new RuntimeError("Invalid expression evaluation");
        }
        private dynamic ExecuteFunction(FunctionCallNode functionCall)
        {
            switch (functionCall.FunctionName.Type)
            {
                case TokenType.GetActualX:
                    if (!HasSpawned)
                    {
                        throw new RuntimeError($"Line {functionCall.FunctionName.LineNumber}: Wall-E must be spawned first");
                    }
                    return GodotCommands.GetActualX();

                case TokenType.GetActualY:
                    if (!HasSpawned)
                    {
                        throw new RuntimeError($"Line {functionCall.FunctionName.LineNumber}: Wall-E must be spawned first");
                    }
                    return GodotCommands.GetActualY();

                case TokenType.GetCanvasSize:
                    return GodotCommands.GetCanvasSize();

                case TokenType.IsBrushColor:
                    if (functionCall.Arguments.Count != 1)
                    {
                        throw new RuntimeError($"Line {functionCall.FunctionName.LineNumber}: IsBrushColor expects 1 argument");
                    }
                    var colorArg = EvaluateExpression(functionCall.Arguments[0]);
                    if (colorArg is string colorStr)
                    {
                        return GodotCommands.IsBrushColor(colorStr);
                    }
                    throw new RuntimeError($"Line {functionCall.FunctionName.LineNumber}: Invalid color argument for IsBrushColor");

                case TokenType.GetColorCount:
                    if (functionCall.Arguments.Count != 5)
                    {
                        throw new RuntimeError($"Line {functionCall.FunctionName.LineNumber}: GetColorCount expects 5 arguments");
                    }
                    var colorArg2 = EvaluateExpression(functionCall.Arguments[0]);
                    var x1 = EvaluateExpression(functionCall.Arguments[1]);
                    var y1 = EvaluateExpression(functionCall.Arguments[2]);
                    var x2 = EvaluateExpression(functionCall.Arguments[3]);
                    var y2 = EvaluateExpression(functionCall.Arguments[4]);
                    if (colorArg2 is string color && x1 is int intx1 && x2 is int intx2 && y1 is int inty1 && y2 is int inty2)
                    {
                        return GodotCommands.GetColorCount(color, intx1, inty1, intx2, inty2);
                    }
                    throw new RuntimeError($"Line {functionCall.FunctionName.LineNumber}: Invalid argument for GetColorCount");


                case TokenType.IsCanvasColor:
                    if (functionCall.Arguments.Count != 3)
                    {
                        throw new RuntimeError($"Line {functionCall.FunctionName.LineNumber}: IsCanvasColor expects 3 arguments");
                    }
                    var colorArg3 = EvaluateExpression(functionCall.Arguments[0]);
                    var h = EvaluateExpression(functionCall.Arguments[1]);
                    var v = EvaluateExpression(functionCall.Arguments[2]);
                    if (colorArg3 is string color2 && h is int inth && v is int intv)
                    {
                        return GodotCommands.IsCanvasColor(color2, h, v);
                    }
                    throw new RuntimeError($"Line{functionCall.FunctionName.LineNumber}: Invalid argument for IsCanvasColor");

                case TokenType.IsBrushSize:
                    if (functionCall.Arguments.Count != 1)
                    {
                        throw new RuntimeError($"Line {functionCall.FunctionName.LineNumber}: IsBrushSize expects 1 argument");
                    }
                    var sizearg = EvaluateExpression(functionCall.Arguments[0]);
                    if (sizearg is int size)
                    {
                        return GodotCommands.IsBrushSize(size);
                    }
                    throw new RuntimeError($"Line {functionCall.FunctionName.LineNumber}: Invalid size argument for IsBrushSize");


                default:
                    throw new RuntimeError($"Line {functionCall.FunctionName.LineNumber}: Unknown function '{functionCall.FunctionName.Value}'");
            }
        }
    }


}