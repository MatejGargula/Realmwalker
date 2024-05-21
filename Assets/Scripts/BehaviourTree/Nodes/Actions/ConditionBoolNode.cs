using UnityEngine;

namespace BT
{
    public class ConditionBoolNode : ConditionNode
    {
        [Header("Input")] [SerializeField] private string boolPropertyName = "condition";

        protected override bool EvaluateCondition()
        {
            if (!blackboard.TryGetProperty(boolPropertyName, out bool condition))
                return false;

            return condition;
        }

        protected override void OnStart()
        {
            RegisterInputProperty(boolPropertyName, BlackboardPropertyType.Bool, 0);
        }

        protected override void Init()
        {
            RegisterInputProperty(boolPropertyName, BlackboardPropertyType.Bool, 0);

            description = "Checks a bool value in the blackboard. If true returns Success. If False returns Failure";
            nodeTitle = "Condition Bool";
        }
    }
}