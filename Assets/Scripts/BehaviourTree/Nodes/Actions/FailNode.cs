namespace BT
{
    public class FailNode : ActionNode
    {
        // Node for testing (always returns fail)
        protected override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        protected override void Init()
        {
            description = "This node always fails";
            nodeTitle = "Fail";
        }

        protected override State OnUpdate()
        {
            return State.Failure;
        }
    }
}