using System.Collections.Generic;

namespace PixelWallE
{
    public class FunctionCallNode : ExpressionNode
    {
        public Token FunctionName { get; }
        public List<ExpressionNode> Arguments { get; }

        public FunctionCallNode(Token functionName, List<ExpressionNode> arguments)
        {
            FunctionName = functionName;
            Arguments = arguments;
        }
    }
}