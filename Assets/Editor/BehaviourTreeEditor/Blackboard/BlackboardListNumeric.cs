using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BT.Editor.Views.BlackboardView
{
    public class BlackboardListNumeric : BlackboardListBase, IBlackboardList<double>
    {
        #region Constructors

        public BlackboardListNumeric(ListView listView, VisualElement container) : base(listView, container)
        {
        }

        #endregion // Constructors

        #region Public methods

        #region Private methods

        private void UpdateList()
        {
            _listView.itemsSource = _listSource;
            _listView.RefreshItems();
        }

        #endregion // Private methods 

        #endregion // Public methods

        #region Data members

        private List<Blackboard.BlackboardProperty<double>> _listSource;

        public List<Blackboard.BlackboardProperty<double>> ListSource
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
            return new FloatField();
        }

        protected override void BindItems(VisualElement property, int index)
        {
            var textField = property.Q<TextField>("blackboard-property-name");
            textField.value = _listSource[index].propName;
            textField.isDelayed = true;
            textField.RegisterCallback<ChangeEvent<string>, int>(OnChangePropertyName, index);

            var valueField = property.Q<FloatField>("blackboard-property-value");
            valueField.value = (float)_listSource[index].value;
            valueField.RegisterCallback<ChangeEvent<float>, int>(OnChangePropertyValue, index);

            var propertyValid = property.Q<Toggle>("property-valid");
            propertyValid.value = _listSource[index].valid;
            propertyValid.RegisterCallback<ChangeEvent<bool>, int>(OnChangePropertyValid, index);
        }

        private void OnChangePropertyValid(ChangeEvent<bool> evt, int index)
        {
            _listSource[index].valid = evt.newValue;
        }


        private void OnChangePropertyValue(ChangeEvent<float> evt, int index)
        {
            _listSource[index].value = evt.newValue;
        }

        private void OnChangePropertyName(ChangeEvent<string> evt, int index)
        {
            var newName = evt.newValue;

            if (_listSource.All(prop => prop.propName != newName)) _listSource[index].propName = newName;

            RefreshList();
        }

        protected override void DeleteElement(int idx)
        {
            _listSource.RemoveAt(idx);
        }

        #endregion // Override methods
    }
}