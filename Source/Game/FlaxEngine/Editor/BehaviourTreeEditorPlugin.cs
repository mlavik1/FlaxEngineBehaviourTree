//#if FLAX_EDITOR
using FlaxEditor;
using FlaxEditor.GUI;

namespace BehaviourTree
{
    public class VisualScriptingEditorPlugin : EditorPlugin
    {
        private MainMenuButton mainMenuButton;

        public override void InitializeEditor()
        {
            base.InitializeEditor();
            mainMenuButton = Editor.UI.MainMenu.AddButton("Behaviour tree");
            mainMenuButton.ContextMenu.AddButton("Show behaviour tree editor").Clicked += () => new BehaviourTreeEditorWindow(Editor).Show();
        }

        public override void Deinitialize()
        {
            if (mainMenuButton != null)
            {
                mainMenuButton.Dispose();
                mainMenuButton = null;
            }

            base.Deinitialize();
        }
    }
}
//#endif
