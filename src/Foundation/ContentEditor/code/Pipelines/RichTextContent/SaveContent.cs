using Sitecore.Shell.Controls.RichTextEditor.Pipelines.SaveRichTextContent;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Pipelines.RichTextContent
{
    public class SaveContent
    {
        // Processes the Logic, saves the content when RTE is closed.
        public void Process(SaveRichTextContentArgs args)
        {
            args.Content = RichText.Content(args.Content);
        }
    }
}