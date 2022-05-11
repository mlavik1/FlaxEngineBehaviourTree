using FlaxEngine;
using FlaxEngine.GUI;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BehaviourTree
{
    public class CompositeNodeView : NodeViewBase
    {
        private ContainerControl parentControl;
        private ContainerControl nodeRoot;
        private NodeContentView compositeView;

        public CompositeNode composite;

        public CompositeNodeView(ContainerControl parent, CompositeNode composite)
        {
            this.parentControl = parent;
            this.composite = composite;

            VerticalPanel rootPanel = parentControl.AddChild<VerticalPanel>();
            rootPanel.Width = 200;
            rootPanel.BackgroundColor = Color.Gray;
            rootPanel.Margin = new Margin(5, 5, 5, 5);
            rootPanel.Spacing = 0;
            nodeRoot = rootPanel;

            RebuildGUI();
        }

        public override void RebuildGUI()
        {
            if (nodeRoot != null)
            {
                nodeRoot.RemoveChildren();
            }

            foreach (DecoratorNode decoratorNode in composite.decorators)
            {
                Decorator decorator = decoratorNode.decorator;
                string[] decoratorProps = decorator.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Select(f => $"{f.Name} = {f.GetValue(decorator).ToString()}").ToArray();
                
                VerticalPanel decoratorControl = nodeRoot.AddChild<VerticalPanel>();
                decoratorControl.BackgroundColor = Color.Blue;
                NodeContentView decoratorView = new NodeContentView(decoratorControl, decoratorNode.GetName(), decoratorProps);
            }

            VerticalPanel compositeControl = nodeRoot.AddChild<VerticalPanel>();
            compositeControl.BackgroundColor = Color.Green;
            compositeView = new NodeContentView(compositeControl, "Composite", new string[] { composite?.type.ToString() });
        }

        public override ContainerControl GetControl()
        {
            return nodeRoot;
        }

        public override NodeBase GetNode()
        {
            return composite;
        }

        public override void SetHighlighted(bool highlight)
        {
            nodeRoot.BackgroundColor = highlight ? Color.GreenYellow : Color.Gray;
        }
    }
}
