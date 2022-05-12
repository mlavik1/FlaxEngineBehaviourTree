namespace BehaviourTree
{
    public abstract class Decorator
    {
        public bool InverseCondition = false;

        /// <summary>
        /// Run a condition, and return whether node should be allowed to execute or not.
        /// </summary>
        /// <returns>true if node should  execute, false if not.</returns>
        public abstract bool ExecuteCondition(IBehaviourTreeAgent agent);

        public virtual void OverridResult(IBehaviourTreeAgent agent, ref NodeExecutionResult result)
        {

        }
    }
}
