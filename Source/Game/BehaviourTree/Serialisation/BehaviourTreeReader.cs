using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Linq;

namespace BehaviourTree
{
    public class BehaviourTreeReader
    {
        public BehaviourTree ReadXml(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            XmlReader reader = XmlReader.Create(filePath);

            reader.MoveToContent();
            if (reader.Name != "Tree")
                throw new FormatException("Expected <Tree> node.");

            BehaviourTree behaviourTree = new BehaviourTree();

            XmlNode xmlNode = doc.ReadNode(reader);
            XmlNode[] children = xmlNode.ChildNodes.Cast<XmlNode>().Where(n => n.NodeType == XmlNodeType.Element).ToArray();
            if (children.Length > 1)
                throw new FormatException("Root node should only have one child. It has: " + children.Length);

            if (xmlNode.ChildNodes.Count > 0)
            {
                NodeBase rootNode = ReadNode(children[0]);
                behaviourTree.SetRootNode(rootNode);
            }

            return behaviourTree;
        }

        private NodeBase ReadNode(XmlNode xmlNode)
        {
            if (xmlNode.Name == "Composite")
                return ReadCompositeNode(xmlNode);
            else if (xmlNode.Name == "Task")
                return ReadTaskNode(xmlNode);
            else
                throw new FormatException("Invalid node type: " + xmlNode.Name);
        }

        private CompositeNode ReadCompositeNode(XmlNode xmlNode)
        {
            var typeAttr = xmlNode.Attributes["type"];
            if (typeAttr == null)
                throw new FormatException("Composite node without type attribute.");
            CompositeType compType = (CompositeType)Enum.Parse(typeof(CompositeType), typeAttr.Value);
            CompositeNode compNode = new CompositeNode(compType);
            AddChildNodes(compNode, xmlNode);
            return compNode;
        }

        private TaskNode ReadTaskNode(XmlNode xmlNode)
        {
            var classAttr = xmlNode.Attributes["class"];
            if (classAttr == null)
                throw new FormatException("Task node without class attribute.");
            Type compType = Type.GetType(classAttr.Value);
            Task task = (Task)Activator.CreateInstance(compType);
            TaskNode taskNode = new TaskNode(task);
            AddChildNodes(taskNode, xmlNode);
            return taskNode;
        }

        private void AddChildNodes(NodeBase node, XmlNode xmlNode)
        {
            XmlNode childContainerNode = xmlNode.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name == "Children");
            if (childContainerNode != null)
            {
                XmlNode[] children = childContainerNode.ChildNodes.Cast<XmlNode>().Where(n => n.NodeType == XmlNodeType.Element).ToArray();
                foreach (XmlNode childXmlNode in children)
                {
                    node.AddChild(ReadNode(childXmlNode));
                }
            }
        }
    }
}
