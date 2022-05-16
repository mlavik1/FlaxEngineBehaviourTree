//#if FLAX_EDITOR
using FlaxEditor;
using FlaxEditor.GUI;

namespace BehaviourTree
{
    public class VisualScriptingEditorPlugin : EditorPlugin
    {
        private MainMenuButton mainMenuButton;
        private BehaviourTreeAssetProxy behaviourTreeProxy;

        public override void InitializeEditor()
        {
            base.InitializeEditor();
            mainMenuButton = Editor.UI.MainMenu.AddButton("Behaviour tree");
            mainMenuButton.ContextMenu.AddButton("Show behaviour tree editor").Clicked += () => new BehaviourTreeEditorWindow(Editor).Show();

            behaviourTreeProxy = new BehaviourTreeAssetProxy();
            Editor.ContentDatabase.Proxy.Insert(0, behaviourTreeProxy);
        }

        public override void Deinitialize()
        {
            if (mainMenuButton != null)
            {
                mainMenuButton.Dispose();
                mainMenuButton = null;
            }

            Editor.ContentDatabase.Proxy.Remove(behaviourTreeProxy);

            base.Deinitialize();
        }
    }
}
//#endif
