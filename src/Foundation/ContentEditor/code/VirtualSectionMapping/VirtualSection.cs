using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Managers;
using Sitecore.Data.Templates;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentManager;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.VirtualSectionMapping
{
    public class VirtualSection : IVirtualSection
    {
        private string filePath { get; set; }
        private Language language { get; set; }

        public VirtualSection()
        {
        }

        public void Initialize(string path, Language lang)
        {
            filePath = path;
            language = lang;
        }

        public Editor.Section Section()
        {
            var id = new ID();
            var nodes = NodeCollection();
            var title = nodes["title"]?.InnerText.Trim() ?? string.Empty;
            var name = MainUtil.NormalizeItemName(title);
            var iconPath = nodes["iconPath"]?.InnerText.Trim() ?? string.Empty;
            var sortOrder = nodes["sortOrder"]?.InnerText.Trim().ToInt() ?? 0;
            var isCollapsed = nodes["isCollapsed"]?.InnerText.Trim().ToBool() ?? false;

            // Allow the Title to be Translatable by using the Sitecore Dictionary
            title = Translate.TextByLanguage(title, language);

            return new Editor.Section(id, title, iconPath, sortOrder)
            {
                CollapsedByDefault = isCollapsed,
                ControlID = $"virtualSection{id.ToShortID()}",
                DisplayName = title,
                Fields = {},
                Icon = iconPath,
                ID = id,
                Name = name,    
                Sortorder = sortOrder
            };
        }

        public IEnumerable<Editor.Field> FavoriteFields(IEnumerable<Field> fieldCollection)
        {
            var fieldNames = NodeCollection()["fields"]?.InnerText.Trim().Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();

            return fieldCollection.Where(x => CanShowField(x, x.GetTemplateField()) && fieldNames.Any(y => y.Equals(x.Name, StringComparison.InvariantCultureIgnoreCase)))
                  .Select(x => new Editor.Field(x, x.GetTemplateField()));
        }

        private HtmlNodeCollection NodeCollection()
        {
            if (string.IsNullOrEmpty(filePath))
                return null;

            var doc = new HtmlDocument();
            doc.Load(filePath);

            if (doc.ParseErrors.Any())
            {
                Log.Error("Enhance the Editor Experience - Virtual Section Xml Error:", this);

                foreach (var error in doc.ParseErrors)
                    Log.Error($"Line: {error.Line} - Code: {error.Code} - Reason: {error.Reason}", this);
            }

            return doc.DocumentNode.FirstChild.ChildNodes;
        }

        private bool CanShowField(Field field, TemplateField templateField)
        {
            var showHiddenFields = true;
            var fldItm = field.Database.GetItem(templateField.ID, Context.Language);

            if (fldItm != null && fldItm.Appearance.Hidden)
                showHiddenFields = false;

            if (field.Name.Length > 0 && field.CanRead)
                showHiddenFields = true;

            return showHiddenFields;
        }
    }
}