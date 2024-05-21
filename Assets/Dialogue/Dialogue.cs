using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Realmwalker.DialogueSystem
{
    [CreateAssetMenu(fileName = "Untitled Dialogue", menuName = "Dialogue System/Dialogue", order = 0)]
    public class Dialogue : ScriptableObject
    {
        #region Serialized fields

        [SerializeField] private List<DialogueNode> startNodes;

        #endregion // Serialized fields

        #region Properties

        private List<DialogueNode> _nodes = new();
        public List<DialogueNode> Nodes => _nodes;

        private DialogueNode _currentNode;
        public DialogueNode CurrentNode => _currentNode;
        
        #endregion // Properties

        #region Public methods

        public DialogueNode CreateDialogNode(Type type)
        {
            DialogueNode node = CreateInstance(type) as DialogueNode;
            node.AddResponse();
            if (node == null)
                return null;

            node.name = type.Name;
            
#if UNITY_EDITOR
            node.SetGuid(GUID.Generate().ToString());
            _nodes.Add(node);
            
            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();
#endif
            return node;
        }

        public List<string> GetResponses()
        {
            return _currentNode.Responses;
        }
        
        #endregion // Public methods
        
        
        
#if UNITY_EDITOR
        public DialogueNode CreateNode(Type type, Vector2 position)
        {
            DialogueNode node = CreateInstance(type) as DialogueNode;

            Undo.RecordObject(this, "Behaviour Tree (CreateNode)");

            if (node == null)
                return null;
            
            node.name = type.Name.Remove(type.Name.Length - 4);
            node.SetGuid(GUID.Generate().ToString());
            node.position = position;

            _nodes.Add(node);

            if (!Application.isPlaying) AssetDatabase.AddObjectToAsset(node, this);

            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");

            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(DialogueNode node)
        {
            Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");

            _nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);

            AssetDatabase.SaveAssets();
        }

        public void AddChild(DialogueNode parent, DialogueNode child)
        {
            Undo.RecordObject(parent, "Behaviour Tree (AddChild)");
            parent.AddChild(child);
            EditorUtility.SetDirty(parent);
        }

        public void RemoveChild(DialogueNode parent, DialogueNode child)
        {
            Undo.RecordObject(parent, "Behaviour Tree (AddChild)");
            parent.RemoveChild(child);
            EditorUtility.SetDirty(parent);
        }
#endif

    }
}