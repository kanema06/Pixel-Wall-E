using System.Collections.Generic;

namespace PixelWallE
{
    public class GoToNode : StatementNode
    {
        public Token Token { get; }
        public Token Label { get; }
        public ExpressionNode Condition { get; }

        public GoToNode(Token token, Token label, ExpressionNode condition)
        {
            Token = token;
            Label = label;
            Condition = condition;
        }
    }
}