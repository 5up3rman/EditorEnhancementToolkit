using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using EditorEnhancementToolkit.Foundation.ContentEditor.Enum;
using EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping;
using EditorEnhancementToolkit.Foundation.ContentEditor.Rules;
using Sitecore.Diagnostics;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.FileMappings
{
    public class XmlMapItem : IFileMapping
    {
        private string FilePath { get; }
        private MapItemType ItemType { get; }
        
        public XmlMapItem(MapItemType itemType, string fileName)
        { 
            FilePath = FileExtensions.GetMappedFilePath(RulesConstants.MappingsDirectory, fileName);
            ItemType = itemType;
        }

        public IEnumerable<T> GetItems<T>() where T : IFileMapping
        {
            var items = new List<T>();

            if (string.IsNullOrEmpty(FilePath))
                return items;

            var doc = new HtmlDocument();
            doc.Load(FilePath);

            if (doc.ParseErrors.Any())
            {
                Log.Error("Enhance the Editor Experience Xml Error:", this);

                foreach (var error in doc.ParseErrors)
                    Log.Error($"Line: {error.Line} - Code: {error.Code} - Reason: {error.Reason}", this);

                return items;
            }

            var parentNode = doc.DocumentNode.ChildNodes.FirstOrDefault(x => x.Name.Equals("itemMappings", StringComparison.OrdinalIgnoreCase));

            if (parentNode==null ||  !parentNode.ChildNodes.Any())
                return items;

            var itemMappings = GetItemMappings(parentNode.ChildNodes);

            items.AddRange(itemMappings.Select(item => new MapItem
            {
                Type = GetMapItemType(item),
                Title = item.ChildNodes["originalname"]?.InnerText.Trim() ?? string.Empty,
                NewTitle = item.ChildNodes["newtitle"]?.InnerText.Trim() ?? string.Empty,
                ShortDescription = item.ChildNodes["shortdescription"]?.InnerText.Trim() ?? string.Empty,
                Hide = item.ChildNodes["hide"]?.InnerText.Trim().ToBool() ?? false
            }).Where(item => item.Type.Equals(MapItemType.Section) || item.Type.Equals(MapItemType.Field)).Cast<T>());

            return items;
        }

        private IEnumerable<HtmlNode> GetItemMappings(HtmlNodeCollection childNodes)
        {
            if (ItemType.Equals(MapItemType.Section))
                return childNodes?.Where(x => x.Name.Equals(MapItemType.Section.ToString(), StringComparison.InvariantCultureIgnoreCase));

            if (ItemType.Equals(MapItemType.Field))
                return childNodes?.Where(x => x.Name.Equals(MapItemType.Field.ToString(), StringComparison.InvariantCultureIgnoreCase));

            return childNodes;
        }

        private MapItemType GetMapItemType(HtmlNode node)
        {
            var type = node.Name.ToLower();

            if (type.Equals(MapItemType.Section.ToString().ToLower()))
                return MapItemType.Section;

            if (type.Equals(MapItemType.Field.ToString().ToLower()))
                return MapItemType.Field;

            return MapItemType.All;
        }
    }
}