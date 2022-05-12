using FlaxEngine;

namespace BehaviourTree
{
    public class PrintTask : Task
    {
        public string StringToPrint = "test";

        public override void Start(IBehaviourTreeAgent agent)
        {

        }
        public override NodeExecutionResult Update(IBehaviourTreeAgent agent)
        {
            Debug.Log(StringToPrint);
            return NodeExecutionResult.Succeeded;
        }

        public override void OnAbort(IBehaviourTreeAgent agent)
        {

        }
    }
}
