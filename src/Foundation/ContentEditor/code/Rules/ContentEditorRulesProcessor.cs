using System.Collections.Generic;
using EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping;
using EditorEnhancementToolkit.Foundation.ContentEditor.VirtualSectionMapping;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Rules;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Rules
{
    public class ContentEditorRulesProcessor : IContentEditor
    {
        public List<IMapItem> MappedItems { get; set; } = new List<IMapItem>();
        public IVirtualSection VirtualSection { get; set; }

        public ContentEditorRulesProcessor()
        {
        }

        public void ProcessRules(Item currentItem)
        {
            if (currentItem == null || !currentItem.Paths.IsContentItem)
                return;

            var rulesFolder = Factory.GetDatabase(RulesConstants.MasterDb).GetItem(RulesConstants.ContentEditorRulesFolder);

            if (rulesFolder == null)
                return;

            var ruleContext = new ContentEditorRuleContext {Item = currentItem};
            var rules = RuleFactory.GetRules<ContentEditorRuleContext>(rulesFolder, "Rule");
            rules.Run(ruleContext);

            // Set the MappedItems to the ruleContext.MappedItems which is a collection of items gathered from the Actions
            MappedItems = ruleContext.MappedItems;
            VirtualSection = ruleContext.VirtualSection;
        }
    }
}