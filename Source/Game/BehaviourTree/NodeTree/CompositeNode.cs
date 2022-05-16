using System.Collections.Generic;

namespace BehaviourTree
{
    public enum CompositeType
    {
        Sequence,
        Selector
    }

    public class CompositeNode : NodeBase
    {
        public List<DecoratorNode> decorators = new List<DecoratorNode>();
        public CompositeType type { get; private set; }

        private int currentChildIndex = 0;

        public CompositeNode(CompositeType type)
        {
            this.type = type;
        }

        public override void AddChild(NodeBase newChild)
        {
            children.Add(newChild);
        }

        public override NodeExecutionResult Execute(IBehaviourTreeAgent agent)
        {
            if (children.Count == 0)
                return NodeExecutionResult.Failed;

            if (currentChildIndex == -1)
                currentChildIndex = 0;

            NodeBase currNode = children[currentChildIndex];

            // Execute decorators
            bool decoratorsSucceeded = true;
            foreach (DecoratorNode decorator in decorators)
            {
                decoratorsSucceeded &= decorator.Execute(agent);
                if (!decoratorsSucceeded)
                    break;
            }

            // Decorators failed => Abort
            if (!decoratorsSucceeded)
            {
                if (currentChildIndex != -1)
                {
                    currNode.OnAbort(agent);
                    currentChildIndex = -1;
                }
                return NodeExecutionResult.Aborted;
            }
            // Decorators succeeded => execute
            else
            {
                NodeExecutionResult childResult = currNode.Execute(agent);

                NodeExecutionResult result = GetResponseResultFromChild(childResult);

                if (result != NodeExecutionResult.InProgress)
                    currentChildIndex = -1;
                else if (childResult != NodeExecutionResult.InProgress)
                    currentChildIndex++;

                // Allow decorators to override result
                foreach (DecoratorNode decorator in decorators)
                {
                    decorator.OverridResult(agent, ref result);
                }

                // Handle overridden result
                if (result == NodeExecutionResult.Aborted)
                    currNode.OnAbort(agent);
                if (result != NodeExecutionResult.InProgress)
                    currentChildIndex = -1;

                return result;
            }
        }

        public override void OnAbort(IBehaviourTreeAgent agent)
        {
            if (currentChildIndex != -1)
            {
                children[currentChildIndex].OnAbort(agent);
                currentChildIndex = -1;
            }
        }

        private NodeExecutionResult GetResponseResultFromChild(NodeExecutionResult childResult)
        {
            switch (childResult)
            {
                case NodeExecutionResult.InProgress:
                    {
                        return NodeExecutionResult.InProgress;
                    }
                case NodeExecutionResult.Aborted:
                    {
                        return NodeExecutionResult.Aborted;
                    }
                case NodeExecutionResult.Succeeded:
                    {
                        if (type == CompositeType.Sequence)
                        {
                            return currentChildIndex == children.Count - 1 ? NodeExecutionResult.Succeeded : NodeExecutionResult.InProgress;
                        }
                        else
                        {
                            return NodeExecutionResult.Succeeded;
                        }
                    }
                case NodeExecutionResult.Failed:
                    {
                        if (type == CompositeType.Sequence)
                        {
                            return NodeExecutionResult.Failed;
                        }
                        else
                        {
                            return currentChildIndex == children.Count - 1 ? NodeExecutionResult.Failed : NodeExecutionResult.InProgress;
                        }
                    }
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}
