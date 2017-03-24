using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using EditorEnhancementToolkit.Foundation.ContentEditor.Enum;
using EditorEnhancementToolkit.Foundation.ContentEditor.ItemMapping;
using EditorEnhancementToolkit.Foundation.ContentEditor.Rules;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Reflection;
using Sitecore.Resources;
using Sitecore.Security;
using Sitecore.Shell;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Shell.Applications.ContentEditor.Pipelines.RenderContentEditor;
using Sitecore.Shell.Applications.ContentManager;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI;
using Sitecore.Web.UI.HtmlControls;
using Memo = Sitecore.Shell.Applications.ContentEditor.Memo;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Shell.Applications.ContentEditor
{
    /// <summary>
    /// Formatter for rendering the editor
    /// 
    /// </summary>
    public class EditorFormatter
    {
        public RenderContentEditorArgs Arguments { get; set; }
        public static List<IMapItem> MappedItems { get; set; } = new List<IMapItem>();

        public EditorFormatter(RenderContentEditorArgs args)
        {
            Arguments = args;

            // We want to make sure the Item is under the Content Node and that the item is not null. 
            if (args.Item == null || !args.Item.Paths.IsContentItem)
                return;
            
            // Run the rules and set it to MappedItems
            var rules = (ContentEditorRulesProcessor)Factory.CreateObject("editorEnhancedToolkit/contentEditorRulesProcessor", false);
            rules.ProcessRules(args.Item);

            MappedItems = rules.MappedItems;
        }

        #region Customizations

        public void RenderSection(Language language, Editor.Section section, System.Web.UI.Control parent, bool readOnly)
        {
            // Get the MappedItem that is a section and matches this Sections Name.
            var mappedSection =
                MappedItems.FirstOrDefault(x => x.Type.Equals(MapItemType.Section) && x.Title.Equals(section.Name));

            // Check to see if this Section is Hidden
            if (mappedSection != null && mappedSection.Hide)
                return;

            // Get the Customized Section Name from the MappedSection if it is not null, if null
            var sectionName = !string.IsNullOrWhiteSpace(mappedSection?.NewTitle) ? Translate.TextByLanguage(mappedSection.NewTitle, language) : Translate.TextByLanguage(section.DisplayName, language);
            var sectionCollapsed = section.IsSectionCollapsed;
            var renderFields = !sectionCollapsed || UserOptions.ContentEditor.RenderCollapsedSections;
            var customInlineStyles = mappedSection?.CustomInlineStyle.Split('|').ToList() ?? new List<string>();
            string sectionTitleBar = string.Empty, sectionOuterPanel = string.Empty, sectionInnerPanel = string.Empty;

            if (customInlineStyles.Count == 3)
            {
                sectionTitleBar = customInlineStyles[0];
                sectionOuterPanel = customInlineStyles[1];
                sectionInnerPanel = customInlineStyles[2];
            }
             
            RenderSectionBegin(parent, section.ControlID, section.Name, sectionName, section.Icon, sectionCollapsed, renderFields, sectionTitleBar, sectionOuterPanel, sectionInnerPanel);

            if (renderFields)
            {
                foreach (var field in section.Fields)
                {
                    RenderField(parent, field, readOnly);
                }
            }

            RenderSectionEnd(parent, renderFields, sectionCollapsed);
        }

        public void RenderField(System.Web.UI.Control parent, Editor.Field field, bool readOnly)
        {
            var itemField = field.ItemField;
            var fieldType = GetFieldType(itemField);

            // Get the Mapped Field
            var mappedField =
                MappedItems.FirstOrDefault(x => x.Type.Equals(MapItemType.Field) && x.Title.Equals(field.ItemField.Name));

            // Determine if the Field is Hidden by the Rules
            if (mappedField != null && mappedField.Hide)
                return;

            if (!itemField.CanWrite)
                readOnly = true;

            RenderMarkerBegin(parent, field.ControlID);
            var typeKey = itemField.TypeKey;

            if (!string.IsNullOrEmpty(typeKey) && typeKey.Equals("checkbox") && !UserOptions.ContentEditor.ShowRawValues)
            {
                RenderField(parent, field, fieldType, readOnly);
                RenderLabel(parent, field, fieldType, mappedField, readOnly);
                RenderMenuButtons(parent, field, fieldType, readOnly);
            }
            else
            {
                RenderLabel(parent, field, fieldType, mappedField, readOnly);
                RenderMenuButtons(parent, field, fieldType, readOnly);
                RenderField(parent, field, fieldType, readOnly);
            }

            RenderMarkerEnd(parent);
        }

        public void RenderLabel(System.Web.UI.Control parent, Editor.Field field, Item fieldType, IMapItem mappedField, bool readOnly)
        {
            var itemField = field.ItemField;
            var language = itemField.Language;
            var typeKey = itemField.TypeKey;
            var helpLink = HttpUtility.HtmlAttributeEncode(itemField.HelpLink);
            var description = string.Empty;
            var toolTip = !string.IsNullOrEmpty(mappedField?.ShortDescription) ? mappedField?.ShortDescription : itemField.ToolTip;
            var labelStyle = !string.IsNullOrEmpty(mappedField?.CustomInlineStyle) ? $" style=\"{mappedField.CustomInlineStyle}\" " : string.Empty;
            var fieldTitle = !string.IsNullOrEmpty(mappedField?.NewTitle) ? Translate.TextByLanguage(mappedField.NewTitle, language) : field.TemplateField.GetTitle(language);

            if (string.IsNullOrEmpty(fieldTitle))
                fieldTitle = field.TemplateField.IgnoreDictionaryTranslations ? itemField.Name : Translate.Text(itemField.Name);

            if (!string.IsNullOrEmpty(mappedField?.NewTitle))
                fieldTitle = !EditorFormatterConstants.ShowOriginalFieldToAdmins ? fieldTitle : $"{fieldTitle} <b>[{mappedField.Title}]</b>";

            if (!string.IsNullOrEmpty(toolTip))
            {
                var text = Translate.Text(toolTip);

                if (text.EndsWith(".", StringComparison.InvariantCulture))
                    text = StringUtil.Left(text, text.Length - 1);

                fieldTitle = $"{fieldTitle} {(!string.IsNullOrWhiteSpace(text) ? string.Concat("- ", text) : string.Empty)}";
            }

            //fieldTitle = HttpUtility.HtmlEncode(fieldTitle);

            var label = field.ItemField.GetLabel(Arguments.IsAdministrator || Settings.ContentEditor.ShowFieldSharingLabels);

            if (!string.IsNullOrEmpty(label))
                fieldTitle = $"{fieldTitle} <span class=\"scEditorFieldLabelAdministrator\">[{label}]</span>";

            if (!string.IsNullOrEmpty(typeKey) && !typeKey.Equals("checkbox"))
                fieldTitle += ":";

            if (readOnly)
                fieldTitle = $"<span class=\"scEditorFieldLabelDisabled\">{fieldTitle}</span>";

            if (!string.IsNullOrWhiteSpace(helpLink))
                fieldTitle = $"<a class=\"scEditorFieldLabelLink\" href=\"{helpLink}\" target=\"__help\">{fieldTitle}</a>";

            if (itemField.Description.Length > 0)
                description = $" title=\"{HttpUtility.HtmlAttributeEncode(itemField.Description)}\"";

            var fieldLabel = $"<div class=\"scEditorFieldLabel\"{description} {labelStyle}>{fieldTitle}</div>";

            AddLiteralControl(parent, fieldLabel);
        }

        public void RenderSectionBegin(System.Web.UI.Control parent, string controlId, string sectionName, string displayName, string icon, bool isCollapsed, bool renderFields, string sectionTitleBarStyle = "", string sectionOuterPanelStyle = "", string sectionInnerPanelStyle = "")
        {
            var htmlTextWriter = new HtmlTextWriter(new StringWriter(new StringBuilder(1024)));

            if (Arguments.ShowSections)
            {
                var collapsedCss = isCollapsed ? "scEditorSectionCaptionCollapsed" : "scEditorSectionCaptionExpanded";
                var sectionJs = UserOptions.ContentEditor.RenderCollapsedSections ? $"javascript:scContent.toggleSection('{controlId}','{sectionName}')" : $"javascript:return scForm.postRequest('','','','{StringUtil.EscapeQuote($"ToggleSection(\"{sectionName}\",\"{(isCollapsed ? "1" : "0")}\")")}')";
                var titleBarStyle = !string.IsNullOrWhiteSpace(sectionTitleBarStyle) ? $" style=\"{sectionTitleBarStyle}\"" : string.Empty;
                var sectionHeader = $"<div id=\"{controlId}\" class=\"{collapsedCss}\"{titleBarStyle} onclick=\"{sectionJs}\">";
                var imageBuilder1 = new ImageBuilder
                {
                    ID = controlId + "_Glyph",
                    Src = isCollapsed ? "Images/accordion_down.png" : "Images/accordion_up.png",
                    Class = "scEditorSectionCaptionGlyph"
                };
                var imageBuilder2 = new ImageBuilder
                {
                    Src = Images.GetThemedImageSource(icon, ImageDimension.id16x16),
                    Class = "scEditorSectionCaptionIcon",
                    Attributes = { {"style", "margin-right:16px;"} }
                };

                htmlTextWriter.Write(sectionHeader);
                htmlTextWriter.Write(imageBuilder1.ToString());

                if(controlId.Contains("virtualSection"))
                    htmlTextWriter.Write(imageBuilder2.ToString());

                htmlTextWriter.Write(displayName);
                htmlTextWriter.Write("</div>");
            }

            if (renderFields || !isCollapsed)
            {
                // Set the Title Bar and Section Panel's custom Inline Style
                var outerPanelStyle = !string.IsNullOrWhiteSpace(sectionOuterPanelStyle) ? $" style=\"{sectionOuterPanelStyle}\"" : string.Empty;
                var innerPanelStyle = !string.IsNullOrWhiteSpace(sectionInnerPanelStyle) ? $" style=\"{sectionInnerPanelStyle}\"" : string.Empty;
                var displaySection = !isCollapsed || !Arguments.ShowSections ? string.Empty : " style=\"display:none\"";
                var sectionPanelCssClass = Arguments.ShowSections ? "scEditorSectionPanelCell" : "scEditorSectionPanelCell_NoSections";
                var sectionPanel = $"<table width=\"100%\" class=\"scEditorSectionPanel\"{displaySection}{outerPanelStyle}><tr><td class=\"{sectionPanelCssClass}\"{innerPanelStyle}>";

                htmlTextWriter.Write(sectionPanel);
            }

            AddLiteralControl(parent, htmlTextWriter.InnerWriter.ToString());
        }

        public static void SetProperties(System.Web.UI.Control editor, Editor.Field field, bool readOnly)
        {
            // Get the Mapped Field
            var mappedField =
                MappedItems.FirstOrDefault(x => x.Type.Equals(MapItemType.Field) && x.Title.Equals(field.ItemField.Name) && !string.IsNullOrEmpty(x.FieldSource));

            ReflectionUtil.SetProperty(editor, "ID", field.ControlID);
            ReflectionUtil.SetProperty(editor, "ItemID", field.ItemField.Item.ID.ToString());
            ReflectionUtil.SetProperty(editor, "ItemVersion", field.ItemField.Item.Version.ToString());
            ReflectionUtil.SetProperty(editor, "ItemLanguage", field.ItemField.Item.Language.ToString());
            ReflectionUtil.SetProperty(editor, "FieldID", field.ItemField.ID.ToString());
            ReflectionUtil.SetProperty(editor, "Source", string.IsNullOrEmpty(mappedField?.FieldSource) ? field.ItemField.Source : mappedField.FieldSource);
            ReflectionUtil.SetProperty(editor, "ReadOnly", readOnly ? 1 : 0);
            ReflectionUtil.SetProperty(editor, "Disabled", readOnly ? 1 : 0);
        }

        #endregion Customizations

        public static void SetAttributes(System.Web.UI.Control editor, Editor.Field field, bool hasRibbon)
        {
            var attributeCollection = ReflectionUtil.GetProperty(editor, "Attributes") as AttributeCollection;

            if (attributeCollection == null)
                return;

            var str = field.ItemField.Item.Uri + "&fld=" + field.ItemField.ID + "&ctl=" + field.ControlID;

            if (hasRibbon)
                str += "&rib=1";

            attributeCollection["onfocus"] = "javascript:return scContent.activate(this,event,'" + str + "')";
            attributeCollection["onblur"] = "javascript:return scContent.deactivate(this,event,'" + str + "')";
        }

        public static void SetStyle(System.Web.UI.Control editor, Editor.Field field)
        {
            if (string.IsNullOrEmpty(field.ItemField.Style))
                return;

            var css = ReflectionUtil.GetProperty(editor, "Style") as CssStyleCollection;

            if (css == null)
                return;

            UIUtil.ParseStyle(css, field.ItemField.Style);
        }

        public void AddEditorControl(System.Web.UI.Control parent, System.Web.UI.Control editor, Editor.Field field,
            bool hasRibbon, bool readOnly, string value)
        {
            SetProperties(editor, field, readOnly);
            this.SetValue(editor, value);

            var editorFieldContainer = new EditorFieldContainer(editor);
            editorFieldContainer.ID = field.ControlID + "_container";
            var control = (System.Web.UI.Control) editorFieldContainer;

            Context.ClientPage.AddControl(parent, control);
            SetProperties(editor, field, readOnly);
            SetAttributes(editor, field, hasRibbon);
            SetStyle(editor, field);

            this.SetValue(editor, value);
        }

        public void AddLiteralControl(System.Web.UI.Control parent, string text)
        {
            if (parent.Controls.Count > 0)
            {
                var literalControl = parent.Controls[parent.Controls.Count - 1] as LiteralControl;
                if (literalControl != null)
                {
                    literalControl.Text += text;
                    return;
                }
            }

            Context.ClientPage.AddControl(parent, new LiteralControl(text));
        }

        public System.Web.UI.Control GetEditor(Item fieldType)
        {
            if (!this.Arguments.ShowInputBoxes)
            {
                var str = fieldType.Name.ToLowerInvariant();
                if (str == "html" || str == "memo" || (str == "rich text" || str == "security") ||
                    str == "multi-line text")
                    return new Memo();

                return fieldType.Name == "password" ? new Password() : new Text();
            }

            var control = Resource.GetWebControl(fieldType["Control"]);

            if (control == null)
            {
                var assembly = fieldType["Assembly"];
                var className = fieldType["Class"];

                if (!string.IsNullOrEmpty(assembly) && !string.IsNullOrEmpty(className))
                    control = ReflectionUtil.CreateObject(assembly, className, new object[0]) as System.Web.UI.Control;
            }

            return control ?? new Text();
        }

        public Item GetFieldType(Sitecore.Data.Fields.Field itemField)
        {
            Assert.ArgumentNotNull(itemField, "itemField");
            return FieldTypeManager.GetFieldTypeItem(StringUtil.GetString(new string[2]
            {
                itemField.Type,
                "text"
            })) ?? FieldTypeManager.GetDefaultFieldTypeItem();
        }

        public virtual void RenderField(System.Web.UI.Control parent, Editor.Field field, Item fieldType, bool readOnly)
        {
            var blobVal = string.Empty;

            if (field.ItemField.IsBlobField && !this.Arguments.ShowInputBoxes)
            {
                readOnly = true;
                blobVal = Translate.Text("[Blob Value]");
            }
            else
                blobVal = field.Value;

            this.RenderField(parent, field, fieldType, readOnly, blobVal);
        }

        public virtual void RenderField(System.Web.UI.Control parent, Editor.Field field, Item fieldType, bool readOnly,
            string value)
        {
            var hasRibbon = false;
            var text = string.Empty;
            var editor = this.GetEditor(fieldType);

            if (this.Arguments.ShowInputBoxes)
            {
                hasRibbon = !UserOptions.ContentEditor.ShowRawValues && fieldType.Children["Ribbon"] != null;
                var typeKey = field.TemplateField.TypeKey;

                if (typeKey == "rich text" || typeKey == "html")
                    hasRibbon = hasRibbon &&
                                UserOptions.HtmlEditor.ContentEditorMode != UserOptions.HtmlEditor.Mode.Preview;
            }

            var str1 = string.Empty;
            var str2 = string.Empty;
            var @int = Registry.GetInt("/Current_User/Content Editor/Field Size/" + field.TemplateField.ID.ToShortID(),
                -1);
            if (@int != -1)
            {
                str1 = $" height:{@int}px";
                var control = editor as Sitecore.Web.UI.HtmlControls.Control;

                if (control != null)
                {
                    control.Height = new Unit((double) @int, UnitType.Pixel);
                }
                else
                {
                    Sitecore.Web.UI.WebControl webControl = editor as Sitecore.Web.UI.WebControl;

                    if (webControl != null)
                        webControl.Height = new Unit((double) @int, UnitType.Pixel);
                }
            }
            else if (editor is Frame)
            {
                var style = field.ItemField.Style;
                if (string.IsNullOrEmpty(style) || !style.ToLowerInvariant().Contains("height"))
                    str2 = " class='defaultFieldEditorsFrameContainer'";
            }
            else if (editor is MultilistEx)
            {
                var style = field.ItemField.Style;
                if (string.IsNullOrEmpty(style) || !style.ToLowerInvariant().Contains("height"))
                    str2 = " class='defaultFieldEditorsMultilistContainer'";
            }
            else
            {
                var typeKey = field.ItemField.TypeKey;
                if (!string.IsNullOrEmpty(typeKey) && typeKey.Equals("checkbox") &&
                    !UserOptions.ContentEditor.ShowRawValues)
                    str2 = "class='scCheckBox'";
            }

            this.AddLiteralControl(parent, "<div style='" + str1 + "' " + str2 + ">");
            this.AddLiteralControl(parent, text);
            this.AddEditorControl(parent, editor, field, hasRibbon, readOnly, value);
            this.AddLiteralControl(parent, "</div>");
            this.RenderResizable(parent, field);
        }

        public void RenderMarkerBegin(System.Web.UI.Control parent, string fieldControlID)
        {
            var text =
                "<table width=\"100%\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" class=\"scEditorFieldMarker\"><tr><td id=\"FieldMarker" +
                fieldControlID +
                "\" class=\"scEditorFieldMarkerBarCell\"><img src=\"/sitecore/images/blank.gif\" width=\"4px\" height=\"1px\" /></td><td class=\"scEditorFieldMarkerInputCell\">";

            this.AddLiteralControl(parent, text);
        }

        public void RenderMarkerEnd(System.Web.UI.Control parent)
        {
            this.AddLiteralControl(parent, "</td></tr></table>");
        }

        public virtual void RenderMenuButtons(System.Web.UI.Control parent, Editor.Field field, Item fieldType,
            bool readOnly)
        {
            if (!this.Arguments.ShowInputBoxes || UserOptions.ContentEditor.ShowRawValues)
                return;

            Item menu = fieldType.Children["Menu"];

            if (menu == null || !menu.HasChildren)
                return;

            this.AddLiteralControl(parent, this.RenderMenuButtons(field, menu, readOnly));
        }
        
        public void RenderSectionEnd(System.Web.UI.Control parent, bool renderFields)
        {
            this.RenderSectionEnd(parent, renderFields, false);
        }

        public void RenderSectionEnd(System.Web.UI.Control parent, bool renderFields, bool isCollapsed)
        {
            if (!renderFields && isCollapsed)
                return;

            this.AddLiteralControl(parent, "</td></tr></table>");
        }

        public void RenderSections(Language language, System.Web.UI.Control parent, Editor.Sections sections, bool readOnly)
        {
            Context.ClientPage.ClientResponse.DisableOutput();
            this.AddLiteralControl(parent, "<div class=\"scEditorSections\">");

            for (int index = 0; index < sections.Count; ++index)
                this.RenderSection(language, sections[index], parent, readOnly);

            this.AddLiteralControl(parent, "</div>");
            Context.ClientPage.ClientResponse.EnableOutput();
        }

        public void SetValue(System.Web.UI.Control editor, string value)
        {
            if (editor is IStreamedContentField)
                return;

            IContentField contentField = editor as IContentField;

            if (contentField != null)
                contentField.SetValue(value);
            else
                ReflectionUtil.SetProperty(editor, "Value", value);
        }

        public bool IsFieldEditor { get; set; }

        private string RenderMenuButtons(Editor.Field field, Item menu, bool readOnly)
        {
            HtmlTextWriter htmlTextWriter = new HtmlTextWriter((TextWriter) new StringWriter());
            htmlTextWriter.Write("<div class=\"scContentButtons\">");
            bool flag1 = true;

            foreach (Item button in menu.Children)
            {
                if (!this.IsFieldEditor || MainUtil.GetBool(button["Show In Field Editor"], false))
                {
                    string str1 = button["Message"];
                    bool flag2 = false;
                    bool flag3 = false;
                    Command command = CommandManager.GetCommand(str1);
                    if (command != null)
                    {
                        CommandState commandState = CommandManager.QueryState(command,
                            new CommandContext(this.Arguments.Item));
                        flag2 = commandState == CommandState.Hidden;
                        flag3 = commandState == CommandState.Disabled;
                    }
                    if (!flag2)
                    {
                        if (!flag1)
                            htmlTextWriter.Write("");
                        flag1 = false;
                        var str2 = Context.ClientPage.GetClientEvent(str1).Replace("$Target", field.ControlID);
                        if (readOnly || this.DisableAssignSecurityButton(button) || flag3)
                        {
                            htmlTextWriter.Write("<span class=\"scContentButtonDisabled\">");
                            htmlTextWriter.Write(button["Display Name"]);
                            htmlTextWriter.Write("</span>");
                        }
                        else
                        {
                            htmlTextWriter.Write("<a href=\"#\" class=\"scContentButton\" onclick=\"" + str2 + "\">");
                            htmlTextWriter.Write(button["Display Name"]);
                            htmlTextWriter.Write("</a>");
                        }
                    }
                }
            }

            htmlTextWriter.Write("</div>");
            return htmlTextWriter.InnerWriter.ToString();
        }

        private void RenderResizable(System.Web.UI.Control parent, Editor.Field field)
        {
            var type = field.TemplateField.Type;
            if (string.IsNullOrEmpty(type))
                return;

            FieldType fieldType = FieldTypeManager.GetFieldType(type);
            if (fieldType == null || !fieldType.Resizable)
                return;

            var text1 =
                "<div style=\"cursor:row-resize; position: relative; height: 5px; width: 100%; top: 0px; left: 0px;\" onmousedown=\"scContent.fieldResizeDown(this, event)\" onmousemove=\"scContent.fieldResizeMove(this, event)\" onmouseup=\"scContent.fieldResizeUp(this, event, '" +
                field.TemplateField.ID.ToShortID() + "')\">" + Images.GetSpacer(1, 4) + "</div>";
            this.AddLiteralControl(parent, text1);
            var text2 = "<div class style=\"display:none\" \">" + Images.GetSpacer(1, 4) + "</div>";
            this.AddLiteralControl(parent, text2);
        }

        private bool DisableAssignSecurityButton(Item button)
        {
            bool flag = button.ID == FieldButtonIDs.AssignSecurity &&
                        (!SecurityHelper.CanRunApplication("Content Editor/Ribbons/Chunks/Security/Assign") ||
                         !SecurityHelper.CanRunApplication("Security/Item Security Editor"));

            return flag;
        }
    }
}