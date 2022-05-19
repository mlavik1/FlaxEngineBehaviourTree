using FlaxEngine;
using System.IO;

namespace BehaviourTree
{
    public class AIController : Script, IBehaviourTreeAgent
    {
        public string behaviourTreeFilePath; // TODO: Use asset?
        public bool pathRelativeToContentDir;
        
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
                if (pathRelativeToContentDir)
                    behaviourTreeFilePath = Path.Combine(Globals.ProjectContentFolder);

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
