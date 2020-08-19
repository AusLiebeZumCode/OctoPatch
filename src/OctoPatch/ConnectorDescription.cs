using System;
using System.Collections.Generic;

namespace OctoPatch
{
    /// <summary>
    /// Basic description for node connectors
    /// </summary>
    public abstract class ConnectorDescription
    {
        /// <summary>
        /// Unique id of this connector
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Name of this connector
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description for this connector
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Optional list of supported types. If this is null
        /// the connector supports all kind of types
        /// </summary>
        public List<string> SupportedTypes { get; set; }
    }
}
