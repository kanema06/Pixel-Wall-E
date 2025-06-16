using System.Collections.Generic;

namespace PixelWallE
{
    
    public class LabelNode : StatementNode
    {
        public Token Label { get; }

        public LabelNode(Token label)
        {
            Label = label;
        }
    }
}