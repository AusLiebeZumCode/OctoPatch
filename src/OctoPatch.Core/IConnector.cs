using System;

namespace OctoPatch.Core
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
