using System.Collections.Generic;
using BT;
using CoreEditor.BehaviourTreeEditor;
using CoreEditor.BehaviourTreeEditor.Blackboard;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class BTEditor : EditorWindow
{
    #region Data members

    private BehaviourTreeView _treeView;
    private InspectorView _inspectorView;
    private BlackboardView _blackboardView;
    private ListView _treeListView;
    private VisualElement _treeListContainer;
    private Label _treeNameLabel;

    private VisualElement _subTreePanel;
    private BehaviourTreeView _subTreeView;
    private Label _subTreeTitleLabel;

    private List<BehaviourTree> _treeList = new();
    private List<BehaviourTree> _treeCachedList = new();
    private BehaviourTree _currentTree;

    private SerializedObject treeObject;
    private SerializedProperty blackboardProperty;

    #endregion // Data members

    #region Static methods

    [MenuItem("BT Editor/Editor ...")]
    public static void OpenWindow()
    {
        var wnd = GetWindow<BTEditor>();
        wnd.titleContent = new GUIContent("BehaviourTreeEditor");
    }

    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviourTree)
        {
            OpenWindow();
            return true;
        }

        return false;
    }

    #endregion // Static methods

    #region Unity callback methods

    public void CreateGUI()
    {
        var root = rootVisualElement;

        var visualTree =
            AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BehaviourTreeEditor/BTEditor.uxml");
        visualTree.CloneTree(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeEditor/BTEditor.uss");
        root.styleSheets.Add(styleSheet);

        _treeView = root.Q<BehaviourTreeView>("main-tree-view");
        _inspectorView = root.Q<InspectorView>();
        _treeListView = root.Q<ListView>();
        _treeNameLabel = root.Q<Label>("tree-name");
        _treeListContainer = root.Q<VisualElement>("tree-list-root");
        _blackboardView = root.Q<BlackboardView>();
        _subTreePanel = root.Q<VisualElement>("sub-tree-panel");
        _subTreeView = root.Q<BehaviourTreeView>("sub-tree-view");
        _subTreeTitleLabel = root.Q<Label>("sub-tree-title-label");

        _treeView.OnNodeSelected = OnNodeSelectionChanged;
        _subTreeView.OnNodeSelected = node => { _inspectorView.UpdateSelection(node); };

        InitTreeListView();
        OnSelectionChange();
        HideSubTreePanel();
    }

    private void HideSubTreePanel()
    {
        _subTreePanel.style.display = DisplayStyle.None;
    }

    private void ShowSubTreePanel(BehaviourTree subTree)
    {
        if (subTree == null)
            return;

        _subTreePanel.style.display = DisplayStyle.Flex;
        _subTreeTitleLabel.text = subTree.name;
        _subTreeView.PopulateView(subTree);
    }

    private void UpdateNameLabel(BehaviourTree behaviourTree)
    {
        _treeNameLabel.text = behaviourTree.name;
    }

    private void InitTreeListView()
    {
        _treeListView.makeItem = () =>
        {
            VisualElement elementContainer = new()
            {
                name = "list-element"
            };
            elementContainer.AddToClassList("list-element-root");

            Label label = new()
            {
                name = "element-label"
            };
            label.AddToClassList("list-element-label");

            Button button = new()
            {
                name = "element-button",
                text = "X"
            };
            button.AddToClassList("list-element-button");

            elementContainer.Add(label);
            elementContainer.Add(button);
            return elementContainer;
        };

        _treeListView.bindItem = (item, idx) =>
        {
            item.Q<Label>("element-label").text = _treeList[idx].name;
            var button = item.Q<Button>("element-button");
            button.RegisterCallback<ClickEvent, int>(OnDeleteElement, idx);
        };

        _treeListView.itemsSource = _treeList;

        _treeListView.onSelectionChange += OnTreeListSelection;
    }

    private void SwitchTreeList()
    {
        _treeList = new List<BehaviourTree>(_treeCachedList);
    }

    private void OnDeleteElement(ClickEvent evt, int idx)
    {
        _treeList.RemoveAt(idx);
        _treeListView.RemoveAt(idx);
        _treeListView.RefreshItems();
    }

    private void OnTreeListSelection(IEnumerable<object> obj)
    {
        var tree = _treeListView.selectedItem as BehaviourTree;
        if (tree == null)
            return;

        _treeView.PopulateView(tree);
        _treeListView.RefreshItems();
        _blackboardView.CurrentBlackboard = tree.blackboard;
        UpdateNameLabel(tree);

        HideSubTreePanel();
    }

    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        if (Application.isPlaying)
            if (_treeListContainer != null)
                _treeListContainer.style.display = DisplayStyle.None;
    }

    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }

    private void OnSelectionChange()
    {
        var tree = Selection.activeObject as BehaviourTree;

        if (!tree && Selection.activeGameObject)
        {
            var runner = Selection.activeGameObject.GetComponent<IBTAgent>();
            tree = runner != null ? runner.Tree : null;
        }

        if (tree == null)
            return;

        if (Application.isPlaying)
        {
            _treeView.PopulateView(tree);
            if (!_treeList.Contains(tree))
            {
                _treeList.Add(tree);
                _treeListView.RefreshItems();
                _currentTree = tree;
            }
        }
        else
        {
            if (AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID()))
            {
                _treeView.PopulateView(tree);
                if (!_treeList.Contains(tree))
                {
                    _treeList.Add(tree);
                    _treeListView.RefreshItems();
                    _currentTree = tree;
                }
            }
        }

        UpdateNameLabel(tree);
        treeObject = new SerializedObject(tree);
        _blackboardView.CurrentBlackboard = tree.blackboard;

        HideSubTreePanel();
    }

    private void OnInspectorUpdate()
    {
        if (_treeView == null || _treeView.tree == null)
            return;

        //if(_treeView.tree.rootNode.Agent != null)
        _treeView.UpdateNodesState();
        if (_subTreeView != null && _subTreeView.tree != null)
            _subTreeView.UpdateNodesState();
    }

    #endregion // Unity callback methods

    #region Private methods

    private void OnPlayModeStateChanged(PlayModeStateChange obj)
    {
        switch (obj)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                if (_treeListContainer != null)
                    _treeListContainer.style.display = DisplayStyle.Flex;

                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                if (_treeListContainer != null)
                    _treeListContainer.style.display = DisplayStyle.None;
                OnSelectionChange();

                _treeCachedList = new List<BehaviourTree>(_treeList);

                break;
            case PlayModeStateChange.ExitingPlayMode:

                _treeListView.Clear();
                _treeList = new List<BehaviourTree>(_treeCachedList);
                _treeListView.itemsSource = _treeList;
                _treeListView.RefreshItems();

                break;
        }
    }

    private void OnNodeSelectionChanged(NodeView node)
    {
        _inspectorView.UpdateSelection(node);

        if (node.Node is SubTreeNode subTreeNode)
            ShowSubTreePanel(subTreeNode.subTreeSource);
        else
            HideSubTreePanel();
    }

    #endregion // Private methods
}