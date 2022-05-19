using FlaxEngine;

namespace BehaviourTree
{
    [BTDecorator(name = "Loop", description = "Loop based on blackgoard condition.")]
    public class LoopDecorator : Decorator
    {
        public enum BlackboardCondition
        {
            KeyIsSet,
            KeyIsNotSet
        }

        public string blackboardKey;
        public BlackboardCondition loopCondition;

        public override bool ExecuteCondition(IBehaviourTreeAgent agent)
        {
            return CheckCondition(agent);
        }

        public override void OverridResult(IBehaviourTreeAgent agent, ref NodeExecutionResult result)
        {
            if (result == NodeExecutionResult.Succeeded || result == NodeExecutionResult.Failed)
            {
                if (CheckCondition(agent))
                    result = NodeExecutionResult.InProgress;
            }
        }

        private bool CheckCondition(IBehaviourTreeAgent agent)
        {
            return agent.GetBlackboard().IsValueSet(blackboardKey) == (loopCondition == BlackboardCondition.KeyIsSet);
        }
    }
}
