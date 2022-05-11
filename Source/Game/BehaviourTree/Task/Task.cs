namespace BehaviourTree
{
    /// <summary>
    /// Base class for all tasks.
    /// </summary>
    public abstract class Task
    {
        public abstract void Start();
        public abstract NodeExecutionResult Update();
        public abstract void OnAbort();
    }
}
