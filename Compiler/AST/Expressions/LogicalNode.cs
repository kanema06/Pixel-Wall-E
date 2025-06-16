using System.Collections.Generic;

namespace PixelWallE
{
    public class LogicalNode : ExpressionNode
    {
        public ExpressionNode Left { get; }
        public Token Operator { get; }
        public ExpressionNode Right { get; }

        public LogicalNode(ExpressionNode left, Token op, ExpressionNode right)
        {
            Left = left;
            Operator = op;
            Right = right;
        }
    }
}