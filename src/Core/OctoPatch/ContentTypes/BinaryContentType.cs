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

        #region nested container type

        /// <summary>
        /// Container type for transporting byte[]
        /// </summary>
        public struct BinaryContainer
        {
            /// <summary>
            /// Holds the actual content
            /// </summary>
            public byte[] Content { get; set; }
        }

        #endregion
    }
}
