using System.Collections.Generic;
using EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Rules
{
    public interface IContentEditor
    {
        List<IMapItem> MappedItems { get; set; }
    }
}