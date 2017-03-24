using System;
using System.Collections.Generic;
using EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping;
using Sitecore.Rules;


namespace EditorEnhancementToolkit.Foundation.ContentEditor.Rules
{
    [Serializable]
    public class ContentEditorRuleContext : RuleContext, IContentEditor
    {
        public List<IMapItem> MappedItems { get; set; } = new List<IMapItem>();
    }
}