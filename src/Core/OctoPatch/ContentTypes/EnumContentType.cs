using System;

namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents a fixed list of values for the content
    /// </summary>
    public sealed class EnumContentType : ContentType
    {
        /// <inheritdoc />
        protected override bool IsSupportedType(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
