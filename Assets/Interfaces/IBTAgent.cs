using UnityEngine;

namespace BT
{
    /// <summary>
    ///     This interface contains definitions for methods called in the nodes.
    ///     Each agent can have different implementation for nodes.
    /// </summary>
    public interface IBTAgent
    {
        public GameObject Go { get; }
        public BehaviourTree Tree { get; set; }

        public Node.State ChooseSkillOnUpdate(int skillNumber);

        public void ChooseSkillOnStart()
        {
        }

        public void ChooseSkillOnStop()
        {
        }
    }
}