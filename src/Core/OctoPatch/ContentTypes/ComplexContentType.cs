using System;
using System.Runtime.Serialization;

namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents a message description based on a complex type
    /// </summary>
    [DataContract]
    public sealed class ComplexContentType : ContentType
    {
        /// <summary>
        /// Holds the supported type (this is server side only)
        /// </summary>
        private Type _supportedType;

        /// <summary>
        /// Gets or sets the key of the complex type
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <inheritdoc />
        public override Type SupportedType => _supportedType;

        /// <summary>
        /// Creates a complex type based on the given type parameter
        /// </summary>
        /// <typeparam name="T">complex type</typeparam>
        /// <param name="pluginId">plugin id to generate the key</param>
        /// <returns>new complex type</returns>
        public static ComplexContentType Create<T>(Guid pluginId) where T : struct
        {
            return new ComplexContentType
            {
                _supportedType = typeof(T),
                Key = $"{pluginId}:{typeof(T).Name}",
            };
        }
    }
}
