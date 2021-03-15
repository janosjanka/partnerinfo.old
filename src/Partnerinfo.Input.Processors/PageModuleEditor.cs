// Copyright (c) János Janka. All rights reserved.

using System;
using System.Web;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace Partnerinfo.Input.Processors
{
    internal class PageModuleEditor
    {
        private const string ModuleOptionsAttr = "data-module-options";

        public PageModuleEditor(string htmlContent)
        {
            Document = new HtmlDocument();
            Document.LoadHtml(htmlContent);
        }

        public HtmlDocument Document { get; set; }

        /// <summary>
        /// Returns true if the document does not contain elements
        /// </summary>
        public bool IsEmpty
        {
            get { return Document.DocumentNode == null; }
        }

        /// <summary>
        /// Returns true if the given element contains a CSS class with the given name.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="typeClass">The style class.</param>
        /// <returns>
        /// True if the CSS class is specified on the element.
        /// </returns>
        public bool IsTypeOf(HtmlNode element, string typeClass)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            var classNames = element.Attributes["class"]?.Value;
            return classNames != null && classNames.Contains(typeClass);
        }

        /// <summary>
        /// Finds an element with the given ID.
        /// </summary>
        /// <param name="id">The ID of the element to search for.</param>
        /// <returns>
        /// Returns a <see cref="HtmlNode" /> or null.
        /// </returns>
        public HtmlNode GetElementById(string id)
        {
            // Fix: Use XPath because 'Document.GetElementbyId' does not work correctly
            return Document.DocumentNode?.SelectSingleNode(string.Format("//*[@id='{0}']", id));
        }

        /// <summary>
        /// Sets the content of the given module element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="htmlContent">The HTML content to update.</param>
        public void SetModuleContent(HtmlNode element, string htmlContent)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.InnerHtml = htmlContent;
        }

        /// <summary>
        /// Gets module options for the given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        /// A set of key/value pairs that define module options.
        /// </returns>
        public object GetModuleOptions(HtmlNode element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            var moduleOptionsValue = element.GetAttributeValue(ModuleOptionsAttr, null);
            if (string.IsNullOrEmpty(moduleOptionsValue))
            {
                return null;
            }
            moduleOptionsValue = HttpUtility.HtmlDecode(moduleOptionsValue);
            return JsonConvert.DeserializeObject(moduleOptionsValue);
        }

        /// <summary>
        /// Gets module options for the given element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="moduleOptions">The module options.</param>
        /// <returns>
        /// A set of key/value pairs that define module options.
        /// </returns>
        public void SetModuleOptions(HtmlNode element, object moduleOptions)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            string moduleOptionsValue;
            if (moduleOptions == null)
            {
                moduleOptionsValue = null;
            }
            else
            {
                moduleOptionsValue = JsonConvert.SerializeObject(moduleOptions, Formatting.None);
                moduleOptionsValue = HttpUtility.HtmlAttributeEncode(moduleOptionsValue);
            }
            element.SetAttributeValue(ModuleOptionsAttr, moduleOptionsValue);
        }

        /// <summary>
        /// Deletes the given element from DOM.
        /// </summary>
        /// <param name="element">The element to delete.</param>
        public void Delete(HtmlNode element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }
            element.Remove();
        }

        /// <summary>
        /// Gets the object and its content in HTML.
        /// </summary>
        /// <returns>
        /// The HTML content.
        /// </returns>
        public override string ToString()
        {
            return Document.DocumentNode?.OuterHtml ?? string.Empty;
        }
    }
}
