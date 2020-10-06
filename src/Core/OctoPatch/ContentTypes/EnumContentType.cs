using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace OctoPatch.ContentTypes
{
    /// <summary>
    /// Represents a fixed list of values for the content
    /// </summary>
    [DataContract]
    public sealed class EnumContentType : ContentType
    {
        /// <summary>
        /// Holds the supported type (this is server side only)
        /// </summary>
        private Type _supportedType;

        /// <inheritdoc />
        public override Type SupportedType => _supportedType;

        /// <summary>
        /// Gets or sets the key of the complex type
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        /// <summary>
        /// Contains a list of possible values for this enum
        /// </summary>
        [DataMember]
        public List<KeyValuePair<int, string>> Values { get; set; }

        public static EnumContentType Create<T>(Guid pluginId) where T : Enum
        {
            // Collect values
            var values = new List<KeyValuePair<int, string>>();
            foreach (var content in Enum.GetValues(typeof(T)))
            {
                values.Add(new KeyValuePair<int, string>((int)content, content.ToString()));
            }

            return new EnumContentType
            {
                _supportedType = typeof(T),
                Key = $"{pluginId}:{typeof(T).Name}",
                Values = values
            };
        }
    }
}
