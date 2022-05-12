namespace BehaviourTree
{
    public class BehaviourTree
    {
        private NodeBase rootNode;

        public void SetRootNode(NodeBase node)
        {
            rootNode = node;
        }

        public void Update(IBehaviourTreeAgent agent)
        {
            rootNode.Execute(agent);
        }

        public NodeBase GetRootNode()
        {
            return rootNode;
        }
    }
}
