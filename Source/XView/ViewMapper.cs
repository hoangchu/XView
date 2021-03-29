using System;
using System.Text.RegularExpressions;

using Tridion.ContentManager.CommunicationManagement;

namespace XView
{
    /// <summary>
    /// Represents the default implementation of IViewMapper.
    /// </summary>
    public class ViewMapper : IViewMapper
    {
        /// <summary>
        /// Regular expression used to validate template name.
        /// </summary>
        public static readonly string TemplateNameRegexPattern =
            @"^(([A-Z]{1,2}[a-z0-9]+\s?)*[A-Z]{1,2}[a-z0-9]*:\s)?([A-Z]{1,2}[a-z0-9]+\s?)*[A-Z]{1,2}[a-z0-9]*$";

        private ViewMapper()
        {
            this.TemplateNamePrefix = string.Empty;
        }

        /// <summary>
        /// ViewMapper constructor with the given root namespace as parameter.
        /// </summary>
        /// <param name="rootNamespace">Root namespace of the current TOM.NET templating project.</param>
        public ViewMapper(string rootNamespace) : this()
        {
            this.ProjectRootNamespace = rootNamespace;
        }

        /// <summary>
        /// Gets the root namespace of the templating project containing the view to map.
        /// </summary>
        protected string ProjectRootNamespace { get; private set; }

        /// <summary>
        /// Gets the template name prefix of the rendering Tridion template.
        /// </summary>
        protected string TemplateNamePrefix { get; private set; }

        /// <summary>
        /// Gets the rendering Tridion template.
        /// </summary>
        protected Template Template { get; private set; }

        protected virtual string ViewTypeNameSuffix
        {
            get { return "View"; }
        }

        /// <summary>
        /// Maps a View for the given Tridion template.
        /// </summary>
        /// <param name="template">Tridion template.</param>
        /// <returns><see cref="ViewMappingResult"/>.</returns>
        public ViewMappingResult MapView(Template template)
        {
            this.Template = template;
            var templateName = template.Title;

            if (!Regex.Match(templateName, TemplateNameRegexPattern, RegexOptions.Compiled).Success)
            {
                return new ViewMappingResult(template);
            }

            var templateNameParts = templateName.Split(':');
            var viewTypeName = templateNameParts[templateNameParts.Length - 1].Replace(" ", "") + (ViewTypeNameSuffix ?? string.Empty);
            this.TemplateNamePrefix = templateNameParts.Length > 1 ? templateNameParts[0].Trim() : string.Empty;
            var viewFullTypeName = string.Format("{0}.{1}", this.GetViewNamespace(), viewTypeName);
            var viewType = this.GetInternalType(viewFullTypeName);

            return new ViewMappingResult(template, viewFullTypeName, viewType, success: true);
        }

        /// <summary>
        /// Gets <see cref="Type"/> in the current assembly for the given type full name.
        /// Override this method if you don't merge (ILMerge) XView.dll with your project dll.
        /// </summary>
        /// <param name="typeFullName"><see cref="Type"/> full name.</param>
        /// <returns>Type.</returns>
        protected virtual Type GetInternalType(string typeFullName)
        {
            return Type.GetType(typeFullName);
        }

        /// <summary>
        /// Gets the namespace containing the view to map. Override this method to provide
        /// different view namespaces.
        /// </summary>
        protected virtual string GetViewNamespace()
        {
            return this.Template is ComponentTemplate
                ? this.ProjectRootNamespace + ".Views.ComponentViews"
                : this.ProjectRootNamespace + ".Views.PageViews";
        }
    }
}