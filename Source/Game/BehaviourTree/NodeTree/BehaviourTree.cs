namespace BehaviourTree
{
    public class BehaviourTree
    {
        private NodeBase rootNode;

        public void SetRootNode(NodeBase node)
        {
            rootNode = node;
        }

        public void Update()
        {
            rootNode.Execute();
        }

        public NodeBase GetRootNode()
        {
            return rootNode;
        }
    }
}
