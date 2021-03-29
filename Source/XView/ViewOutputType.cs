namespace XView
{
    /// <summary>
    /// <see cref="ViewOutputType"/> allows fine-grained control over view output decoration and validation.
    /// </summary>
    public enum ViewOutputType
    {
        /// <summary>
        /// ASP.NET User Controler.
        /// </summary>
        Ascx,

        /// <summary>
        /// ASP.NET Active Server Page Extended.
        /// </summary>
        Aspx,

        /// <summary>
        /// ASP.NET Razor.
        /// </summary>
        Cshtml,

        /// <summary>
        /// Cascading Style Sheet.
        /// </summary>
        Css,

        /// <summary>
        /// Custom Output Type.
        /// </summary>
        Custom,

        /// <summary>
        /// Hypertext Markup Language.
        /// </summary>
        Html,

        /// <summary>
        /// JavaScript.
        /// </summary>
        Js,

        /// <summary>
        /// JavaScript Object Notation.
        /// </summary>
        Json,

        /// <summary>
        /// Java Server Page.
        /// </summary>
        Jsp,

        /// <summary>
        /// Plain Text.
        /// </summary>
        Text,

        /// <summary>
        /// Velocity Markup.
        /// </summary>
        Vm,

        /// <summary>
        /// Extensible Hypertext Markup Language.
        /// </summary>
        Xhtml,

        /// <summary>
        /// Extensible Markup Language.
        /// </summary>
        Xml,

        /// <summary>
        /// Extensible Stylesheet Language Transformations.
        /// </summary>
        Xslt
    }
}