using System.Collections.Generic;
using EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping;
using EditorEnhancementToolkit.Foundation.ContentEditor.VirtualSectionMapping;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Rules
{
    public interface IContentEditor
    {
        List<IMapItem> MappedItems { get; set; }
        IVirtualSection VirtualSection { get; set; }
    }
}