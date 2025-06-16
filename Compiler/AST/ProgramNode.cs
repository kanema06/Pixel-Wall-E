    using System.Collections.Generic;

    namespace PixelWallE
    {
    public class ProgramNode : Node
    {
        public List<StatementNode> Statements { get; }
        public Dictionary<string, int> Labels { get; }

        public ProgramNode(List<StatementNode> statements, Dictionary<string, int> labels)
        {
            Statements = statements;
            Labels = labels;
        }
    }
    }