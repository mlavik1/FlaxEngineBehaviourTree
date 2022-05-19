using FlaxEngine;
using FlaxEngine.GUI;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourTree
{
    public abstract class NodeViewBase
    {
        public List<NodeViewBase> children = new List<NodeViewBase>();
        public NodeViewBase parent;

        private List<Panel> connectorPanels = new List<Panel>();

        public abstract ContainerControl GetControl();
        public abstract NodeBase GetNode();
        public abstract void SetHighlighted(bool highlight);
        public abstract void RebuildGUI();

        public void DisposeContent()
        {
            GetControl().Dispose();
            connectorPanels.ForEach(c => c.Dispose());
            connectorPanels.Clear();
        }

        public void UnlinkNode()
        {
            GetNode().UnlinkNode();

            foreach (NodeViewBase childNodeView in children)
            {
                childNodeView.parent = parent;
                parent?.children.Add(childNodeView);
            }

            parent.children.Remove(this);

            parent.UpdateConnectors();
            foreach (NodeViewBase childNodeView in children)
                childNodeView.UpdateConnectors();

            connectorPanels.ForEach(c => c.Dispose());
            connectorPanels.Clear();

            children.Clear();
            parent = null;
        }

        public void UpdateConnectors()
        {
            int lineCount = children.Count > 0 ? children.Count + 2 : 0;
            for (int iPanel = connectorPanels.Count; iPanel < lineCount; iPanel++)
            {
                connectorPanels.Add(GetControl().Parent.AddChild<Panel>());
                connectorPanels[iPanel].BackgroundColor = Color.White;
            }

            connectorPanels.ForEach(c => c.Visible = false);
            if (children.Count > 0)
            {
                float topChildY = float.PositiveInfinity, minChildX = float.PositiveInfinity, maxChildX = float.NegativeInfinity;
                foreach (NodeViewBase childNode in children)
                {
                    Vector2 childAnchor = childNode.GetControl().Location + new Vector2(childNode.GetControl().Width / 2, 0.0f);
                    topChildY = Mathf.Min(topChildY, childAnchor.Y);
                    minChildX = Mathf.Min(minChildX, childAnchor.X);
                    maxChildX = Mathf.Max(maxChildX, childAnchor.X);
                }

                Vector2 rootAnchor = GetControl().Location + new Vector2(GetControl().Width / 2, GetControl().Height);
                Vector2 midAnchor = new Vector2(Mathf.Min(minChildX, rootAnchor.X), (rootAnchor.Y + topChildY) / 2);
                float midAnchorWidth = Mathf.Max(midAnchor.X, maxChildX, rootAnchor.X) - Mathf.Min(midAnchor.X, minChildX, rootAnchor.X);

                connectorPanels[0].Visible = true;
                connectorPanels[0].Location = rootAnchor;
                connectorPanels[0].Height = midAnchor.Y - rootAnchor.Y;
                connectorPanels[0].Width = 1;

                connectorPanels[1].Visible = true;
                connectorPanels[1].Location = midAnchor;
                connectorPanels[1].Height = 1;
                connectorPanels[1].Width = midAnchorWidth;

                for (int iChild = 0; iChild < children.Count; iChild++)
                {
                    NodeViewBase currChild = children[iChild];
                    Vector2 childAnchor = currChild.GetControl().Location + new Vector2(currChild.GetControl().Width / 2, 0.0f);

                    Panel currPanel = connectorPanels[iChild + 2];
                    currPanel.Visible = true;
                    currPanel.Location = new Vector2(childAnchor.X, midAnchor.Y);
                    currPanel.Height = childAnchor.Y - midAnchor.Y;
                    currPanel.Width = 1;
                }
            }
        }
    }
}
