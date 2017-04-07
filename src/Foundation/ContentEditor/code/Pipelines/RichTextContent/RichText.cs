using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using HtmlAgilityPack;

namespace EditorEnhancementToolkit.Foundation.ContentEditor.Pipelines.RichTextContent
{
    //TODO: Discuss whether to add option to completely remove all dimensions both inline and regular attribute
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
                var inlineStyleList = imgAttr.Value.Split(';').Where(x => x.Contains("width") || x.Contains("height"));

                if(!inlineStyleList.Any())
                    continue;

                foreach (var style in inlineStyleList)
                {
                    var attrList = style.Split(':');

                    if (!attrList.Count().Equals(2))
                        continue;

                    var property = attrList[0].Trim();
                    var val = attrList[1].Trim();

                    //if (!RemoveImageDimensions() && (property.Equals("width", StringComparison.InvariantCultureIgnoreCase) || property.Equals("height", StringComparison.InvariantCultureIgnoreCase)))
                    if (property.Equals("width", StringComparison.InvariantCultureIgnoreCase) || property.Equals("height", StringComparison.InvariantCultureIgnoreCase))
                        img.Attributes.Add(property, val);
                    else
                        alteredStyles.Append($"{property}:{val}; ");
                }

                // If the inline style attr contains other styles than the dimensions place those back in
                if (alteredStyles.Length > 0)
                    img.Attributes["style"].Value = alteredStyles.ToString();
                else
                    img.Attributes.Remove("style");
            }

            return doc.DocumentNode.OuterHtml;
        }

        //private static HtmlNode ImageDimensions(HtmlNode imgNode)
        //{
        //    if(!RemoveImageDimensions())
        //        return imgNode;
            
        //    var width = imgNode.Attributes["width"]?.Value;
        //    var height = imgNode.Attributes["height"]?.Value;

        //    if(!string.IsNullOrEmpty(width))
        //        imgNode.Attributes["width"].Remove();

        //    if(!string.IsNullOrEmpty(height))
        //        imgNode.Attributes["height"].Remove();

        //    return imgNode;
        //}

        //private static bool RemoveImageDimensions()
        //{
        //    return Settings.GetBoolSetting("EnhanceTheEditorExperience.RichTextEditor.RemoveDimensions", false);
        //}
    }
}
