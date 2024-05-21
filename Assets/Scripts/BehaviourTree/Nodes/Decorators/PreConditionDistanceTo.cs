using UnityEngine;

namespace BT
{
    public class PreConditionDistanceTo : PreConditionNode
    {
        [Header("Input")] [SerializeField] private string gameObjectProperty = "DangerSource";
        [SerializeField] private string numericDistanceProperty = "distance";
        [SerializeField] private double defaultDistance = 10.0f;

        protected override bool EvaluateCondition()
        {
            if (blackboard.TryGetProperty(gameObjectProperty, out GameObject target))
            {
                if (!blackboard.TryGetProperty(numericDistanceProperty, out double distance))
                    distance = defaultDistance;

                return Vector3.Distance(target.transform.position,
                    Agent.Go.gameObject.transform.position) < distance;
            }

            Debug.LogError($"Blackboard vector property {gameObjectProperty} is missing.");
            return true;
        }

        protected override void Init()
        {
            nodeTitle = "Close to condition.";
            description = "Checks if the agents is close to a vector position.";
        }
    }
}