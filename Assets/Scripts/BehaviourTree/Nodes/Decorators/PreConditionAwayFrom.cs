using UnityEngine;

namespace BT
{
    public class PreConditionAwayFrom : PreConditionNode
    {
        [Header("Input")] [SerializeField] private string vectorProperty = "DangerSource";

        [SerializeField] private float distance = 10.0f;


        protected override bool EvaluateCondition()
        {
            if (blackboard.TryGetProperty(vectorProperty, out Vector3 dangerSourcePosition))
                return Vector3.Distance(dangerSourcePosition, Agent.Go.gameObject.transform.position) > distance;

            Debug.LogError($"Blackboard vector property {vectorProperty} is missing.");
            return false;
        }

        protected override void Init()
        {
            nodeTitle = "Away from condition.";
            description = "Checks if the agents is in a safe distance to a vector position.";
        }
    }
}