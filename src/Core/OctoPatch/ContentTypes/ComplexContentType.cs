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

        public static ComplexContentType Create<T>(Guid pluginId)
        {
            return new ComplexContentType
            {
                _supportedType = typeof(T),
                Key = $"{pluginId}:{typeof(T).Name}"
            };
        }

        public override bool IsSupportedType(Type type)
        {
            if (_supportedType == null)
            {
                throw new NotSupportedException("complex content type does not have a type definition");
            }

            return _supportedType == type;
        }
    }
}
