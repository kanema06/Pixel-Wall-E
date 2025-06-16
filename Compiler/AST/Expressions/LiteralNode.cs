using System.Collections.Generic;

namespace PixelWallE
{
    
    public class LiteralNode : ExpressionNode
    {
        public Token Value { get; }

        public LiteralNode(Token value)
        {
            Value = value;
        }
    }
}