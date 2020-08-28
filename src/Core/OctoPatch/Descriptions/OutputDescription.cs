using System;
using OctoPatch.ContentTypes;

namespace OctoPatch.Descriptions
{
    /// <summary>
    /// Representation of a single output connector
    /// </summary>
    public sealed class OutputDescription : ConnectorDescription
    {
        public OutputDescription(Guid guid, string name, ContentType contentType, string description = null) 
            : base(guid, name, contentType, description)
        {
        }
    }
}
