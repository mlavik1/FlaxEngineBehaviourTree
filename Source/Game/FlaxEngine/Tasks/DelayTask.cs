using FlaxEngine;

namespace BehaviourTree
{
    [BTTask(name = "Delay", description = "Wait for X seconds.")]
    public class DelayTask : Task
    {
        public float DelayInSeconds;

        private float TimeAtStart;

        public override void Start(IBehaviourTreeAgent agent)
        {
            TimeAtStart = Time.GameTime;
        }
        public override NodeExecutionResult Update(IBehaviourTreeAgent agent)
        {
            float timeWaited = Time.GameTime - TimeAtStart;
            return timeWaited >= DelayInSeconds ? NodeExecutionResult.Succeeded : NodeExecutionResult.InProgress;
        }

        public override void OnAbort(IBehaviourTreeAgent agent)
        {

        }
    }
}
