using FlaxEngine;

namespace BehaviourTree
{
    [BTTask(name = "Print", description = "Prints text to the console.")]
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
