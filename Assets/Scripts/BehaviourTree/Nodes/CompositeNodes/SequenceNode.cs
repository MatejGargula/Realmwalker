namespace BT
{
    public class SequenceNode : CompositeNode
    {
        public int current;

        protected override void OnStart()
        {
            current = 0;
        }

        public override void OnStop()
        {
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
                    return State.Failure;
                case State.Success:
                    current++;
                    break;
            }

            if (current >= children.Count)
                return State.Success;

            return State.Running;
        }

        protected override void Init()
        {
            description = "Runs each child. Returns SUCCESS if all children return SUCCESS";
            nodeTitle = "Sequence";
        }
    }
}