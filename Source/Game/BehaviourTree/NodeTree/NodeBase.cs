using System.Collections.Generic;

namespace BehaviourTree
{
    /// <summary>
    /// Base class for all behaviour tree nodes.
    /// </summary>
    public abstract class NodeBase
    {
        protected NodeBase parent;
        protected List<NodeBase> children = new List<NodeBase>();

        public abstract NodeExecutionResult Execute(IBehaviourTreeAgent agent);

        public abstract void OnAbort(IBehaviourTreeAgent agent);

        public abstract void AddChild(NodeBase newChild);

        public void UnlinkNode()
        {
            foreach (NodeBase childNode in children)
            {
                childNode.parent = parent;
                parent?.children.Add(childNode);
            }
            parent?.children.Remove(this);
            children.Clear();
            parent = null;
        }

        public void SwapChildren(NodeBase firstChild, NodeBase secondChild)
        {
            int i1 = children.IndexOf(firstChild);
            int i2 = children.IndexOf(secondChild);
            bool foundIndices = i1 != -1 && i2 != -1;
            if (foundIndices)
            {
                children[i2] = firstChild;
                children[i1] = secondChild;
            }
            else
                System.Console.WriteLine("SwapChildren called with invalid nodes.");
        }

        public IReadOnlyCollection<NodeBase> GetChildren()
        {
            return children.AsReadOnly();
        }
    }
}
