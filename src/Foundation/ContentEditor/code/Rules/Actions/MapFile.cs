using EditorEnhancementToolkit.Foundation.ContentEditor.Enum;
using EditorEnhancementToolkit.Foundation.ContentEditor.FileMappings;
using Sitecore.Rules.Actions;
using EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping;


namespace EditorEnhancementToolkit.Foundation.ContentEditor.Rules.Actions
{
    public class MapFile<T> : RuleAction<T> where T : ContentEditorRuleContext
    {
        public string FileName { get; set; }

        public override void Apply(T ruleContext)
        {
            var mappedFileItems = new XmlMapItem(MapItemType.All, FileName);

            foreach (var item in mappedFileItems.GetItems<IMapItem>())
            {
                ruleContext.MappedItems.AddOrReplaceItem(item);
            }
        }
    }
}