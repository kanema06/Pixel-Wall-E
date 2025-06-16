using System.Collections.Generic;

namespace PixelWallE
{
    public class SizeNode : StatementNode
    {
        public Token Token { get; }
        public ExpressionNode Size { get; }

        public SizeNode(Token token, ExpressionNode size)
        {
            Token = token;
            Size = size;
        }
    }
}