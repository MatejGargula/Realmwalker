using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BT
{
    [CreateAssetMenu(fileName = "BehaviourTree", menuName = "Behaviour Tree", order = 1)]
    public class BehaviourTree : ScriptableObject
    {
        public Node.State Update()
        {
            if (rootNode.state == Node.State.Running) treesState = rootNode.Update();

            return treesState;
        }

        public List<Node> GetChildren(Node parent)
        {
            var children = new List<Node>();

            var decorator = parent as DecoratorNode;
            if (decorator && decorator.child != null) children.Add(decorator.child);

            var root = parent as RootNode;
            if (root && root.child != null) children.Add(root.child);

            var composite = parent as CompositeNode;
            if (composite) return composite.children;

            return children;
        }

        public void Traverse(Node node, Action<Node> visitor)
        {
            if (node)
            {
                visitor.Invoke(node);
                var children = GetChildren(node);
                children.ForEach(n => Traverse(n, visitor));
            }
        }

        public BehaviourTree Clone(IBTAgent agent)
        {
            var tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<Node>();

            Traverse(tree.rootNode, n =>
            {
                var newNode = n;
                newNode.Agent = agent;
                newNode.blackboard = tree.blackboard;
                tree.nodes.Add(newNode);
            });
            return tree;
        }

        public BehaviourTree Clone(IBTAgent agent, Blackboard newBlackboard)
        {
            var tree = Instantiate(this);
            tree.rootNode = tree.rootNode.Clone();
            tree.nodes = new List<Node>();

            Traverse(tree.rootNode, n =>
            {
                var newNode = n;
                newNode.Agent = agent;
                newNode.blackboard = newBlackboard;
                tree.nodes.Add(newNode);
            });
            return tree;
        }

        #region Data members

        public Node rootNode;
        public Node.State treesState = Node.State.Running;
        public List<Node> nodes = new();

        public Blackboard blackboard = new();

        #endregion // Data members

#if UNITY_EDITOR
        public Node CreateNode(Type type, Vector2 position)
        {
            var node = CreateInstance(type) as Node;

            Undo.RecordObject(this, "Behaviour Tree (CreateNode)");

            node.name = type.Name.Remove(type.Name.Length - 4);
            node.guid = GUID.Generate().ToString();
            node.position = position;

            nodes.Add(node);

            if (!Application.isPlaying) AssetDatabase.AddObjectToAsset(node, this);

            Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreateNode)");

            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(Node node)
        {
            Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");

            nodes.Remove(node);
            //AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);

            AssetDatabase.SaveAssets();
        }

        public void AddChild(Node parent, Node child)
        {
            var root = parent as RootNode;
            if (root)
            {
                Undo.RecordObject(root, "Behaviour Tree (AddChild)");

                root.child = child;

                EditorUtility.SetDirty(root);
            }

            var decorator = parent as DecoratorNode;
            if (decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (AddChild)");

                decorator.child = child;

                EditorUtility.SetDirty(decorator);
            }

            var composite = parent as CompositeNode;
            if (composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (AddChild)");

                composite.children.Add(child);

                EditorUtility.SetDirty(composite);
            }
        }

        public void RemoveChild(Node parent, Node child)
        {
            var root = parent as RootNode;
            if (root)
            {
                Undo.RecordObject(root, "Behaviour Tree (RemoveChild)");

                root.child = null;

                EditorUtility.SetDirty(root);
            }

            var decorator = parent as DecoratorNode;
            if (decorator)
            {
                Undo.RecordObject(decorator, "Behaviour Tree (RemoveChild)");

                decorator.child = null;

                EditorUtility.SetDirty(decorator);
            }

            var composite = parent as CompositeNode;
            if (composite)
            {
                Undo.RecordObject(composite, "Behaviour Tree (RemoveChild)");

                composite.children.Remove(child);

                EditorUtility.SetDirty(composite);
            }
        }
#endif
    }
}