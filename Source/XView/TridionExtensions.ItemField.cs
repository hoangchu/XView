using Tridion.ContentManager.ContentManagement.Fields;

namespace XView
{
    public static partial class TridionExtensions
    {
        /// <summary>
        /// <see cref="TextField"/> has value?
        /// </summary>
        /// <param name="field"><see cref="TextField"/>.</param>
        /// <returns>true/false.</returns>
        public static bool HasValue(this TextField field)
        {
            if (field == null)
            {
                return false;
            }

            return field.Values.Count > 0;
        }

        /// <summary>
        /// <see cref="ComponentLinkField"/> has value?
        /// </summary>
        /// <param name="field"><see cref="ComponentLinkField"/>.</param>
        /// <returns>true/false.</returns>
        public static bool HasValue(this ComponentLinkField field)
        {
            if (field == null)
            {
                return false;
            }

            return field.Values.Count > 0;
        }

        /// <summary>
        /// <see cref="EmbeddedSchemaField"/> has value?
        /// </summary>
        /// <param name="field"><see cref="EmbeddedSchemaField"/>.</param>
        /// <returns>true/false.</returns>
        public static bool HasValue(this EmbeddedSchemaField field)
        {
            if (field == null)
            {
                return false;
            }

            return field.Values.Count > 0;
        }

        /// <summary>
        /// <see cref="KeywordField"/> has value?
        /// </summary>
        /// <param name="field"><see cref="KeywordField"/>.</param>
        /// <returns>true/false.</returns>
        public static bool HasValue(this KeywordField field)
        {
            if (field == null)
            {
                return false;
            }

            return field.Values.Count > 0;
        }

        /// <summary>
        /// <see cref="DateField"/> has value?
        /// </summary>
        /// <param name="field"><see cref="DateField"/>.</param>
        /// <returns>true/false.</returns>
        public static bool HasValue(this DateField field)
        {
            if (field == null)
            {
                return false;
            }

            return field.Values.Count > 0;
        }

        /// <summary>
        /// <see cref="NumberField"/> has value?
        /// </summary>
        /// <param name="field"><see cref="NumberField"/>.</param>
        /// <returns>true/false.</returns>
        public static bool HasValue(this NumberField field)
        {
            if (field == null)
            {
                return false;
            }

            return field.Values.Count > 0;
        }
    }
}