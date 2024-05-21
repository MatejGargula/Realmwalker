using System;
using System.Collections.Generic;
using System.Linq;
using BT;
using CoreEditor.BehaviourTreeEditor;
using Realmwalker.DialogueSystem;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Realmwalker.DialogueEditor
{
    public class DialogEditorView : GraphView
    {
        #region Data members

        public Dialogue dialogue;

        public Action<DialogEditorNodeView> OnNodeSelected;

        #endregion // Data members 

        #region Constructors

        public DialogEditorView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/DialogueEditor/DialogEditorStyle.uss");
            styleSheets.Add(styleSheet);
        }

        #endregion // Constructors

        private void OnUndoRedo()
        {
            PopulateView(dialogue);
            AssetDatabase.SaveAssets();
        }

        private DialogEditorNodeView FindNodeView(DialogueNode node)
        {
            return GetNodeByGuid(node.Guid) as DialogEditorNodeView;
        }

        internal void PopulateView(Dialogue dialogue)
        {
            this.dialogue = dialogue;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            if (dialogue.Nodes.Count == 0)
            {
                dialogue.CreateDialogNode(typeof(DialogueNode));
                EditorUtility.SetDirty(dialogue);
                AssetDatabase.SaveAssets();

                return;
            }

            // Creates node views for every node in a dialog graph
            dialogue.Nodes.ForEach(CreateNodeView);

            // Creates edges for each nodeView
            dialogue.Nodes.ForEach(n =>
            {
                List<DialogueNode> children = n.ChildNodes; //tree.GetChildren(n);
                DialogEditorNodeView parentView = FindNodeView(n);

                for (int i = 0; i < children.Count; i++)
                {
                    var childView = FindNodeView(children[i]);
                    var edge = parentView.Outputs[i].ConnectTo(childView.Input);

                    AddElement(edge);
                }
            });
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction &&
                endPort.node != startPort.node).ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
                graphViewChange.elementsToRemove.ForEach(element =>
                {
                    DialogEditorNodeView nodeViewView = element as DialogEditorNodeView;
                    if (nodeViewView != null) dialogue.DeleteNode(nodeViewView.Node);

                    var edge = element as Edge;
                    if (edge != null)
                    {
                        DialogEditorNodeView parentView = edge.output.node as DialogEditorNodeView;
                        DialogEditorNodeView childView = edge.input.node as DialogEditorNodeView;

                        dialogue.RemoveChild(parentView.Node, childView.Node);
                    }
                });

            if (graphViewChange.edgesToCreate != null)
                graphViewChange.edgesToCreate.ForEach(edge =>
                {
                    DialogEditorNodeView parentView = edge.output.node as DialogEditorNodeView;
                    DialogEditorNodeView childView = edge.input.node as DialogEditorNodeView;

                    dialogue.AddChild(parentView.Node, childView.Node);
                });
            
            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var localPosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
            {
                evt.menu.AppendAction($"Create/{typeof(DialogueNode).Name} ",
                    a => CreateNodeFromMenu(typeof(DialogueNode), localPosition));
            }
        }

        #region Private methods

        private void CreateNodeFromMenu(Type type, Vector2 position)
        {
            DialogueNode node = dialogue.CreateNode(type, position);

            Debug.Log("Creating node at: " + position);

            DialogEditorNodeView nodeViewView = new DialogEditorNodeView(node);
            nodeViewView.OnNodeSelected = OnNodeSelected;

            AddElement(nodeViewView);
        }

        private void CreateNodeView(DialogueNode node)
        {
            var nodeView = new DialogEditorNodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;

            AddElement(nodeView);
        }

        #endregion

        #region Nested class

        public new class UxmlFactory : UxmlFactory<DialogEditorView, UxmlTraits>
        {
        }

        #endregion // Nested class
    }
}