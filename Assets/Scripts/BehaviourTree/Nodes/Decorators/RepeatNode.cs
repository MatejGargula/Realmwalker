namespace BT
{
    public class RepeatNode : DecoratorNode
    {
        protected override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        protected override void Init()
        {
            nodeTitle = "Repeat";
            description = "Runs it's child in an infinite loop";
        }

        protected override State OnUpdate()
        {
            var childState = child.Update();

            return State.Running;
        }
    }
}