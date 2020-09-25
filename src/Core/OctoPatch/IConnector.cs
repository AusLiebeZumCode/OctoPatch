using System;
using OctoPatch.ContentTypes;
using OctoPatch.Descriptions;

namespace OctoPatch
{
    /// <summary>
    /// Common connector interface
    /// </summary>
    public interface IConnector
    {
        /// <summary>
        /// Gets the id of the related node
        /// </summary>
        Guid NodeId { get; }

        /// <summary>
        /// Gets the unique id of the current connector
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Returns the description
        /// </summary>
        ConnectorDescription Description { get; }

        /// <summary>
        /// Returns the related content type
        /// </summary>
        ContentType ContentType { get; }
    }
}
