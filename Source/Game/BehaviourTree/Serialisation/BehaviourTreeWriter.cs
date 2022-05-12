using System;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

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
            XmlStringWriter stringWriter = new XmlStringWriter(stringBuilder, Encoding.UTF8); 

            System.Xml.XmlWriter writer = XmlWriter.Create(stringWriter, settings);
            
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
            WriteFields(task, writer);
            WriteChildNodes(node, writer);
            writer.WriteEndElement();
        }

        private void WriteDecoratorNode(DecoratorNode node, XmlWriter writer)
        {
            Decorator decorator = node.decorator;
            Type decoratorType = decorator.GetType();
            writer.WriteStartElement("Decorator");
            writer.WriteAttributeString("class", decoratorType.FullName);
            WriteFields(decorator, writer);
            writer.WriteEndElement();
        }

        private void WriteFields(object obj, XmlWriter writer)
        {
            writer.WriteStartElement("Fields");
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
            foreach (FieldInfo field in fields)
            {
                writer.WriteStartElement(field.Name);
                XmlSerializer serializer = new XmlSerializer(field.FieldType);
                object fieldValue = field.GetValue(obj);
                serializer.Serialize(writer, fieldValue, ns);
                writer.WriteEndElement();
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
