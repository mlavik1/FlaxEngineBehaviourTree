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

        public bool Execute(IBehaviourTreeAgent agent)
        {
            return decorator.ExecuteCondition(agent) != decorator.InverseCondition;
        }

        public void OverridResult(IBehaviourTreeAgent agent, ref NodeExecutionResult result)
        {
            decorator.OverridResult(agent, ref result);
        }

        public string GetName()
        {
            return decorator.GetType().Name;
        }
    }
}
