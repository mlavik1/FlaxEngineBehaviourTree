namespace BehaviourTree
{
    // TODO: Rename? (since it's not really a "node")
    public class DecoratorNode
    {
        public Decorator decorator; // TODO: Hide? (inconsistet with TaskNode.GetTask())

        public DecoratorNode(Decorator decor)
        {
            decorator = decor;
        }

        public bool Execute()
        {
            return decorator.ExecuteCondition() != decorator.InverseCondition;
        }

        public void OverridResult(ref NodeExecutionResult result)
        {
            decorator.OverridResult(ref result);
        }

        public string GetName()
        {
            return decorator.GetType().Name;
        }
    }
}
