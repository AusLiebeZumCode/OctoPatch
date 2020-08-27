using System;

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
        /// connector content type
        /// </summary>
        public ContentType ContentType { get; set; }

        protected ConnectorDescription(Guid guid, string name, ContentType contentType, string description = null)
        {
            Guid = guid;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
            Description = description;
        }
    }
}
