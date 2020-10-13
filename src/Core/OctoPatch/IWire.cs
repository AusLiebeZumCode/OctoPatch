using System;

namespace OctoPatch
{
    /// <summary>
    /// Interface for a node wire
    /// </summary>
    public interface IWire
    {
        /// <summary>
        /// Gets the wire id. This is only set when initialized.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Reference to the output connector. This is the input connector of the target node.
        /// </summary>
        IInputConnector Output { get; }

        /// <summary>
        /// Reference to the output connector. This is the output connector of the source node.
        /// </summary>
        IOutputConnector Input { get; }
    }
}
