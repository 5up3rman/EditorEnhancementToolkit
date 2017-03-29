using System.Collections.Generic;
using System.Linq;
using EditorEnhancementToolkit.Foundation.ContentEditor.Rules;
using Sitecore;
using Sitecore.Collections;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.SecurityModel;
using Sitecore.Shell.Applications.ContentEditor.Pipelines.GetContentEditorFields;
using Sitecore.Shell.Applications.ContentManager;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Pipelines.GetContentEditorFields
{
    public class GetFields
    {
        public bool ShowDataFieldsOnly { get; private set; }
        public bool ShowHiddenFields { get; set; }

        public void Process(GetContentEditorFieldsArgs args)
        {
            if (args.Item == null)
                return;

            ShowDataFieldsOnly = args.ShowDataFieldsOnly;
            ShowHiddenFields = args.ShowHiddenFields;
            GetSections(args);
        }

        private static FieldCollection GetFieldCollection(Item item)
        {
            var fields = item.Fields;
            fields.ReadAll();
            fields.Sort();

            return fields;
        }

        private void GetSections(GetContentEditorFieldsArgs args)
        {
            var item = args.Item;
            var sections = args.Sections;
            var fieldCollection = GetFieldCollection(item).ToList();
            var favoriteFields = new List<Editor.Field>();

            if (ShowDataFieldsOnly)
            {
                var rules = (ContentEditorRulesProcessor) Factory.CreateObject("editorEnhancedToolkit/contentEditorRulesProcessor", false);
                rules.ProcessRules(item);

                var virtualSection = rules.VirtualSection;
                var section = virtualSection?.Section();
                favoriteFields = virtualSection?.FavoriteFields(fieldCollection).ToList() ?? new List<Editor.Field>();

                if (section != null && favoriteFields.Any())
                {
                    sections.Add(section);

                    foreach (var field in favoriteFields)
                    {
                        section.Fields.Add(field);
                        args.AddFieldInfo(field.ItemField, field.ControlID);
                    }
                }
            }
            foreach (var field in fieldCollection)
            {
                if (favoriteFields.Any(x => x.ItemField.ID == field.ID))
                    continue;

                var templateField = field.GetTemplateField();

                if (templateField != null && CanShowField(field, templateField))
                {
                    var templateFieldSection = templateField.Section;

                    if (templateFieldSection == null)
                        continue;

                    var newSection = sections[templateFieldSection.Name];

                    if (newSection == null)
                    {
                        newSection = new Editor.Section(templateFieldSection);

                        sections.Add(newSection);

                        using (new SecurityDisabler())
                        {
                            var obj2 = item.RuntimeSettings.TemplateDatabase.GetItem(templateFieldSection.ID, Context.Language);
                            if (obj2 != null)
                            {
                                newSection.CollapsedByDefault = obj2["Collapsed by Default"] == "1";
                                newSection.DisplayName = obj2.DisplayName;
                            }
                        }
                    }

                    Editor.Field field2 = field.ID == FieldIDs.WorkflowState || field.ID == FieldIDs.Workflow ? new Editor.Field(field, templateField, field.GetValue(false, false) ?? string.Empty) : new Editor.Field(field, templateField);
                    newSection.Fields.Add(field2);

                    args.AddFieldInfo(field, field2.ControlID);
                }
            }

            sections.SortSections();
        }

        private bool CanShowField(Field field, TemplateField templateField)
        {
            var flag1 = true;
            var flag2 = true;

            if (ShowDataFieldsOnly)
                flag1 = ItemUtil.IsDataField(templateField);

            if (!ShowHiddenFields)
            {
                var itm = field.Database.GetItem(templateField.ID, Context.Language);

                if (itm != null && itm.Appearance.Hidden)
                    flag2 = false;
            }

            if (field.Name.Length > 0 && field.CanRead && (!ShowDataFieldsOnly || flag1))
                return flag2;

            return false;
        }
    }
}