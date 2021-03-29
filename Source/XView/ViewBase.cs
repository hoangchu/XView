using System;
using System.Collections.Generic;
using System.Linq;

using Tridion.ContentManager.Templating;

namespace XView
{
    /// <summary>
    /// Represents the base View in XView's MVC implementation.
    /// </summary>
    /// <typeparam name="TContext"><see cref="TContext"/> type.</typeparam>
    public abstract class ViewBase<TContext> : IDisposable
        where TContext : TridionContext
    {
        private readonly IList<OutputDecorationFilter> decorationFilters;
        private readonly IList<OutputValidationFilter> validationFilters;
        private IDictionary<string, object> viewData;

        /// <summary>
        /// Default ctor.
        /// </summary>
        protected ViewBase()
        {
            this.EnableOutputDecoration = false;
            this.EnableOutputValidation = false;
            this.decorationFilters = new List<OutputDecorationFilter>();
            this.validationFilters = new List<OutputValidationFilter>();
        }

        /// <summary>
        /// Ctor accepting a <see cref="TContext"/> as parameter.
        /// </summary>
        /// <param name="context"><see cref="TContext"/> object.</param>
        protected ViewBase(TContext context)
            : this()
        {
            this.Context = context;
        }

        /// <summary>
        /// Ctor accepting a <see cref="TContext"/> and a parent <see cref="ViewBase{TContext}"/>.
        /// </summary>
        /// <param name="context"><see cref="TContext"/> object.</param>
        /// <param name="parentView"><see cref="ViewBase{TContext}"/> object.</param>
        protected ViewBase(TContext context, ViewBase<TContext> parentView)
            : this()
        {
            this.Context = context;
            this.Parent = parentView;
        }

        /// <summary>
        /// Gets the model.
        /// </summary>
        public object Model { get; private set; }

        /// <summary>
        /// Gets an <see cref="IDictionary{String, Object}"/> containing data shared among related (parent/child) views.
        /// </summary>
        protected IDictionary<string, object> ViewData
        {
            get
            {
                if (this.Parent != null)
                {
                    return this.Parent.ViewData;
                }

                return this.viewData ?? (this.viewData = new Dictionary<string, object>());
            }
        }

        /// <summary>
        /// Gets the parent <see cref="ViewBase{TContext}"/>.
        /// </summary>
        public ViewBase<TContext> Parent { get; internal set; }

        /// <summary>
        /// Gets <see cref="TContext"/> object.
        /// </summary>
        public TContext Context { get; set; }

        /// <summary>
        /// Gets <see cref="ViewOutputType"/>.
        /// </summary>
        public virtual ViewOutputType OutputType
        {
            get { return ViewOutputType.Html; }
        }

        /// <summary>
        /// Gets or sets a boolean to enable or disable output decoration.
        /// </summary>
        public bool EnableOutputDecoration { get; protected set; }

        /// <summary>
        /// Gets or sets a boolean to enable or disable output validation.
        /// </summary>
        public bool EnableOutputValidation { get; protected set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Renders the given model.
        /// </summary>
        /// <param name="model">Object represents the model.</param>
        /// <returns>Output string.</returns>
        public string Render(object model)
        {
            if (model == null)
            {
                throw new NullReferenceException("Cannot render a null model");
            }

            this.CheckRenderCircularReferencing();
            this.Model = model;
            this.InitializeRender();
            this.PreRender();
            var output = this.Render() ?? string.Empty;
            output = this.DecorateOutput(output);
            Exception invalidOutputException = null;

            try
            {
                this.ValidateOutput(output);
            }
            catch (Exception ex)
            {
                if (this.Context.IsPublishing)
                {
                    throw new Exception(string.Format("View output failed validation: {0}", ex.Message), ex);
                }

                invalidOutputException = ex;
            }

            if (invalidOutputException != null)
            {
                var invalidOutputContext = new InvalidOutputContext(
                    output,
                    this.OutputType,
                    invalidOutputException);

                this.PushValidationErrorToPackage(invalidOutputContext);
                output = this.HandleAndRenderInvalidOutputInPreviewMode(invalidOutputContext) ?? string.Empty;

                if (!invalidOutputContext.ExceptionHandled)
                {
                    throw invalidOutputContext.Exception;
                }

                return output;
            }

            this.PostRender(output);

            return output;
        }

        /// <summary>
        /// Creates the given <see cref="ViewBase{TContext}"/> having the same <typeparamref name="TContext"/> type
        /// as that of the current <see cref="ViewBase{TContext}"/>.
        /// </summary>
        /// <typeparam name="TView">
        /// <see cref="ViewBase{TContext}"/> having the same <typeparamref name="TContext"/> 
        /// type as that of the current <see cref="ViewBase{TContext}"/>.
        /// </typeparam>
        /// <returns>
        /// <see cref="ViewBase{TContext}"/> where <typeparamref name="TContext"/> is the same <typeparamref name="TContext"/> 
        /// type as that of the current <see cref="ViewBase{TContext}"/>.
        /// </returns>
        public TView CreatePartial<TView>() where TView : ViewBase<TContext>, new()
        {
            return new TView { Context = this.Context, Parent = this };
        }

        /// <summary>
        /// Renders the given <see cref="ViewBase{TContext}"/> with the given model object. The <typeparamref name="TContext"/> 
        /// has the same <typeparamref name="TContext"/> type as that of the current <see cref="ViewBase{TContext}"/>.
        /// </summary>
        /// <typeparam name="TView">
        /// <see cref="ViewBase{TContext}"/> having the same <typeparamref name="TContext"/> 
        /// type as that of the current <see cref="ViewBase{TContext}"/>.
        /// </typeparam>
        /// <param name="model">Object represents the model.</param>
        /// <returns>Output string.</returns>
        public string RenderPartial<TView>(object model) where TView : ViewBase<TContext>, new()
        {
            using (var view = this.CreatePartial<TView>())
            {
                return view.Render(model);
            }
        }

        /// <summary>
        /// Registers the given <see cref="OutputValidationFilter"/>.
        /// </summary>
        /// <param name="filter"><see cref="OutputValidationFilter"/> object.</param>
        public void RegisterOutputFilter(OutputValidationFilter filter)
        {
            if (!this.validationFilters.Contains(filter))
            {
                this.validationFilters.Add(filter);
            }

            this.EnableOutputValidation = true;
        }

        /// <summary>
        /// Registers the given <see cref="OutputDecorationFilter"/>.
        /// </summary>
        /// <param name="filter"><see cref="OutputDecorationFilter"/> object.</param>
        public void RegisterOutputFilter(OutputDecorationFilter filter)
        {
            if (!this.decorationFilters.Contains(filter))
            {
                this.decorationFilters.Add(filter);
            }

            this.EnableOutputDecoration = true;
        }

        /// <summary>
        /// Renders output.
        /// </summary>
        /// <returns>Output string.</returns>
        protected abstract string Render();

        /// <summary>
        /// Override this method to do custom initialization.
        /// </summary>
        protected virtual void InitializeRender()
        {
            // Leave this to derived types to do custom InitializeRender() actions.
        }

        /// <summary>
        /// Override this method to do custom pre-render actions.
        /// </summary>
        protected virtual void PreRender()
        {
            // Leave this to derived types to do custom PreRender() actions.
        }

        /// <summary>
        /// Override this method to do custom post-render actions.
        /// </summary>
        /// <param name="viewOutput">Decorated and validated view output.</param>
        protected virtual void PostRender(string viewOutput)
        {
            // Leave this to derived types to do custom PostRender() actions.
        }

        /// <summary>
        /// Handles and renders the <see cref="InvalidOutputContext"/> caused an output validation failure in 
        /// preview mode. Override this method to provide a userfriendly presentation of the invalid output and 
        /// set exception to handled.
        /// </summary>
        /// <param name="invalidOutputContext"><see cref="InvalidOutputContext"/> object.</param>
        /// <returns>String presenting invalid view output.</returns>
        protected virtual string HandleAndRenderInvalidOutputInPreviewMode(InvalidOutputContext invalidOutputContext)
        {
            throw invalidOutputContext.Exception;
        }

        private string DecorateOutput(string text)
        {
            if (!this.EnableOutputDecoration)
            {
                return text;
            }

            return this.decorationFilters.Where(filter => filter.CanHandle(this.OutputType))
                .Aggregate(text, (current, filter) => filter.Decorate(current));
        }

        private void ValidateOutput(string text)
        {
            if (this.EnableOutputValidation)
            {
                foreach (var filter in this.validationFilters.Where(filter => filter.CanHandle(this.OutputType)))
                {
                    filter.Validate(text);
                }
            }
        }

        private void PushValidationErrorToPackage(InvalidOutputContext invalidOutputContext)
        {
            this.Context.PushStringToPackage("ErrorMessage", invalidOutputContext.Exception.Message, ContentType.Text);
            this.Context.PushStringToPackage("ErrorOutput", invalidOutputContext.ViewOutput, ContentType.Text);
        }

        private void CheckRenderCircularReferencing()
        {
            var thisViewType = this.GetType();
            var parentView = this.Parent;

            while (parentView != null)
            {
                var parentViewType = parentView.GetType();

                if (parentViewType == thisViewType)
                {
                    throw new Exception(
                        string.Format(
                            "Partial view circular referencing detected. View \"{0}\" renders view \"{1}\" which is itself or one of its parents",
                            thisViewType, parentViewType));
                }

                parentView = parentView.Parent;
            }
        }

        /// <summary>
        /// Disposes the <see cref="TContext"/>. Overriding this method to dispose extended <see cref="TContext"/> if needed.
        /// </summary>
        /// <param name="disposing">Boolean to specify whether or not to perform explicit disposal.</param>
        protected virtual void Dispose(bool disposing)
        {
        }
    }
}