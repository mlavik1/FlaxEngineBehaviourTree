using FlaxEditor.CustomEditors;
using FlaxEditor.CustomEditors.Editors;
using FlaxEditor.Scripting;
using FlaxEngine;
using System;
using System.Reflection;

namespace BehaviourTree
{
    [CustomEditor(typeof(NodeViewBase))]
    public class NodeViewEditor : GenericEditor
    {
        public override void Initialize(LayoutElementsContainer layout)
        {
            NodeViewBase nodeView = Values[0] as NodeViewBase;
            Type nodeType = nodeView.GetType();

            layout.Header(nodeType.Name);
            layout.Space(10.0f);

            // Composite node
            if (nodeType == typeof(CompositeNodeView))
            {
                CompositeNodeView compNodeView = (CompositeNodeView)nodeView;
                CompositeNode compNode = (CompositeNode)compNodeView.GetNode();

                var decorGroup = layout.Group("Decorators");
                foreach (DecoratorNode decoratorNode in compNode.decorators)
                {
                    Decorator decorator = decoratorNode.decorator;
                    FieldInfo[] fields = decorator.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                    Debug.Log(fields.Length);
                    foreach (FieldInfo field in fields)
                    {
                        object fieldValue = field.GetValue(decorator);
                        ScriptType scriptType = TypeUtils.GetType(field.FieldType.FullName);
                        CustomValueContainer.GetDelegate fieldGetter = (instance, index) => { return field.GetValue(decorator); };
                        CustomValueContainer.SetDelegate fieldSetter = (instance, index, value) =>
                        {
                            field.SetValue(decorator, value);
                            compNodeView.RebuildGUI();
                        };
                        ValueContainer valueContainer = new CustomValueContainer(scriptType, fieldValue, fieldGetter, fieldSetter);
                        decorGroup.Object(field.Name, valueContainer, null);
                    }
                }
            }

            // Task node
            if (nodeType == typeof(TaskNodeView))
            {
                TaskNodeView taskNodeView = (TaskNodeView)nodeView;
                TaskNode taskNode = (TaskNode)taskNodeView.GetNode();
                Task task = taskNode.GetTask();
                var taskGroup = layout.Group(task.GetType().Name);

                FieldInfo[] fields = task.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(task);
                    ScriptType scriptType = TypeUtils.GetType(field.FieldType.FullName);
                    CustomValueContainer.GetDelegate fieldGetter = (instance, index) => { return field.GetValue(task); };
                    CustomValueContainer.SetDelegate fieldSetter = (instance, index, value) =>
                    {
                        field.SetValue(task, value);
                        taskNodeView.RebuildGUI();
                    };
                    ValueContainer valueContainer = new CustomValueContainer(scriptType, fieldValue, fieldGetter, fieldSetter);
                    taskGroup.Object(field.Name, valueContainer, null);
                }
            }
        }
    }
}
