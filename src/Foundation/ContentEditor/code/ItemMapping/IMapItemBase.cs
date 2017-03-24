using EditorEnhancementToolkit.Foundation.ContentEditor.Enum;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping
{
    public interface IMapItemBase
    {
        MapItemType Type { get; set; }
        string Title { get; set; }
    }
}