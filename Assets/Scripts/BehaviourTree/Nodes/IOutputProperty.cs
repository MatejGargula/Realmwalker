using System.Collections.Generic;

public interface IOutputProperty
{
    public List<(string, BlackboardPropertyType)> OutputProperties { get; set; }

    public void RegisterOutputProperty(string propertyName, BlackboardPropertyType type, int order);
}