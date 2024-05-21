namespace BT
{
    public class RootNode : Node
    {
        public Node child;

        protected override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        protected override void Init()
        {
            description = "Root node of the behaviour tree";
            nodeTitle = "Root";
        }

        protected override State OnUpdate()
        {
            return child.Update();
        }

        public override Node Clone()
        {
            var node = Instantiate(this);
            node.child = child.Clone();
            return node;
        }
    }
}