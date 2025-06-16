using System.Collections.Generic;

namespace PixelWallE
{
    public class FillNode : StatementNode
    {
        public Token Token { get; }

        public FillNode(Token token)
        {
            Token = token;
        }
    }
}