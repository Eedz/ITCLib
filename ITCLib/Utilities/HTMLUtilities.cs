using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace ITCLib
{
    public static class HTMLUtilities
    {
        public static string RemoveHtmlTag(string html, string tagName)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            var nodesToRemove = doc.DocumentNode.Descendants(tagName).ToArray();
            foreach (var node in nodesToRemove)
            {
                // Preserve the inner text and replace the node with its children
                string innerText = node.InnerHtml;
                HtmlNode[] children = node.ChildNodes.ToArray();
                foreach (var child in children)
                {
                    node.ParentNode.InsertBefore(child, node);
                }
                node.Remove();
                // Add a space between the preserved text if it's adjacent to other text
                if (!string.IsNullOrWhiteSpace(innerText))
                {
                    HtmlNode prevSibling = node.PreviousSibling;
                    if (prevSibling != null && prevSibling.NodeType == HtmlNodeType.Text)
                    {
                        prevSibling.InnerHtml += " ";
                    }
                    HtmlNode nextSibling = node.NextSibling;
                    if (nextSibling != null && nextSibling.NodeType == HtmlNodeType.Text)
                    {
                        nextSibling.InnerHtml = " " + nextSibling.InnerHtml;
                    }
                }
            }

            return doc.DocumentNode.OuterHtml;
        }

        public static string RemoveStyleAttribute(string html, string tagName)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Select all elements of the specified tag name with a style attribute
            var elementsWithStyle = doc.DocumentNode.SelectNodes($"//{tagName}[@style]");

            // Loop through each element and remove the style attribute
            if (elementsWithStyle != null)
            {
                foreach (var element in elementsWithStyle)
                {
                    // Remove the style attribute
                    element.Attributes.Remove("style");
                }
            }

            return doc.DocumentNode.OuterHtml;
        }

        public static string RemoveEmptyParagraphsWithBr(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Select all <p> elements containing only one <br> child node
            var paragraphsToRemove = doc.DocumentNode.SelectNodes("//p[count(br) = 1 and count(*) = 1]");

            if (paragraphsToRemove != null)
            {
                foreach (var paragraph in paragraphsToRemove)
                {
                    // Replace the <p> tag with its child <br> node
                    var brNode = paragraph.SelectSingleNode("br");
                    paragraph.ParentNode.ReplaceChild(brNode, paragraph);
                }
            }

            return doc.DocumentNode.OuterHtml;
        }
    }
}
