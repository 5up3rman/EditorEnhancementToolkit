using Sitecore.Data;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Rules.Conditions
{
    public class IsItemCondition<T> : WhenCondition<T> where T : ContentEditorRuleContext
    {
        public ID ItemId { get; set; }
        
        public IsItemCondition()
        {
            this.ItemId = ID.Null;
        }

             protected override bool Execute(T ruleContext)
        {
            var itm = ruleContext.Item;

            return itm != null && itm.ID == ItemId;
        }
    }
}
