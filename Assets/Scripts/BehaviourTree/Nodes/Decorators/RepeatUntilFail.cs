namespace BT
{
    public class RepeatUntilFail : DecoratorNode
    {
        protected override State OnUpdate()
        {
            var childState = child.Update();

            if (childState == State.Failure)
                return State.Failure;

            return State.Running;
        }

        protected override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        protected override void Init()
        {
            nodeTitle = "Repeat Until Fail";
            description = "Repeats child node until it returns FAILURE.";
        }
    }
}