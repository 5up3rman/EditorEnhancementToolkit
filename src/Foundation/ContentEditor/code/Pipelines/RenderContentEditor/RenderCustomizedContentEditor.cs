using EditorEnhancementToolkit.Foundation.ContentEditor.Shell.Applications.ContentEditor;
using Sitecore.Shell.Applications.ContentEditor.Pipelines.RenderContentEditor;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Pipelines.RenderContentEditor
{
    public class RenderCustomizedContentEditor
    {
        public void Process(RenderContentEditorArgs args)
        {
            var editorFormatter = new EditorFormatter(args);
            editorFormatter.RenderSections(args.Parent, args.Sections, args.ReadOnly);
        }
    }
}