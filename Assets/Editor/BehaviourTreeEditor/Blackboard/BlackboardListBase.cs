using UnityEngine.UIElements;

namespace BT.Editor.Views.BlackboardView
{
    public abstract class BlackboardListBase
    {
        #region Constructors

        protected BlackboardListBase(ListView listView, VisualElement container)
        {
            _listView = listView;
            _container = container;

            _listView.makeItem = MakeProperty;
            _listView.bindItem = BindProperty;
        }

        #endregion // Constructors

        #region Data members

        protected ListView _listView;
        private readonly VisualElement _container;

        #endregion // Data members

        #region Public methods

        public void RefreshList()
        {
            _listView.RefreshItems();
        }

        public void Hide()
        {
            _container.style.display = DisplayStyle.None;
        }

        public void Show()
        {
            _container.style.display = DisplayStyle.Flex;
        }

        #endregion // Public methods

        #region Private methods

        private VisualElement MakeProperty()
        {
            VisualElement propertyContainer = new()
            {
                name = "blackboard-property-root"
            };
            propertyContainer.AddToClassList("blackboard-property-root");

            TextField propertyNameTextField = new()
            {
                name = "blackboard-property-name"
            };
            propertyNameTextField.AddToClassList("blackboard-property-name");

            var propertyValue = MakeValueField();
            propertyValue.name = "blackboard-property-value";
            propertyValue.AddToClassList("blackboard-property-value");

            Button deletePropertyButton = new()
            {
                name = "delete-property-button",
                text = "X"
            };
            deletePropertyButton.AddToClassList("blackboard-property-delete-button");

            Toggle propertyValid = new()
            {
                name = "property-valid"
            };

            propertyContainer.Add(propertyValid);
            propertyContainer.Add(propertyNameTextField);
            propertyContainer.Add(propertyValue);
            propertyContainer.Add(deletePropertyButton);

            return propertyContainer;
        }

        private void BindProperty(VisualElement property, int index)
        {
            BindItems(property, index);
            var deletePropertyButton = property.Q<Button>("delete-property-button");
            deletePropertyButton.RegisterCallback<ClickEvent, int>(DeleteElementFromList, index);
        }

        private void DeleteElementFromList(ClickEvent evt, int idx)
        {
            _listView.RemoveAt(idx);
            _listView.RefreshItems();

            DeleteElement(idx);
        }

        protected abstract VisualElement MakeValueField();

        protected abstract void BindItems(VisualElement property, int index);

        protected abstract void DeleteElement(int idx);

        #endregion // Private methods
    }
}