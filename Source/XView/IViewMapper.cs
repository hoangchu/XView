using Tridion.ContentManager.CommunicationManagement;

namespace XView
{
    /// <summary>
    /// Represents the IViewMapper interface.
    /// </summary>
    public interface IViewMapper
    {
        /// <summary>
        /// Maps View class for the given Tridion <see cref="Template"/>.
        /// </summary>
        /// <param name="template">Tridion <see cref="Template"/> object.</param>
        /// <returns><see cref="ViewMappingResult"/>.</returns>
        ViewMappingResult MapView(Template template);
    }
}