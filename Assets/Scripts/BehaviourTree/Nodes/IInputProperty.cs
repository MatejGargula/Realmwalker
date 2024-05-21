using System.Collections.Generic;

public interface IInputProperty
{
    public List<(string, BlackboardPropertyType)> InputProperty { get; set; }

    public void RegisterInputProperty(string propertyName, BlackboardPropertyType type, int order);
}