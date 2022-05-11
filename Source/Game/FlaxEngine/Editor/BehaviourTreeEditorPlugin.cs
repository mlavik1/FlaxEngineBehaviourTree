//#if FLAX_EDITOR
using FlaxEditor;
using FlaxEditor.GUI;

namespace BehaviourTree
{
    public class VisualScriptingEditorPlugin : EditorPlugin
    {
        private ToolStripButton _button;

        public override void InitializeEditor()
        {
            base.InitializeEditor();

            _button = Editor.UI.ToolStrip.AddButton("BEHAVIOUR TREE TEST");
            //_button.Clicked += () => new BehaviourTreeEditorWindow().Show();
            _button.Clicked += () => new BehaviourTreeEditorWindow(Editor).Show();
        }

        public override void Deinitialize()
        {
            if (_button != null)
            {
                _button.Dispose();
                _button = null;
            }

            base.Deinitialize();
        }
    }
}
//#endif
