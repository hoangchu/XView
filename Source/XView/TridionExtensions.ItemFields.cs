using System;
using System.Collections.Generic;
using System.Linq;

using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.ContentManagement.Fields;
using Tridion.ContentManager.Templating;

namespace XView
{
    /// <summary>
    /// This class contains extension methods to TOM.NET API's types.
    /// </summary>
    public static partial class TridionExtensions
    {
        /// <summary>
        /// Assign a handler to resolve xhtml from <see cref="XhtmlField"/>. The assigned handler will
        /// be utilized by the extension method ItemFields.GetText() when the target
        /// <see cref="ItemFields"/> is an <see cref="XhtmlField"/>. The TOM.NET built-in static method
        /// <see cref="TemplateUtilities.ResolveRichTextFieldXhtml"/> is a perfect handler to assign to
        /// this delegate. IMPORTANT NOTE: Do not assign an instance handler! This can lead 
        /// to undesired behaviour and memory leak. Assign a static handler instead.
        /// </summary>
        public static Func<string, string> XhtmlResolver;

        /// <summary>
        /// Gets text value from the given field name.
        /// This method does Tridion xhtml resolving if a <see cref="XhtmlResolver"/> handler is provided.
        /// </summary>
        /// <param name="fields"><see cref="ItemFields"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="String"/> or null.</returns>
        public static string GetText(this ItemFields fields, string fieldName)
        {
            var field = fields.GetField<TextField>(fieldName);

            if (field == null)
            {
                return null;
            }

            if (field is XhtmlField && XhtmlResolver != null)
            {
                return XhtmlResolver(field.Value);
            }

            return field.Value;
        }

        /// <summary>
        /// Gets <see cref="IList{String}"/> value from the given the field name.
        /// This method does Tridion xhtml resolving if a <see cref="XhtmlResolver"/> handler is provided.
        /// </summary>
        /// <param name="fields"><see cref="ItemFields"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="IList{String}"/> collection.</returns>
        public static IList<string> GetTexts(this ItemFields fields, string fieldName)
        {
            var field = fields.GetField<TextField>(fieldName);

            if (field == null)
            {
                return null;
            }

            if (field is XhtmlField && XhtmlResolver != null)
            {
                return field.Values.Select(x => XhtmlResolver(x)).ToList();
            }

            return field.Values;
        }

        /// <summary>
        /// Gets <see cref="Component"/> value from the given field name.
        /// </summary>
        /// <param name="fields"><see cref="ItemFields"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="Component"/> or null.</returns>
        public static Component GetComponent(this ItemFields fields, string fieldName)
        {
            var components = GetComponents(fields, fieldName);
            return components != null && components.Count > 0 ? components[0] : null;
        }

        /// <summary>
        /// Gets <see cref="IList{Component}"/> value from the given field name.
        /// </summary>
        /// <param name="fields"><see cref="ItemFields"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="IList{Component}"/> collection.</returns>
        public static IList<Component> GetComponents(this ItemFields fields, string fieldName)
        {
            var field = fields.GetField<ComponentLinkField>(fieldName);
            return field != null ? field.Values : null;
        }

        /// <summary>
        /// Gets <see cref="Keyword"/> value from the given field name.
        /// </summary>
        /// <param name="fields"><see cref="ItemFields"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="Keyword"/> or null.</returns>
        public static Keyword GetKeyword(this ItemFields fields, string fieldName)
        {
            var keywords = GetKeywords(fields, fieldName);
            return keywords != null && keywords.Count > 0 ? keywords[0] : null;
        }

        /// <summary>
        /// Gets <see cref="IList{Keyword}"/> value from the given field name.
        /// </summary>
        /// <param name="fields"><see cref="ItemFields"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="IList{Keyword}"/> collection.</returns>
        public static IList<Keyword> GetKeywords(this ItemFields fields, string fieldName)
        {
            var field = fields.GetField<KeywordField>(fieldName);
            return field != null ? field.Values : null;
        }

        /// <summary>
        /// Gets <see cref="DateTime"/> value from the given fieldd name.
        /// </summary>
        /// <param name="fields"><see cref="ItemFields"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="DateTime"/> or <see cref="DateTime.MinValue"/>.</returns>
        public static DateTime GetDate(this ItemFields fields, string fieldName)
        {
            var dates = GetDates(fields, fieldName);
            return dates != null && dates.Count > 0 ? dates[0] : DateTime.MinValue;
        }

