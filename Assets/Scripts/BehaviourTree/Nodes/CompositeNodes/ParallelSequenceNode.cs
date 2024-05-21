using System.Collections.Generic;

namespace BT
{
    public class ParallelSequenceNode : CompositeNode
    {
        public List<Node> remainingChildren;

        protected override void OnStart()
        {
            foreach (var child in children) remainingChildren.Add(child);
        }

        public override void OnStop()
        {
        }

        protected override void Init()
        {
            description = "Runs all child nodes in parralel. Returns SUCCESS if all children return SUCCESS";
            nodeTitle = "Parallel Sequence";
        }

        protected override State OnUpdate()
        {
            for (var i = 0; i < remainingChildren.Count; i++)
            {
                var child = remainingChildren[i];
                var childState = child.Update();
                switch (childState)
                {
                    case State.Running:
                        break;
                    case State.Failure:
                        return State.Failure;
                    case State.Success:
                        //successCounter++;
                        remainingChildren.RemoveAt(i);
                        break;
                }
            }

            if (remainingChildren.Count < 1) return State.Success;

            return State.Running;
        }
    }
}