using EditorEnhancementToolkit.Foundation.ContentEditor.Enum;
using EditorEnhancementToolkit.Foundation.ContentEditor.FileMappings;
using Sitecore.Rules.Actions;
using EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping;
using Sitecore.Configuration;


namespace EditorEnhancementToolkit.Foundation.ContentEditor.Rules.Actions
{
    public class MapFile<T> : RuleAction<T> where T : ContentEditorRuleContext
    {
        public string FileName { get; set; }

        public override void Apply(T ruleContext)
        {
            var mappedFileItems = (XmlMapItem)Factory.CreateObject("editorEnhancedToolkit/xmlMapItem", false);
            mappedFileItems.Initialize(MapItemType.All, FileName);

            foreach (var item in mappedFileItems.GetItems<IMapItem>())
            {
                ruleContext.MappedItems.AddOrReplaceItem(item);
            }
        }
    }
}