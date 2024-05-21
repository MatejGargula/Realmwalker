using System.Collections.Generic;
using BT;

public abstract class InputActionNode : ActionNode, IInputProperty
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
}