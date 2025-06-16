using System.Collections.Generic;

namespace PixelWallE
{
    public class DrawRectangleNode : StatementNode
    {
        public Token Token { get; }
        public ExpressionNode DirX { get; }
        public ExpressionNode DirY { get; }
        public ExpressionNode Distance { get; }
        public ExpressionNode Width { get; }
        public ExpressionNode Height { get; }

        public DrawRectangleNode(Token token, ExpressionNode dirX, ExpressionNode dirY, 
                              ExpressionNode distance, ExpressionNode width, ExpressionNode height)
        {
            Token = token;
            DirX = dirX;
            DirY = dirY;
            Distance = distance;
            Width = width;
            Height = height;
        }
    }
}