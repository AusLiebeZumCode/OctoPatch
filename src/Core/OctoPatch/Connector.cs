using System;
using OctoPatch.Descriptions;

namespace OctoPatch
{
    /// <summary>
    /// Base class for all kind of connectors
    /// </summary>
    public abstract class Connector : IConnector
    {
        /// <inheritdoc />
        public Guid NodeId { get; }

        /// <inheritdoc />
        public string Key { get; }

        /// <inheritdoc />
        public ConnectorDescription Description { get; }

        protected Connector(Guid nodeId, Type supportedType, ConnectorDescription description)
        {
            if (description is null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            // Compare supported type with description type
            if (!description.ContentType.IsSupportedType(supportedType))
            {
                throw new ArgumentException("types do not match", nameof(description));
            }

            NodeId = nodeId;
            Key = description.Key;
            Description = description;
        }
    }
}
