using FlaxEngine;

namespace BehaviourTree
{
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
