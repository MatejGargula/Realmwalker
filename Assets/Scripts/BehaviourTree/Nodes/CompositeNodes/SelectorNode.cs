namespace BT
{
    public class SelectorNode : CompositeNode
    {
        private int current;

        protected override void OnStart()
        {
            current = 0;
        }

        public override void OnStop()
        {
        }

        protected override void Init()
        {
            description = "Runs each child. Returns SUCCESS on the first child SUCCESS";
            nodeTitle = "Selector";
        }

        protected override State OnUpdate()
        {
            var child = children[current];
            var childState = child.Update();
            switch (childState)
            {
                case State.Running:
                    return State.Running;
                case State.Failure:
                    current++;
                    break;
                case State.Success:
                    return State.Success;
            }

            if (current >= children.Count) return State.Failure;

            return State.Running;
        }
    }
}