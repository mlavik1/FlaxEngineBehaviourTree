using FlaxEditor.CustomEditors;
using FlaxEditor.CustomEditors.Editors;
using FlaxEditor.GUI;
using FlaxEditor.Scripting;
using FlaxEngine;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace BehaviourTree
{
    [CustomEditor(typeof(AIController))]
    public class AIControllerEditor : GenericEditor
    {
        private string btFilePaths;

        public override void Initialize(LayoutElementsContainer layout)
        {
            AIController aiController = Values[0] as AIController;

            string[] btFilePaths = Directory.GetFiles(Globals.ProjectContentFolder, "*.btxml", SearchOption.AllDirectories)
                .Select(path => path.Replace(Globals.ProjectContentFolder, "")).ToArray();;

            var btCmb = layout.ComboBox("Behaviour tree");
            btCmb.ComboBox.AddItems(btFilePaths);
            btCmb.ComboBox.SelectedIndexChanged += (ComboBox cmb) =>
            {
                aiController.behaviourTreeFilePath = btFilePaths[cmb.SelectedIndex];
                aiController.pathRelativeToContentDir = true;
            };
        }
    }
}
