using System.Collections.Generic;
using BT;

public abstract class ConditionNode : Node, IInputProperty
{
    public List<(string, BlackboardPropertyType)> InputProperty { get; set; } = new();

    public void RegisterInputProperty(string propertyName, BlackboardPropertyType type, int order)
    {
        if (InputProperty.Count < order + 1)
        {
            InputProperty.Add((propertyName, type));
        }
        else
        {
            var property = InputProperty[order];
            property.Item1 = propertyName;
            property.Item2 = type;
            InputProperty[order] = property;
        }
    }

    protected override State OnUpdate()
    {
        var conditionResult = EvaluateCondition();

        if (conditionResult) return State.Success;

        return State.Failure;
    }

    public override void OnStop()
    {
    }

    protected abstract bool EvaluateCondition();
}