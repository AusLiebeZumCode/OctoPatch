using System;

namespace OctoPatch
{
    /// <summary>
    /// Representation of a single input connector
    /// </summary>
    public sealed class InputDescription : ConnectorDescription
    {
        public InputDescription(Guid guid, string name, ContentType contentType, string description = null) 
            : base(guid, name, contentType, description)
        {
        }
    }
}
