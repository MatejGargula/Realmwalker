namespace BT
{
    public class InverterNode : DecoratorNode
    {
        protected override State OnUpdate()
        {
            var childState = child.Update();

            if (childState == State.Success) return State.Failure;

            if (childState == State.Failure) return State.Success;

            return childState;
        }

        protected override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        protected override void Init()
        {
            nodeTitle = "Inverter";
            description =
                "Inverts the return code of the child task. If child returns Success the inverter returns Failure, etc.";
        }
    }
}