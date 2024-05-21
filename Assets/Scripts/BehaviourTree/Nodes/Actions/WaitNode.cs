using UnityEngine;

namespace BT
{
    public class WaitNode : InputActionNode
    {
        [SerializeField] private string numericDurationProperty = "duration";
        [SerializeField] private double duration = 1;

        private float startTime;

        protected override void OnStart()
        {
            RegisterInputProperty(numericDurationProperty, BlackboardPropertyType.Numeric, 0);

            startTime = Time.time;

            if (blackboard.TryGetProperty(numericDurationProperty, out double newDuration)) duration = newDuration;
        }

        public override void OnStop()
        {
        }

        protected override void Init()
        {
            RegisterInputProperty(numericDurationProperty, BlackboardPropertyType.Numeric, 0);

            description = "Returns SUCCESS after given duration (seconds)";
            nodeTitle = "Wait";
        }

        protected override State OnUpdate()
        {
            if (Time.time - startTime > duration) return State.Success;

            return State.Running;
        }
    }
}