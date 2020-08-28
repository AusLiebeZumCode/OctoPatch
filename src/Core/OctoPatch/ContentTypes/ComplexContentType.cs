using System;

namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents a message description based on a complex type
    /// </summary>
    public sealed class ComplexContentType : ContentType
    {
        /// <summary>
        /// Gets or sets the key of the complex type
        /// </summary>
        public string Key { get; set; }

        public static ComplexContentType Create<T>(Guid pluginId)
        {
            return new ComplexContentType
            {
                Key = $"{pluginId}:{typeof(T).Name}"
            };
        }
    }
}
