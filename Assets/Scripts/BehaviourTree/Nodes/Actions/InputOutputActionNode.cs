using System.Collections.Generic;

public abstract class InputOutputActionNode : InputActionNode, IOutputProperty
{
    public List<(string, BlackboardPropertyType)> OutputProperties { get; set; } = new();

    public void RegisterOutputProperty(string propertyName, BlackboardPropertyType type, int order)
    {
        if (OutputProperties.Count < order + 1)
        {
            OutputProperties.Add((propertyName, type));
        }
        else
        {
            var property = OutputProperties[order];
            property.Item1 = propertyName;
            property.Item2 = type;
            OutputProperties[order] = property;
        }
    }
}