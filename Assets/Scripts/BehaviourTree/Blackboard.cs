#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BT
{
    [Serializable]
    public class Blackboard
    {
        public event Action? OnBlackboardUpdateEvent;

        #region Private methods

        private void OnOnBlackboardUpdate()
        {
            OnBlackboardUpdateEvent?.Invoke();
        }

        #endregion // Private methods

        #region Nested class

        [Serializable]
        public class BlackboardProperty<T>
        {
            public string propName;
            public T value;
            public bool valid;

            public BlackboardProperty(string inPropName, T inValue)
            {
                propName = inPropName;
                value = inValue;
                valid = true;
            }
        }

        #endregion // Nested class

        #region Data members

        public List<BlackboardProperty<double>> numericProperties = new();
        public List<BlackboardProperty<bool>> boolProperties = new();
        public List<BlackboardProperty<string>> stringProperties = new();
        public List<BlackboardProperty<Vector3>> vector3Properties = new();
        public List<BlackboardProperty<GameObject>> gameObjectProperties = new();

        #endregion // Data members

        #region Public methods

        public bool CheckPropertyValid(string propertyName, BlackboardPropertyType type)
        {
            switch (type)
            {
                case BlackboardPropertyType.Numeric:
                    if (numericProperties.Any(prop => prop.propName == propertyName))
                    {
                        var property = numericProperties.Find(x => x.propName == propertyName);
                        return property.valid;
                    }

                    break;
                case BlackboardPropertyType.Bool:
                    if (boolProperties.Any(prop => prop.propName == propertyName))
                    {
                        var property = boolProperties.Find(x => x.propName == propertyName);
                        return property.valid;
                    }

                    break;
                case BlackboardPropertyType.String:
                    if (stringProperties.Any(prop => prop.propName == propertyName))
                    {
                        var property = stringProperties.Find(x => x.propName == propertyName);
                        return property.valid;
                    }

                    break;
                case BlackboardPropertyType.Vector3:
                    if (vector3Properties.Any(prop => prop.propName == propertyName))
                    {
                        var property = vector3Properties.Find(x => x.propName == propertyName);
                        return property.valid;
                    }

                    break;
                case BlackboardPropertyType.GameObject:
                    if (gameObjectProperties.Any(prop => prop.propName == propertyName))
                    {
                        var property = gameObjectProperties.Find(x => x.propName == propertyName);

                        if (property.value == null) property.valid = false;

                        return property.valid;
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return false;
        }

        #region Setters

        public void SetProperty(string propertyName, double value)
        {
            if (numericProperties.Any(prop => prop.propName == propertyName))
            {
                var property = numericProperties.Find(x => x.propName == propertyName);
                property.value = value;
                property.valid = true;
            }
            else
            {
                //Debug.LogError($"Error: Numeric Property {propertyName} does not exist.");
                CreateProperty(propertyName, value);
                //numericProperties.Add(new BlackboardItem<double>(propertyName, value));
            }

            OnOnBlackboardUpdate();
        }

        public void SetProperty(string propertyName, bool value)
        {
            if (boolProperties.Any(prop => prop.propName == propertyName))
            {
                var property = boolProperties.Find(x => x.propName == propertyName);
                property.value = value;
                property.valid = true;
            }
            else
            {
                //Debug.LogError($"Error: Boolean Property {propertyName} does not exist.");
                CreateProperty(propertyName, value);
            }

            OnOnBlackboardUpdate();
        }

        public void SetProperty(string propertyName, string value)
        {
            if (stringProperties.Any(prop => prop.propName == propertyName))
            {
                var property = stringProperties.Find(x => x.propName == propertyName);
                property.value = value;
                property.valid = true;
            }
            else
            {
                //Debug.LogError($"Error: String Property {propertyName} does not exist.");
                CreateProperty(propertyName, value);
            }

            OnOnBlackboardUpdate();
        }

        public void SetProperty(string propertyName, Vector3 value)
        {
            if (vector3Properties.Any(prop => prop.propName == propertyName))
            {
                var property = vector3Properties.Find(x => x.propName == propertyName);
                property.value = value;
                property.valid = true;
            }
            else
            {
                // Debug.LogError($"Error: Vector3 Property {propertyName} does not exist.");
                CreateProperty(propertyName, value);
            }

            OnOnBlackboardUpdate();
        }

        public void SetProperty(string propertyName, GameObject value)
        {
            if (gameObjectProperties.Any(prop => prop.propName == propertyName))
            {
                var property = gameObjectProperties.Find(x => x.propName == propertyName);
                property.value = value;
                property.valid = true;
            }
            else
            {
                // Debug.LogError($"Error: GameObject Property {propertyName} does not exist.");
                CreateProperty(propertyName, value);
            }

            OnOnBlackboardUpdate();
        }

        #endregion // Setters

        #region Getters

        public bool TryGetProperty(string propertyName, out double value)
        {
            if (numericProperties.Any(prop => prop.propName == propertyName))
            {
                value = numericProperties.First(p => p.propName == propertyName).value;
                return true;
            }

            value = 0;

            // Debug.LogError($"Error: Property - {propertyName} - is not set in the blackboard.");
            return false;
        }

        public bool TryGetProperty(string propertyName, out bool value)
        {
            if (boolProperties.Any(prop => prop.propName == propertyName))
            {
                value = boolProperties.First(p => p.propName == propertyName).value;
                return true;
            }

            value = false;

            Debug.LogError($"Error: Property - {propertyName} - is not set in the blackboard.");
            return false;
        }

        public bool TryGetProperty(string propertyName, out string value)
        {
            if (stringProperties.Any(prop => prop.propName == propertyName))
            {
                value = stringProperties.First(p => p.propName == propertyName).value;
                return true;
            }

            value = "";

            Debug.LogError($"Error: Property - {propertyName} - is not set in the blackboard.");
            return false;
        }

        public bool TryGetProperty(string propertyName, out Vector3 value)
        {
            if (vector3Properties.Any(prop => prop.propName == propertyName))
            {
                value = vector3Properties.First(p => p.propName == propertyName).value;
                return true;
            }

            value = Vector3.zero;

            Debug.LogError($"Error: Property - {propertyName} - is not set in the blackboard.");
            return false;
        }

        public bool TryGetProperty(string propertyName, out GameObject value)
        {
            if (gameObjectProperties.Any(prop => prop.propName == propertyName))
            {
                value = gameObjectProperties.First(p => p.propName == propertyName).value;
                return true;
            }

            value = null;

            Debug.LogError($"Error: Property - {propertyName} - is not set in the blackboard.");
            return false;
        }

        #endregion // Getters

        #region Property Delete

        public bool DeleteNumericProperty(string propertyName)
        {
            if (numericProperties.Any(prop => prop.propName == propertyName))
            {
                var toRemove = numericProperties.First(prop => prop.propName == propertyName);
                toRemove.valid = false;

                OnOnBlackboardUpdate();

                return true;
            }

            return false;
        }

        public bool DeleteBoolProperty(string propertyName)
        {
            if (boolProperties.Any(prop => prop.propName == propertyName))
            {
                var toRemove = boolProperties.First(prop => prop.propName == propertyName);
                toRemove.valid = false;

                OnOnBlackboardUpdate();

                return true;
            }

            return false;
        }

        public bool DeleteStringProperty(string propertyName)
        {
            if (stringProperties.Any(prop => prop.propName == propertyName))
            {
                var toRemove = stringProperties.First(prop => prop.propName == propertyName);
                toRemove.valid = false;

                OnOnBlackboardUpdate();

                return true;
            }

            return false;
        }

        public bool DeleteVector3Property(string propertyName)
        {
            if (vector3Properties.Any(prop => prop.propName == propertyName))
            {
                var toRemove = vector3Properties.First(prop => prop.propName == propertyName);
                toRemove.valid = false;

                OnOnBlackboardUpdate();

                return true;
            }

            return false;
        }

        public bool DeleteGameObjectProperty(string propertyName)
        {
            if (gameObjectProperties.Any(prop => prop.propName == propertyName))
            {
                var toRemove = gameObjectProperties.First(prop => prop.propName == propertyName);
                toRemove.valid = false;

                OnOnBlackboardUpdate();

                return true;
            }

            return false;
        }

        #endregion // Property Delete

        #region Property Create

        public void CreateProperty(string propertyName, double value, int repeats = 1)
        {
            var newPropertyName = propertyName;
            if (repeats > 1) newPropertyName = $"{propertyName}({repeats})";

            if (numericProperties.All(prop => prop.propName != newPropertyName))
                numericProperties.Add(new BlackboardProperty<double>(newPropertyName, value));
            else
                CreateProperty(propertyName, value, repeats + 1);

            OnOnBlackboardUpdate();
        }

        public void CreateProperty(string propertyName, bool value, int repeats = 1)
        {
            var newPropertyName = propertyName;
            if (repeats > 1) newPropertyName = $"{propertyName}({repeats})";

            if (boolProperties.All(prop => prop.propName != newPropertyName))
                boolProperties.Add(new BlackboardProperty<bool>(newPropertyName, value));
            else
                CreateProperty(propertyName, value, repeats + 1);

            OnOnBlackboardUpdate();
        }

        public void CreateProperty(string propertyName, string value, int repeats = 1)
        {
            var newPropertyName = propertyName;
            if (repeats > 1) newPropertyName = $"{propertyName}({repeats})";

            if (stringProperties.All(prop => prop.propName != newPropertyName))
                stringProperties.Add(new BlackboardProperty<string>(newPropertyName, value));
            else
                CreateProperty(propertyName, value, repeats + 1);

            OnOnBlackboardUpdate();
        }

        public void CreateProperty(string propertyName, Vector3 value, int repeats = 1)
        {
            var newPropertyName = propertyName;
            if (repeats > 1) newPropertyName = $"{propertyName}({repeats})";

            if (vector3Properties.All(prop => prop.propName != newPropertyName))
                vector3Properties.Add(new BlackboardProperty<Vector3>(newPropertyName, value));
            else
                CreateProperty(propertyName, value, repeats + 1);

            OnOnBlackboardUpdate();
        }

        public void CreateProperty(string propertyName, GameObject value, int repeats = 1)
        {
            var newPropertyName = propertyName;
            if (repeats > 1) newPropertyName = $"{propertyName}({repeats})";

            if (gameObjectProperties.All(prop => prop.propName != newPropertyName))
                gameObjectProperties.Add(new BlackboardProperty<GameObject>(newPropertyName, value));
            else
                CreateProperty(propertyName, value, repeats + 1);

            OnOnBlackboardUpdate();
        }

        #endregion // Property Create

        public void Clear()
        {
            numericProperties.Clear();
            vector3Properties.Clear();
            boolProperties.Clear();
        }

        public Blackboard Clone()
        {
            Blackboard newBlackboard = new()
            {
                numericProperties = new List<BlackboardProperty<double>>(numericProperties),
                boolProperties = new List<BlackboardProperty<bool>>(boolProperties),
                stringProperties = new List<BlackboardProperty<string>>(stringProperties),
                vector3Properties = new List<BlackboardProperty<Vector3>>(vector3Properties),
                gameObjectProperties = new List<BlackboardProperty<GameObject>>(gameObjectProperties)
            };

            return newBlackboard;
        }

        #endregion // Public methods
    }
}