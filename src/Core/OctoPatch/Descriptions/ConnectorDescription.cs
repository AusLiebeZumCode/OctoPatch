using System;
using OctoPatch.ContentTypes;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// description for node connectors
    /// </summary>
    public sealed class ConnectorDescription : Description
    {
        /// <summary>
        /// Unique key of this connector
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// connector content type
        /// </summary>
        public ContentType ContentType { get; set; }

        public ConnectorDescription(string key, string displayName, string displayDescription, ContentType contentType) 
            : base(displayName, displayDescription)
        {
            Key = key;
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
        }
    }
}
