namespace XView
{
    /// <summary>
    /// Represents the base View with a strongly typed Model.
    /// </summary>
    /// <typeparam name="TContext">TridionContext or derived type.</typeparam>
    /// <typeparam name="TModel">Model type.</typeparam>
    public abstract class View<TContext, TModel> : ViewBase<TContext>
        where TContext : TridionContext
    {
        /// <summary>
        /// Default ctor.
        /// </summary>
        protected View()
        {
        }

        /// <summary>
        /// Ctor taking a TridionContext or derived object.
        /// </summary>
        /// <param name="context">TridionContext or derived object.</param>
        protected View(TContext context) : base(context)
        {
        }

        /// <summary>
        /// Ctor accepting a TridionContext or derived and a parent <see cref="ViewBase{TContext}"/>.
        /// </summary>
        /// <param name="context">TridionContext or derived object.</param>
        /// <param name="parentView"><see cref="ViewBase{TContext}"/> object.</param>
        protected View(TContext context, ViewBase<TContext> parentView) : base(context, parentView)
        {
        }

        /// <summary>
        /// Gets strongly typed model.
        /// </summary>
        new public TModel Model
        {
            get { return (TModel)base.Model; }
        }

        /// <summary>
        /// Renders the given strongly type <see cref="TModel"/>.
        /// </summary>
        /// <param name="model">Strongly type <see cref="TModel"/>.</param>
        /// <returns>Output string.</returns>
        public string Render(TModel model)
        {
            return base.Render(model);
        }
    }
}