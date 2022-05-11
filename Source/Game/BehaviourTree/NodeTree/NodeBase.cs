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

        public abstract NodeExecutionResult Execute();

        public abstract void OnAbort();

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

        public IReadOnlyCollection<NodeBase> GetChildren()
        {
            return children.AsReadOnly();
        }
    }
}
