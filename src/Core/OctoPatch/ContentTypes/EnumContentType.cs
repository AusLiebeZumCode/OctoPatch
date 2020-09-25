using System;
using System.Runtime.Serialization;

namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents a fixed list of values for the content
    /// </summary>
    [DataContract]
    public sealed class EnumContentType : ContentType
    {
        /// <inheritdoc />
        public override bool IsSupportedType(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
