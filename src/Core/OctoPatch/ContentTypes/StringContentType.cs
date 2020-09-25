using System;

namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents a string based message
    /// </summary>
    public sealed class StringContentType : ContentType
    {
        /// <summary>
        /// Gets the optional maximum length for this string
        /// </summary>
        public int? MaximumLength { get; set; }

        /// <inheritdoc />
        public override bool IsSupportedType(Type type)
        {
            return type == typeof(string);
        }

        /// <inheritdoc />
        public override ValueType NormalizeValue(ValueType value)
        {
            string input = "";

            // Cap the length
            if (MaximumLength.HasValue && input.Length > MaximumLength.Value)
            {
                input = input.Substring(0, MaximumLength.Value);
            }

            // TODO: Wrap string into struct

            return 2;
        }

        /// <summary>
        /// Internal container for the string type
        /// </summary>
        public struct StringContainer
        {
            /// <summary>
            /// Holds the actual content
            /// </summary>
            public string Content { get; set; }
        }
    }
}
