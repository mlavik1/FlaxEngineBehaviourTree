//#if FLAX_EDITOR
using FlaxEditor;
using FlaxEditor.CustomEditors;
using FlaxEditor.GUI;
using FlaxEditor.GUI.ContextMenu;
using FlaxEditor.Scripting;
using FlaxEditor.Windows;
using FlaxEngine;
using FlaxEngine.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BehaviourTree
{
    public class BehaviourTreeEditorWindow : EditorWindow
    {
        private SplitPanel splitPanel;
        private CustomEditorPresenter propertiesEditor;
        private Undo undo;
        private ContainerControl canvasControl;
        private ToolStrip toolStrip;
        private Vector2 mousePosRelToNode;
        private NodeViewBase movingNode;
        private NodeViewBase selectedNode;
        private RootNodeView rootNodeView;
        private List<NodeViewBase> nodes = new List<NodeViewBase>();
        //private BehaviourTree behaviourTree; // REMOVE ME (For test)

        public BehaviourTreeEditorWindow(Editor editor)
        : base(editor, true, ScrollBars.None)
        {
            undo = new Undo();
            undo.UndoDone += OnUndoRedo;
            undo.RedoDone += OnUndoRedo;
            undo.ActionDone += OnUndoRedo;

            toolStrip = new ToolStrip();
            toolStrip.Parent = this;
            toolStrip.AddButton(editor.Icons.Save64, SaveBehaviourTree).LinkTooltip("Save behaviour tree");
            toolStrip.AddButton(editor.Icons.FolderOpen32, LoadBehaviourTree).LinkTooltip("Load behaviour tree");

            splitPanel = new SplitPanel(Orientation.Horizontal, ScrollBars.None, ScrollBars.Vertical)
            {
                AnchorPreset = AnchorPresets.StretchAll,
                SplitterValue = 0.7f,
                Parent = this
            };
            canvasControl = splitPanel.Panel1;

            propertiesEditor = new CustomEditorPresenter(undo);
            propertiesEditor.Features = FeatureFlags.None;
            propertiesEditor.Panel.Parent = splitPanel.Panel2;
            propertiesEditor.Modified += OnPropertyEdited;

            rootNodeView = new RootNodeView(canvasControl);
            nodes.Add(rootNodeView);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            Vector2 mousePos = canvasControl.PointFromScreen(Input.Mouse.Position);

            // Right click => Show context menu
            if (Input.GetMouseButtonDown(MouseButton.Right) && canvasControl.ContainsPoint(ref mousePos))
            {
                NodeViewBase node = GetNodeAtLocation(mousePos);
                SelectNode(node);
                if (node != null)
                    ShowContextMenu(mousePos);
            }

            // MouseDown => Select new node
            if (Input.GetMouseButtonDown(MouseButton.Left) && canvasControl.ContainsPoint(ref mousePos))
            {
                movingNode = GetNodeAtLocation(mousePos);
                SelectNode(movingNode);
                if (movingNode != null)
                {
                    mousePosRelToNode = mousePos - movingNode.GetControl().Location;
                }
            }
            else if (Input.GetMouseButtonUp(MouseButton.Left) && movingNode != null)
            {
                movingNode = null;
            }

            // MouseHold => Move selected node
            if (movingNode != null && Input.GetMouseButton(MouseButton.Left))
            {
                movingNode.GetControl().Location = mousePos - mousePosRelToNode;
                movingNode.UpdateConnectors();
                movingNode.parent?.UpdateConnectors();
            }

            if (Input.GetKeyDown(KeyboardKeys.Delete) && selectedNode != null)
            {
                DeleteNode(selectedNode);
            }

            //if (behaviourTree != null)
            //{
            //    behaviourTree.Update(); // Test
            //}
        }

        private void OnUndoRedo(IUndoAction action)
        {

        }

        private void OnPropertyEdited()
        {

        }

        private NodeViewBase GetNodeAtLocation(Vector2 mousePos)
        {
            foreach (NodeViewBase node in nodes)
            {
                if (node.GetControl().GetClientArea().Contains(node.GetControl().PointFromParent(mousePos)))
                {
                    return node;
                }
            }
            return null;
        }

        private void ShowContextMenu(Vector2 mousePos)
        {
            if (selectedNode == null)
                return;

            ContextMenu contextMenu = new ContextMenu();

            NodeViewBase node = selectedNode;
            Type nodeType = selectedNode.GetType();

            // Root node or composite => Allow adding child composites or tasks.
            if (nodeType == typeof(CompositeNodeView) || (nodeType == typeof(RootNodeView) && node.children.Count < 1))
            {
                ContextMenuChildMenu compSubMenu = contextMenu.AddChildMenu("New Composite");

                // Composite
                foreach (var compTypeObj in Enum.GetValues(typeof(CompositeType)))
                {
                    CompositeType compType = (CompositeType)compTypeObj;
                    var btnAddComp = compSubMenu.ContextMenu.AddButton($"{Enum.GetName(typeof(CompositeType), compType)} composite");
                    btnAddComp.Clicked += () =>
                    {
                        CompositeNode composite = new CompositeNode(compType);
                        CompositeNodeView newCompView = new CompositeNodeView(canvasControl, composite);
                        AddNode(newCompView, node);
                    };
                }

                ContextMenuChildMenu taskSubMenu = contextMenu.AddChildMenu("New Task");

                // Task
                List<Type> taskTypes = Assembly.GetAssembly(typeof(Task)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Task))).ToList();
                foreach (Type taskType in taskTypes)
                {
                    var btnAddTask = taskSubMenu.ContextMenu.AddButton(taskType.Name);
                    btnAddTask.Clicked += () =>
                    {
                        TaskNode task = new TaskNode((Task)Activator.CreateInstance(taskType));
                        TaskNodeView taskView = new TaskNodeView(canvasControl, task);
                        AddNode(taskView, node);
                    };
                }
            }

            // Composite => Add decorators
            if (nodeType == typeof(CompositeNodeView))
            {
                ContextMenuChildMenu decoratorSubMenu = contextMenu.AddChildMenu("Add decorator");

                CompositeNodeView compNodeView = (CompositeNodeView)node;
                List<Type> decoratorTypes = Assembly.GetAssembly(typeof(Decorator)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Decorator))).ToList();
                foreach (Type decoratorType in decoratorTypes)
                {
                    var btnAddDecorator = decoratorSubMenu.ContextMenu.AddButton(decoratorType.Name);
                    btnAddDecorator.Clicked += () =>
                    {
                        DecoratorNode decoratorNode = new DecoratorNode((Decorator)Activator.CreateInstance(decoratorType));
                        compNodeView.composite.decorators.Add(decoratorNode);
                        compNodeView.RebuildGUI();
                    };
                }
            }

            contextMenu.Show(canvasControl, mousePos);
        }

        private void SelectNode(NodeViewBase node)
        {
            selectedNode?.SetHighlighted(false);
            selectedNode = node;
            selectedNode?.SetHighlighted(true);

            InspectNode(node);
        }

        private void InspectNode(NodeViewBase node)
        {
            propertiesEditor.Select(node);
        }

        private void AddNode(NodeViewBase nodeView, NodeViewBase parent)
        {
            nodes.Add(nodeView);
            parent.children.Add(nodeView);
            nodeView.parent = parent;

            nodeView.GetControl().Location = parent.GetControl().Location + new Vector2(0.0f, parent.GetControl().Height + 50.0f);

            // Only sequence nodes can have children
            NodeBase parentNode = parent.GetNode();
            if (parentNode != null && parentNode.GetType() == typeof(CompositeNode))
                ((CompositeNode)parentNode).AddChild(nodeView.GetNode());

            parent.UpdateConnectors();
        }

        private void DeleteNode(NodeViewBase nodeView)
        {
            if (nodeView.parent != null)
            {
                // Copy old parent and children (since they will change)
                NodeViewBase oldParent = nodeView.parent;
                List<NodeViewBase> oldChildren = new List<NodeViewBase>(nodeView.children);

                // Unlink node and remove node view
                nodeView.UnlinkNode();
                this.nodes.Remove(nodeView);
                nodeView.GetControl().Dispose();

                // Update connectors
                oldParent.UpdateConnectors();
                foreach (NodeViewBase childNodeView in oldChildren)
                    childNodeView.UpdateConnectors();
            }
        }

        private void SaveBehaviourTree()
        {
            BehaviourTree behaviourTree = new BehaviourTree();
            behaviourTree.SetRootNode(rootNodeView.children[0].GetNode()); // TODO!
            Debug.Log(behaviourTree.GetRootNode().GetChildren().Count);

            string[] filenames;
            FileSystem.ShowSaveFileDialog(null, "", "", false, "Save behaviour tree", out filenames);
            if (filenames != null && filenames.Length > 0)
            {
                BehaviourTreeWriter treeWriter = new BehaviourTreeWriter();
                string xmlString = treeWriter.WriteXml(behaviourTree);
                Debug.Log("XML: " + xmlString);
                StreamWriter streamWriter = new StreamWriter(filenames[0]);
                streamWriter.Write(xmlString);
                streamWriter.Flush();
                streamWriter.Close();
            }
        }

        private void LoadBehaviourTree()
        {
            string[] filenames;
            FileSystem.ShowOpenFileDialog(null, "", "", false, "Save behaviour tree", out filenames);
            if (filenames != null && filenames.Length > 0)
            {
                BehaviourTreeReader treeReader = new BehaviourTreeReader();
                BehaviourTree behaviourTree = treeReader.ReadXml(filenames[0]);
                rootNodeView = null;
                foreach (NodeViewBase nodeView in nodes)
                    nodeView.GetControl().Dispose();
                nodes.Clear();
                rootNodeView = new RootNodeView(canvasControl);
                CreateNodeView(behaviourTree.GetRootNode(), rootNodeView);
            }
        }

        private void CreateNodeView(NodeBase node, NodeViewBase parent)
        {
            NodeViewBase nodeView = null;
            if (node.GetType() == typeof(CompositeNode))
                nodeView = new CompositeNodeView(canvasControl, (CompositeNode)node);
            else if (node.GetType() == typeof(TaskNode))
                nodeView = new TaskNodeView(canvasControl, (TaskNode)node);
            else
                throw new NotImplementedException("Unimplemented node type: " + node.GetType().FullName);

            nodes.Add(nodeView);
            parent.children.Add(nodeView);
            nodeView.parent = parent;
            nodeView.UpdateConnectors();
            parent.UpdateConnectors();

            nodeView.GetControl().Location = parent.GetControl().Location + new Vector2(0.0f, parent.GetControl().Height + 50.0f);

            foreach (NodeBase childNode in node.GetChildren())
                CreateNodeView(childNode, nodeView);
        }
    }
}
//#endif
