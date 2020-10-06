using System;
using System.Runtime.Serialization;

namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents a bool based message description
    /// </summary>
    [DataContract]
    public sealed class BoolContentType : ContentType
    {
        /// <inheritdoc />
        public override Type SupportedType => typeof(bool);
    }
}
