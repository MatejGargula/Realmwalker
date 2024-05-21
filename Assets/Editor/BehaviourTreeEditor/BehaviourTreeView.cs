using System;
using System.Collections.Generic;
using System.Linq;
using BT;
using CoreEditor.BehaviourTreeEditor;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Node = BT.Node;

public class BehaviourTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;
    public BehaviourTree tree;

    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeEditor/BTEditor.uss");
        styleSheets.Add(styleSheet);

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }

    private NodeView FindNodeView(Node node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }

    internal void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (tree.rootNode == null)
        {
            tree.rootNode = tree.CreateNode(typeof(RootNode), new Vector2(0.0f, 0.0f)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        // Creates node views for every node in a tree
        tree.nodes.ForEach(CreateNodeView);

        // Creates edges for each nodeView
        tree.nodes.ForEach(n =>
        {
            var children = tree.GetChildren(n);
            children.ForEach(c =>
            {
                var parentView = FindNodeView(n);
                var childView = FindNodeView(c);

                var edge = parentView.Output.ConnectTo(childView.Input);

                AddElement(edge);
            });
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
                var nodeView = element as NodeView;
                if (nodeView != null) tree.DeleteNode(nodeView.Node);

                var edge = element as Edge;
                if (edge != null)
                {
                    var parentView = edge.output.node as NodeView;
                    var childView = edge.input.node as NodeView;

                    tree.RemoveChild(parentView.Node, childView.Node);
                }
            });

        if (graphViewChange.edgesToCreate != null)
            graphViewChange.edgesToCreate.ForEach(edge =>
            {
                var parentView = edge.output.node as NodeView;
                var childView = edge.input.node as NodeView;

                tree.AddChild(parentView.Node, childView.Node);
            });

        if (graphViewChange.movedElements != null)
            nodes.ForEach(n =>
            {
                var view = n as NodeView;
                view.SortChildren();
            });

        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        var localPosition = this.ChangeCoordinatesTo(contentViewContainer, evt.localMousePosition);
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>().ToList();

            types = types.OrderBy(n => n.Name).ToList();

            foreach (var type in types)
            {
                if (type.Name == "SubTreeNode")
                    continue;
                evt.menu.AppendAction($"Actions/ {type.Name} ", a => CreateNodeFromMenu(type, localPosition));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<ConditionNode>().ToList();

            types = types.OrderBy(n => n.Name).ToList();

            foreach (var type in types)
            {
                if (type.Name == "SubTreeNode")
                    continue;
                evt.menu.AppendAction($"Conditions/{type.Name} ", a => CreateNodeFromMenu(type, localPosition));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>().ToList();

            types = types.OrderBy(n => n.Name).ToList();

            foreach (var type in types)
            {
                if (type.IsAbstract)
                    continue;

                evt.menu.AppendAction($"Decorators/ {type.Name}", a => CreateNodeFromMenu(type, localPosition));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>().ToList();

            types = types.OrderBy(n => n.Name).ToList();

            foreach (var type in types)
                evt.menu.AppendAction($"Composites/ {type.Name}", a => CreateNodeFromMenu(type, localPosition));
        }

        evt.menu.AppendAction("Sub-tree node", a => { CreateNodeFromMenu(typeof(SubTreeNode), localPosition); });
    }

    private void CreateNodeFromMenu(Type type, Vector2 position)
    {
        var node = tree.CreateNode(type, position);

        Debug.Log("Creating node at: " + position);

        var nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;

        AddElement(nodeView);
    }

    private void CreateNode(Type type, Vector2 position)
    {
        var node = tree.CreateNode(type, position);
        CreateNodeView(node);
    }

    private void CreateNodeView(Node node)
    {
        var nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;

        AddElement(nodeView);
    }

    public void UpdateNodesState()
    {
        nodes.ForEach(n =>
        {
            var view = n as NodeView;
            view.UpdateState();
        });
    }

    #region Nested class

    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits>
    {
    }

    #endregion // Nested class
}