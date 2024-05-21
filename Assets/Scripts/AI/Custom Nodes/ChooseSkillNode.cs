using BT;
using UnityEngine;

public class ChooseSkillNode : ActionNode
{
    [SerializeField] private int skillNumber;

    protected override State OnUpdate()
    {
        return Agent.ChooseSkillOnUpdate(skillNumber);
    }

    protected override void OnStart()
    {
        Agent.ChooseSkillOnStart();
    }

    public override void OnStop()
    {
        Agent.ChooseSkillOnStop();
    }

    protected override void Init()
    {
        nodeTitle = "Choose Skill";
        description = "Chooses a skill to be placed in a queue";
    }
}