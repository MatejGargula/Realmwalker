using System.Collections.Generic;

namespace BT
{
    public abstract class CompositeNode : Node
    {
        public List<Node> children = new();

        public override Node Clone()
        {
            var node = Instantiate(this);
            node.children = children.ConvertAll(c => c.Clone());
            return node;
        }
    }
}