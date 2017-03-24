﻿using System;
using System.Linq;
using EditorEnhancementToolkit.Foundation.ContentEditor.Enum;
using EditorEnhancementToolkit.Foundation.ContentEditor.Rules;
using Sitecore.Data.Fields;
using Sitecore.Pipelines.GetChromeData;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Pipelines.GetChromeData
{
    /// <summary>
    /// Displays the Vanity Field Name in the Experience Editor in place of the Field Name or Field Title
    /// </summary>
    public class InitializeChromeCustomFieldDisplayName : Sitecore.ExperienceEditor.Pipelines.GetChromeData.InitializeChromeFieldDisplayName
    {
        public override void Process(GetChromeDataArgs args)
        {
            if (!args.ChromeType.Equals("field", StringComparison.OrdinalIgnoreCase))
                return;

            var field = args.CustomData["field"] as Field;

            if (args.Item != null)
            {
                var rules = new ContentEditorRulesProcessor(args.Item);
                var fld = rules.MappedItems.FirstOrDefault(x => x.Type.Equals(MapItemType.Field) && x.Title.Equals(field?.Name) && !string.IsNullOrWhiteSpace(x.NewTitle));

                if (fld != null)
                {
                    args.ChromeData.DisplayName = fld.NewTitle;
                    return;
                }
            }

            var containsFallbackValue = field?.ContainsFallbackValue ?? false;

            if (containsFallbackValue)
                return;

            if (!string.IsNullOrEmpty(field?.GetLabel(false)))
                args.ChromeData.DisplayName += GetFieldDisplayName(field);
        }
    }
}