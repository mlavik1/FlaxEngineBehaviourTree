using FlaxEditor;
using FlaxEditor.Content;
using FlaxEditor.Windows;
using FlaxEngine;
using System;
using System.IO;

namespace BehaviourTree
{
    public class BehaviourTreeAssetProxy : ContentProxy
    {
        public override string Name => "Behaviour tree";

        public override Color AccentColor => Color.Blue;

        public override string FileExtension => "btxml";

        public override EditorWindow Open(Editor editor, ContentItem item)
        {
            BehaviourTreeEditorWindow editorWindow = new BehaviourTreeEditorWindow(Editor.Instance, item.Path);
            return editorWindow;
        }

        public override bool IsProxyFor(ContentItem item)
        {
            return item is FileItem && Path.GetExtension(item.Path).Remove(0, 1) == FileExtension;
        }

        public override bool CanCreate(ContentFolder targetLocation)
        {
            return true;
        }

        public override void Create(string outputPath, object arg)
        {
            BehaviourTreeWriter treeWriter = new BehaviourTreeWriter();
            string xmlString = treeWriter.WriteXml(new BehaviourTree());
            StreamWriter streamWriter = new StreamWriter(outputPath);
            streamWriter.Write(xmlString);
            streamWriter.Flush();
            streamWriter.Close();
        }
    }
}