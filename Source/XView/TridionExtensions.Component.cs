using System;
using System.Collections.Generic;

using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.ContentManagement.Fields;

namespace XView
{
    public static partial class TridionExtensions
    {
        /// <summary>
        /// Gets content <see cref="ItemFields"/> from a Component.
        /// </summary>
        /// <param name="component"><see cref="Component"/> object.</param>
        /// <returns><see cref="ItemFields"/>.</returns>
        public static ItemFields GetFields(this Component component)
        {
            return new ItemFields(component.Content, component.Schema);
        }

        /// <summary>
        /// Gets an <see cref="ItemField"/> of a given type from a <see cref="Component"/>'s content <see cref="ItemFields"/> for the given field name.
        /// </summary>
        /// <typeparam name="T"><see cref="ItemField"/> type.</typeparam>
        /// <param name="component"><see cref="Component"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="ItemField"/> or null.</returns>
        public static T GetField<T>(this Component component, string fieldName) where T : ItemField
        {
            return component.GetFields().GetField<T>(fieldName);
        }

        /// <summary>
        /// Gets text value from the given field name. This method does
        /// Tridion xhtml resolving if an <see cref="XhtmlResolver"/> is available.
        /// </summary>
        /// <param name="component"><see cref="Component"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>String or null.</returns>
        public static string GetText(this Component component, string fieldName)
        {
            return component.GetFields().GetText(fieldName);
        }

        /// <summary>
        /// Gets <see cref="IList{String}"/> value from the given field name.
        /// </summary>
        /// <param name="component"><see cref="Component"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="IList{String}"/> collection.</returns>
        public static IList<string> GetTexts(this Component component, string fieldName)
        {
            return component.GetFields().GetTexts(fieldName);
        }

        /// <summary>
        /// Get <see cref="Component"/> value from the given field name.
        /// </summary>
        /// <param name="component"><see cref="Component"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="Component"/> or null.</returns>
        public static Component GetComponent(this Component component, string fieldName)
        {
            return component.GetFields().GetComponent(fieldName);
        }

        /// <summary>
        /// Gets <see cref="IList{Component}"/> value from the given field name.
        /// </summary>
        /// <param name="component"><see cref="Component"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="IList{Component}"/> collection.</returns>
        public static IList<Component> GetComponents(this Component component, string fieldName)
        {
            return component.GetFields().GetComponents(fieldName);
        }

        /// <summary>
        /// Gets <see cref="Keyword"/> value from the given field name.
        /// </summary>
        /// <param name="component"><see cref="Component"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="Keyword"/> or null.</returns>
        public static Keyword GetKeyword(this Component component, string fieldName)
        {
            return component.GetFields().GetKeyword(fieldName);
        }

        /// <summary>
        /// Gets <see cref="IList{Keyword}"/> from the given field name.
        /// </summary>
        /// <param name="component"><see cref="Component"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="IList{Keyword}"/> collection.</returns>
        public static IList<Keyword> GetKeywords(this Component component, string fieldName)
        {
            return component.GetFields().GetKeywords(fieldName);
        }

        /// <summary>
        /// Gets <see cref="DateTime"/> value from the given field name.
        /// </summary>
        /// <param name="component"><see cref="Component"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="DateTime"/> or <see cref="DateTime.MinValue"/>.</returns>
        public static DateTime GetDate(this Component component, string fieldName)
        {
            return component.GetFields().GetDate(fieldName);
        }

        /// <summary>
        /// Gets <see cref="IList{DateTime}"/> value from the given field name.
        /// </summary>
        /// <param name="component"><see cref="Component"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="IList{DateTime}"/> collection.</returns>
        public static IList<DateTime> GetDates(this Component component, string fieldName)
        {
            return component.GetFields().GetDates(fieldName);
        }

        /// <summary>
        /// Gets <see cref="Double"/> value from the given field name.
        /// </summary>
        /// <param name="component"><see cref="Component"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="Double"/> or <see cref="Double.MinValue"/>.</returns>
        public static double GetNumber(this Component component, string fieldName)
        {
            return component.GetFields().GetNumber(fieldName);
        }

        /// <summary>
        /// Gets <see cref="IList{Double}"/> value from the given field name.
        /// </summary>
        /// <param name="component"><see cref="Component"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="IList{Double}"/> collection.</returns>
        public static IList<double> GetNumbers(this Component component, string fieldName)
        {
            return component.GetFields().GetNumbers(fieldName);
        }

        /// <summary>
        /// Gets <see cref="ItemFields"/> value from the given field name.
        /// </summary>
        /// <param name="component"><see cref="Component"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="ItemFields"/> or null.</returns>
        public static ItemFields GetEmbeddedField(this Component component, string fieldName)
        {
            return component.GetFields().GetEmbeddedField(fieldName);
        }

        /// <summary>
        /// Gets <see cref="IList{ItemFields}"/> value from the given field name.
        /// </summary>
        /// <param name="component"><see cref="Component"/> object.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns><see cref="IList{ItemFields}"/> collection.</returns>
        public static IList<ItemFields> GetEmbeddedFields(this Component component, string fieldName)
        {
            return component.GetFields().GetEmbeddedFields(fieldName);
        }
    }
}