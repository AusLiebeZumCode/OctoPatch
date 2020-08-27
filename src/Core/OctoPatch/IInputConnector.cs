using System;

namespace OctoPatch
{
    /// <summary>
    /// Interface for an input connector
    /// </summary>
    public interface IInputConnector : IConnector
    {
        /// <summary>
        /// Returns the description for this input
        /// </summary>
        InputDescription InputDescription { get; }
    }
}
