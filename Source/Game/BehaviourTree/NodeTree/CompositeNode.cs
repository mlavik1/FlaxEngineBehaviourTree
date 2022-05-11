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

        public override NodeExecutionResult Execute()
        {
            if (children.Count == 0)
                return NodeExecutionResult.Faied;

            if (currentChildIndex == -1)
                currentChildIndex = 0;

            NodeBase currNode = children[currentChildIndex];

            // Execute decorators
            bool decoratorsSucceeded = true;
            foreach (DecoratorNode decorator in decorators)
            {
                decoratorsSucceeded &= decorator.Execute();
                if (!decoratorsSucceeded)
                    break;
            }

            // Decorators failed => Abort
            if (!decoratorsSucceeded)
            {
                if (currentChildIndex != -1)
                {
                    currNode.OnAbort();
                    currentChildIndex = -1;
                }
                return NodeExecutionResult.Aborted;
            }
            // Decorators succeeded => execute
            else
            {
                NodeExecutionResult childResult = currNode.Execute();

                NodeExecutionResult result = GetResponseResultFromChild(childResult);

                if (result != NodeExecutionResult.InProgress)
                    currentChildIndex = -1;
                else if (childResult != NodeExecutionResult.InProgress)
                    currentChildIndex++;

                // Allow decorators to override result
                foreach (DecoratorNode decorator in decorators)
                {
                    decorator.OverridResult(ref result);
                }

                // Handle overridden result
                if (result == NodeExecutionResult.Aborted)
                    currNode.OnAbort();
                else if (result != NodeExecutionResult.InProgress)
                    currentChildIndex = -1;

                return result;
            }
        }

        public override void OnAbort()
        {
            if (currentChildIndex != -1)
            {
                children[currentChildIndex].OnAbort();
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
                case NodeExecutionResult.Faied:
                    {
                        if (type == CompositeType.Sequence)
                        {
                            return NodeExecutionResult.Faied;
                        }
                        else
                        {
                            return currentChildIndex == children.Count - 1 ? NodeExecutionResult.Faied : NodeExecutionResult.InProgress;
                        }
                    }
                default:
                    throw new System.NotImplementedException();
            }
        }
    }
}
