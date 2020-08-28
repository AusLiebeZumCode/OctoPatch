using System;
using OctoPatch.Descriptions;

namespace OctoPatch
{
    /// <summary>
    /// Base class for all kind of connectors
    /// </summary>
    internal abstract class Connector : IConnector
    {
        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// <inheritdoc />
        /// </summary>
        public ConnectorDescription Description { get; }

        protected Connector(ConnectorDescription description)
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            Key = description.Key;
            Description = description;
        }
    }
}
