using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Caching;

using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.Publishing;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Assembly;
using Tridion.ContentManager.Templating.Templates;

namespace XView
{
    /// <summary>
    /// Represents the rendering context of a ComponentTemplate or PageTemplate. This class defines 
    /// the basic attributes and behaviours that are applicable to all Tridion implementations. 
    /// Extend this class to add implementation specific properties and methods.
    /// </summary>
    public class TridionContext : IDisposable
    {
        private ICache cache;
        private Publication publication;
        private Template template;

        /// <summary>
        /// Default ctor.
        /// </summary>
        public TridionContext()
        {
        }

        /// <summary>
        /// TridionContext constructor accepting an <see cref="Engine"/> and a <see cref="Package"/>.
        /// </summary>
        /// <param name="engine"><see cref="Engine"/> object.</param>
        /// <param name="package"><see cref="Package"/> object.</param>
        public TridionContext(Engine engine, Package package)
        {
            this.InitializeContext(engine, package);
        }

        /// <summary>
        /// Gets the <see cref="IDictionary{String, Object}"/> containing data shared between static templates on the same page.
        /// </summary>
        public IDictionary<string, object> PageData { get; private set; }

        /// <summary>
        /// Gets <see cref="Engine"/> object.
        /// </summary>
        public Engine Engine { get; private set; }

        /// <summary>
        /// Gets the context <see cref="Component"/>.
        /// </summary>
        public Component Component { get; private set; }

        /// <summary>
        /// Gets the context <see cref="Page"/>.
        /// </summary>
        public Page Page { get; private set; }

        /// <summary>
        /// Gets the context Publication.
        /// </summary>
        public Publication Publication
        {
            get
            {
                return this.publication ??
                       (this.publication =
                           (Publication)(this.Component != null ? this.Component.ContextRepository : this.Page.ContextRepository));
            }
        }

        /// <summary>
        /// Gets the context <see cref="Template"/>.
        /// </summary>
        public Template Template
        {
            get { return this.template ?? (this.template = this.Engine.PublishingContext.ResolvedItem.Template); }
        }

        /// <summary>
        /// Is the current <see cref="RenderMode"/> equals <see cref="RenderMode.Publish"/>?
        /// </summary>
        public bool IsPublishing
        {
            get { return this.Engine.RenderMode == RenderMode.Publish; }
        }

        /// <summary>
        /// Gets a <see cref="DefaultMemoryCache"/> object representing .NET Runtime <see cref="MemoryCache.Default"/> cache object.
        /// IMPORTANT NOTE: This is not distributed cache. In scenarios of multiple publishers cache is 
        /// not shared among the different publishers.
        /// </summary>
        public virtual ICache Cache
        {
            get { return this.cache ?? (this.cache = new DefaultMemoryCache()); }
        }

        /// <summary>
        /// Gets the <see cref="IDictionary{TKey,TValue}"/> containing data that persists throughout a publishing context.
        /// </summary>
        protected IDictionary<string, object> ContextData { get; private set; }

