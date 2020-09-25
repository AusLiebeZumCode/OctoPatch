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
            if (!(value is StringContainer stringContainer) || stringContainer.Content == null)
            {
                return value;
            }

            var input = stringContainer.Content;

            // Cap the length
            if (MaximumLength.HasValue && input.Length > MaximumLength.Value)
            {
                input = input.Substring(0, MaximumLength.Value);
            }

            return new StringContainer(input);
        }

        #region nested container type

        /// <summary>
        /// Internal container for the string type
        /// </summary>
        public struct StringContainer
        {
            /// <summary>
            /// Holds the actual content
            /// </summary>
            public string Content { get; set; }

            public StringContainer(string content)
            {
                Content = content;
            }

            /// <summary>
            /// Common to string which returns the actual string
            /// </summary>
            /// <returns>content</returns>
            public override string ToString()
            {
                return Content;
            }
        }

        #endregion
    }
}
