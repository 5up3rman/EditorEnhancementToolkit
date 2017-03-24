using EditorEnhancementToolkit.Foundation.ContentEditor.FileMappings;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping
{
    public interface IMapItem : IMapItemBase, IFileMapping
    {
        string NewTitle { get; set; }
        string ShortDescription { get; set; }
        string CustomInlineStyle { get; set; }
        string FieldSource { get; set; }
        bool Hide { get; set; }
    }
}