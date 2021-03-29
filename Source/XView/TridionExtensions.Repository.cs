using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.ContentManagement.Fields;

namespace XView
{
    public static partial class TridionExtensions
    {
        /// <summary>
        /// Gets metadata <see cref="ItemFields"/> object from a <see cref="Repository"/> derived object.
        /// </summary>
        /// <param name="repository"><see cref="Repository"/> derived object.</param>
        /// <returns><see cref="ItemFields"/> object.</returns>
        public static ItemFields GetMetadataFields(this Repository repository)
        {
            return repository.Metadata != null ? new ItemFields(repository.Metadata, repository.MetadataSchema) : null;
        }

        /// <summary>
        /// Gets an <see cref="ItemField"/> from a <see cref="Repository"/>'s metadata <see cref="ItemFields"/> from the given field name.
        /// </summary>
        /// <typeparam name="T">Type of <see cref="ItemField"/>.</typeparam>
        /// <param name="repository">Repository derived object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="ItemField"/> object of the given type.</returns>
        public static T GetMetadataField<T>(this Repository repository, string fieldName) where T : ItemField
        {
            var fields = repository.GetMetadataFields();
            return fields != null ? fields.GetField<T>(fieldName) : null;
        }
    }
}