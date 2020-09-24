using System;

namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents a bool based message description
    /// </summary>
    public sealed class BoolContentType : ContentType
    {
        /// <inheritdoc />
        public override bool IsSupportedType(Type type)
        {
            return type == typeof(bool);
        }
    }
}
