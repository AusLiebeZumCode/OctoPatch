using System;
using OctoPatch.ContentTypes;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// description for node connectors
    /// </summary>
    public sealed class ConnectorDescription : KeyDescription
    {
        /// <summary>
        /// connector content type
        /// </summary>
        public ContentType ContentType { get; set; }

        public ConnectorDescription(string key, string displayName, string displayDescription, ContentType contentType) 
            : base(key, displayName, displayDescription)
        {
            Key = key;
            ContentType = contentType ?? throw new ArgumentNullException(nameof(contentType));
        }
    }
}
