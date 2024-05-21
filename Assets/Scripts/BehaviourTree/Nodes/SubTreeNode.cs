using UnityEngine;

namespace BT
{
    public class SubTreeNode : ActionNode
    {
        [SerializeField] public BehaviourTree subTreeSource;

        private bool _subTreeIsValid;

        private void OnValidate()
        {
            if (subTreeSource == null)
                return;

            nodeTitle = subTreeSource.name;
        }

        protected override void OnStart()
        {
            if (!_subTreeIsValid && subTreeSource != null)
            {
                subTreeSource = subTreeSource.Clone(Agent, blackboard);
                subTreeSource.blackboard = blackboard;
                _subTreeIsValid = true;
            }
        }

        protected override State OnUpdate()
        {
            if (!_subTreeIsValid)
            {
                Debug.Log($"Subtree: {subTreeSource.name} is not valid.");
                return State.Failure;
            }

            return subTreeSource.rootNode.Update();
        }

        public override void OnStop()
        {
        }


        protected override void Init()
        {
            nodeTitle = "Empty sub-tree";
        }
    }
}