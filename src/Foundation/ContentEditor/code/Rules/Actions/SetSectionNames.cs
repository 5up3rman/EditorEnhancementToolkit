using EditorEnhancementToolkit.Foundation.ContentEditor.Enum;
using EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping;

using Sitecore.Rules.Actions;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Rules.Actions
{
    public class SetSectionNames<T> : RuleAction<T> where T : ContentEditorRuleContext
    {
        public string SectionName { get; set; }
        public string NewName { get; set; }

        public override void Apply(T ruleContext)
        {
            var mappedItem =MapItemExtensions.GetItem(ruleContext.MappedItems, MapItemType.Section, SectionName) ?? new MapItem();
            mappedItem.Type = MapItemType.Section;
            mappedItem.Title = SectionName;
            mappedItem.NewTitle = NewName;

            ruleContext.MappedItems.AddOrReplaceItem(mappedItem);
        }
    }
}