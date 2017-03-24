using EditorEnhancementToolkit.Foundation.ContentEditor.VirtualSectionMapping;
using Sitecore.Configuration;
using Sitecore.Rules.Actions;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Rules.Actions
{
    public class MapVirtualSectionFile<T> : RuleAction<T> where T : ContentEditorRuleContext
    {
        public string FileName { get; set; }

        public override void Apply(T ruleContext)
        {
            var filePath = FileExtensions.GetMappedFilePath(RulesConstants.VirtualSectionMappingsDirectory, FileName);
            var virtualSection = (VirtualSection)Factory.CreateObject("editorEnhancedToolkit/virtualSection", false);
            virtualSection.Initialize(filePath, ruleContext.Item.Language);

            ruleContext.VirtualSection = virtualSection;
        }
    }
}