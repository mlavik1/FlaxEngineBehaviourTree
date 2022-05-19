using FlaxEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourTree
{
    [BTTask(name = "Select target", description = "Select nearest target.")]
    public class SlectTargetTask : Task
    {
        public string targetTag = "Player";
        public string targetBlackboardKey = "Target";

        public override void Start(IBehaviourTreeAgent agent)
        {

        }
        public override NodeExecutionResult Update(IBehaviourTreeAgent agent)
        {
            AIController controller = (AIController)agent;
            Debug.Assert(controller != null, "AIController is null!");

            // Find potential targets
            List<Actor> targets = controller.Actor.Scene.GetChildren<Actor>().Where(actor => actor.Tag == targetTag).ToList();

            if (targets.Count > 0)
            {
                // Pick nearest target
                Actor target = targets.OrderBy(t => (t.Position - controller.Actor.Position).Length).First();
                controller.GetBlackboard().SetValue(targetBlackboardKey, target);
                return NodeExecutionResult.Succeeded;
            }
            else
                return NodeExecutionResult.Failed;
        }

        public override void OnAbort(IBehaviourTreeAgent agent)
        {

        }
    }
}
