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
        /// Gets or sets the key of the complex type
        /// </summary>
        [DataMember]
        public string Key { get; set; }

        public static ComplexContentType Create<T>(Guid pluginId)
        {
            return new ComplexContentType
            {
                Key = $"{pluginId}:{typeof(T).Name}"
            };
        }

        public override bool IsSupportedType(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
