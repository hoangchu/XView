using Tridion.ContentManager.CommunicationManagement;

namespace XView
{
    /// <summary>
    /// Represents the View that will be shown when in preview mode a Component Template 
    /// or Page Template name is invalid, or when no View can be found for the rendering
    /// Component Template or Page Template.
    /// </summary>
    internal class MappingErrorView<TContext> : View<TContext, ViewMappingResult>
        where TContext : TridionContext
    {
        protected override void InitializeRender()
        {
            this.EnableOutputDecoration = false;
            this.EnableOutputValidation = false;
        }

        /// <summary>
        /// Renders output.
        /// </summary>
        /// <returns>String output.</returns>
        protected override string Render()
        {
            var xt = XTemplate.LoadFromResource(this.GetType().Namespace + ".MappingErrorView.html");

            xt.Assign("TemplateTypeName",
                this.Model.Template is ComponentTemplate ? "component template" : "page template");

            xt.Assign("ErrorMessage",
                this.Model.Success ? "View Not Found" : "Invalid Template Name");

            xt.Assign("TemplateName", this.Model.Template.Title);
            xt.Assign("ViewFullTypeName", this.Model.ViewFullTypeName);
            xt.Assign("TemplateNameRegexPattern", ViewMapper.TemplateNameRegexPattern);
            xt.Parse(!this.Model.Success ? "root.invalidTemplateName" : "root.viewNotFound");

            return xt.ToString();
        }
    }
}