using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.ContentManagement.Fields;

namespace XView
{
    public static partial class TridionExtensions
    {
        /// <summary>
        /// Gets metadata <see cref="ItemFields"/> object from a <see cref="RepositoryLocalObject"/> derived object.
        /// </summary>
        /// <param name="repoLocalObject"><see cref="RepositoryLocalObject"/> object.</param>
        /// <returns><see cref="ItemFields"/> object.</returns>
        public static ItemFields GetMetadataFields(this RepositoryLocalObject repoLocalObject)
        {
            return repoLocalObject.Metadata != null
                ? new ItemFields(repoLocalObject.Metadata, repoLocalObject.MetadataSchema)
                : null;
        }

        /// <summary>
        /// Gets an <see cref="ItemField"/> from a <see cref="RepositoryLocalObject"/>'s metadata <see cref="ItemFields"/> from the given field name.
        /// </summary>
        /// <typeparam name="T"><see cref="ItemField"/> type.</typeparam>
        /// <param name="repoLocalObject"><see cref="RepositoryLocalObject"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="ItemField"/> object of the given type.</returns>
        public static T GetMetadataField<T>(this RepositoryLocalObject repoLocalObject, string fieldName) where T : ItemField
        {
            var fields = repoLocalObject.GetMetadataFields();
            return fields != null ? fields.GetField<T>(fieldName) : null;
        }
    }
}