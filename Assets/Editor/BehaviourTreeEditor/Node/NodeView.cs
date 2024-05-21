using System;
using System.Collections.Generic;
using BT;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using Node = UnityEditor.Experimental.GraphView.Node;

namespace CoreEditor.BehaviourTreeEditor
{
    public class NodeView : Node
    {
        #region Constructor

        public NodeView(BT.Node node) : base("Assets/Editor/BehaviourTreeEditor/Node/NodeView.uxml")
        {
            if (node is null)
                return;

            Node = node;
            title = node.nodeTitle;
            viewDataKey = node.guid;

            name = "nodeView";

            style.left = node.position.x;
            style.top = node.position.y;

            CreateInputPorts();
            CreateOutputPorts();
            SetUpColorClasses();

            SerializedObject n = new(node);

            SetUpInputOutpuProperties();

            _descriptionLable = this.Q<Label>("description");
            _descriptionLable.bindingPath = "description";
            _descriptionLable.Bind(n);

            _nodeStateBorder = this.Q<VisualElement>("node-state-border");
            _nodeStateBorder.style.borderBottomColor = Color.clear;
            _nodeStateBorder.style.borderTopColor = Color.clear;
            _nodeStateBorder.style.borderRightColor = Color.clear;
            _nodeStateBorder.style.borderLeftColor = Color.clear;

            _titleLable = this.Q<Label>("title-label");
            _titleLable.bindingPath = "nodeTitle";
            _titleLable.Bind(n);

            node.StateChangedEvent += OnStateChanged;
        }

        #endregion // Constructor

        #region Data Members

        public Action<NodeView> OnNodeSelected;
        public readonly BT.Node Node;
        public Port Input;
        public Port Output;

        private VisualElement _inputContainer;
        private VisualElement _outputContainer;

        private List<(string, BlackboardPropertyType)> _inputPropertyList;
        private List<(string, BlackboardPropertyType)> _outputPropertyList;

        private List<VisualElement> _inputPropertyViews;
        private List<VisualElement> _outputPropertyViews;

        private readonly VisualElement _nodeStateBorder;
        private readonly Label _descriptionLable;
        private readonly Label _titleLable;

        private ValueAnimation<Color> _stateChangedAnim;

        #endregion // Data Members

        #region Private methods

        private void SetUpInputOutpuProperties()
        {
            _inputContainer = this.Q<VisualElement>("input-properties-container");
            _outputContainer = this.Q<VisualElement>("output-properties-container");


            if (Node is IInputProperty inputProperty)
            {
                _inputPropertyViews = InitProperties(_inputContainer, inputProperty.InputProperty);
                _inputPropertyList = inputProperty.InputProperty;
            }

            if (Node is IOutputProperty outputProperty)
            {
                _outputPropertyViews = InitProperties(_outputContainer, outputProperty.OutputProperties);
                _outputPropertyList = outputProperty.OutputProperties;
            }

            if (!Node.hasRun) HideProperties();
        }

        private List<VisualElement> InitProperties(VisualElement container,
            List<(string, BlackboardPropertyType)> propertyList)
        {
            List<VisualElement> propertyViews = new();

            foreach (var property in propertyList)
            {
                VisualTreeAsset template = null;
                switch (property.Item2)
                {
                    case BlackboardPropertyType.Numeric:
                        template = Resources.Load<VisualTreeAsset>("Views/PropertyViews/NumericPropertyView");
                        break;
                    case BlackboardPropertyType.Bool:
                        template = Resources.Load<VisualTreeAsset>("Views/PropertyViews/BoolPropertyView");
                        break;
                    case BlackboardPropertyType.String:
                        template = Resources.Load<VisualTreeAsset>("Views/PropertyViews/StringPropertyView");
                        break;
                    case BlackboardPropertyType.Vector3:
                        template = Resources.Load<VisualTreeAsset>("Views/PropertyViews/Vector3PropertyView");
                        break;
                    case BlackboardPropertyType.GameObject:
                        template = Resources.Load<VisualTreeAsset>("Views/PropertyViews/GameObjectPropertyView");
                        break;
                    default:
                        Debug.LogError("WARNING: Incorrect property view");
                        template = Resources.Load<VisualTreeAsset>("Views/PropertyViews/NumericPropertyView");
                        break;
                }

                if (template != null)
                {
                    VisualElement propertyView = template.Instantiate();
                    propertyViews.Add(propertyView);

                    container.Add(propertyView);
                }
            }

            return propertyViews;
        }

        private void SetUpColorClasses()
        {
            if (Node is RootNode) AddToClassList("root");

            if (Node is ActionNode) AddToClassList("action");

            if (Node is ConditionNode) AddToClassList("condition");

            if (Node is DecoratorNode) AddToClassList("decorator");

            if (Node is CompositeNode) AddToClassList("composite");

            if (Node is SubTreeNode) AddToClassList("sub-tree");
        }

        private void CreateInputPorts()
        {
            // Root node has not input ports
            // Rest of the nodes have one input port
            if (Node is not RootNode)
                Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));

