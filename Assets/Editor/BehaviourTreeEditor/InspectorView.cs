using Realmwalker.DialogueEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace CoreEditor.BehaviourTreeEditor
{
    public class InspectorView : VisualElement
    {
        private Editor editor;

        internal void UpdateSelection(NodeView nodeView)
        {
            Clear();

            Object.DestroyImmediate(editor);

            editor = Editor.CreateEditor(nodeView.Node);
            var container = new IMGUIContainer(() =>
            {
                if (editor.target) editor.OnInspectorGUI();
            });
            Add(container);
        }

        internal void UpdateSelection(DialogEditorNodeView nodeView)
        {
            Clear();

            Object.DestroyImmediate(editor);

            editor = Editor.CreateEditor(nodeView.Node);
            var container = new IMGUIContainer(() =>
            {
                if (editor.target) editor.OnInspectorGUI();
            });
            Add(container);
        }
        
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits>
        {
        }
    }
}