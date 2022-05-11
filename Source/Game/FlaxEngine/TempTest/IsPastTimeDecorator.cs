using FlaxEngine;

namespace BehaviourTree
{
    public class IsPastTimeDecorator : Decorator
    {
        public float TimeToWait;

        private float waited;

        public override bool ExecuteCondition()
        {
            waited += Time.DeltaTime;
            return waited > TimeToWait;
        }

        public override void OverridResult(ref NodeExecutionResult result)
        {

        }
    }
}
