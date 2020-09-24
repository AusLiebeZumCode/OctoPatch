using System;

namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents an empty type which is used just for triggering stuff. no content at all
    /// </summary>
    public sealed class EmptyContentType : ContentType
    {
        /// <inheritdoc />
        protected override bool IsSupportedType(Type type)
        {
            return type == typeof(void);
        }
    }
}
