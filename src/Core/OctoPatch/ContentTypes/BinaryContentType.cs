using System;

namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents a binary based message description
    /// </summary>
    public sealed class BinaryContentType : ContentType
    {
        /// <inheritdoc />
        public override bool IsSupportedType(Type type)
        {
            return type == typeof(byte[]);
        }
    }
}
