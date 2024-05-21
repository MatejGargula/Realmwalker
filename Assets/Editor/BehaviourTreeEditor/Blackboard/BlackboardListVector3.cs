using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace BT.Editor.Views.BlackboardView
{
    public class BlackboardListVector3 : BlackboardListBase, IBlackboardList<Vector3>
    {
        #region Constructors

        public BlackboardListVector3(ListView listView, VisualElement container) : base(listView, container)
        {
        }

        #endregion // Constructors

        #region Data members

        private List<Blackboard.BlackboardProperty<Vector3>> _listSource;

        public List<Blackboard.BlackboardProperty<Vector3>> ListSource
        {
            get => _listSource;
            set
            {
                _listSource = value;
                UpdateList();
            }
        }

        #endregion // Data members

        #region Override methods

        protected override VisualElement MakeValueField()
        {
            return new Vector3Field();
        }

        protected override void BindItems(VisualElement property, int index)
        {
            var textField = property.Q<TextField>("blackboard-property-name");
            textField.value = _listSource[index].propName;
            textField.isDelayed = true;
            textField.RegisterCallback<ChangeEvent<string>, int>(OnChangePropertyName, index);

            var valueField = property.Q<Vector3Field>("blackboard-property-value");
            valueField.value = _listSource[index].value;
            valueField.RegisterCallback<ChangeEvent<Vector3>, int>(OnChangePropertyValue, index);

            var propertyValid = property.Q<Toggle>("property-valid");
            propertyValid.value = _listSource[index].valid;
            propertyValid.RegisterCallback<ChangeEvent<bool>, int>(OnChangePropertyValid, index);
        }

        private void OnChangePropertyValid(ChangeEvent<bool> evt, int index)
        {
            _listSource[index].valid = evt.newValue;
        }

        private void OnChangePropertyValue(ChangeEvent<Vector3> evt, int index)
        {
            _listSource[index].value = evt.newValue;
        }

        #endregion // Override methods

        #region Public methods

        #region Private methods

        private void OnChangePropertyName(ChangeEvent<string> evt, int index)
        {
            var newName = evt.newValue;

            if (_listSource.All(prop => prop.propName != newName)) _listSource[index].propName = newName;

            RefreshList();
        }

        private void UpdateList()
        {
            _listView.itemsSource = _listSource;
            _listView.RefreshItems();
        }

        protected override void DeleteElement(int idx)
        {
            _listSource.RemoveAt(idx);
        }

        #endregion // Private methods 

        #endregion // Public methods
    }
}