using CoreEditor.BehaviourTreeEditor;
using Realmwalker.DialogueSystem;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Realmwalker.DialogueEditor
{
    public class DialogEditor : EditorWindow
    {
        #region Data members

        private Label _dialogNameLabel; 
        
        private DialogEditorView _graphView;
        private InspectorView _inspectorView;

        #endregion // Data members

        #region Unity callback methods

        public void CreateGUI()
        {
            var root = rootVisualElement;

            var visualTree =
                AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/DialogueEditor/DialogEditorView.uxml");
            visualTree.CloneTree(root);

            _graphView = root.Q<DialogEditorView>();
            _inspectorView = root.Q<InspectorView>();
            _dialogNameLabel = root.Q<Label>("dialog-name-label");

            _graphView.OnNodeSelected = OnNodeSelectionChanged;
        }

        #endregion // Unity callback methods

        #region Static methods

        [MenuItem("Dialogues/Editor ...")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<DialogEditor>();
            wnd.titleContent = new GUIContent("Dialogue Editor");
        }

        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int line)
        {
            if (Selection.activeObject is Dialogue)
            {
                OpenWindow();
                return true;
            }

            return false;
        }

        #endregion // Static methods

        #region Private methods

        private void OnSelectionChange()
        {
            Dialogue dialogue = Selection.activeObject as Dialogue;

            if (!dialogue && Selection.activeGameObject)
            {
                // TODO: Add in game visualization support 
            }

            if (dialogue == null)
                return;
            
            if (AssetDatabase.CanOpenAssetInEditor(dialogue.GetInstanceID()))
            {
                _graphView.PopulateView(dialogue);
            }
            
            //if (Application.isPlaying)
            //{
            //    //TODO: add in-game visualization
            //    //_graphView.PopulateView(dialogue);
            //}
            //else
            //{
            //if (AssetDatabase.CanOpenAssetInEditor(dialogue.GetInstanceID()))
            //{
            //    _graphView.PopulateView(dialogue);
            //}    
            //}

            UpdateNameLabel(dialogue);
            //TODO: UpdateNameLabel(tree);
        }

        private void UpdateNameLabel(Dialogue dialogue)
        {
            _dialogNameLabel.text = dialogue.name;
        }

        private void OnNodeSelectionChanged(DialogEditorNodeView node)
        {
            _inspectorView.UpdateSelection(node);
        }
        
        #endregion // Private methods
        
    }
}