using System;
using BT.Editor.Views.BlackboardView;
using UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace CoreEditor.BehaviourTreeEditor.Blackboard
{
    public class BlackboardView : UIElementBase
    {
        #region Nested class

        public new class UxmlFactory : UxmlFactory<BlackboardView, UxmlTraits>
        {
        }

        #endregion // Nested class

        #region Data members

        private IBlackboardList<double> _numericPropertyList;
        private IBlackboardList<bool> _boolPropertyList;
        private IBlackboardList<string> _stringPropertyList;
        private IBlackboardList<Vector3> _vector3PropertyList;
        private IBlackboardList<GameObject> _gameObjectPropertyList;

        private Button _selectNumericButton;
        private Button _selectBoolButton;
        private Button _selectStringButton;
        private Button _selectVector3Button;
        private Button _selectGameObjectButton;

        private Button _currentlyActiveButton;

        private Button _creationButton;

        private const string ActiveClassStyle = "active-select-button";

        private bool _initialized;
        private BlackboardPropertyType _currentType = BlackboardPropertyType.Numeric;

        #endregion // Data members

        #region Properties

        private BT.Blackboard _blackboard;

        public BT.Blackboard CurrentBlackboard
        {
            get => _blackboard;
            set
            {
                if (_blackboard != value)
                {
                    if (_blackboard != null) _blackboard.OnBlackboardUpdateEvent -= OnBlackboardChanged;

                    _blackboard = value;

                    if (_creationButton != null)
                        _creationButton.style.display = DisplayStyle.Flex;

                    UpdateBlackboardView();
                }
            }
        }

        #endregion // Properties

        #region Override methods

        protected override string GetTemplateResourcePath()
        {
            return "Views/BlackboardView";
        }

        protected override void Init()
        {
            SetUpListViews();
            SetUpSelectButtons();

            SetUpCreation();

            _initialized = true;
        }

        #endregion // Override methods

        #region Private methods

        private void SetUpCreation()
        {
            _creationButton = this.Q<Button>("create-button");
            _creationButton.RegisterCallback<ClickEvent>(CreateProperty);

            _creationButton.style.display = DisplayStyle.None;
        }

        private void CreateProperty(ClickEvent evt)
        {
            if (_blackboard == null)
                return;

            switch (_currentType)
            {
                case BlackboardPropertyType.Numeric:
                    _blackboard.CreateProperty("New Numeric", 0f);
                    _numericPropertyList.RefreshList();
                    break;
                case BlackboardPropertyType.Bool:
                    _blackboard.CreateProperty("New Bool", false);
                    _boolPropertyList.RefreshList();
                    break;
                case BlackboardPropertyType.String:
                    _blackboard.CreateProperty("New String", "");
                    _stringPropertyList.RefreshList();
                    break;
                case BlackboardPropertyType.Vector3:
                    _blackboard.CreateProperty("New Vector3", Vector3.zero);
                    _vector3PropertyList.RefreshList();
                    break;
                case BlackboardPropertyType.GameObject:
                    GameObject defaultValue = null;
                    _blackboard.CreateProperty("New GameObject", defaultValue);
                    _gameObjectPropertyList.RefreshList();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void SetActiveButton(Button button)
        {
            if (_currentlyActiveButton != null)
                _currentlyActiveButton.RemoveFromClassList(ActiveClassStyle);

            button.AddToClassList(ActiveClassStyle);
            _currentlyActiveButton = button;
        }

        private void SetUpListViews()
        {
            {
                var listView = this.Q<ListView>("numeric-property-list");
                var container = this.Q<VisualElement>("list-numeric-container");

                if (listView != null)
                {
                    _numericPropertyList = new BlackboardListNumeric(listView, container);
                    _numericPropertyList.Show();
                }
                else
                {
                    Debug.LogError("Error: Cannot find numeric blackboard property list.");
                }
            }

            {
                var listView = this.Q<ListView>("boolean-property-list");
                var container = this.Q<VisualElement>("list-bool-container");

                if (listView != null)
                {
                    _boolPropertyList = new BlackboardListBool(listView, container);
                    _boolPropertyList.Hide();
                }
                else
                {
                    Debug.LogError("Error: Cannot find boolean blackboard property list.");
                }
            }

            {
                var listView = this.Q<ListView>("string-property-list");
                var container = this.Q<VisualElement>("list-string-container");

                if (listView != null)
                {
                    _stringPropertyList = new BlackboardListString(listView, container);
                    _stringPropertyList.Hide();
                }
                else
                {
                    Debug.LogError("Error: Cannot find string blackboard property list.");
                }
            }

            {
                var listView = this.Q<ListView>("vector3-property-list");
                var container = this.Q<VisualElement>("list-vector3-container");

                if (listView != null)
                {
                    _vector3PropertyList = new BlackboardListVector3(listView, container);
                    _vector3PropertyList.Hide();
                }
                else
                {
                    Debug.LogError("Error: Cannot find vector3 blackboard property list.");
                }
            }

            {
                var listView = this.Q<ListView>("gameobject-property-list");
                var container = this.Q<VisualElement>("list-gameobject-container");

                if (listView != null)
                {
                    _gameObjectPropertyList = new BlackboardListGameObject(listView, container);
                    _gameObjectPropertyList.Hide();
                }
                else
                {
                    Debug.LogError("Error: Cannot find gameobject blackboard property list.");
                }
            }

            if (_blackboard != null)
            {
                _numericPropertyList.ListSource = _blackboard.numericProperties;
                _boolPropertyList.ListSource = _blackboard.boolProperties;
                _stringPropertyList.ListSource = _blackboard.stringProperties;
                _vector3PropertyList.ListSource = _blackboard.vector3Properties;
                _gameObjectPropertyList.ListSource = _blackboard.gameObjectProperties;
            }
        }

        private void SetUpSelectButtons()
        {
            _selectNumericButton = this.Q<Button>("blackboard-select-numeric-properties-button");

            if (_selectNumericButton != null) SetActiveButton(_selectNumericButton);

            _selectNumericButton?.RegisterCallback<ClickEvent>(evt =>
            {
                SetActiveButton(_selectNumericButton);

                _numericPropertyList.Show();
                _boolPropertyList.Hide();
                _stringPropertyList.Hide();
                _vector3PropertyList.Hide();
                _gameObjectPropertyList.Hide();

                _currentType = BlackboardPropertyType.Numeric;

                RefreshLists();
            });

            _selectBoolButton = this.Q<Button>("blackboard-select-boolean-properties-button");
            _selectBoolButton?.RegisterCallback<ClickEvent>(evt =>
            {
                SetActiveButton(_selectBoolButton);

                _numericPropertyList.Hide();
                _boolPropertyList.Show();
                _stringPropertyList.Hide();
                _vector3PropertyList.Hide();
                _gameObjectPropertyList.Hide();

                _currentType = BlackboardPropertyType.Bool;

                RefreshLists();
            });

            _selectStringButton = this.Q<Button>("blackboard-select-string-properties-button");
            _selectStringButton?.RegisterCallback<ClickEvent>(evt =>
            {
                SetActiveButton(_selectStringButton);

                _numericPropertyList.Hide();
                _boolPropertyList.Hide();
                _stringPropertyList.Show();
                _vector3PropertyList.Hide();
                _gameObjectPropertyList.Hide();

                _currentType = BlackboardPropertyType.String;

                RefreshLists();
            });

            _selectVector3Button = this.Q<Button>("blackboard-select-vector3-properties-button");
            _selectVector3Button?.RegisterCallback<ClickEvent>(evt =>
            {
                SetActiveButton(_selectVector3Button);

                _numericPropertyList.Hide();
                _boolPropertyList.Hide();
                _stringPropertyList.Hide();
                _vector3PropertyList.Show();
                _gameObjectPropertyList.Hide();

                _currentType = BlackboardPropertyType.Vector3;

                RefreshLists();
            });

            _selectGameObjectButton = this.Q<Button>("blackboard-select-gameobject-properties-button");
            _selectGameObjectButton?.RegisterCallback<ClickEvent>(evt =>
            {
                SetActiveButton(_selectGameObjectButton);

                _numericPropertyList.Hide();
                _boolPropertyList.Hide();
                _stringPropertyList.Hide();
                _vector3PropertyList.Hide();
                _gameObjectPropertyList.Show();

                _currentType = BlackboardPropertyType.GameObject;

                RefreshLists();
            });
        }

        private void UpdateBlackboardView()
        {
            if (_blackboard == null || !_initialized)
                return;

            _numericPropertyList.ListSource = _blackboard.numericProperties;
            _numericPropertyList.RefreshList();

            _boolPropertyList.ListSource = _blackboard.boolProperties;
            _boolPropertyList.RefreshList();

            _stringPropertyList.ListSource = _blackboard.stringProperties;
            _stringPropertyList.RefreshList();

            _vector3PropertyList.ListSource = _blackboard.vector3Properties;
            _vector3PropertyList.RefreshList();

            _gameObjectPropertyList.ListSource = _blackboard.gameObjectProperties;
            _gameObjectPropertyList.RefreshList();

            _blackboard.OnBlackboardUpdateEvent += OnBlackboardChanged;
        }

        private void OnBlackboardChanged()
        {
            RefreshLists();
        }

        private void RefreshLists()
        {
            _numericPropertyList.RefreshList();
            _boolPropertyList.RefreshList();
            _stringPropertyList.RefreshList();
            _vector3PropertyList.RefreshList();
            _gameObjectPropertyList.RefreshList();
        }

        #endregion // Private methods
    }
}