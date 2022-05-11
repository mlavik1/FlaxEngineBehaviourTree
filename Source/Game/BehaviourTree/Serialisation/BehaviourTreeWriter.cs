using System;
using System.Reflection;
using System.Text;
using System.Xml;

namespace BehaviourTree
{
    public class BehaviourTreeWriter
    {
        public string WriteXml(BehaviourTree behaviourTree)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.Indent = true;

            StringBuilder stringBuilder = new StringBuilder();
            System.Xml.XmlWriter writer = XmlWriter.Create(stringBuilder, settings);
            
            writer.WriteStartDocument();
            writer.WriteStartElement("Tree");
            WriteNode(behaviourTree.GetRootNode(), writer);
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
            return stringBuilder.ToString();
        }

        private void WriteNode(NodeBase node, XmlWriter writer)
        {
            Type type = node.GetType();
            if (type == typeof(CompositeNode))
                WriteCompositeNode((CompositeNode)node, writer);
            else if (type == typeof(TaskNode))
                WriteTaskNode((TaskNode)node, writer);
            else
                throw new NotImplementedException("Unsupported node type.");
        }

        private void WriteCompositeNode(CompositeNode node, XmlWriter writer)
        {
            writer.WriteStartElement("Composite");
            writer.WriteAttributeString("type", node.type.ToString());
            if (node.decorators.Count > 0)
            {
                writer.WriteStartElement("Decorators");
                foreach (DecoratorNode decoratorNode in node.decorators)
                    WriteDecoratorNode(decoratorNode, writer);
                writer.WriteEndElement();
            }
            WriteChildNodes(node, writer);
            writer.WriteEndElement();
        }

        private void WriteTaskNode(TaskNode node, XmlWriter writer)
        {
            Task task = node.GetTask();
            Type taskType = task.GetType();
            writer.WriteStartElement("Task");
            writer.WriteAttributeString("class", taskType.FullName);

            FieldInfo[] fields = taskType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                object fieldValue = field.GetValue(task);
                writer.WriteAttributeString(field.Name, fieldValue.ToString());
            }
            WriteChildNodes(node, writer);
            writer.WriteEndElement();
        }

        private void WriteDecoratorNode(DecoratorNode node, XmlWriter writer)
        {
            Decorator decorator = node.decorator;
            Type decoratorType = decorator.GetType();
            writer.WriteStartElement("Decorator");
            writer.WriteAttributeString("class", decoratorType.FullName);

            FieldInfo[] fields = decoratorType.GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                object fieldValue = field.GetValue(decorator);
                writer.WriteAttributeString(field.Name, fieldValue.ToString());
            }
            writer.WriteEndElement();
        }

        private void WriteChildNodes(NodeBase node, XmlWriter writer)
        {
            var children = node.GetChildren();
            if (children.Count > 0)
            {
                writer.WriteStartElement("Children");
                foreach (NodeBase childNode in children)
                {
                    WriteNode(childNode, writer);
                }
                writer.WriteEndElement();
            }
        }
    }
}
