using System.Collections.Generic;

namespace PixelWallE
{

    public class SpawnNode : StatementNode
    {
        public Token Token { get; }
        public ExpressionNode X { get; }
        public ExpressionNode Y { get; }

        public SpawnNode(Token token, ExpressionNode x, ExpressionNode y)
        {
            Token = token;
            X = x;
            Y = y;
        }
    }
}