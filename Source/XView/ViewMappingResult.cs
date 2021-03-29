using System;

using Tridion.ContentManager.CommunicationManagement;

namespace XView
{
    /// <summary>
    /// Represents the View mapping result of the ViewMapper.
    /// </summary>
    public class ViewMappingResult
    {
        public bool Success { get; private set; }

        /// <summary>
        /// Gets the Tridion <see cref="Template"/> that is subjected to View mapping.
        /// </summary>
        public Template Template { get; private set; }

        /// <summary>
        /// Gets the fully qualified type name of the mapped View.
        /// </summary>
        public string ViewFullTypeName { get; private set; }

        /// <summary>
        /// Gets the Type of the mapped View.
        /// </summary>
        public Type ViewType { get; private set; }

        public ViewMappingResult(Template template, string viewFullTypeName = "", Type viewType = null, bool success = false)
        {
            this.Template = template;
            this.ViewFullTypeName = viewFullTypeName;
            this.ViewType = viewType;
            this.Success = success;
        }
    }
}