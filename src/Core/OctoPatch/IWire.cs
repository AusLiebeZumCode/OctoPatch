using System;

namespace OctoPatch
{
    /// <summary>
    /// Interface for a node wire
    /// </summary>
    public interface IWire : IDisposable
    {
        /// <summary>
        /// Gets the wire id. This is only set when initialized.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Reference to the input connector
        /// </summary>
        IInputConnector Input { get; }

        /// <summary>
        /// Reference to the output connector
        /// </summary>
        IOutputConnector Output { get; }
    }
}
