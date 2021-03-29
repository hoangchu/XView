using System;

namespace XView
{
    /// <summary>
    /// Represents the invalid view output context.
    /// </summary>
    public class InvalidOutputContext
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="viewOutput">View output string.</param>
        /// <param name="outputType"><see cref="ViewOutputType"/>.</param>
        /// <param name="invalidOutputException"><see cref="Exception"/> or null.</param>
        /// <param name="exceptionHandled">Is validation <see cref="Exception"/> handled?</param>
        public InvalidOutputContext(
            string viewOutput,
            ViewOutputType outputType,
            Exception invalidOutputException,
            bool exceptionHandled = false)
        {
            this.ViewOutput = viewOutput;
            this.OutputType = outputType;
            this.Exception = invalidOutputException;
            this.ExceptionHandled = exceptionHandled;
        }

        /// <summary>
        /// Gets the output of the rendered view.
        /// </summary>
        public string ViewOutput { get; private set; }

        /// <summary>
        /// Gets the <see cref="ViewOutputType"/> of the rendered view.
        /// </summary>
        public ViewOutputType OutputType { get; private set; }

        /// <summary>
        /// Gets the <see cref="Exception"/> thrown during the rendering of the view. If no exception, 
        /// this properties returns null.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets or sets a boolean stating whether a possible <see cref="Exception"/> thrown during 
        /// the rendering of the view is handle.
        /// </summary>
        public bool ExceptionHandled { get; set; }
    }
}