using BT;
using UnityEngine;
using UnityEngine.Events;

public class CustomAction : ActionNode
{
    [SerializeField] private UnityEvent customEvent;

    protected override State OnUpdate()
    {
        customEvent.Invoke();
        return State.Success;
    }

    protected override void OnStart()
    {
    }

    public override void OnStop()
    {
    }

    protected override void Init()
    {
        nodeTitle = "Custom Action";
        description = "This is a custom action with a unity event.";
    }
}