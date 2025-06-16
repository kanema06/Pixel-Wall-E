using System.Collections.Generic;

namespace PixelWallE
{

    public class DrawCircleNode : StatementNode
    {
        public Token Token { get; }
        public ExpressionNode DirX { get; }
        public ExpressionNode DirY { get; }
        public ExpressionNode Radius { get; }

        public DrawCircleNode(Token token, ExpressionNode dirX, ExpressionNode dirY, ExpressionNode radius)
        {
            Token = token;
            DirX = dirX;
            DirY = dirY;
            Radius = radius;
        }
    }
}