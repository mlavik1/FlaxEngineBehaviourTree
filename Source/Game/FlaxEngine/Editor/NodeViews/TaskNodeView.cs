using FlaxEngine;
using FlaxEngine.GUI;
using System.Reflection;
using System.Linq;

namespace BehaviourTree
{
    public class TaskNodeView : NodeViewBase
    {
        private ContainerControl nodeRoot;
        private NodeContentView compositeView;
        private NodeContentView decoratorView;

        private TaskNode taskNode;

        public TaskNodeView(ContainerControl parent, TaskNode task)
        {
            this.taskNode = task;

            // Root node
            VerticalPanel rootPanel = parent.AddChild<VerticalPanel>();
            rootPanel.Width = 200;
            rootPanel.BackgroundColor = Color.BlueViolet;
            rootPanel.Margin = new Margin(5, 5, 5, 5);
            nodeRoot = rootPanel;

            RebuildGUI();
        }

        public override void RebuildGUI()
        {
            if (nodeRoot != null)
            {
                nodeRoot.RemoveChildren();
            }

            VerticalPanel containerControl = nodeRoot.AddChild<VerticalPanel>();
            containerControl.BackgroundColor = Color.Gray;

            Task task = taskNode.GetTask();
            string[] taskProps = task.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Select(f => $"{f.Name} = {f.GetValue(task).ToString()}").ToArray();
            NodeContentView contentView = new NodeContentView(containerControl, taskNode.GetTask().GetType().Name, taskProps);
        }

        public override ContainerControl GetControl()
        {
            return nodeRoot;
        }

        public override NodeBase GetNode()
        {
            return taskNode;
        }

        public override void SetHighlighted(bool highlight)
        {
            nodeRoot.BackgroundColor = highlight ? Color.GreenYellow : Color.BlueViolet;
        }
    }
}
