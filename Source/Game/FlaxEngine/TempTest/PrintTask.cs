using FlaxEngine;

namespace BehaviourTree
{
    public class PrintTask : Task
    {
        public string StringToPrint = "test";

        public override void Start()
        {

        }
        public override NodeExecutionResult Update()
        {
            Debug.Log(StringToPrint);
            return NodeExecutionResult.Succeeded;
        }

        public override void OnAbort()
        {

        }
    }
}
