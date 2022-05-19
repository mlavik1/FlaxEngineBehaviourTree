using FlaxEngine;
using FlaxEngine.GUI;

namespace BehaviourTree
{
    public class RootNodeView : NodeViewBase
    {
        private ContainerControl nodeRoot;
        private NodeContentView compositeView;
        private NodeContentView decoratorView;

        public RootNodeView(ContainerControl parent)
        {
            // Root node
            VerticalPanel rootPanel = parent.AddChild<VerticalPanel>();
            rootPanel.Width = 200;
            rootPanel.BackgroundColor = Color.Gray;
            rootPanel.Margin = new Margin(5, 5, 5, 5);
            nodeRoot = rootPanel;

            RebuildGUI();
        }

        public override ContainerControl GetControl()
        {
            return nodeRoot;
        }

        public override NodeBase GetNode()
        {
            return null;
        }

        public override void RebuildGUI()
        {
            if (nodeRoot != null)
            {
                nodeRoot.RemoveChildren();
            }

            VerticalPanel containerControl = nodeRoot.AddChild<VerticalPanel>();
            containerControl.BackgroundColor = Color.Gray;

            var titleLabel = containerControl.AddChild<Label>();
            titleLabel.Text = "Root";
            titleLabel.Font.Size = 13;
        }

        public override void SetHighlighted(bool highlight)
        {
            nodeRoot.BackgroundColor = highlight ? Color.GreenYellow : Color.Gray;
        }
    }
}
