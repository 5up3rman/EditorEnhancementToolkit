using EditorEnhancementToolkit.Foundation.ContentEditor.Enum;
using EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping;

using Sitecore.Rules.Actions;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Rules.Actions
{
    public class SetShortDescription<T> : RuleAction<T> where T : ContentEditorRuleContext
    {
        public string FieldName { get; set; }
        public string Text { get; set; }

        public override void Apply(T ruleContext)
        {
            var mappedItem = MapItemExtensions.GetItem(ruleContext.MappedItems, MapItemType.Field, FieldName) ?? new MapItem();
            mappedItem.Type = MapItemType.Field;
            mappedItem.Title = FieldName;
            mappedItem.ShortDescription = Text;

            ruleContext.MappedItems.AddOrReplaceItem(mappedItem);
        }
    }
}