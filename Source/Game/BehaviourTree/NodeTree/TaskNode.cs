using System;

namespace BehaviourTree
{
    public class TaskNode : NodeBase
    {
        private Task task;
        private bool isRunning = false;

        public TaskNode(Task task)
        {
            this.task = task;
        }

        public override void AddChild(NodeBase newChild)
        {
            throw new InvalidOperationException("Cannot add child to task node.");
        }

        public override NodeExecutionResult Execute(IBehaviourTreeAgent agent)
        {
            if (!isRunning)
                task.Start(agent);

            NodeExecutionResult result = task.Update(agent);
            isRunning = result == NodeExecutionResult.InProgress;
            return result;
        }

        public override void OnAbort(IBehaviourTreeAgent agent)
        {
            task.OnAbort(agent);
            isRunning = false;
        }

        public Task GetTask()
        {
            return task;
        }
    }
}
