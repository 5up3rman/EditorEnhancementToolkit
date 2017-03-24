using System.Collections.Generic;
using EditorEnhancementToolkit.Foundation.ContentEditor.Enum;
using EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping;
using Sitecore;

using Sitecore.Rules.Actions;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Rules.Actions
{
    public class SetSectionStyle<T> : RuleAction<T> where T : ContentEditorRuleContext
    {
        public string SectionName { get; set; }
        public string TitleBarStyle { get; set; }
        public string OuterPanelStyle { get; set; }
        public string InnerPanelStyle { get; set; }

        public override void Apply(T ruleContext)
        {
            var mappedItem =MapItemExtensions.GetItem(ruleContext.MappedItems, MapItemType.Section, SectionName) ?? new MapItem();
            mappedItem.Type = MapItemType.Section;
            mappedItem.Title = SectionName;

            var stylesList = new List<string> { TitleBarStyle, OuterPanelStyle, InnerPanelStyle };
            var styles = string.Join("|", stylesList);

            mappedItem.CustomInlineStyle = styles;

            ruleContext.MappedItems.AddOrReplaceItem(mappedItem);
        }
    }
}