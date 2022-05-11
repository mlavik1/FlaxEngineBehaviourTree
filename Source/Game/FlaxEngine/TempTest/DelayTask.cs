using FlaxEngine;

namespace BehaviourTree
{
    public class DelayTask : Task
    {
        public float DelayInSeconds;

        private float TimeAtStart;

        public override void Start()
        {
            TimeAtStart = Time.GameTime;
        }
        public override NodeExecutionResult Update()
        {
            float timeWaited = Time.GameTime - TimeAtStart;
            return timeWaited >= DelayInSeconds ? NodeExecutionResult.Succeeded : NodeExecutionResult.InProgress;
        }

        public override void OnAbort()
        {

        }
    }
}
