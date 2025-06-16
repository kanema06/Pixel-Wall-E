using System.Collections.Generic;

namespace PixelWallE
{
    public class GroupingNode : ExpressionNode
    {
        public ExpressionNode Expression { get; }

        public GroupingNode(ExpressionNode expression)
        {
            Expression = expression;
        }
    }
}