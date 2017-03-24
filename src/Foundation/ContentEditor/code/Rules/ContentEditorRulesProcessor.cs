using System.Collections.Generic;
using EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Rules;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Rules
{
    public class ContentEditorRulesProcessor : IContentEditor
    {
        private Item CurrentItem { get; }

        public List<IMapItem> MappedItems { get; set; } = new List<IMapItem>(); 

        public ContentEditorRulesProcessor(Item currentItem)
        {
            CurrentItem = currentItem;

            if (CurrentItem == null || !CurrentItem.Paths.IsContentItem)
                return;

            ProcessRules();
        }

        private void ProcessRules()
        {
            var rulesFolder = Factory.GetDatabase(RulesConstants.MasterDb).GetItem(RulesConstants.ContentEditorRulesFolder);

            if (rulesFolder == null)
                return;

            var ruleContext = new ContentEditorRuleContext { Item = CurrentItem };
            var rules = RuleFactory.GetRules<ContentEditorRuleContext>(rulesFolder, "Rule");
            rules.Run(ruleContext);
           
            // Set the MappedItems to the ruleContext.MappedItems which is a collection of items gathered from the Actions
            MappedItems = ruleContext.MappedItems;
        }
    }
}