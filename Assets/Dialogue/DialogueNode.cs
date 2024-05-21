using System;
using System.Collections.Generic;
using UnityEngine;

namespace Realmwalker.DialogueSystem
{
    [Serializable]
    public class DialogueNode : ScriptableObject
    {
        #region Data members

        private bool isStarting = false;
        [HideInInspector] public Vector2 position;
        
        public event EventHandler StateChangedEvent;
        
        #endregion // Data members

        #region Properties

        [SerializeField] private string _name;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        [SerializeField] private string _text;

        public string Text
        {
            get => _text;
            set => _text = value;
        }
        
        [SerializeField] private List<string> _responses = new();
        public List<string> Responses => _responses;

        private List<DialogueNode> _childNodes = new();
        public List<DialogueNode> ChildNodes => _childNodes;

        public string Guid { get; private set; }

        #endregion // Properties

        #region Constructors

        public DialogueNode()
        {
            AddResponse();
        }

        #endregion // Constructors
        
        #region Public methods
        
        public void SetGuid(string guid)
        {
            Guid = guid;
        }

        public void AddChild(DialogueNode child)
        {
            _childNodes.Add(child);
            OnStateChanged();
        }

        public void AddResponse()
        {
            _responses.Add("Put response here.");
        }
        
        public void RemoveChild(DialogueNode child)
        {
            _childNodes.Remove(child);
            OnStateChanged();
        }
        
        #endregion // Public methods

        #region Private methods

        private void OnStateChanged()
        {
            StateChangedEvent?.Invoke(this, EventArgs.Empty);
        }
        
        #endregion // Private methods 
        
    }
}
