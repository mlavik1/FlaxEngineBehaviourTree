using FlaxEngine;

namespace BehaviourTree
{
    public class AIController : Script, IBehaviourTreeAgent
    {
        public string behaviourTreeFilePath; // TODO: Use asset!
        
        private BehaviourTree behaviourTree;
        private Blackboard blackboard;

        public Blackboard GetBlackboard()
        {
            return blackboard;
        }

        public override void OnStart()
        {
            blackboard = new Blackboard();

            if (behaviourTreeFilePath != "")
            {
                BehaviourTreeReader reader = new BehaviourTreeReader();
                behaviourTree = reader.ReadXml(behaviourTreeFilePath);
            }
        }

        public override void OnUpdate()
        {
            if (behaviourTree != null)
                behaviourTree.Update(this);
        }
    }
}
