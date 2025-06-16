using System.Collections.Generic;

namespace PixelWallE
{
    public class AssignmentNode : StatementNode
    {
        public Token Variable { get; }
        public ExpressionNode Expression { get; }

        public AssignmentNode(Token variable, ExpressionNode expression)
        {
            Variable = variable;
            Expression = expression;
        }
    }
}