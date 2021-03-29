using System;

namespace XView
{
    /// <summary>
    /// Representing the base class for view output decoration.
    /// </summary>
    public abstract class OutputDecorationFilter : IEquatable<OutputDecorationFilter>
    {
        /// <summary>
        /// Can handle the given <see cref="ViewOutputType"/>?
        /// </summary>
        /// <param name="viewOutputType"><see cref="ViewOutputType"/>.</param>
        /// <returns>true/false.</returns>
        public abstract bool CanHandle(ViewOutputType viewOutputType);

        /// <summary>
        /// Decorates the given text.
        /// </summary>
        /// <param name="text">Text string.</param>
        /// <returns>Decorated text string.</returns>
        public abstract string Decorate(string text);

        #region IEquatable<OutputDecorationFilter> Member

        public bool Equals(OutputDecorationFilter other)
        {
            if (other == null)
            {
                return false;
            }

            return other.GetType() == this.GetType();
        }

        #endregion

        #region Object Members

        public override bool Equals(object obj)
        {
            return this.Equals(obj as OutputDecorationFilter);
        }

        public override int GetHashCode()
        {
            return this.GetType().Name.GetHashCode();
        }

        #endregion
    }
}