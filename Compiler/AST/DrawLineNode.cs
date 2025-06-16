using System.Collections.Generic;

namespace PixelWallE
{
    public class DrawLineNode : StatementNode
    {
        public Token Token { get; }
        public ExpressionNode DirX { get; }
        public ExpressionNode DirY { get; }
        public ExpressionNode Distance { get; }

        public DrawLineNode(Token token, ExpressionNode dirX, ExpressionNode dirY, ExpressionNode distance)
        {
            Token = token;
            DirX = dirX;
            DirY = dirY;
            Distance = distance;
        }
    }
}