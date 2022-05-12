namespace BehaviourTree
{
    /// <summary>
    /// Base class for all tasks.
    /// </summary>
    public abstract class Task
    {
        public abstract void Start(IBehaviourTreeAgent agent);
        public abstract NodeExecutionResult Update(IBehaviourTreeAgent agent);
        public abstract void OnAbort(IBehaviourTreeAgent agent);
    }
}
