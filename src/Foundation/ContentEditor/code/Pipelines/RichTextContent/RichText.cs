﻿using System;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Pipelines.RichTextContent
{
    public static class RichText
    {
        public static string Content(string content)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            if (!doc.DocumentNode.Descendants("img").Any())
                return content;

            foreach (var img in doc.DocumentNode.Descendants("img"))
            {
                var imgAttr = img.Attributes["style"];

                if (imgAttr == null)
                    continue;

                var alteredStyles = new StringBuilder();
                var inlineStyleList = imgAttr.Value.Split(';');

                if(!inlineStyleList.Any())
                    continue;

                foreach (var style in inlineStyleList)
                {
                    var attrList = style.Split(':');

                    if (!attrList.Count().Equals(2))
                        continue;

                    var property = attrList[0].Trim();
                    var val = attrList[1].Trim();

                    if (property.Equals("width", StringComparison.InvariantCultureIgnoreCase) || property.Equals("height", StringComparison.InvariantCultureIgnoreCase))
                        img.Attributes.Add(property, val);
                    else
                        alteredStyles.Append($"{property}:{val}; ");
                }

                if (alteredStyles.Length > 0)
                    img.Attributes["style"].Value = alteredStyles.ToString();
                else
                    img.Attributes.Remove("style");
            }

            return doc.DocumentNode.OuterHtml;
        }
    }
}