        /// <summary>
        /// Gets <see cref="IList{DateTime}"/> value from the given field name.
        /// </summary>
        /// <param name="fields"><see cref="ItemFields"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="IList{DateTime}"/> collection.</returns>
        public static IList<DateTime> GetDates(this ItemFields fields, string fieldName)
        {
            var field = fields.GetField<DateField>(fieldName);
            return field != null ? field.Values : null;
        }

        /// <summary>
        /// Gets <see cref="double"/> value from the given field name.
        /// </summary>
        /// <param name="fields"><see cref="ItemFields"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="double"/> or <see cref="double.MinValue"/>.</returns>
        public static double GetNumber(this ItemFields fields, string fieldName)
        {
            var numbers = GetNumbers(fields, fieldName);
            return numbers != null && numbers.Count > 0 ? numbers[0] : double.MinValue;
        }

        /// <summary>
        /// Gets <see cref="IList{Double}"/> value from the given field name.
        /// </summary>
        /// <param name="fields"><see cref="ItemFields"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="IList{Double}"/> collection.</returns>
        public static IList<double> GetNumbers(this ItemFields fields, string fieldName)
        {
            var field = fields.GetField<NumberField>(fieldName);
            return field != null ? field.Values : null;
        }

        /// <summary>
        /// Gets <see cref="ItemFields"/> value from the given field name.
        /// </summary>
        /// <param name="fields"><see cref="ItemFields"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="ItemFields"/> or null.</returns>
        public static ItemFields GetEmbeddedField(this ItemFields fields, string fieldName)
        {
            var embeddedFields = GetEmbeddedFields(fields, fieldName);
            return embeddedFields != null && embeddedFields.Count > 0 ? embeddedFields[0] : null;
        }

        /// <summary>
        /// Gets <see cref="IList{ItemFields}"/> value from the given field name.
        /// </summary>
        /// <param name="fields"><see cref="ItemFields"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="IList{ItemFields}"/> collection.</returns>
        public static IList<ItemFields> GetEmbeddedFields(this ItemFields fields, string fieldName)
        {
            var field = fields.GetField<EmbeddedSchemaField>(fieldName);
            return field != null ? field.Values : null;
        }

        /// <summary>
        /// Gets an <see cref="ItemField"/> of a given type from given field name.
        /// </summary>
        /// <typeparam name="T"><see cref="ItemField"/> type.</typeparam>
        /// <param name="fields"><see cref="ItemFields"/> object.</param>
        /// <param name="fieldPath">Field name or path to field name.</param>
        /// <returns><see cref="ItemField"/> object of the given type.</returns>
        public static T GetField<T>(this ItemFields fields, string fieldPath) where T : ItemField
        {
            if (string.IsNullOrEmpty(fieldPath))
            {
                throw new ArgumentNullException("fieldPath", "Parameter fieldPath cannot be null or empty.");
            }

            var fieldPathParts = fieldPath.Trim(new[] { ' ', '/' }).Split('/');
            var fieldPathDepth = fieldPathParts.Length;
            var lookupFields = fields;

            for (var i = 0; i < fieldPathDepth; i++)
            {
                var fieldName = fieldPathParts[i].Trim();

                if (lookupFields == null || !lookupFields.Contains(fieldName))
                {
                    break;
                }

                if (i + 1 == fieldPathDepth)
                {
                    if (lookupFields[fieldName] is T)
                    {
                        return (T)lookupFields[fieldName];
                    }

                    throw new InvalidOperationException(
                        string.Format(
                            "Field found for the given field name \"{0}\" has field type {1} that does not match with the given field type {2}",
                            fieldName, lookupFields[fieldName].GetType().Name, typeof(T).Name));
                }

                var embeddedField = lookupFields.GetField<ItemField>(fieldName);

                if (!(embeddedField is EmbeddedSchemaField))
                {
                    throw new Exception(
                        string.Format("Field found for field name \"{0}\" in the given field path \"{1}\" is not an EmbeddedSchemaField",
                            fieldName, fieldPath));
                }

                lookupFields = ((EmbeddedSchemaField)embeddedField).Value;
            }

            return null;
        }
    }
}