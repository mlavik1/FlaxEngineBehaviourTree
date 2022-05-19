using FlaxEngine;

namespace BehaviourTree
{
    [BTTask(name = "Destroy target", description = "Destroyes the target, set in the blackboard.")]
    public class DestroyTargetTask : Task
    {
        public string targetBlackboardKey = "Target";

        public override void Start(IBehaviourTreeAgent agent)
        {

        }

        public override NodeExecutionResult Update(IBehaviourTreeAgent agent)
        {
            AIController controller = (AIController)agent;

            Actor target;
            if (controller.GetBlackboard().GetValue(targetBlackboardKey, out target))
            {
                Actor.Destroy(target);
                return NodeExecutionResult.Succeeded;
            }
            else
            {
                return NodeExecutionResult.Failed;
            }
        }

        public override void OnAbort(IBehaviourTreeAgent agent)
        {

        }
    }
}
