using System;

namespace OctoPatch
{
    /// <summary>
    /// Common connector interface
    /// </summary>
    public interface IConnector
    {
        /// <summary>
        /// Gets the unique id of the current connector
        /// </summary>
        Guid Guid { get; }
    }
}
