using System;

namespace XView
{
    /// <summary>
    /// Representing the base class for view output validation.
    /// </summary>
    public abstract class OutputValidationFilter : IEquatable<OutputValidationFilter>
    {
        /// <summary>
        /// Can handle the given <see cref="ViewOutputType"/>?
        /// </summary>
        /// <param name="viewOutputType"><see cref="ViewOutputType"/>.</param>
        /// <returns>true/false.</returns>
        public abstract bool CanHandle(ViewOutputType viewOutputType);

        /// <summary>
        /// Validates the given text string.
        /// </summary>
        /// <param name="text">Text to validate.</param>
        public abstract void Validate(string text);

        #region IEquatable<OutputValidationFilter> Member

        public bool Equals(OutputValidationFilter other)
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
            return this.Equals(obj as OutputValidationFilter);
        }

        public override int GetHashCode()
        {
            return this.GetType().Name.GetHashCode();
        }

        #endregion
    }
}