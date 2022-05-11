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

        public override NodeExecutionResult Execute()
        {
            if (!isRunning)
                task.Start();

            NodeExecutionResult result = task.Update();
            isRunning = result == NodeExecutionResult.InProgress;
            return result;
        }

        public override void OnAbort()
        {
            task.OnAbort();
            isRunning = false;
        }

        public Task GetTask()
        {
            return task;
        }
    }
}
