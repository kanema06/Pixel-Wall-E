using System.Collections.Generic;

namespace PixelWallE
{
    public class UnaryNode : ExpressionNode
    {
        public Token Operator { get; }
        public ExpressionNode Right { get; }

        public UnaryNode(Token op, ExpressionNode right)
        {
            Operator = op;
            Right = right;
        }
    }
}