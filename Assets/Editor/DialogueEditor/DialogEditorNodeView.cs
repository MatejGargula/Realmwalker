using System;
using System.Collections.Generic;
using Realmwalker.DialogueSystem;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace Realmwalker.DialogueEditor
{
    public class DialogEditorNodeView : Node
    {
        #region Data members

        public Action<DialogEditorNodeView> OnNodeSelected;

        private Label _titleLabel;
        private VisualElement _addResponseButton;
        public Port Input;
        public List<Port> Outputs = new();

        #endregion // Data members

        #region Properties

        public DialogueNode Node { get; }

        public string Guid { get; }

        public string Text { get; set; }

        public bool EntryPoint = false;

        #endregion // Properties
        
        #region Constructor

        public DialogEditorNodeView(DialogueNode node) : base("Assets/Editor/DialogueEditor/Node/DialogueEditorNodeView.uxml")
        {
            if (node is null)
                return;

            Node = node;
            name = "NodeView";

            _titleLabel = this.Q<Label>("title-label");
            _titleLabel.text = name;
            
            _addResponseButton = this.Q<VisualElement>("add-response-button");
            _addResponseButton.RegisterCallback<ClickEvent>(OnAddResponseClick);

            style.left = node.position.x;
            style.top = node.position.y;

            CreateInputPorts();
            CreateOutputPorts();
            
            node.StateChangedEvent += OnStateChanged;
        }

        private void OnAddResponseClick(ClickEvent evt)
        {
            Node.AddResponse();
            CreateOutputPort($"port: {Outputs.Count} ", Outputs.Count);
        }

        #endregion // Constructor



        #region Public methods

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }

        #endregion // Public methods

        #region Private methods

        private void CreateInputPorts()
        {
            Input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Multi, typeof(bool));
            Input.portName = "";
            inputContainer.Add(Input);
            Input.name = "input-port";
        }

        private void CreateOutputPorts()
        {
            for (int i = 0; i < Node.Responses.Count; i++)
            {
                string childNode = Node.Responses[i];
                CreateOutputPort($"out Port {i + 1}.", i + 1);                
            }
        }

        private void CreateOutputPort(string portName, int order)
        {
            var output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
            output.portName = portName;
            outputContainer.Add(output);
            output.name = $"output-port-{order}";

            Outputs.Add(output);
        }

        private void OnStateChanged(object sender, EventArgs e)
        {
            
        }

        #endregion
    }
}