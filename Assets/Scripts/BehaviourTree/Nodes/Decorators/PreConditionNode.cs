namespace BT
{
    public abstract class PreConditionNode : DecoratorNode
    {
        protected override State OnUpdate()
        {
            var result = EvaluateCondition();

            if (!result)
            {
                child.OnStop();
                return State.Failure;
            }

            return child.Update();
        }

        protected override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        protected abstract bool EvaluateCondition();
    }
}