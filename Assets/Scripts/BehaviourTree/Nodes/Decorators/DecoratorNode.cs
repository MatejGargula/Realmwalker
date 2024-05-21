namespace BT
{
    public abstract class DecoratorNode : Node
    {
        public Node child;

        public override Node Clone()
        {
            var node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }
}