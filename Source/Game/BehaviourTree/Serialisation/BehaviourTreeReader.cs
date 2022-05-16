using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Serialization;

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

            reader.Close();

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
            AddDecorators(compNode, xmlNode);
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
            AddFields(task, xmlNode);
            TaskNode taskNode = new TaskNode(task);
            AddChildNodes(taskNode, xmlNode);
            return taskNode;
        }

        private void AddDecorators(CompositeNode compNode, XmlNode xmlNode)
        {
            XmlNode decoratorContainerNode = xmlNode.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name == "Decorators");
            if (decoratorContainerNode == null)
                return;

            XmlNode[] decoratorNodes = decoratorContainerNode.ChildNodes.Cast<XmlNode>().Where(n => n.NodeType == XmlNodeType.Element).ToArray();
            foreach (XmlNode decoratorNode in decoratorNodes)
            {
                var classAttr = decoratorNode.Attributes["class"];
                if (classAttr == null)
                    throw new FormatException("Decorator without class attribute.");
                Type decoratorType = Type.GetType(classAttr.Value);
                Decorator decorator = (Decorator)Activator.CreateInstance(decoratorType);
                AddFields(decorator, decoratorNode);
                compNode.decorators.Add(new DecoratorNode(decorator));
            }
        }

        private void AddFields(object obj, XmlNode xmlNode)
        {
            XmlNode fieldContainerNode = xmlNode.ChildNodes.Cast<XmlNode>().FirstOrDefault(n => n.Name == "Fields");
            if (fieldContainerNode == null)
                return;

            XmlNode[] fieldNodes = fieldContainerNode.ChildNodes.Cast<XmlNode>().Where(n => n.NodeType == XmlNodeType.Element).ToArray();

            Dictionary<string, FieldInfo> fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).ToDictionary(f => f.Name, f => f);
            foreach (XmlNode fieldNode in fieldNodes)
            {
                if (fields.ContainsKey(fieldNode.Name))
                {
                    FieldInfo field = fields[fieldNode.Name];
                    using (TextReader innerXmlReader = new StringReader(fieldNode.InnerXml))
                    {
                        XmlSerializer serializer = new XmlSerializer(field.FieldType);
                        object deserialisedInnerNode = serializer.Deserialize(innerXmlReader);
                        field.SetValue(obj, Convert.ChangeType(deserialisedInnerNode, field.FieldType));
                    }
                }
            }
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
