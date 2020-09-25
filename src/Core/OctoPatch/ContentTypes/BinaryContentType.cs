using System;

namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents a binary based message description
    /// </summary>
    public sealed class BinaryContentType : ContentType
    {
        /// <summary>
        /// Gets or sets the optional limiter for the maximum length
        /// </summary>
        public int? MaximumLength { get; set; }

        /// <inheritdoc />
        public override bool IsSupportedType(Type type)
        {
            return type == typeof(byte[]);
        }

        /// <inheritdoc />
        public override ValueType NormalizeValue(ValueType value)
        {
            if (!(value is BinaryContainer binaryContainer && binaryContainer.Content != null))
            {
                return value;
            }

            var input = binaryContainer.Content;

            if (MaximumLength.HasValue && input.Length > MaximumLength.Value)
            {
                var shorten = new byte[MaximumLength.Value];
                Array.Copy(input, shorten, MaximumLength.Value);
                input = shorten;
            }

            return new BinaryContainer(input);
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

            public BinaryContainer(byte[] content)
            {
                Content = content;
            }
        }

        #endregion
    }
}
