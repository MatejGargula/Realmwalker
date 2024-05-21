using UnityEngine;

namespace BT
{
    public class PreConditionBool : PreConditionNode
    {
        [Header("Input")] [SerializeField] private string boolProperty = "condition";

        protected override void Init()
        {
            nodeTitle = "Bool Condition";
            description =
                "Checks a blackboard bool property before running the condition. If the condition is false the node fails.";
        }

        protected override bool EvaluateCondition()
        {
            if (blackboard.TryGetProperty(boolProperty, out bool condition)) return condition;
            Debug.Log($"Can't find property {boolProperty} in blackboard.");

            return false;
        }
    }
}