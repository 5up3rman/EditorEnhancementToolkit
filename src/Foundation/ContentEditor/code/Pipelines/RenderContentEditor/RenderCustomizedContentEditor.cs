using EditorEnhancementToolkit.Foundation.ContentEditor.Shell.Applications.ContentEditor;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentEditor.Pipelines.RenderContentEditor;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Pipelines.RenderContentEditor
{
    public class RenderCustomizedContentEditor
    {
        public void Process(RenderContentEditorArgs args)
        {
            var editorFormatter = new EditorFormatter(args);
            var language = args.Item?.Language ?? args.Language ?? Language.Current;
            editorFormatter.RenderSections(language, args.Parent, args.Sections, args.ReadOnly);
        }
    }
}