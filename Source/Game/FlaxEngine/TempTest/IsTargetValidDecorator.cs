using FlaxEngine;

namespace BehaviourTree
{
    [BTDecorator(name = "Is target valid?", description = "Checks if the target (set in the behaviour tree) is valid.")]
    public class IsTargetValidDecorator : Decorator
    {
        public string targetBlackboardKey;

        public override bool ExecuteCondition(IBehaviourTreeAgent agent)
        {
            AIController controller = (AIController)agent;
            Actor targetActor;
            if (controller.GetBlackboard().GetValue(targetBlackboardKey, out targetActor))
            {
                return targetActor ? true : false;
            }
            else
                return false;
        }

        public override void OverridResult(IBehaviourTreeAgent agent, ref NodeExecutionResult result)
        {

        }
    }
}
