using FlaxEngine;

namespace BehaviourTree
{
    public class MoveToTargetTask : Task
    {
        public string targetBlackboardKey = "Target";
        public float movementSpeed = 1.0f;

        public override void Start(IBehaviourTreeAgent agent)
        {

        }

        public override NodeExecutionResult Update(IBehaviourTreeAgent agent)
        {
            AIController controller = (AIController)agent;

            Actor target;
            if (controller.GetBlackboard().GetValue(targetBlackboardKey, out target))
            {
                Vector3 targetLocation = target.Position;
                Vector3 targetDir = targetLocation - controller.Actor.Position;
                float targetDist = targetDir.Length;
                float stepDist = movementSpeed * Time.DeltaTime;
                if (stepDist >= targetDist)
                {
                    controller.Actor.Position = targetLocation;
                    return NodeExecutionResult.Succeeded;

                }
                else
                {
                    controller.Actor.Position += targetDir.Normalized * stepDist;
                    return NodeExecutionResult.InProgress;
                }
            }
            else
            {
                return NodeExecutionResult.Failed;
            }
        }

        public override void OnAbort(IBehaviourTreeAgent agent)
        {

        }
    }
}
