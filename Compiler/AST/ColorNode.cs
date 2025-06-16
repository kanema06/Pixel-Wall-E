using System.Collections.Generic;

namespace PixelWallE
{
    public class ColorNode : StatementNode
    {
        public Token Token { get; }
        public Token Color { get; }

        public ColorNode(Token token, Token color)
        {
            Token = token;
            Color = color;
        }
    }
}