        /// <summary>
        /// Gets the <see cref="Package"/> object.
        /// </summary>
        protected Package Package { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initialize <see cref="TridionContext"/> with the given <see cref="Engine"/> and <see cref="Package"/> objects.
        /// </summary>
        /// <param name="engine"><see cref="Engine"/> object.</param>
        /// <param name="package"><see cref="Package"/> object.</param>
        internal void InitializeContext(Engine engine, Package package)
        {
            this.Engine = engine;
            this.Package = package;
            this.Component = this.GetContextComponent();
            this.Page = this.GetContextPage();
            this.ContextData = this.GetCustomContextData() ?? this.GetContextData();
            this.PageData = this.GetCustomPageData() ?? this.GetPageData();
        }

        /// <summary>
        /// Renders an <see cref="ITemplate"/> template for the given html string.
        /// </summary>
        /// <typeparam name="T"><see cref="ITemplate"/> type.</typeparam>
        /// <param name="html">Html string.</param>
        /// <returns>Rendered html string.</returns>
        public string RenderITemplate<T>(string html) where T : ITemplate, new()
        {
            return this.RenderITemplate<T>(html, Package.OutputName, ContentType.Html);
        }

        /// <summary>
        /// Renders an <see cref="ITemplate"/> template for the given html string with the given <see cref="Package"/> variable name.
        /// </summary>
        /// <typeparam name="T"><see cref="ITemplate"/> type.</typeparam>
        /// <param name="html">Html string.</param>
        /// <param name="variableName"><see cref="Package"/> variable name.</param>
        /// <returns>Rendered html string.</returns>
        public string RenderITemplate<T>(string html, string variableName) where T : ITemplate, new()
        {
            return this.RenderITemplate<T>(html, variableName, ContentType.Html);
        }

        /// <summary>
        /// Renders the Tridion built-in <see cref="ExtractBinariesFromHtmlTemplate"/> to extract images from
        /// Html output and registers each extracted image with the context <see cref="Template"/>.
        /// </summary>
        /// <param name="html">Html string containing image references.</param>
        /// <returns>Html string.</returns>
        public string RenderExtractBinariesFromHtml(string html)
        {
            var packageItem = this.Package.CreateStringItem(ContentType.Html, html);
            packageItem.Properties[Item.ItemPropertyBaseTcmUri] = this.Template.Id.ToString();
            this.Package.PushItem(Package.OutputName, packageItem);
            new ExtractBinariesFromHtmlTemplate().Transform(this.Engine, this.Package);
            html = packageItem.GetAsString();
            this.Package.Remove(packageItem);
            return html;
        }

        /// <summary>
        /// Pushes the given string with the given <see cref="ContentType"/> to the <see cref="Package"/> as an "Output" <see cref="Item"/>.
        /// Only the following string typed <see cref="ContentType"/>s are allowed: Html, Xhtml, Xml and Text.
        /// </summary>
        /// <param name="text">String to push to package.</param>
        /// <param name="contentType">ContentType.</param>
        public void PushOutputToPackage(string text, ContentType contentType)
        {
            this.PushStringToPackage(Package.OutputName, text, contentType);
        }

        /// <summary>
        /// Pushes the given string with given <see cref="ViewOutputType"/> to the <see cref="Package"/> as an "Output" <see cref="Item"/>.
        /// </summary>
        /// <param name="text">String to be pushed to <see cref="Package"/>.</param>
        /// <param name="outputType"><see cref="ViewOutputType"/>.</param>
        public void PushOutputToPackage(string text, ViewOutputType outputType)
        {
            this.PushStringToPackage(Package.OutputName, text, GetContentTypeByOutputType(outputType));
        }

        /// <summary>
        /// Pushes item to <see cref="Package"/> with the given variable name, string and <see cref="ContentType"/>. The following
        /// string typed <see cref="ContentType"/>s are allowed: Html, Xhtml, Xml and Text.
        /// </summary>
        /// <param name="variableName"><see cref="Package"/> variable name.</param>
        /// <param name="text">String to push to <see cref="Package"/>.</param>
        /// <param name="contentType"><see cref="ContentType"/>.</param>
        public void PushStringToPackage(string variableName, string text, ContentType contentType)
        {
            if (!contentType.Equals(ContentType.Html) && !contentType.Equals(ContentType.Xhtml)
                && !contentType.Equals(ContentType.Xml) && !contentType.Equals(ContentType.Text))
            {
                throw new NotSupportedException(
                    string.Format(
                        "ContentType {0} is not supported. Supported ContentTypes are: Html, Xhtml, Xml and Text",
                        contentType));
            }

            var packageItem = this.Package.CreateStringItem(contentType, text);
            this.Package.PushItem(variableName, packageItem);
        }

        /// <summary>
        /// Disposes the <see cref="TridionContext"/>. Overriding this method to dispose extended <see cref="TridionContext"/> if needed.
        /// </summary>
        /// <param name="disposing">Boolean to specify whether or not to perform explicit disposal.</param>
        protected virtual void Dispose(bool disposing)
        {
        }

        /// <summary>
        /// Gets custom <see cref="PageData"/>. Override this method to provide a custom <see cref="PageData"/>.
        /// </summary>
        /// <returns><see cref="IDictionary{String, Object}"/> representing the <see cref="PageData"/>.</returns>
        protected virtual IDictionary<string, object> GetCustomPageData()
        {
            return null;
        }

        /// <summary>
        /// Gets custom <see cref="ContextData"/>. Override this method to provide a custom <see cref="ContextData"/>.
        /// </summary>
        /// <returns><see cref="IDictionary{String, Object}"/> representing the <see cref="ContextData"/>.</returns>
        protected virtual IDictionary<string, object> GetCustomContextData()
        {
            return null;
        }

        private IDictionary<string, object> GetPageData()
        {
            IDictionary<string, object> pageData;
            var pageDataKey = "xviewpagedata|{4119B11E-43BC-4BD8-B19B-DD12A248C0BA}|";

            pageDataKey += this.Page != null
                ? this.Page.Id.GetVersionlessUri().ToString()
                : Path.GetRandomFileName();

            if (this.ContextData.ContainsKey(pageDataKey))
            {
                pageData = (IDictionary<string, object>)this.ContextData[pageDataKey];
            }
            else
            {
                pageData = new Dictionary<string, object>();
                this.ContextData.Add(pageDataKey, pageData);
            }

            return pageData;
        }

        private IDictionary<string, object> GetContextData()
        {
            const string contextDataKey = "xviewcontextdata|{D0FA8EA0-CAC4-4FF0-BBBD-D224475EB414}";
            IDictionary<string, object> contextData;
            var sessionContextData = this.Engine.GetSession().ContextData;

            if (sessionContextData.ContainsKey(contextDataKey))
            {
                contextData = (IDictionary<string, object>)sessionContextData[contextDataKey];
            }
            else
            {
                contextData = new Dictionary<string, object>();
                sessionContextData.Add(contextDataKey, contextData);
            }

            return contextData;
        }

        private Component GetContextComponent()
        {
            var item = this.Package.GetByType(ContentType.Component);
            return item != null ? (Component)this.Engine.GetObject(item) : null;
        }

        private Page GetContextPage()
        {
            var pageItem = this.Package.GetByType(ContentType.Page);

            if (pageItem != null)
            {
                return (Page)this.Engine.GetObject(pageItem);
            }

            if (this.Engine.PublishingContext.RenderContext.ContextItem != null)
            {
                return this.Engine.PublishingContext.RenderContext.ContextItem as Page;
            }

            return null;
        }

        /// <summary>
        /// Gets a matching <see cref="ContentType"/> for the given <see cref="ViewOutputType"/>.
        /// </summary>
        /// <param name="outputType"><see cref="ViewOutputType"/>.</param>
        /// <returns><see cref="ContentType"/>.</returns>
        private static ContentType GetContentTypeByOutputType(ViewOutputType outputType)
        {
            switch (outputType)
            {
                case ViewOutputType.Html:
                    return ContentType.Html;
                case ViewOutputType.Xhtml:
                    return ContentType.Xhtml;
                case ViewOutputType.Xml:
                    return ContentType.Xml;
                default:
                    return ContentType.Text;
            }
        }

        /// <summary>
        /// Renders an <see cref="ITemplate"/> template for the given text string with the given <see cref="Package"/> variable name and <see cref="ContentType"/>.
        /// </summary>
        /// <typeparam name="T"><see cref="ITemplate"/> type.</typeparam>
        /// <param name="text">Text string.</param>
        /// <param name="variableName"><see cref="Package"/> variable name.</param>
        /// <param name="contentType"><see cref="ContentType"/>.</param>
        /// <returns>Rendered text string.</returns>
        private string RenderITemplate<T>(string text, string variableName, ContentType contentType) where T : ITemplate, new()
        {
            var packageItem = this.Package.CreateStringItem(contentType, text);
            this.Package.PushItem(variableName, packageItem);
            new T().Transform(this.Engine, this.Package);
            var output = this.Package.GetValue(variableName);
            this.Package.Remove(packageItem);
            return output;
        }
    }
}