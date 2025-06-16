using System.Collections.Generic;

namespace PixelWallE
{
    
    public class VariableNode : ExpressionNode
    {
        public Token Name { get; }

        public VariableNode(Token name)
        {
            Name = name;
        }
    }
}