            if (Input != null)
            {
                Input.portName = "";
                inputContainer.Add(Input);
                Input.name = "input-port";
            }
        }

        private void CreateOutputPorts()
        {
            // Action and SubTree node has no outports 
            // Decorator has one output port
            if (Node is DecoratorNode)
                Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));

            // Composite nodes have multiple output ports
            if (Node is CompositeNode)
                Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));

            // Root node has one output port
            if (Node is RootNode)
                Output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));

            if (Output != null)
            {
                Output.portName = "";
                outputContainer.Add(Output);
                Output.name = "output-port";
            }
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            if (_stateChangedAnim != null) _stateChangedAnim.Stop();

            switch (Node.state)
            {
                case BT.Node.State.Running:
                {
                    _stateChangedAnim = _nodeStateBorder.experimental.animation.Start(Color.yellow, Color.clear, 60000,
                        (element, color1) =>
                        {
                            _nodeStateBorder.style.borderBottomColor = color1;
                            _nodeStateBorder.style.borderTopColor = color1;
                            _nodeStateBorder.style.borderRightColor = color1;
                            _nodeStateBorder.style.borderLeftColor = color1;
                        });
                    _stateChangedAnim.KeepAlive();
                }
                    break;
                case BT.Node.State.Failure:
                    _stateChangedAnim = _nodeStateBorder.experimental.animation.Start(Color.red, Color.clear, 6000,
                        (element, color1) =>
                        {
                            _nodeStateBorder.style.borderBottomColor = color1;
                            _nodeStateBorder.style.borderTopColor = color1;
                            _nodeStateBorder.style.borderRightColor = color1;
                            _nodeStateBorder.style.borderLeftColor = color1;
                        });
                    _stateChangedAnim.KeepAlive();
                    break;
                case BT.Node.State.Success:
                    _stateChangedAnim = _nodeStateBorder.experimental.animation.Start(Color.green, Color.clear, 6000,
                        (element, color1) =>
                        {
                            _nodeStateBorder.style.borderBottomColor = color1;
                            _nodeStateBorder.style.borderTopColor = color1;
                            _nodeStateBorder.style.borderRightColor = color1;
                            _nodeStateBorder.style.borderLeftColor = color1;
                        });
                    _stateChangedAnim.KeepAlive();
                    break;
            }
        }

        private void ShowProperties()
        {
            if (_inputPropertyViews != null)
                foreach (var inputPropertyView in _inputPropertyViews)
                    inputPropertyView.style.display = DisplayStyle.Flex;

            if (_outputPropertyViews != null)
                foreach (var outputPropertyView in _outputPropertyViews)
                    outputPropertyView.style.display = DisplayStyle.Flex;
        }

        private void HideProperties()
        {
            if (_inputPropertyViews != null)
                foreach (var inputPropertyView in _inputPropertyViews)
                    inputPropertyView.style.display = DisplayStyle.None;

            if (_outputPropertyViews != null)
                foreach (var outputPropertyView in _outputPropertyViews)
                    outputPropertyView.style.display = DisplayStyle.None;
        }

        private void UpdateProperties(List<(string, BlackboardPropertyType)> propertyList,
            List<VisualElement> propertyViews)
        {
            if (propertyList == null || propertyList.Count != propertyViews.Count)
                return;

            for (var i = 0; i < propertyList.Count; i++)
                switch (propertyList[i].Item2)
                {
                    case BlackboardPropertyType.Numeric:
                    {
                        var nameLabel = propertyViews[i].Q<Label>("property-name");
                        nameLabel.text = propertyList[i].Item1;

                        if (Node.blackboard.TryGetProperty(propertyList[i].Item1, out double value))
                        {
                            var valueField = propertyViews[i].Q<FloatField>("property-value");

                            valueField.value = (float)value;
                        }
                    }

                        break;
                    case BlackboardPropertyType.Bool:
                    {
                        var nameLabel = propertyViews[i].Q<Label>("property-name");
                        nameLabel.text = propertyList[i].Item1;


                        if (Node.blackboard.TryGetProperty(propertyList[i].Item1, out bool value))
                        {
                            var valueField = propertyViews[i].Q<Toggle>("property-value");

                            valueField.value = value;
                        }
                    }
                        break;
                    case BlackboardPropertyType.String:
                    {
                        var nameLabel = propertyViews[i].Q<Label>("property-name");
                        nameLabel.text = propertyList[i].Item1;

                        if (Node.blackboard.TryGetProperty(propertyList[i].Item1, out string value))
                        {
                            var valueField = propertyViews[i].Q<Label>("property-value");

                            valueField.text = value;
                        }
                    }
                        break;
                    case BlackboardPropertyType.Vector3:
                    {
                        var nameLabel = propertyViews[i].Q<Label>("property-name");
                        nameLabel.text = propertyList[i].Item1;

                        if (Node.blackboard.TryGetProperty(propertyList[i].Item1, out Vector3 value))
                        {
                            var valueField = propertyViews[i].Q<Vector3Field>("property-value");

                            valueField.value = value;
                        }
                    }
                        break;
                    case BlackboardPropertyType.GameObject:
                    {
                        var nameLabel = propertyViews[i].Q<Label>("property-name");
                        nameLabel.text = propertyList[i].Item1;

                        if (Node.blackboard.TryGetProperty(propertyList[i].Item1, out GameObject value))
                        {
                            var valueField = propertyViews[i].Q<ObjectField>("property-value");

                            valueField.objectType = typeof(GameObject);
                            valueField.value = value;
                        }
                    }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
        }

        private int SorByHorizontalPosition(BT.Node a, BT.Node b)
        {
            return a.position.x < b.position.x ? -1 : 1;
        }

        #endregion // Private methods

        #region Public methods

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            Undo.RecordObject(Node, "Behaviour Tree (Set Position)");

            Node.position.x = newPos.xMin;
            Node.position.y = newPos.yMin;

            EditorUtility.SetDirty(Node);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            if (OnNodeSelected != null) OnNodeSelected.Invoke(this);
        }

        public void SortChildren()
        {
            var composite = Node as CompositeNode;
            if (composite) composite.children.Sort(SorByHorizontalPosition);
        }

        public void UpdateState()
        {
            if (Node.hasRun)
            {
                UpdateProperties(_inputPropertyList, _inputPropertyViews);
                UpdateProperties(_outputPropertyList, _outputPropertyViews);

                ShowProperties();
            }
        }

        #endregion // Public methods
    }
}