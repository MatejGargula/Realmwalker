using UnityEngine;

namespace BT
{
    public class DebugLogNode : ActionNode
    {
        public string message;

        protected override void OnStart()
        {
        }

        public override void OnStop()
        {
        }

        protected override void Init()
        {
            description = "Logs a message in the console";
            nodeTitle = "Debug Log";
        }

        protected override State OnUpdate()
        {
            Debug.Log("OnUpdate(): " + message);
            return State.Success;
        }
    }
}