using UnityEngine;

public class PropertyValidConditionNode : ConditionNode
{
    [SerializeField] private BlackboardPropertyType propertyType;
    [SerializeField] private string propertyName;

    protected override bool EvaluateCondition()
    {
        return blackboard.CheckPropertyValid(propertyName, propertyType);
    }

    protected override void OnStart()
    {
        RegisterInputProperty(propertyName, propertyType, 0);
    }

    protected override void Init()
    {
        RegisterInputProperty(propertyName, propertyType, 0);

        nodeTitle = "Is Property Valid";
        description = "Checks if a given property is valid";
    }
}