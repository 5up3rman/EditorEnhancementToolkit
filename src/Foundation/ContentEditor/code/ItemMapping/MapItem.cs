
using System.Collections.Generic;
using EditorEnhancementToolkit.Foundation.ContentEditor.Enum;
using EditorEnhancementToolkit.Foundation.ContentEditor.FileMappings;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping
{
	public class MapItem : IMapItem
    {
        public  MapItemType Type { get; set; }
	    public string Title { get; set; } = string.Empty;
	    public string NewTitle { get; set; } = string.Empty;
	    public string ShortDescription { get; set; } = string.Empty;
        public string CustomInlineStyle { get; set; } = string.Empty;
	    public string FieldSource { get; set; } = string.Empty;
	    public bool Hide { get; set; } = false;
	    public IEnumerable<T> GetItems<T>() where T : IFileMapping
	    {
	        yield break;
	    }
    }
}