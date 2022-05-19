using FlaxEngine;
using FlaxEngine.GUI;

namespace BehaviourTree
{
    public class NodeContentView
    {
        public NodeContentView(VerticalPanel nodeRoot, string nodeName, string[] nodeDescriptions)
        {
            // Title label (function name, etc.)
            var titleLabel = nodeRoot.AddChild<Label>();
            titleLabel.Text = nodeName;
            titleLabel.Font.Size = 13;

            // Descriptions
            foreach (string description in nodeDescriptions)
            {
                // Description label
                var descriptionLabel = nodeRoot.AddChild<Label>();
                descriptionLabel.Text = description;
                descriptionLabel.HorizontalAlignment = TextAlignment.Near;
            }
        }
    }
}
