using System.Collections.Generic;
using System.Linq;
using EditorEnhancementToolkit.Foundation.ContentEditor.Enum;
using EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping;

namespace EditorEnhancementToolkit.Foundation.ContentEditor
{
    public static class MapItemExtensions
    {
        public static IMapItem GetItem(IEnumerable<IMapItem> mappedItems, MapItemType itemType, string originalName)
        {
            return mappedItems.FirstOrDefault(x => x.Title.EndsWith(originalName) && x.Type.Equals(itemType));
        }
    }
}