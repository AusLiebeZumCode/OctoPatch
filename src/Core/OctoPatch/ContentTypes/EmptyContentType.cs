using System;
using System.Runtime.Serialization;

namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents an empty type which is used just for triggering stuff. no content at all
    /// </summary>
    [DataContract]
    public sealed class EmptyContentType : ContentType
    {
        /// <inheritdoc />
        public override Type SupportedType => typeof(void);
    }
}
