using EditorEnhancementToolkit.Foundation.ContentEditor.Enum;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping
{
    public class MapItemBase : IMapItemBase
    {
        public MapItemType Type { get; set; }
        public string Title { get; set; }
    }
}