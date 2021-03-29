using System;

using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Assembly;

namespace XView
{
    /// <summary>
    /// Represents the base Front Controller.
    /// </summary>
    public abstract class Controller<TContext> : ITemplate
        where TContext : TridionContext, new()
    {
        private bool disposed;
        private Engine engine;
        private object model;
        private Package package;
        private ViewBase<TContext> view;
        private ViewMappingResult viewMappingResult;

        /// <summary>
        /// Gets the <see cref="TContext"/>.
        /// </summary>
        protected TContext Context { get; private set; }

        /// <summary>
        /// The single point of contact with Tridion.
        /// </summary>
        /// <param name="engine"><see cref="Engine"/> object.</param>
        /// <param name="package"><see cref="Package"/> object.</param>
        public void Transform(Engine engine, Package package)
        {
            this.engine = engine;
            this.package = package;
            this.HandleViewRequest();
        }

        private void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void HandleViewRequest()
        {
            this.LoadContext();
            this.LoadViewMappingResult();

            if (this.MapView())
            {
                this.LoadView();
                this.Initialize();
                this.SelectModel();
                this.RenderView();
            }
            else
            {
                this.ShowFailedMappingView();
            }

            this.Dispose();
        }

        private void LoadContext()
        {
            this.Context = new TContext();
            this.Context.InitializeContext(this.engine, this.package);
        }

        private void LoadViewMappingResult()
        {
            var viewMapper = this.GetCustomViewMapper() ?? new ViewMapper(this.GetRootNamespace());
            this.viewMappingResult = viewMapper.MapView(this.Context.Template);
        }

        private bool MapView()
        {
            if (!this.viewMappingResult.Success)
            {
                if (!this.Context.IsPublishing)
                {
                    return false;
                }

                throw new Exception(string.Format("Template name \"{0}\" is invalid", this.viewMappingResult.Template.Title));
            }

            if (this.viewMappingResult.ViewType == null)
            {
                if (!this.Context.IsPublishing)
                {
                    return false;
                }

                throw new Exception(string.Format("Could not find View {0} for template {1}. Please implement the missing View",
                    this.viewMappingResult.ViewFullTypeName, this.viewMappingResult.Template.Title));
            }

            this.CheckMappedViewIsViewBase();

            return true;
        }

        private void LoadView()
        {
            this.view = (ViewBase<TContext>)Activator.CreateInstance(this.viewMappingResult.ViewType);
            this.view.Context = this.Context;
        }

        private void SelectModel()
        {
            this.model = this.GetCustomViewModel() ?? (this.Context.Component ?? this.Context.Page as object);
        }

        private void RenderView()
        {
            this.Context.PushOutputToPackage(this.view.Render(this.model), this.view.OutputType);
        }

        private void ShowFailedMappingView()
        {
            var controllerView = this.GetCustomMappingErrorView() ?? new MappingErrorView<TContext> { Context = this.Context };
            this.Context.PushOutputToPackage(controllerView.Render(this.viewMappingResult), controllerView.OutputType);
        }

        private void CheckMappedViewIsViewBase()
        {
            if (!typeof(ViewBase<TContext>).IsAssignableFrom(this.viewMappingResult.ViewType))
            {
                throw new Exception(string.Format("View class {0} is not derived from {1}", this.viewMappingResult.ViewFullTypeName,
                    typeof(ViewBase<TContext>).FullName));
            }
        }

        /// <summary>
        /// Gets a custom <see cref="View{TContext, ViewMappingResult}"/> to present when View mapping fails.  
        /// Override this method to provide a custom <see cref="View{TContext, ViewMappingResult}"/> View.
        /// </summary>
        /// <returns><see cref="View{TContext, ViewMappingResult}"/> object.</returns>
        protected virtual View<TContext, ViewMappingResult> GetCustomMappingErrorView()
        {
            // Leave this to derived Controller.

            return null;
        }

        /// <summary>
        /// Gets a custom <see cref="IViewMapper"/>. Override this method to provide a custom <see cref="IViewMapper"/>.
        /// </summary>
        /// <returns><see cref="IViewMapper"/> object or null.</returns>
        protected virtual IViewMapper GetCustomViewMapper()
        {
            // Leave this to derived Controller.

            return null;
        }

        /// <summary>
        /// Gets a custom view model. Override this method to provide a custom model for a specific view.
        /// </summary>
        /// <returns>Object or null.</returns>
        protected virtual object GetCustomViewModel()
        {
            // Leave this to derived Controller.

            return null;
        }

        /// <summary>
        /// Performs custom initialization logic. Override this method to provide custom initialization logic.
        /// </summary>
        protected virtual void Initialize()
        {
            // Leave this to derived Controller.
        }

        /// <summary>
        /// Adds an <see cref="OutputDecorationFilter"/> to do view output decoration.
        /// </summary>
        /// <param name="filter"><see cref="OutputDecorationFilter"/> object.</param>
        protected void RegisterOutputFilter(OutputDecorationFilter filter)
        {
            this.view.RegisterOutputFilter(filter);
        }

        /// <summary>
        /// Adds an <see cref="OutputValidationFilter"/> to do view output validation.
        /// </summary>
        /// <param name="filter"><see cref="OutputValidationFilter"/> object.</param>
        protected void RegisterOutputFilter(OutputValidationFilter filter)
        {
            this.view.RegisterOutputFilter(filter);
        }

        /// <summary>
        /// Gets the root namespace of the current templating project/assembly.
        /// </summary>
        /// <returns>String represents the root namespace.</returns>
        protected string GetRootNamespace()
        {
            const string controllersNamespace = "Controllers";
            var fullNamespace = this.GetType().Namespace + string.Empty;
            var subNamespaces = fullNamespace.Split('.');

            if (subNamespaces.Length < 2 || subNamespaces[subNamespaces.Length - 1] != controllersNamespace)
            {
                throw new Exception(
                    string.Format(
                        "Controller {0} must reside in at least two namespace-levels deep, and the last subnamespace must be named {1}",
                        this.GetType().FullName, controllersNamespace));
            }

            return fullNamespace.Substring(0, fullNamespace.Length - controllersNamespace.Length - 1);
        }

        /// <summary>
        /// Disposes the <see cref="Controller{TContext}"/>. Overriding this method to dispose 
        /// extended <see cref="TridionContext"/> if needed.
        /// </summary>
        /// <param name="disposing">Boolean to specify whether or not to perform explicit disposal.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    if (this.view != null)
                    {
                        this.view.Dispose();
                    }

                    if (this.Context != null)
                    {
                        this.Context.Dispose();
                    }
                }
            }

            this.disposed = true;
        }
    }
}